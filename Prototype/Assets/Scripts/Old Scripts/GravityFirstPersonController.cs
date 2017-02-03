using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFirstPersonController : MonoBehaviour {

	public Camera cam;

	ConstantForce cf;

	public Rigidbody rb;

	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	Vector3 gravityDir = Vector3.down;

	public LayerMask groundedMask;

	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float playerSpeed;
	public float jumpForce;
	public float gravity = -9.8f;

	float verticalLookRotation;

	bool grounded;

	void awake(){
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		cf = GetComponent<ConstantForce> ();
		rb = this.gameObject.GetComponent<Rigidbody> ();
		rb.useGravity = false;
		rb.constraints = RigidbodyConstraints.FreezeRotation;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		// Look rotation for mouse
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-60,60);
		cam.transform.localEulerAngles = Vector3.left * verticalLookRotation;

		// Calculate movement:
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");

		Vector3 moveDir = new Vector3(inputX,0, inputY).normalized;
		Vector3 targetMoveAmount = moveDir * playerSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount,targetMoveAmount,ref smoothMoveVelocity,.15f);

		//Grounded Check
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask)) {
			grounded = true;
		}
		else {
			grounded = false;
		}

		if (Input.GetMouseButton (0)) {
			FindGravity ();
		}
	}

	void FixedUpdate(){
		
		ExertGravity (rb);

		// Apply movement to rigidbody
		Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
		rb.MovePosition(rb.position + localMove);
	}

	void FindGravity(){

		Ray gravityRay = new Ray(transform.position, transform.forward);
		RaycastHit gHit;

		if (Physics.Raycast (gravityRay, out gHit, 100f, groundedMask)) {
			switch (gHit.transform.tag) {
			case("Other"):
				gravityDir = gHit.normal;
				break;
			}
		}
	}

	void ExertGravity (Rigidbody _body){

		Vector3 localUp = _body.transform.up;

		_body.AddForce (gravityDir * gravity);
		_body.rotation = Quaternion.FromToRotation(localUp,gravityDir) * _body.rotation;

	}
}
