using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skelly : MonoBehaviour
{
    public bool isMoving = false;
    public bool hasHead = false;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if (hasHead)
        {
            GameObject.FindObjectOfType<Head>().activeBody = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasHead)
        {
            
        }

    }
}
