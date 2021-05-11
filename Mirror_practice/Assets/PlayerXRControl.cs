using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.Interaction.Toolkit;
public class PlayerXRControl : NetworkBehaviour
{

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isLocalPlayer)
        {
            Debug.Log("not local player ");
        }
        else {

            Debug.Log("is local player ");
        }
        if (!hasAuthority)
        {
            gameObject.SetActive(false);
            Debug.Log("no authority");
        }
        else {
            Debug.Log("has authority");
        }
        //xrRig = gameObject.GetComponent<XRRig>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
