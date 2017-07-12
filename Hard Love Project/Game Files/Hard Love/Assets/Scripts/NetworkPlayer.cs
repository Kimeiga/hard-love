using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour
{

    /*
    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    Quaternion realHeadRotation = Quaternion.identity;
    public GameObject head;
    */

    public GameObject playerCamera;
    public MouseLook headMouseLook;
    public MouseLook bodyMouseLook;
    public FPSWalker fpsWalker;

    public TextMesh playerNameBack;
    public TextMesh playerInitialsBack;
    public TextMesh playerInitialsFront;
    public string playerName;
    private string playerInitials;

    public PhotonTransformView playerTransformView;
    public SpeedCalculator speedCalculatorScript;
    
    // Use this for initialization
    void Start()
    {

        playerNameBack.text = photonView.owner.name;

        if (photonView.isMine)
        {
            

            playerInitials = (string)PhotonNetwork.player.customProperties["Ini"];
            playerInitials = playerInitials.Substring(0, 3);
            playerInitialsBack.text = playerInitials;
            playerInitialsFront.text = playerInitials;

        }

        else
        {
            for(int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if(PhotonNetwork.playerList[i].name == photonView.owner.name)
                {
                    playerInitials = (string)PhotonNetwork.playerList[i].customProperties["Ini"];
                    playerInitials = playerInitials.Substring(0, 3);
                    playerInitialsBack.text = playerInitials;
                    playerInitialsFront.text = playerInitials;
                }
            }

        }

        

    }

    void Update()
    {

        if (NetworkManager2.optionsMenuActive)
        {

            headMouseLook.canRotate = false;
            bodyMouseLook.canRotate = false;
            fpsWalker.canControl = false;
            Cursor.visible = true;
        }
        else
        {
            headMouseLook.canRotate = true;
            bodyMouseLook.canRotate = true;
            fpsWalker.canControl = true;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (photonView.isMine)
        {
            // Do nothing -- the character motor/input/etc... is moving us


            playerCamera.SetActive(true);
            headMouseLook.enabled = true;
            bodyMouseLook.enabled = true;

            playerTransformView.SetSynchronizedValues(speedCalculatorScript.measured3DSpeed, 0);
        }
        else
        {
            /*
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.5f);
             head.transform.rotation = Quaternion.Lerp(head.transform.rotation, realHeadRotation, 0.5f);
            */



        }
    }

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // This is OUR player. We need to send our actual position to the network.
            
            stream.SendNext(PhotonNetwork.player.name);
        }
        else
        {
            // This is someone else's player. We need to receive their position (as of a few
            // millisecond ago, and update our version of that player.
            

            playerName = (string)stream.ReceiveNext();
        }

    }
    */
}
