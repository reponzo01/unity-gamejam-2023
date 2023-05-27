using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 _cameraLookDownPosition = new Vector3(0f, 6.5f, -8f);
    private Vector3 _cameraLookUpPosition = new Vector3(0f, -6.5f, -8f);
    private Vector3 _camera2DPosition = new Vector3(0f, 0f, -8f);
    private Vector3 _cameraRequestedPosition = new Vector3(0f, 0f, 0f);
    private float _cameraLookDownRotationAngle = 40f;
    private float _cameraLookUpRotationAngle = -40f;
    private float _cameraRequestedRotationAngle = 0f;
    private float _moveSpeed = 100f;
    private float _rotateSpeed = 600f;
    private bool _moving = false;
    private bool _rotating = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_moving)
        {
            if (!Mathf.Approximately(Camera.main.transform.position.y, _cameraRequestedPosition.y))
            {
                Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, _cameraRequestedPosition, _moveSpeed * Time.deltaTime);
            }
            else
            {
                _moving = false;
            }
        }

        if (_rotating)
        {
            if (Camera.main.transform.rotation != Quaternion.Euler(_cameraRequestedRotationAngle, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z))
            {
                Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, Quaternion.Euler(_cameraRequestedRotationAngle, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z), _rotateSpeed * Time.deltaTime);
            }
            else
            {
                _rotating = false;
            }
        }
    }

    public void Play3D()
    {
        Camera.main.orthographicSize = 4.5f;
        _cameraRequestedPosition = _cameraLookDownPosition;
        _cameraRequestedRotationAngle = _cameraLookDownRotationAngle;
        _moving = true;
        _rotating = true;
    }

    public void Play2D()
    {
        Camera.main.orthographicSize = 3f;
        _cameraRequestedPosition = _camera2DPosition;
        _cameraRequestedRotationAngle = 0f;
        _moving = true;
        _rotating = true;
    }

    public void LookUp()
    {
        _cameraRequestedPosition = _cameraLookUpPosition;
        _cameraRequestedRotationAngle = _cameraLookUpRotationAngle;
        _moving = true;
        _rotating = true;
    }

    public void LookDown()
    {
        _cameraRequestedPosition = _cameraLookDownPosition;
        _cameraRequestedRotationAngle = _cameraLookDownRotationAngle;
        _moving = true;
        _rotating = true;
    }
}
