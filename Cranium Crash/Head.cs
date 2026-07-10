using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Head : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    public Skelly activeBody;

    [Header("Launch Settings")]
    [SerializeField] private float launchForce = 10.0f;
    //[SerializeField] private Vector2 resetLocation;

    [Header("Camera Effects")]
    [SerializeField] private CameraShake cameraShaker;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;

    [Header("Trajectory")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField][Range(10, 100)] private int trajectoryPoints;
    [SerializeField][Range(0.01f, 0.5f)] private float timeStep = 0.1f;
    [SerializeField] private LayerMask trajectoryStopLayers;

    [Header("Effects")]
    [SerializeField] private ParticleSystem flameEffects;
    [SerializeField] private TrailRenderer trail;

    [Header("Wall Chunks")]
    [SerializeField] private int wallChunksNum = 3;
    [SerializeField] private float minChunkSize = 0.2f;
    [SerializeField] private float maxChunkSize = 0.5f;

    private ObjectPooler pooler;

    private Vector2 screenMousePos;
    private Vector3 worldMousePos;

    private Rigidbody2D rb;
    private Collider2D col;
    private float mass;

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 currentDragPos;
    private bool isLaunched;
    private bool isDragging = false;

    private Vector2 lastVelocity;

    public float AFKTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mass = rb.mass;
        rb.isKinematic = true;

        col = GetComponent<Collider2D>();
        flameEffects = GetComponentInChildren<ParticleSystem>();
        if (flameEffects != null) flameEffects.Stop();
        HideTrajectory();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            currentDragPos = GetMouseWorldPos();
            Vector2 launchVector = (startPos - currentDragPos) * launchForce;
            UpdateTrajectory(launchVector);
        }

        if (!isLaunched)
        {
            transform.position = activeBody.transform.position + activeBody.offset;
            AFKTimer += Time.deltaTime;
        }

        GetComponent<SpriteRenderer>().enabled = isLaunched;

        if (AFKTimer >= 20)
        {
            int variant = Random.Range(1, 4);
            GameManager.Instance.PlaySound("AFK", variant);
            GameManager.Instance.DisplayDialogue("AFK", 3, variant);
            AFKTimer -= 20;
        }

    }

    private void FixedUpdate()
    {    
        lastVelocity = rb.velocity;

        if (Mathf.Abs(rb.velocity.x) > 20f || Mathf.Abs(rb.velocity.y) > 20f)
        {
            if (!flameEffects.isPlaying)
            {
                flameEffects.Play();

                if (Random.Range(0, 4) == 0)
                {
                    int variant = Random.Range(1, 4);
                    GameManager.Instance.PlaySound("FIRE", variant);
                    GameManager.Instance.DisplayDialogue("FIRE", 3, variant);
                }
            }
        }
    }

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Application.Quit();
        }
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.performed && isLaunched)
        {
            isLaunched = false;
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            //transform.position = resetLocation;
            transform.position = GameManager.Instance.RespawnPoint + activeBody.offset;
            transform.rotation = Quaternion.identity;
            //movement.hasHead = true;

            Camera.main.GetComponent<CameraMovement>().ReturnToSafeRoom();

            if (Random.Range(0, 5) == 0)
            {
                int variant = Random.Range(1, 4);
                GameManager.Instance.PlaySound("RESET", variant);
                GameManager.Instance.DisplayDialogue("RESET", 3, variant);
            }

            AFKTimer = 0;
        }
    }

    public void OnLaunch(InputAction.CallbackContext context)
    {
        if (isLaunched || !GameManager.Instance.controllable) return;

        if (!activeBody.isMoving)
        {

            if (context.started)
            {
                startPos = GetMouseWorldPos();
                isDragging = true;
                trajectoryLine.enabled = true;
            }

            if (context.canceled && isDragging)
            {
                isDragging = false;
                HideTrajectory();
                activeBody.hasHead = false;
                endPos = GetMouseWorldPos();
                Vector2 launchVector = (startPos - endPos) * launchForce;

                //transform.position = activeBody.transform.position + new Vector3(-0.1f, 1.5f, 0f);
                rb.isKinematic = false;
                rb.AddForce(launchVector, ForceMode2D.Impulse);
                isLaunched = true;
                if (trail != null) trail.emitting = true;

            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        if (Mouse.current != null)
        {
            screenMousePos = Mouse.current.position.ReadValue();
            worldMousePos = mainCamera.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, mainCamera.nearClipPlane));
            worldMousePos.z = 0f;
        }
        Debug.Log("World Mouse Position: " + worldMousePos);
        return worldMousePos;
    }

    public void UpdateTrajectory(Vector2 launchVector)
    {
        Vector2 velocity = launchVector / mass;
        Vector2 gravity = Physics2D.gravity * rb.gravityScale;
        float drag = rb.drag;

        List<Vector3> points = new List<Vector3>();
        Vector2 currentPos = transform.position;

        // simulate future physics steps to calculate flight path
        for (int i = 0; i < trajectoryPoints; i++)
        {
            points.Add(currentPos);

            // apply gravity and linear drag over time step
            velocity += gravity * timeStep;
            velocity *= (1 - drag * timeStep);
            Vector2 newPosition = currentPos + velocity * timeStep;

            currentPos = newPosition;
            trajectoryLine.positionCount = points.Count;
            trajectoryLine.SetPositions(points.ToArray());
        }
        AFKTimer = 0;
    }

    public void HideTrajectory()
    {
        trajectoryLine.enabled = false;
        trajectoryLine.positionCount = 0;
    }

    private void SpawnWallChuncks(Collision2D collision)
    {
        float impactSpeed = lastVelocity.magnitude;
        Vector2 impactNormal = collision.contacts[0].normal;

        for (int i = 0; i < wallChunksNum; i++)
        {
            GameObject piece = ObjectPooler.instance.GetPooledObject("Wall");
            if (piece != null)
            {
                piece.SetActive(true);
                piece.transform.position = transform.position;

                // randomize visuals
                piece.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                float randomScale = Random.Range(minChunkSize, maxChunkSize);
                piece.transform.localScale = new Vector3(randomScale, randomScale, 1f);

                // calcluate scatter physics based on wall's impact normal
                Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
                Vector2 scatterDirection = (impactNormal + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0.2f, 0.8f))).normalized;

                float chunkSpeed = impactSpeed * Random.Range(0.1f, 0.3f);
                rb.velocity = scatterDirection * chunkSpeed;
                rb.angularVelocity = Random.Range(-360f, 360f);
            }
        }

        GameManager.Instance.PlaySound("CRASH" + Random.Range(1, 5));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (cameraShaker != null)
        {
            cameraShaker.StartShake(shakeDuration, shakeMagnitude * (rb.velocity.magnitude/5.0f));
            Debug.Log("MAGNITUDE" + shakeMagnitude * (rb.velocity.magnitude / 5.0f));
        }

        if (collision.gameObject.CompareTag("Breakable"))
        {
            Debug.Log(lastVelocity.x + ", " + lastVelocity.y);

            if (collision.gameObject.GetComponent<BreakableBlock>().hitPoints == -1)
            {
                collision.gameObject.GetComponent<BreakableBlock>().BreakBlock(lastVelocity);
                rb.velocity = lastVelocity;
            }
            else
            {
                collision.gameObject.GetComponent<BreakableBlock>().BreakBlock(lastVelocity);
            }
        }

        SpawnWallChuncks(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Skeleton")
        {
            activeBody = collision.GetComponent<Skelly>();
            collision.GetComponent<Skelly>().hasHead = true;
            GameManager.Instance.RespawnPoint = collision.transform.position;

            isLaunched = false;
            if (trail != null) trail.emitting = false;
            if (flameEffects.isPlaying)
            {
                flameEffects.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.position = GameManager.Instance.RespawnPoint + activeBody.offset;
            transform.rotation = Quaternion.identity;
            Camera.main.GetComponent<CameraMovement>().SetSafeRoom();

            //if (Random.Range(0, 4) == 0)
            //{
            //    int variant = Random.Range(1, 4);
            //    GameManager.Instance.PlaySound("BODY", variant);
            //    GameManager.Instance.DisplayDialogue("BODY", 1, variant);
            //}
        }

        if (collision.gameObject.tag == "Web")
        {
            if (Mathf.Abs(lastVelocity.x) >= 20 || Mathf.Abs(lastVelocity.y) >= 20)
            {
                collision.GetComponent<AreaEffector2D>().enabled = false;
                Destroy(collision.gameObject);
            }
        }
    }

    public bool IsLaunched
    {
        get { return isLaunched; }
    }

    public void AFKReset()
    {
        AFKTimer = 0;
    }
}
