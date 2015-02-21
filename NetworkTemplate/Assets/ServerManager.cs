using UnityEngine;
using System.Collections;

/// <summary>
/// Server manager handles the hosting and connecting to the game server. Provides a basic GUI for starting, finding, and connecting to games.
/// </summary>
public class ServerManager : MonoBehaviour {
	string registeredName = "somekindofuniquename";
	float refreshRequestLength = 3.0f;
	HostData[] hostData;
	public string chosenGameName = "";

	private void StartServer ()
	{
		Network.InitializeServer (16, Random.Range (2000, 2500), !Network.HavePublicAddress ());
		MasterServer.RegisterHost (registeredName, chosenGameName);
	}
	
	private IEnumerator RefreshHostList ()
	{
		MasterServer.RequestHostList (registeredName);
		float timeEnd = Time.time + refreshRequestLength;
		while (Time.time < timeEnd) {
			hostData = MasterServer.PollHostList ();
			yield return new WaitForEndOfFrame ();
		}
	}

	public void OnGUI ()
	{
		if (Network.isClient || Network.isServer) {
			return;
		}
		if (chosenGameName == "") {
			GUI.Label (new Rect (Screen.width / 2 - Screen.width / 10, Screen.height / 2 - Screen.height / 20, Screen.width / 5, Screen.height / 20), "Game Name");
		}
		chosenGameName = GUI.TextField (new Rect (Screen.width / 2 - Screen.width / 10, Screen.height / 2 - Screen.height / 20, Screen.width / 5, Screen.height / 20), chosenGameName, 25);
		if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 10, Screen.height / 2, Screen.width / 5, Screen.height / 10), "Start New Server")) {
			StartServer ();
		}
		if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 10, Screen.height / 2 + Screen.height / 10, Screen.width / 5, Screen.height / 10), "Find Servers")) {
			StartCoroutine (RefreshHostList ());
		}
		if (hostData != null) {
			for (int i = 0; i < hostData.Length; i++) {
				if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 10, Screen.height / 2 + ((Screen.height / 20) * (i + 4)), Screen.width / 5, Screen.height / 20), hostData [i].gameName)) {
					Network.Connect (hostData [i]);
				}
			}
		}
	}
}
