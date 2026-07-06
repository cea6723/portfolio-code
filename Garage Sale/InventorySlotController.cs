using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySlotController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_FontAsset font;
    private GameObject itemPrefab;
    private int itemId;
    private TextMeshProUGUI text;

    void Start()
    {
        // set text for number in top right
        text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "" + GameManager.instance.CurrentItemNums[itemId].ToString();
        text.font = font;
    }

    /// <summary>
    /// Populate Item slot with corresponding sprite and text
    /// </summary>
    public void DisplayItem(GameObject itemToDisplay)
    {
        itemPrefab = itemToDisplay;
        Item item = itemPrefab.GetComponent<Item>();

        if (item != null)
        {
            icon.sprite = item.ItemIcon;
            text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "" + GameManager.instance.CurrentItemNums[itemId].ToString();
            text.font = font;
        }
    }

    public void SetId(int id)
    {
        itemId = id;
    }

    /// <summary>
    /// When inveontory slot is dragged, spawn item
    /// transfers drag to MouseManager
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemPrefab != null && 
            GameManager.instance.CurrentItemNums[itemId] > 0 && 
            GameManager.instance.CurrentState == GameState.Organizing)
        {
            GameObject prefabToSpawn = itemPrefab;
            Vector3 worldPosition = GameManager.instance.MouseManager.GetMouseWorldPosition();
            GameObject itemInScene = Instantiate(prefabToSpawn, worldPosition, itemPrefab.transform.rotation);
            GameManager.instance.CurrentItemNums[itemId]--;
            text.text = "" + GameManager.instance.CurrentItemNums[itemId].ToString();

            GameManager.instance.MouseManager.UIDrag(itemInScene);
        }
    }

    /// <summary>
    /// Calls MouseManager drag when dragging from inventory slot
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        GameManager.instance.MouseManager.ItemDrag();
    }

    /// <summary>
    /// verifies placement validity via the MouseManager.
    /// If the player drops the item in an invalid location, 
    /// the instantiation is canceled and the inventory count is refunded
    /// </summary>
    public void OnEndDrag(PointerEventData eventData) 
    {
        bool wasPlaced = GameManager.instance.MouseManager.ItemRelease();

        if (!wasPlaced)
        {
            GameManager.instance.CurrentItemNums[itemId]++;
            text.text = "" + GameManager.instance.CurrentItemNums[itemId];
        }
    }
}
