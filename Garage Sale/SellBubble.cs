using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellBubble : MonoBehaviour
{
    [SerializeField] private GameObject particlePopPrefab;

    private Transform targetToFollow;
    private Camera mainCamera;

    private RectTransform bubbleRectTransform;
    private RectTransform canvasRectTransform;

    private bool isActive = false;

    public bool IsActive { get { return isActive; } set { isActive = value; } }

    private void Awake()
    {
        bubbleRectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        GetComponent<Button>().onClick.AddListener(OnBubbleClicked);
        mainCamera = Camera.main;
    }

    public void Initialize(Transform followTarget)
    {
        this.targetToFollow = followTarget;
    }

    void Update()
    {
        if (targetToFollow == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 screenPoint = mainCamera.WorldToScreenPoint(targetToFollow.position + new Vector3(0.0f, 4.0f, 0.0f));

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPoint, mainCamera, out localPoint);
        bubbleRectTransform.anchoredPosition = localPoint;

        gameObject.SetActive(isActive);
    }

    public void OnBubbleClicked()
    {
        Destroy(gameObject);
    }

}
