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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Mathf.Approximately(Camera.main.transform.position.y, _cameraRequestedPosition.y))
        {
            //Debug.Log("Moving Cam");
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, _cameraRequestedPosition, _moveSpeed * Time.deltaTime);
        }

        if (Camera.main.transform.rotation != Quaternion.Euler(_cameraRequestedRotationAngle, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z))
        {
            //Debug.Log($"Rotating Cam ({Camera.main.transform.rotation.eulerAngles.x}) to {_cameraRequestedRotationAngle}");
            Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, Quaternion.Euler(_cameraRequestedRotationAngle, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z), _rotateSpeed * Time.deltaTime);
        }
    }

    public void SwitchTo3D()
    {
        Camera.main.orthographicSize = 4.5f;
        _cameraRequestedPosition = _cameraLookDownPosition;
        _cameraRequestedRotationAngle = _cameraLookDownRotationAngle;
    }

    public void SwitchTo2D()
    {
        Camera.main.orthographicSize = 3f;
        _cameraRequestedPosition = _camera2DPosition;
        _cameraRequestedRotationAngle = 0f;
    }

    public void LookUp()
    {
        _cameraRequestedPosition = _cameraLookUpPosition;
        _cameraRequestedRotationAngle = _cameraLookUpRotationAngle;
    }

    public void LookDown()
    {
        _cameraRequestedPosition = _cameraLookDownPosition;
        _cameraRequestedRotationAngle = _cameraLookDownRotationAngle;
    }
}
