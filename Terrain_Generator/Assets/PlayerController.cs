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

//	public Text playerPositionText;
//	public Text mouseLookText;

	private int size;
	private int scale;
	private float tall;

	private Vector3 maxMapBound;
	private Vector3 minMapBound;
	private int maxPosition;
	private int minPosition;


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
		transform.position = new Vector3(0f, terrainScript.GetHeight(0, 0) + 50, 0f); // player start position
		Debug.Log("Start Position: ("+ transform.position.x +","+ transform.position.y +","+transform.position.x +")");

		minPosition = 0;
		maxPosition = size - 1;

		Debug.Log ("minPosition:" + minPosition);
		Debug.Log ("maxPosition:" + maxPosition);

		minMapBound = new Vector3(minPosition, terrainScript.GetHeight(minPosition, minPosition), minPosition);
		maxMapBound = new Vector3 (maxPosition, terrainScript.GetHeight(maxPosition, maxPosition), maxPosition);
		Debug.Log("minMapBound: ("+ minMapBound.x +","+ minMapBound.y +","+minMapBound.z +")");
		Debug.Log("maxMapBound: ("+ maxMapBound.x +","+ maxMapBound.y +","+maxMapBound.z +")");
	}
	
	// Update is called once per frame
	void Update () {


		PositionUpdate ();
		RotationUpdate ();


		if(Input.GetKeyDown(KeyCode.Escape)) {
			// when escape pressed, show the cursor in the game view
			Cursor.lockState = CursorLockMode.None;
			Debug.Log("escape pressed");
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

		if (transform.position.x > maxMapBound.x) {
			transform.position = new Vector3(maxMapBound.x, transform.position.y, transform.position.z);
			Debug.Log ("x too large: " + transform.position.x);
		} else if (transform.position.x < minMapBound.x) {
			transform.position = new Vector3(minMapBound.x, transform.position.y, transform.position.z);
			Debug.Log ("x too small: " + transform.position.x);
		}
		if (transform.position.z > maxMapBound.z) {
			transform.position = new Vector3(transform.position.x, transform.position.y, maxMapBound.z);
			Debug.Log ("z too large: " + transform.position.z);
		} else if (transform.position.z < minMapBound.z) {
			transform.position = new Vector3(transform.position.x, transform.position.y, minMapBound.z);
			Debug.Log ("z too small: " + transform.position.z);
		}



//		playerPositionText.text = "Translation: " + translation + "\nHeight: " + height + "\nstraffe: " + straffe +
//			"\nPosition: (" + transform.position.x + ", " + transform.position.y +", " +transform.position.z + ")";


	}


	void RotationUpdate () {
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
//		mouseLookText.text = "\n\n\n\nMouseLook.x: " + mouseLook.x;
//		mouseLookText.text += ("\nMouseLook.y: " + mouseLook.y);


		if (mouseLook.x >= 360 || mouseLook.x <= -360) {
			mouseLook.x = 0;
		}

//		Debug.Log ("Velocity" + this.gameObject.GetComponent<Rigidbody> ().velocity);

	}

//	void OnCollisionEnter () {
//		this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
//		
//	}
}


