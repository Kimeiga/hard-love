using UnityEngine; using System.Collections;

/// MouseLook rotates the transform based on the mouse delta. /// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character: /// - Create a capsule. /// - Add a rigid body to the capsule /// - Add the MouseLook script to the capsule. /// -> Set the mouse look to use LookX. (You want to only turn character but not tilt it) /// - Add FPSWalker script to the capsule

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform. /// - Add a MouseLook script to the camera. /// -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)

[AddComponentMenu("Camera-Control/Mouse Look")] public class MouseLook : MonoBehaviour {

    public PhotonView photonView;

    public bool canRotate;

public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
public RotationAxes axes = RotationAxes.MouseXAndY;
public float sensitivityX = 2F;
public float sensitivityY = 2F;

public float minimumX = -360F;
public float maximumX = 360F;

public float minimumY = -90F;
public float maximumY = 90F;

public float rotationX = 0F;
public float rotationY = 0F;

    public float rotationXPerSecond;
    public float rotationYPerSecond;

    private float lastRotationX = 0;
    private float lastRotationY = 0;

Quaternion originalRotation;


void FixedUpdate ()
{
        

    if (canRotate) {

            if (photonView.isMine)
            {

                sensitivityX = (float)PhotonNetwork.player.customProperties["Sen"];
                sensitivityY = sensitivityX;
            }
            else
            {
                for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
                {
                    if (PhotonNetwork.playerList[i].name == photonView.owner.name)
                    {
                        sensitivityX = (float)PhotonNetwork.playerList[i].customProperties["Sen"];
                        sensitivityY = sensitivityX;
                    }
                }
                
            }

            if (axes == RotationAxes.MouseXAndY)
            {
                if (photonView.isMine)
                {

                    // Read the mouse input axis
                    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                }


                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

                transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            }
            else if (axes == RotationAxes.MouseX)
            {

                if (photonView.isMine)
                {

                    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                }
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation = originalRotation * xQuaternion;
            }
            else
            {
                if (photonView.isMine)
                {

                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                }

                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
                transform.localRotation = originalRotation * yQuaternion;
            }

            rotationXPerSecond = (rotationX - lastRotationX) / Time.deltaTime;
            rotationYPerSecond = (rotationY - lastRotationY) / Time.deltaTime;

            lastRotationX = rotationX;
            lastRotationY = rotationY;

        }
	
}

void Start ()
{
	// Make the rigid body not change rotation
	if (GetComponent<Rigidbody>())
		GetComponent<Rigidbody>().freezeRotation = true;
	originalRotation = transform.localRotation;

        canRotate = true;

        


    }

public static float ClampAngle (float angle, float min, float max)
{
	if (angle < -360F){
			angle += 360F;
		}
	if (angle > 360F){
		angle -= 360F;
		}
	return Mathf.Clamp (angle, min, max);
}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            if (canRotate)
            {
                stream.SendNext(Input.GetAxis("Mouse X"));
                stream.SendNext(Input.GetAxis("Mouse Y"));
            }
        }
        else
        {


            rotationX += (float)stream.ReceiveNext() * sensitivityX;
            rotationY += (float)stream.ReceiveNext() * sensitivityY;


        }

    }

}