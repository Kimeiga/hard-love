using UnityEngine;
using System.Collections;

public class SpeedCalculator : MonoBehaviour {
    
    
    public float measuredSpeed;
    public Vector3 measured3DSpeed;
    private Vector3 lastPosition = Vector3.zero;
    public float clamp = 0.16f;
    

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate() {

        measured3DSpeed = (transform.position - lastPosition) / Time.deltaTime;

        

        measuredSpeed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;

        measuredSpeed *= Time.deltaTime;

        //measuredSpeed = Mathf.Clamp(measuredSpeed, 0, clamp);
        
    }
    
}
