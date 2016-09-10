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

	private TerrainScript terrainScript;

//	public Text playerPositionText;
//	public Text mouseLookText;

	private int size;
	private int scale;
	private float tall;

//	private Vector3 maxMapBound;
//	private Vector3 minMapBound;
//	private float maxPositionX = 0;
//	private float minPositionX = 0;
//	private	float maxPositionZ = 100;
//	private float minPositionZ = 100;


	GameObject gameCamera;	// controlling the camera for rotating purpose

	// Use this for initialization
	void Start () {
		// cursor is not going to be displayed in the game view
		Cursor.lockState = CursorLockMode.Locked;
		gameCamera = this.transform.GetChild(0).gameObject;	// which is the camera object

//		terrainScript = GetComponent<TerrainScript>().gameObject;

		size = terrainScript.GetSize ();
		scale = terrainScript.GetScale ();
		tall = terrainScript.GetHeight(0, 0);
		transform.position = new Vector3(0f, tall, 0f); // player start position
//		Debug.Log("Start Position: ("+ transform.position.x +","+ transform.position.y +","+transform.position.x +")");

//		minMapBound = new Vector3(minPositionX, 0, minPositionZ);
//		maxMapBound = new Vector3 (maxPositionX, 0, maxPositionZ);
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

//		if (transform.position.x >= maxPositionX) {
//			transform.position.x = maxMapBound.x;
//		} else if (transform.position.x <= minPositionX) {
//			transform.position.x = minMapBound.x;
//		}
//		if (transform.position.z >= maxPositionZ) {
//			transform.position.z = maxMapBound.x;
//		} else if (transform.position.z <= maxPositionZ) {
//			transform.position.z = minMapBound.x;
//		}



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


