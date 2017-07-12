using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{

    public GameObject standbyCamera;
    public GameObject[] spawnSpots;

    // Use this for initialization
    void Start()
    {


        //spawnSpots = GameObject.Find<"SpawnSpot">();

        Connect();

    }

    void Update() {
        //PhotonVoiceNetwork.Client.DebugEchoMode = true;
    }

    void Connect()
    {

        PhotonNetwork.ConnectUsingSettings("Project What v001");

    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        SpawnMyPlayer();
    }

    
    void SpawnMyPlayer()
    {
        if (spawnSpots == null)
        {
            Debug.LogError("wut");
            return;
        }

        GameObject mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];

        GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate("Player 1", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        standbyCamera.SetActive(false);

        //myPlayerGO.GetComponent<FPSWalkerEnhanced>().enabled = true;
        myPlayerGO.GetComponent<FPSWalker>().enabled = true;
        myPlayerGO.GetComponent<MouseLook>().enabled = true;
        GameObject myPlayerCam = myPlayerGO.transform.FindChild("Camera").gameObject;
        myPlayerCam.GetComponent<MouseLook>().enabled = true;
        myPlayerCam.GetComponent<Camera>().enabled = true;

    }
    

}
