using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject prefab2;
    [SerializeField] GameObject mousePointer;
    private ChildTracker childTracker = null;
    private bool toggle;

    private void Start()
    {
        // Try to find a child tracker on this spawner and save a reference to it
        childTracker = GetComponent<ChildTracker>();
        toggle = false;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mousePointer.transform.position = hit.point;
        }
    }

    /// <summary>
    /// Spawn a new instance of the prefab as a child of the gameObject this
    /// component is attached to
    /// </summary>
    public void Spawn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 loc;

            // Find the mouse position in the world
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                loc = hit.point;

                if (!toggle)
                {
                    // Create a new object at that location with this spawner as the parent
                    GameObject newObject = Instantiate(prefab, loc, Quaternion.identity, transform);

                    // If we have a child tracker & if this new child is trackable, tell it who it's parent it
                    if (childTracker != null)
                    {
                        ITrackedChild trackedChild = newObject.GetComponent<ITrackedChild>();
                        if (trackedChild != null)
                        {
                            trackedChild.SetTracker(childTracker);
                        }
                    }
                }
                else
                {
                    // Create a new object at that location with this spawner as the parent
                    Instantiate(prefab2, loc, Quaternion.identity, transform);
                }
            }   
        }
    }

    public void DestroyAll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            for(int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }

    public void Toggle(InputAction.CallbackContext context)
    {
        toggle = !toggle; 
    }
}
