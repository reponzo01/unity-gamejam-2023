using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCube : MonoBehaviour
{
    private float _rotateYAngle = 0f;
    private float _rotateSpeed = 600f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (!Mathf.Approximately(transform.eulerAngles.y, _rotateYAngle))
        if (transform.rotation != Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateYAngle, transform.rotation.eulerAngles.z))
        {
            //Debug.Log($"Rotating main cube ({transform.eulerAngles.y}) to {_rotateYAngle}");
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateYAngle, transform.rotation.eulerAngles.z), _rotateSpeed * Time.deltaTime);
        }
    }

    public void SwitchTo3D()
    {
        _rotateYAngle = 45f;
        transform.Find("Panel1").Find("Light").gameObject.SetActive(true);
    }

    public void SwitchTo2D()
    {
        _rotateYAngle = 0f;
        transform.Find("Panel1").Find("Light").gameObject.SetActive(false);
    }

    public void TurnLeft()
    {
        _rotateYAngle += 90f;
    }

    public void TurnRight()
    {
        _rotateYAngle -= 90f;
    }

    public void EnablePanel(int panelNumber)
    {
        var panel = transform.Find($"Panel{panelNumber}");
        panel.gameObject.SetActive(true);
        for (var i = 0; i < panel.childCount; i++)
        {
            var child = panel.transform.GetChild(i);
            if (child.CompareTag("Tile"))
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void DisablePanel(int panelNumber)
    {
        var panel = transform.Find($"Panel{panelNumber}");
        panel.gameObject.SetActive(false);
    }
}
