using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float RotationSpeedMin = 50f;
    public float RotationSpeedMax = 100f;
    private float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = Random.Range(RotationSpeedMin, RotationSpeedMax);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
