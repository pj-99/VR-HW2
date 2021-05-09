using Mirror;
using UnityEngine;

namespace pj.mirror.Mirror_practice
{
    public class Player : NetworkBehaviour
    {

        [SerializeField] private Vector3 movement = new Vector3();

        [Client] // on client call this

        private void Update()
        {
            if (!hasAuthority) return;
            if (!Input.GetKeyDown(KeyCode.Space)) { return; }
            //this.transform.Translate(movement);
            //Debug.Log("space pressed"+ movement);

            CmdMove();
        }
        [Command]
        private void CmdMove() {
            RpcMove();
        }
        [ClientRpc]
        private void RpcMove() => transform.Translate(movement);
    }
}