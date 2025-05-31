using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRFloating : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatHeight = 0.5f;

    private Vector3 startPos;

    void OnEnable()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time * floatSpeed) * floatHeight, 0);
    }
}
