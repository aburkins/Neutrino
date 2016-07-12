using UnityEngine;
using System.Collections;

public class FlyThrough : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    //simple flythrough script adapted from Slin under https://creativecommons.org/licenses/by-sa/3.0/ 
    float lookSpeed = 10.0f;
    float moveSpeed = .05f;

    float rotationX = 0.0f;
    float rotationY = 0.0f;
    void Update () {
        rotationX += Input.GetAxis("Mouse X") * lookSpeed;
        rotationY += Input.GetAxis("Mouse Y") * lookSpeed;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical");
        transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal");
    }
}
