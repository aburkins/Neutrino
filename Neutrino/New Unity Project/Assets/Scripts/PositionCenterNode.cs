using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class PositionCenterNode : MonoBehaviour {
	GameObject center;
	// Use this for initialization
	void Start () {
		center = GameObject.Find ("VRSystemCenterNode");
		center.transform.position = new Vector3 (0, 0, 20);
		center.transform.Rotate (new Vector3 (270, 0, 180));
		GameObject camera0 = GameObject.Find ("CameraStereo0");
		Camera camera = (Camera) camera0.GetComponent(typeof(Camera));
		camera.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
