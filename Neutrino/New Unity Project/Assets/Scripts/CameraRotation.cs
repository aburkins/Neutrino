using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.z > 0) {
			this.transform.RotateAround(this.transform.position, Vector3.forward, 20 * Time.deltaTime);
			this.transform.position = this.transform.position - Vector3.forward * Time.deltaTime;
		}


	}
}
