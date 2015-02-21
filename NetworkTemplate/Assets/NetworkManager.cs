using UnityEngine;
using System.Collections;
[ExecuteInEditMode]

//http://createathingunity.blogspot.co.uk/
/// <summary>
/// Network manager handles initialization of players connecting to a server
/// </summary>
public class NetworkManager : MonoBehaviour
{
		public Transform playerPrefab;

		void OnServerInitialized ()
		{ 
				if (Network.isServer) {
					MakePlayer (Network.player);
				}
		}
	
		void OnConnectedToServer ()
		{
			networkView.RPC ("MakePlayer", RPCMode.Server, Network.player);
		}

		[RPC]
		void MakePlayer (NetworkPlayer thisPlayer)
		{
				Transform newPlayer = Network.Instantiate (playerPrefab, playerPrefab.position, playerPrefab.rotation, 0) as Transform;
				
				//if (Network.isServer) {
				//		var rigidBody = newPlayer.gameObject.AddComponent<Rigidbody> ();
				//		rigidBody.freezeRotation = true;
				//}

				if (thisPlayer != Network.player) {
						networkView.RPC ("EnableCamera", thisPlayer, newPlayer.networkView.viewID);
				} else {
						EnableCamera (newPlayer.networkView.viewID);
				}
		}
	
		[RPC]
		void EnableCamera (NetworkViewID viewID)
		{
				GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
				foreach (GameObject playerObject in players) {
						if (playerObject.networkView && playerObject.networkView.viewID == viewID) {
								playerObject.GetComponent<NetworkMovement> ().haveControl = true;
								Transform myCamera = playerObject.transform.Find ("Camera");
								myCamera.camera.enabled = true;
								myCamera.camera.GetComponent<AudioListener> ().enabled = true;
								break;
						}
				}
		}
}
