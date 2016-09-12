using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	// for position calculation
	public float movingSpeed = 50.0f;

	// for rotation calculation
	public float sensitivity = 5.0f;
	public float smoothing = 2.0f;
	private Vector2 mouseLook; // how much total movement the mouse has made
	private Vector2 smoothV;

	// for Q/E rolling control
	private float rollingSpeed = 2.0f;
	private float rollingAngle = 2.0f;
	private Vector3 targetEuler = new Vector3 (0, 0, 0);
	private Vector3 currEuler = new Vector3 (0, 0, 0);

	// for terrain mesh generating
	public Transform terrain;
	private TerrainScript terrainScript;

	// for text display
	public Text playerPositionText;
	public Text mouseLookText;

	// property of terrain
	private int size;
	private int scale;

	// the upperbond and the lowerbond position value of the terrain
	private int maxPosition;
	private int minPosition;

	// the upperbond and the lowerbond position value of the player
	private int maxBound;
	private int minBound;

	// the start position of the player
	private int playerStartPositionX = 300;	// x coordinator of the player start position
	private int playerStartPositionZ = 300; // y coordinator of the player start position
	private float higher = 100.0f;	// how much the player is higher than the terrain at the start point

	// how far the player is allowed to reach the terrain border
	private int errorValue = 50; // prevent the player from being out of the terrain

	private GameObject gameCamera;	// controlling the camera for rotating purpose

	// Use this for initialization
	void Start () {

		// cursor is not going to be displayed in the game view
		Cursor.lockState = CursorLockMode.Locked;

		// get the camera object which is the child of player
		gameCamera = this.transform.GetChild(0).gameObject;	// which is the camera object

		// get the terrain size and scale value
		terrainScript = terrain.GetComponent<TerrainScript>();
		size = terrainScript.GetSize ();
		scale = terrainScript.GetScale ();

		minPosition = 0;
		maxPosition = size*scale;

		maxBound = maxPosition - errorValue;
		minBound = minPosition + errorValue;

		// put the player to the start position
		transform.position = new Vector3(playerStartPositionX, 
			terrainScript.GetHeight(playerStartPositionX / scale, playerStartPositionZ / scale) + higher, playerStartPositionZ);
	}

	// Update is called once per frame
	void Update () {

		PositionUpdate ();
		MouseViewRotationUpdate ();
		CameraRollingUpdate ();

		// when escape button pressed, show the cursor in the game view
		if(Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
		}
	}


	/**
	 * player's movement control
	 */
	void PositionUpdate() {

		int forwards = 0; // when W pressed, it is 1, when S pressed, it is -1
		float translation = 0;	// movement in z axis
		float height = 0;	// movement in y axis
		float straffe = 0;	// movement in x axis

		// record the previousPosition
//		Vector3 previousPosition = transform.position;

		// get the input of W,S,A,D button and calculate the movement 
		if(Input.GetKey(KeyCode.W)) {
			translation += movingSpeed * Time.deltaTime;
			forwards = 1;
		}
		if(Input.GetKey(KeyCode.S)) {
			translation -= movingSpeed * Time.deltaTime;
			forwards = -1;
		}
		if(Input.GetKey(KeyCode.A)) {
			straffe -= movingSpeed * Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.D)) {
			straffe += movingSpeed * Time.deltaTime;
		}

		height = Mathf.Sqrt(translation*translation+straffe*straffe) * Mathf.Sin (Mathf.Deg2Rad * mouseLook.y) * forwards;

		// when no movement control key pressed, avoid to change player's position
		if (!(Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.D))) {
			this.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		}

		// do the movement translation based on the key pressed
		transform.Translate (straffe, height, translation); 

		// if the player is out of bound, set the position to previous position 
		float tempX, tempZ;
		if (isXOutOfBound () == 1) {
			tempX = maxBound;
		} else if (isXOutOfBound () == 2) {
			tempX = minBound;
		} else {
			tempX = transform.position.x;
		}

		if (isZOutOfBound () == 1) {
			tempZ = maxBound;
		} else if (isZOutOfBound () == 2) {
			tempZ = minBound;
		} else {
			tempZ = transform.position.z;
		}
		transform.position = new Vector3 (tempX, transform.position.y, tempZ);

		// display the player's current position on the screen
		playerPositionText.text = "Position: (" + transform.position.x + ", " + transform.position.y +", " +transform.position.z + ")";

	}


	/**
	 * player's mouse rotation control
	 */
	void MouseViewRotationUpdate () {
		Vector2 mouseDelta;	// for changing smoothily
		mouseDelta = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
		mouseDelta = Vector2.Scale (mouseDelta, new Vector2 (sensitivity * smoothing, sensitivity * smoothing));
		// Mathf.Lerp(): Linearly interpolates between smoothV and mouseDelta by "1.0f/smoothing".
		// move from one to the other smoothly
		smoothV.x = Mathf.Lerp (smoothV.x, mouseDelta.x, 1.0f / smoothing);	
		smoothV.y = Mathf.Lerp (smoothV.y, mouseDelta.y, 1.0f / smoothing);

		mouseLook += smoothV;
		mouseLook.y = Mathf.Clamp (mouseLook.y, -90f, 90f);	// prevent the view from over-rotation

		// -mouseLook.y converts the up/down system to the inverted system
		gameCamera.transform.localRotation = Quaternion.AngleAxis (-mouseLook.y, Vector3.right);

		// rotate the character rather than only the camera itself
		transform.localRotation = Quaternion.AngleAxis (mouseLook.x, transform.up);
		mouseLookText.text = "\n\n\n\nMouseLook.x: " + mouseLook.x;
		mouseLookText.text += ("\nMouseLook.y: " + mouseLook.y);


		if (mouseLook.x >= 360 || mouseLook.x <= -360) {
			mouseLook.x = 0;
		}
	}
		

	/**
	 * player's view rolling control via Q E key press
	 */
	void CameraRollingUpdate() {

		if (Input.GetKey (KeyCode.Q)) {
			targetEuler.z += rollingAngle;
		}
		if (Input.GetKey (KeyCode.E)) {
			targetEuler.z -= rollingAngle;
		}

		currEuler = Vector3.Lerp (currEuler, targetEuler, Time.deltaTime * rollingSpeed);
		gameCamera.transform.localRotation = Quaternion.AngleAxis (currEuler.z, Vector3.forward) * gameCamera.transform.localRotation;

	}



	/**
	 * check whether the player's current X axis position is out of bound
	 * if so, return true, otherwise return false
	 * return 0 : not out of bound
	 * return 1 : excceed maximum pos
	 * return 2 : below minimum pos
	 */
	private int isXOutOfBound(){
		if (transform.position.x > (maxPosition - errorValue)) {
			return 1;
		} else if (transform.position.x < minPosition + errorValue) {
			return 2;
		}
		return 0;
	}

	/**
	 * check whether the player's current Z axis position is out of bound
	 * if so, return true, otherwise return false
	 * return 0 : not out of bound
	 * return 1 : excceed maximum pos
	 * return 2 : below minimum pos
	 */
	private int isZOutOfBound(){
		if (transform.position.z > (maxPosition - errorValue)) {
			return 1;
		} else if (transform.position.z < minPosition + errorValue) {
			return 2;
		}
		return 0;
	}

}


