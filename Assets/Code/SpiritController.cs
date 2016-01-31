using UnityEngine;
using System.Collections;

public class SpiritController : MonoBehaviour {

    public static bool isEnchanting;
    public float enchantDuration = 3f;
    private float endEnchantTime;

	public Vector3 modelLookDirection;

	public float cameraSensitivity = 100f;
	public float desiredCameraDistance = 60f;
	public float cameraAvoidDistance = 8f;
	public float maxUpAngle = 30f;
	public float maxDownAngle = 60f;
	public Transform cameraBoostLock;
	public float cameraLockTime = 0.1f;

	public Vector3 floatingGravity = new Vector3 (0f, -150f, 0f);
	public Vector3 walkingGravity = new Vector3 (0f, -240f, 0f);
	public float horizWalkSensitivity = 40000f;
	public float vertMoveSensitivity = 20000f;
	public float accelForce = 1000f;
	public float horizFlySensitivity = 10000f;
	public float flyForce = 500f;
	public float standingDistToGround = 20f;
	public float jumpForce = 5000f;
	public float floatingDrag = 1;
	public float walkingDrag = 5;

	public GameObject playerRotator;

	public bool isFloating {
		get { return m_floating; }
		set {
			if (value) {
				rb.drag = floatingDrag;
			} else {
				rb.drag = walkingDrag;
				playerRotator.transform.up = Vector3.up;//TODO: keep rotation around y
			}
			//playerAnimator.SetBool ("isFloating", value);
			m_floating = value;
		}
	}
	private bool m_floating;
	private Vector3 lockCamVel = Vector3.zero;

	private Transform desiredCamTrans;
	private Camera playerCam;
	private Animator playerAnimator;
	private Rigidbody rb;

    public bool IsEnchanting
    {
        get { return isEnchanting; }
    }

