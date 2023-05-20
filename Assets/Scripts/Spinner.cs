using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 5f;

    // private void OnMouseDrag()
    // {
    //     float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;
    //     float rotY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;

    //     transform.RotateAround(Vector3.up, -rotX);
    //     transform.RotateAround(Vector3.right, rotY);
    // }

    private void Update()
    {
        transform.Rotate((Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime), (Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime), 0, Space.World);
    }
}
