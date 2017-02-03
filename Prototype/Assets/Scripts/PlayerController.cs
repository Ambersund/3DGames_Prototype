using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Camera cam;

	Rigidbody rb;

	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	Vector3 gravityDirection = Vector3.up;
	Vector3 tetherPoint;

	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float playerSpeed;
	public float jumpForce;
	public float gravity = -9.8f;

	public LayerMask groundedMask;

	float verticalLookRotation;

	public bool grounded;
	public bool isTethered = false;
	public bool hasJumped = false;
	public bool canLeap = false;

	void Awake(){

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		rb = this.gameObject.GetComponent<Rigidbody> ();
		rb.useGravity = false;
		rb.constraints = RigidbodyConstraints.FreezeRotation;
		
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

		//INPUTS
		if(Input.GetMouseButton (1)){
			Debug.Log ("Click");
			Application.CaptureScreenshot ("jjowers_prototype.png");
		}
		if (Input.GetMouseButton (0)) {
			ChangeGravity ();
		}
		if (Input.GetButtonDown ("Jump") && grounded && !hasJumped && !canLeap) {
			rb.AddForce (transform.up * jumpForce);
		}
		if (Input.GetButtonDown ("Jump") && !grounded && hasJumped && canLeap) {
			rb.AddForce (cam.transform.forward * jumpForce);
			canLeap = false;
		}

		// Look rotation for mouse
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-85,85);
		cam.transform.localEulerAngles = Vector3.left * verticalLookRotation;

		// Calculate movement:
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");

		Vector3 moveDir = new Vector3(inputX,0, inputY).normalized;
		Vector3 targetMoveAmount = moveDir * playerSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount,targetMoveAmount,ref smoothMoveVelocity,.15f);

		//GROUNDED CHECK
		Ray groundRay = new Ray(transform.position, -transform.up);
		RaycastHit groundHit;

		if (Physics.Raycast (groundRay, out groundHit, 1, groundedMask)) {
			grounded = true;
			hasJumped = false;
			canLeap = false;

			if(groundHit.transform.tag == "Wall"){
				gravityDirection = groundHit.normal;
			}
		}else{
			grounded = false;
			canLeap = true;
			hasJumped = true; 
		}

		//LEAP CHECK
		if(!grounded){

			Ray leapRay = new Ray (cam.transform.position, cam.transform.forward);
			RaycastHit leapHit;

			if (Physics.Raycast (leapRay, out leapHit, .6f, groundedMask)) {
				switch (leapHit.transform.tag) {
				case("Sphere"):
					isTethered = true;
					tetherPoint = leapHit.transform.Find ("TetherPoint").transform.position;
					break;
				case("Wall"):
					isTethered = false;
					gravityDirection = leapHit.normal;
					break;
				case("Plane"):
					isTethered = false;
					gravityDirection = leapHit.normal;
					break;
				default:
					break;
				}
			}
		}
	}

	void FixedUpdate(){

		//Gravity
		ExertGravity();

		//Movement
		Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
		rb.MovePosition(rb.position + localMove);
	}

	void ChangeGravity(){

		Ray checkRay = new Ray (cam.transform.position, cam.transform.forward);
		RaycastHit checkHit;

		if (Physics.Raycast (checkRay, out checkHit, 100f, groundedMask)) {

			switch (checkHit.transform.tag) {
			case("Sphere"):
				isTethered = true;
				tetherPoint = checkHit.transform.Find ("TetherPoint").transform.position;
				break;
			case("Wall"):
				isTethered = false;
				gravityDirection = checkHit.normal;
				break;
			case("Plane"):
				isTethered = false;
				gravityDirection = checkHit.normal;
				break;
			default:
				break;
			}
		}
		
	}

	void ExertGravity(){
		Debug.Log (gravityDirection);
		Vector3 localUp = rb.transform.up;

		//if we are tethered to a sphere
		if (isTethered) {
			//sphere gravity
			gravityDirection = (rb.position - tetherPoint).normalized;
		}

		rb.AddForce (gravityDirection * gravity);
		rb.rotation = Quaternion.FromToRotation (localUp, gravityDirection) * rb.rotation;
	}
}
