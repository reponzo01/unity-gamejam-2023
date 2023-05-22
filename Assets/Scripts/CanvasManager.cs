using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SwitchTo2D();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchTo3D()
    {
        //Debug.Log("Switching canvas to 3D");
        transform.Find("3DControls").gameObject.SetActive(true);
    }

    public void SwitchTo2D()
    {
        //Debug.Log("Switching canvas to 2D");
        transform.Find("3DControls").gameObject.SetActive(false);
    }
}
