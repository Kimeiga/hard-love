using UnityEngine;
using System.Collections;

public class VoiceFeedback : Photon.MonoBehaviour {

    public PhotonVoiceRecorder voiceRecorderScript;
    public PhotonView photonView;
    public float loudness;

    public GameObject mouth;
    public GameObject head;
    public Renderer headRenderer;

    private Color originalHeadColor;
    private Color vibrantHeadColor;
    

	// Use this for initialization
	void Start () {
        
        headRenderer.material = new Material(Shader.Find("Standard"));

        headRenderer.material.EnableKeyword("_EMISSION");

        if (photonView.isMine)
        {


            originalHeadColor = new Color((float)PhotonNetwork.player.customProperties["R"],
                (float)PhotonNetwork.player.customProperties["G"], (float)PhotonNetwork.player.customProperties["B"]);



        }

        else
        {
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (PhotonNetwork.playerList[i].name == photonView.owner.name)
                {

                    originalHeadColor = new Color((float)PhotonNetwork.playerList[i].customProperties["R"],
                        (float)PhotonNetwork.playerList[i].customProperties["G"], (float)PhotonNetwork.playerList[i].customProperties["B"]);


                }
            }

        }

        float h;
        float s;
        float v;
        

        Color.RGBToHSV(originalHeadColor, out h, out s, out v);

        vibrantHeadColor = Color.HSVToRGB(h, 1, 1);


	}

    // Update is called once per frame
    void Update()
    {

        if (photonView.isMine)
        {


            originalHeadColor = new Color((float)PhotonNetwork.player.customProperties["R"],
                (float)PhotonNetwork.player.customProperties["G"], (float)PhotonNetwork.player.customProperties["B"]);



        }

        else
        {
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (PhotonNetwork.playerList[i].name == photonView.owner.name)
                {

                    originalHeadColor = new Color((float)PhotonNetwork.playerList[i].customProperties["R"],
                        (float)PhotonNetwork.playerList[i].customProperties["G"], (float)PhotonNetwork.playerList[i].customProperties["B"]);


                }
            }

        }



        if (photonView.isMine)
        {
            if (voiceRecorderScript != null && voiceRecorderScript.LevelMeter != null && voiceRecorderScript.IsTransmitting && voiceRecorderScript.LevelMeter.CurrentPeakAmp != 0)
            {
                loudness = voiceRecorderScript.LevelMeter.CurrentAvgAmp * 14;
            }

            else
            {
                loudness = 0;
            }

        }
            Vector3 temp = mouth.transform.localScale;
            temp.y = loudness;
            mouth.transform.localScale = temp;

        headRenderer.material.color = Color.Lerp(originalHeadColor, vibrantHeadColor, loudness);


        headRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.black, vibrantHeadColor, loudness));

        //DynamicGI.SetEmissive(headRenderer, Color.Lerp(Color.black, vibrantHeadColor, loudness));

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // This is OUR player. We need to send our actual position to the network.

            stream.SendNext(loudness);
            stream.SendNext(loudness);
        }
        else
        {
            // This is someone else's player. We need to receive their position (as of a few
            // millisecond ago, and update our version of that player.

            loudness = (float)stream.ReceiveNext();
            loudness = (float)stream.ReceiveNext();
        }

    }
}
