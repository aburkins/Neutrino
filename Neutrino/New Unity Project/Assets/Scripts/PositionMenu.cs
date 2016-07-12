using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
public class PositionMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject camera = GameObject.Find ("CameraStereo0");
		this.transform.parent = camera.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
