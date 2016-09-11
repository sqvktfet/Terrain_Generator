using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	// for position calculation
	public float speed = 50.0f;

	// for rotation calculation
	public float sensitivity = 5.0f;
	public float smoothing = 2.0f;
	private Vector2 mouseLook; // how much total movement the mouse has made
	private Vector2 smoothV;


	public Transform terrain;
	private TerrainScript terrainScript;

	public Text playerPositionText;
	public Text mouseLookText;

	private int size;
	private int scale;
	private float tall;

	private Vector3 maxMapBound;
	private Vector3 minMapBound;
	private int maxPosition;
	private int minPosition;



	private Vector3 targetEuler = new Vector3 (0, 0, 0);
	private Vector3 currEuler = new Vector3 (0, 0, 0);
	private float rollingSpeed = 5.0f;
	private float rollingAngle = 5.0f;


	GameObject gameCamera;	// controlling the camera for rotating purpose

	// Use this for initialization
	void Start () {
		// cursor is not going to be displayed in the game view
		Cursor.lockState = CursorLockMode.Locked;
		gameCamera = this.transform.GetChild(0).gameObject;	// which is the camera object

//		var targetScript: ScriptName = targetObj.GetComponent(ScriptName);
		terrainScript = terrain.GetComponent<TerrainScript>();


		size = terrainScript.GetSize ();
		scale = terrainScript.GetScale ();
		Debug.Log("size: "+size+"scale: " + scale);
		transform.position = new Vector3(0f, terrainScript.GetHeight(0, 0) + 50, 0f); // player start position
		Debug.Log("Start Position: ("+ transform.position.x +","+ transform.position.y +","+transform.position.x +")");

		minPosition = 0;
		maxPosition = size*scale;

		Debug.Log ("minPosition:" + minPosition);
		Debug.Log ("maxPosition:" + maxPosition);

		minMapBound = new Vector3(0, terrainScript.GetHeight(0, 0), 0);
		maxMapBound = new Vector3 (size-1, terrainScript.GetHeight(size-1, size-1), size-1);
		Debug.Log("minMapBound: ("+ minMapBound.x +","+ minMapBound.y +","+minMapBound.z +")");
		Debug.Log("maxMapBound: ("+ maxMapBound.x +","+ maxMapBound.y +","+maxMapBound.z +")");



	}
	
	// Update is called once per frame
	void Update () {


		PositionUpdate ();
		MouseViewRotationUpdate ();
		CameraRollingUpdate ();


		if(Input.GetKeyDown(KeyCode.Escape)) {
			// when escape pressed, show the cursor in the game view
			Cursor.lockState = CursorLockMode.None;
//			Debug.Log("escape pressed");
		}
	}


	void PositionUpdate() {
				
		int forwards = 0; // when W pressed, it is 1, when S pressed, it is -1
		float translation = 0;
		float height = 0;
		float straffe = 0;


		if(Input.GetKey(KeyCode.W)) {
			translation += speed * Time.deltaTime;
			forwards = 1;
		}
		if(Input.GetKey(KeyCode.S)) {
			translation -= speed * Time.deltaTime;
			forwards = -1;
		}
		if(Input.GetKey(KeyCode.A)) {
			straffe -= speed * Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.D)) {
			straffe += speed * Time.deltaTime;
		}
			
		height = Mathf.Sqrt(translation*translation+straffe*straffe) * Mathf.Sin (Mathf.Deg2Rad * mouseLook.y) * forwards;

		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.D)) {
//			Debug.Log ("-mouseLook.y: " + -mouseLook.y);
//			Debug.Log ("Sin: " + Mathf.Sin (Mathf.Deg2Rad * mouseLook.y));
//			Debug.Log ("Sqrt: " + Mathf.Sqrt (translation * translation + straffe * straffe));
//			Debug.Log ("Height: " + height);
		} else {
			// 
			this.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		}




		transform.Translate (straffe, height, translation); 

		if (transform.position.x > maxPosition) {
			transform.position = new Vector3(maxPosition, transform.position.y, transform.position.z);
			Debug.Log ("x too large: " + maxPosition);
		} else if (transform.position.x < 0) {
			transform.position = new Vector3(0, transform.position.y, transform.position.z);
			Debug.Log ("x too small: " + 0);
		}
		if (transform.position.z > maxPosition) {
			transform.position = new Vector3(transform.position.x, transform.position.y, maxPosition);
			Debug.Log ("z too large: " + maxPosition);
		} else if (transform.position.z < 0) {
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
			Debug.Log ("z too small: " + 0);
		}



		playerPositionText.text = "Translation: " + translation + "\nHeight: " + height + "\nstraffe: " + straffe +
			"\nPosition: (" + transform.position.x + ", " + transform.position.y +", " +transform.position.z + ")";


	}


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

	void CameraRollingUpdate() {
		
		
		if (Input.GetKey (KeyCode.Q)) {
			Debug.Log ("Q pressed");
//			gameCamera.transform.Rotate (Vector3.forward * speed * Time.deltaTime);
//			gameCamera.transform.Rotate(0,0,speed*Time.deltaTime);
//			gameCamera.transform.localEulerAngles = new Vector3(0, 0, rollingSpeed*Time.deltaTime);
			targetEuler.z += rollingAngle;

		}
		if (Input.GetKey (KeyCode.E)) {
			Debug.Log ("E pressed");
//			gameCamera.transform.Rotate (-Vector3.forward * speed * Time.deltaTime);
//			gameCamera.transform.localEulerAngles = new Vector3(0, 0, -rollingSpeed*Time.deltaTime);
			targetEuler.z -= rollingAngle;

		}

		currEuler = Vector3.Lerp (currEuler, targetEuler, Time.deltaTime * rollingSpeed);
		Debug.Log ("currEuler(" + currEuler.x + "," + currEuler.y + "," + currEuler.z + ")");
		gameCamera.transform.localEulerAngles =  currEuler;

	}

//	void OnCollisionEnter () {
//		this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
//		
//	}
}


