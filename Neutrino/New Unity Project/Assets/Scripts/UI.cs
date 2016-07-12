using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
public class UI : MonoBehaviour {
	public GameObject tablet;
	public GameObject tabletball;
	public GameObject tNode;
	vrTracker tracker = null;
	// Use this for initialization
	void Start () {
		tNode = GameObject.Find ("TabletNode");
	
		tabletball.transform.position = tNode.transform.position + 2*tNode.transform.forward;
		tabletball.transform.parent = tNode.transform;
		tablet.GetComponent<Renderer>().enabled = false;
	
	}
	
	// Update is called once per frame
	void Update () {
		if(MiddleVR.VRDeviceMgr != null){
			tracker = MiddleVR.VRDeviceMgr.GetTracker("RazerHydra.Tracker0");
		}
		float pitch = tracker.GetPitch ();
		print (pitch);
		if (pitch >60) {
						tablet.GetComponent<Renderer>().enabled = true;
				} else {
						tablet.GetComponent<Renderer>().enabled = false;
				}
	}
}
