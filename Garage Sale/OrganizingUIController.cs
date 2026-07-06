using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganizingUIController : MonoBehaviour
{
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform gridParent;

    private void OnEnable()
    {
        BuildInventoryDisplay();
    }

    public void OnSellPress()
    {
        GameManager.instance.UpdateGameState(GameState.Selling);
    }

    private void BuildInventoryDisplay()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        List<GameObject> itemsToDisplay = GameManager.instance.CurrentLevel.ItemPrefabs;

        int id = 0;
        foreach (GameObject prefab in itemsToDisplay)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, gridParent);
            InventorySlotController controller = slot.GetComponent<InventorySlotController>();
            controller.DisplayItem(prefab);
            controller.SetId(id);
            id++;
        }
    }
}
