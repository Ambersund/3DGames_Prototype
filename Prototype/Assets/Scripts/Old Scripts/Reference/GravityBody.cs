using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class GravityBody : MonoBehaviour {
	
	Rigidbody rb;
	Vector3 currentDir;
	public LayerMask player;
	public Camera cam;

	public float gravity = 9.8f;
	
	void Awake () {
		rb = GetComponent<Rigidbody> ();

		// Disable rigidbody gravity and rotation as this is simulated in GravityAttractor script
		currentDir = Vector3.down;

		rb.useGravity = false;
		rb.constraints = RigidbodyConstraints.FreezeRotation;
	}

	void Update(){

		//this is where we check input for changing the gravity
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = new Ray (cam.transform.position, Vector3.forward);

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, player)) {

				currentDir = hit.normal;
			
			}
			Debug.Log ("Change Gravity: " + currentDir);
			Debug.Log ("Hit Name: " + hit.transform.gameObject.name);

		}
		Debug.DrawRay (cam.transform.position, Vector3.forward);
	}

	void FixedUpdate () {
		// Allow this body to be influenced by planet's gravity
		Attract (currentDir);
	}

	public void Attract(Vector3 dir){
		Vector3 localDown = rb.transform.up * -1;

		//exert force on the main character
		rb.AddForce (dir * gravity);
		rb.rotation = Quaternion.FromToRotation (localDown, dir) * rb.rotation;
	}
}