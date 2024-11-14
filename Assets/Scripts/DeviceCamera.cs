using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceCamera : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    public GameObject plane;

    void Start()
    {
        
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            
            webCamTexture = new WebCamTexture(devices[0].name);
            // Renderer renderer = GetComponent<Renderer>();
            plane.GetComponent<Renderer>().material.mainTexture = webCamTexture;

            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("No camera found!");
        }
    }

    void OnDisable()
    {
        
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}

