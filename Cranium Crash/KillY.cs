using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillY : MonoBehaviour
{
    [SerializeField] private float yThreshold;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10) gameObject.SetActive(false);
    }
}
