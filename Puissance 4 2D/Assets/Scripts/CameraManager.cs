using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    
    private void Start()
    {
        cameraTransform = GetComponent<Transform>();
        CameraPlacement();
    }

    private void CameraPlacement()
    {
        var x = (float)MapManager.instance.x/2;
        var y = (float)MapManager.instance.y/2;
        cameraTransform.position = new Vector3(x,y,cameraTransform.position.z);


    }
}