	void Start () {
		playerCam = gameObject.GetComponentInChildren<Camera> ();
		playerAnimator = gameObject.GetComponentInChildren<Animator> ();
		rb = gameObject.GetComponentInChildren<Rigidbody> ();
		desiredCamTrans = Instantiate<GameObject> (new GameObject("DesiredCamPos")).transform;
		desiredCamTrans.parent = this.transform;
		desiredCamTrans.position = playerCam.transform.position;
		desiredCamTrans.rotation = playerCam.transform.rotation;
		isFloating = true;
	}
	
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetButtonDown("enchant"))
        {
            isEnchanting = true;
            endEnchantTime = Time.timeSinceLevelLoad + enchantDuration;
        }
        else if (Time.timeSinceLevelLoad >= endEnchantTime) {
            isEnchanting = false;
        }

		float cameraX = Input.GetAxis ("cameraX");
		float cameraY = Input.GetAxis ("cameraY");

		if (isFloating && Input.GetAxis ("accel") > 0f) {

			Debug.Log (desiredCamTrans.position+", "+cameraBoostLock.position+"\n"+desiredCamTrans.rotation+", "+cameraBoostLock.rotation);

			float moveSide = Input.GetAxis ("moveSide");
			float moveUp = Input.GetAxis ("moveUp");

			//cameraX = -cameraX;

			float rotX = Mathf.Abs (cameraX) > Mathf.Abs (moveSide) ? cameraX : moveSide;
			float rotY = Mathf.Abs (cameraY) > Mathf.Abs (moveUp) ? cameraY : moveUp;

			float horizDegrees = Time.deltaTime * cameraSensitivity * rotX;
			float vertDegrees = Time.deltaTime * cameraSensitivity * rotY;
			float angleToUp = Vector3.Angle (playerRotator.transform.forward, Vector3.down);
			float angleToDown = angleToUp - 180f;

			if (vertDegrees > angleToUp - maxUpAngle) {
				vertDegrees = angleToUp - maxUpAngle;
			} else if (vertDegrees < angleToDown + maxDownAngle) {
				vertDegrees = angleToDown + maxDownAngle;
			}

			Debug.Log (horizDegrees+", "+vertDegrees);

			playerRotator.transform.RotateAround (playerRotator.transform.position, Vector3.up, horizDegrees);
			playerRotator.transform.RotateAround (playerRotator.transform.position, playerRotator.transform.right, vertDegrees);

			desiredCamTrans.position = cameraBoostLock.position;
			desiredCamTrans.rotation = cameraBoostLock.rotation;

			modelLookDirection = playerRotator.transform.forward;

			Debug.Log (desiredCamTrans.position+", "+cameraBoostLock.position+"\n"+desiredCamTrans.rotation+", "+cameraBoostLock.rotation);

		} else {
			
			// Rotate Camera
			
			float horizDegrees = Time.deltaTime * cameraSensitivity * cameraX;
			float vertDegrees = Time.deltaTime * cameraSensitivity * cameraY;
			float angleToUp = Vector3.Angle (desiredCamTrans.forward, Vector3.down);
			float angleToDown = angleToUp - 180f;
			
			if (vertDegrees > angleToUp - maxUpAngle) {
				vertDegrees = angleToUp - maxUpAngle;
			} else if (vertDegrees < angleToDown + maxDownAngle) {
				vertDegrees = angleToDown + maxDownAngle;
			}
			
			desiredCamTrans.RotateAround (this.transform.position, Vector3.up, horizDegrees);
			desiredCamTrans.RotateAround (this.transform.position, desiredCamTrans.right, vertDegrees);
			
			// Move Camera in/out
			
			Ray rayToCam = new Ray (this.transform.position, desiredCamTrans.position - this.transform.position);
			
			RaycastHit hit;
			if (Physics.Raycast (rayToCam, out hit, desiredCameraDistance + cameraAvoidDistance)) {
				desiredCamTrans.position = rayToCam.GetPoint (hit.distance - cameraAvoidDistance);
			} else {
				desiredCamTrans.position = rayToCam.GetPoint (desiredCameraDistance);
			}
		}

		playerCam.transform.position =
				Vector3.SmoothDamp (playerCam.transform.position, desiredCamTrans.position, ref lockCamVel, cameraLockTime);
		playerCam.transform.LookAt (this.transform.position);

		if (modelLookDirection.sqrMagnitude > 0.0001f) {
			playerRotator.transform.rotation = Quaternion.LookRotation (modelLookDirection);
		}
	}

	void FixedUpdate () {

		float moveSide = Input.GetAxis ("moveSide");
		float moveUp = Input.GetAxis ("moveUp");
		float accel = Input.GetAxis ("accel");

		bool onGround = Physics.Raycast (this.transform.position, Vector3.down, standingDistToGround);
		if (isFloating) {
			if (accel == 0f && rb.velocity.y <= 0f && onGround) {
				isFloating = false;
			}
		} /*else*/ if (Input.GetButtonDown ("jump")) {
//			if (onGround)
				rb.AddForce (Vector3.up * jumpForce);
			isFloating = true;
		}

		// Move
		if (isFloating) {

			if (accel == 0f) {

				rb.AddForce (floatingGravity);

				Vector3 camRelForward = playerCam.transform.forward;
				if (Mathf.Abs (camRelForward.x) <= 0.0001f && Mathf.Abs (camRelForward.z) <= 0.0001f) {
					camRelForward = playerCam.transform.up;
				}
				camRelForward.y = 0f;
				
				float forceMultiplier = Time.fixedDeltaTime * horizFlySensitivity;
				Vector3 force = Quaternion.LookRotation (camRelForward) *
					new Vector3 (forceMultiplier * moveSide, 0f, forceMultiplier * moveUp);

				modelLookDirection = force;
				
				rb.AddForce (force);

			} else {

				rb.AddForce (playerRotator.transform.forward * (flyForce * accel));

			}

		} else {

			rb.AddForce (walkingGravity);

			Vector3 camRelForward = playerCam.transform.forward;
			if (Mathf.Abs (camRelForward.x) <= 0.0001f && Mathf.Abs (camRelForward.z) <= 0.0001f) {
				camRelForward = playerCam.transform.up;
			}
			camRelForward.y = 0f;
			
			float forceMultiplier = Time.fixedDeltaTime * horizWalkSensitivity + (accel * accelForce);
			Vector3 force = Quaternion.LookRotation (camRelForward) *
				new Vector3 (forceMultiplier * moveSide, 0f, forceMultiplier * moveUp);

			modelLookDirection = force;

			rb.AddForce (force);
		}
	}
}
