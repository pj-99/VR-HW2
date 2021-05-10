using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.Interaction.Toolkit;
public class CameraAttach : NetworkBehaviour
{
    private GameObject xrRig = null;
    // Start is called before the first frame update
    void Start()
    {
        if (!hasAuthority)
        {
            xrRig = GameObject.Find("XR Rig"); 
            Debug.Log("hellllllllllllllo");
            Transform cameraTransform = xrRig.gameObject.transform;  //Find main camera which is part of the scene instead of the prefab
            Debug.Log("camreTansform" + cameraTransform);
                cameraTransform.parent = this.gameObject.transform;  //Make the camera a child of the mount point
                cameraTransform.position = this.gameObject.transform.position;  //Set position/rotation same as the mount point
                cameraTransform.rotation = this.gameObject.transform.rotation;
            return;
        }
        Debug.Log("noAuthoirty");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
