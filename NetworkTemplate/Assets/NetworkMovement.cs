using UnityEngine;
using System.Collections;

//http://createathingunity.blogspot.co.uk/
/// <summary>
/// Network movement handles basic movement across the network.
/// </summary>
public class NetworkMovement : MonoBehaviour
{
    public int moveSpeed = 8;
    public bool haveControl = false;

    void FixedUpdate()
    {
        if (haveControl)
        {
            float vert = Input.GetAxis("Vertical");
            float horiz = Input.GetAxis("Horizontal");

            if (Network.isServer)
            {
                MovePlayer(vert, horiz);
            }
            else
            {
                networkView.RPC("MovePlayer", RPCMode.Server, vert, horiz);
            }
        }
    }

    [RPC]
    void MovePlayer(float vert, float horiz)
    {
        Vector3 newVelocity = (transform.right * horiz * moveSpeed) + (transform.forward * vert * moveSpeed);
        Vector3 myVelocity = rigidbody.velocity;
        myVelocity.x = newVelocity.x;
        myVelocity.z = newVelocity.z;

        rigidbody.velocity = myVelocity;
        networkView.RPC("UpdatePlayer", RPCMode.OthersBuffered, transform.position);
    }

    [RPC]
    void UpdatePlayer(Vector3 playerPos)
    {
        transform.position = playerPos;
    }
}