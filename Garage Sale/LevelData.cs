using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Items")]
    [Tooltip("Prefabs for each item that will be placeable in this level.")]
    [SerializeField] private List<GameObject> itemPrefabs;

    [Tooltip("Max number of items that can be placed. Corresponds with index of itemPrefabs.")]
    [SerializeField] private int[] maxItemNums;

    [Header("Customers")]
    [Tooltip("Max number of customers that will walk through this level.")]
    [Range(1.0f, 50.0f)]
    [SerializeField] private int customerNum;

    [Header("Money")]
    [Tooltip("Amount of money player must make to complete this level.")]
    [SerializeField] private int moneyGoal;

    // *** PUBLIC PROPERTIES ***
    public List<GameObject> ItemPrefabs { get { return itemPrefabs; } }
    public int[] MaxItemNums { get { return maxItemNums; } }
    public int CustomerNum { get { return customerNum; } }
    public int MoneyGoal { get { return moneyGoal; } }
}
