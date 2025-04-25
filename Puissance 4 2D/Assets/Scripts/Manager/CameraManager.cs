using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    public static CameraManager instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        cameraTransform = GetComponent<Transform>();
        CameraPlacement();
    }

    private void CameraPlacement()
    {
        var x = (float)MapManager.instance.w/2;
        var y = (float)MapManager.instance.h/2;
        cameraTransform.position = new Vector3(x,y,cameraTransform.position.z);


    }
}
