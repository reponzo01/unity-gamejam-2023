using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public bool clockwise = false;

    private void Update()
    {
        var spinDirection = clockwise ? -1 : 1;
        transform.Rotate (0, 0, 50f * spinDirection * Time.deltaTime);
    }
}
