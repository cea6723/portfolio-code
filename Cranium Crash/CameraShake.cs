using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        SetBasePos();
    }

    public void StartShake(float duration, float magnitude)
    {
        if (Camera.main.GetComponent<CameraMovement>().ActivelyMoving())
        {
            return;
        }
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(Shake(duration, magnitude));
    }

    public void SetBasePos()
    {
        originalPos = transform.localPosition;
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // decay shake intensity over time
            float currentMagnitude = Mathf.Lerp(magnitude, 0f, elapsed / duration);

            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            //Ensure camera is at -10 z
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;

        //Ensure camera is at -10 z
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        coroutine = null;
    }
   
}
