using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager2 : Photon.MonoBehaviour {


    public InputField playerNameInput;
    public Text playerNamePrompt;
    public Button connectButton;

    public Text playerColorText;
    public ColorPicker colorPicker;

    public Text sensitivityText;
    public Slider sensitivitySlider;
    public Text sensitivityNumber;

    public Text playerInitialText;
    public Text playerInitialHintText;
    public InputField playerInitialInput;

    //from OnJoinedInstantiate.cs

    public Transform SpawnPosition;
    public float PositionOffset = 2.0f;
    public GameObject prefabToInstantiate;   // set in inspector



    public GameObject standbyCamera;


    //from ConnectAndJoinRandom.cs

    /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
    public bool AutoConnect = false;

    public byte Version = 1;

    /// <summary>if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()</summary>
    private bool ConnectInUpdate = true;

    PhotonPlayer photonPlayer;

    public Color playerColor;

    public static bool optionsMenuActive;

    public virtual void Start()
    {

        optionsMenuActive = false;

        colorPicker.onValueChanged.AddListener(color =>
        {
            playerColor = color;
        });

        photonPlayer = PhotonNetwork.player;

        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.

        PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
        PhotonVoiceNetwork.Client.loadBalancingPeer.TrafficStatsEnabled = true;

        connectButton.onClick.AddListener(Connect);

        sensitivitySlider.onValueChanged.AddListener(ChangeSensitivity);
        ChangeSensitivity(sensitivitySlider.value);
        

    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        
    }

    void ChangeSensitivity( float sensitivity)
    {

        

        sensitivityNumber.text = sensitivity.ToString();

        photonPlayer.customProperties = new ExitGames.Client.Photon.Hashtable() { { "Sen", sensitivitySlider.value } };
        photonPlayer.SetCustomProperties(photonPlayer.customProperties);

    }

    void Connect() {

        if(playerNameInput.text != "" && playerInitialInput.text != "")
        {
            PhotonNetwork.player.name = playerNameInput.text;

            photonPlayer.customProperties = new ExitGames.Client.Photon.Hashtable() { { "Ini", playerInitialInput.text }, { "R", playerColor.r },
                {"G", playerColor.g }, {"B", playerColor.b },{ "Sen", sensitivitySlider.value }};
            photonPlayer.SetCustomProperties(photonPlayer.customProperties);

            AutoConnect = true;

            playerNameInput.gameObject.SetActive(false);
            playerNamePrompt.gameObject.SetActive(false);
            connectButton.gameObject.SetActive(false);

            playerColorText.gameObject.SetActive(false);
            colorPicker.gameObject.SetActive(false);

            sensitivityText.gameObject.SetActive(false);
            sensitivitySlider.gameObject.SetActive(false);
            sensitivityNumber.gameObject.SetActive(false);

            playerInitialText.gameObject.SetActive(false);
            playerInitialInput.gameObject.SetActive(false);
            playerInitialHintText.gameObject.SetActive(false);

            optionsMenuActive = false;
        }
    }

    public virtual void Update()
    {


        
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");

            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        }

        if (PhotonNetwork.connected)
        {
            if (Input.GetButtonDown("Options"))
            {
                optionsMenuActive = !optionsMenuActive;
            }
            if (optionsMenuActive)
            {
                sensitivityNumber.gameObject.SetActive(true);
                sensitivitySlider.gameObject.SetActive(true);
                sensitivityText.gameObject.SetActive(true);
            }
            else
            {
                sensitivityNumber.gameObject.SetActive(false);
                sensitivitySlider.gameObject.SetActive(false);
                sensitivityText.gameObject.SetActive(false);
            }
        }

        

    }


    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage


    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 20}, null);");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 20 }, null);
    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");

        if (this.prefabToInstantiate != null)
        {
           
                Debug.Log("Instantiating: " + prefabToInstantiate.name);

                Vector3 spawnPos = Vector3.up;
                if (this.SpawnPosition != null)
                {
                    spawnPos = this.SpawnPosition.position;
                }

                Vector3 random = Random.insideUnitSphere;
                random.y = 0;
                random = random.normalized;
                Vector3 itempos = spawnPos + this.PositionOffset * random;

                PhotonNetwork.Instantiate(prefabToInstantiate.name, itempos, Quaternion.identity, 0);
            
                



            standbyCamera.SetActive(false);
        }

    }
    



}
