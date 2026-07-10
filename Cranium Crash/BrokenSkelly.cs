using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenSkelly : Skelly
{
    Transform childTransform;
    GameObject child;

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(0.4f, -0.8f, 0f);
        hasHead = false;
        childTransform = gameObject.transform.GetChild(0);
        child = childTransform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        child.SetActive(hasHead);
    }
}
