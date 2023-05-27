using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCube : MonoBehaviour
{
    [SerializeField] private GameObject panel1Light;

    private float _rotateYAngle = 0f;
    private float _rotateSpeed = 600f;
    private bool _rotating = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_rotating)
        {
            if (transform.rotation != Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateYAngle, transform.rotation.eulerAngles.z))
            {
                //Debug.Log($"Rotating main cube ({transform.eulerAngles.y}) to {_rotateYAngle}");
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, _rotateYAngle, transform.rotation.eulerAngles.z), _rotateSpeed * Time.deltaTime);
            }
            else
            {
                _rotating = false;
            }
        }
    }

    public void Play3D()
    {
        _rotateYAngle = 45f;
        EnablePanel(2);
        //panel1Light.SetActive(true);
        _rotating = true;
    }

    public void Play2D()
    {
        _rotateYAngle = 0f;
        //panel1Light.SetActive(false);
        _rotating = true;
    }

    public void TurnLeft()
    {
        _rotateYAngle += 90f;
        _rotating = true;
    }

    public void TurnRight()
    {
        _rotateYAngle -= 90f;
        _rotating = true;
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
