using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class PlayerCameraControl : NetworkBehaviour
{


    public Transform head = null;
    public Transform right = null;
    public Transform left = null;


    private Transform headRig = null;
    private Transform rightRig = null;
    public Transform leftRig = null;
    public NetworkIdentity ni = null;



    private XRIDefaultInputActions controls;
    private XRIDefaultInputActions Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new XRIDefaultInputActions();
        }
    }


    private void Start()
    {

        XRRig xrrig = FindObjectOfType<XRRig>();
        headRig = xrrig.transform.Find("Camera Offset/Main Camera");
        leftRig = xrrig.transform.Find("Camera Offset/LeftHand Controller");
        rightRig = xrrig.transform.Find("Camera Offset/RightHand Controller");

    }

    /*
    public override void OnStartAuthority()
    {
        Debug.Log("on start auth");
        XRRig.gameObject.SetActive(true);
        //base.OnStartAuthority();
        enabled = true;


       
    }*/

    /**
    [ClientCallback]
    private void OnEnable()
    {
        Debug.Log("onEnable");
        Controls.Enable();
    }
    [ClientCallback]
    private void OnDisable()
    {
        Debug.Log("onDisable");
        Controls.Disable();
    }*/


    private void Update()
    {
        if (!hasAuthority)
        {
            {
                head.gameObject.SetActive(false);
                right.gameObject.SetActive(false);
                left.gameObject.SetActive(false);
                mapPosition(head, headRig);
                mapPosition(right, rightRig);
                mapPosition(left, leftRig);
            }
        }

    }

    void mapPosition(Transform target, Transform rigTransform)
    {

        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;

    }
}
