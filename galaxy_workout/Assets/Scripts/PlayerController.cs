using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Boundary {
	
	public float Xmin, Xmax, Zmin, Zmax;
}

public class PlayerController : MonoBehaviour {

	static private Rigidbody rb;
	static private AudioSource shot_sound;
	public float speed;
	public float tilt;
	public Boundary boundary; 

	public GameObject shot;
	public Transform shotSpawn;

	public float fireRate;
	private float nextFire;

	private SerialController serialController;
	private GameController gameController;

	private float fire;
	private float moveHorizontal;
	private float moveVertical;
	private float[] playerControls;

	private int gameState;

	void Start() {
		rb = GetComponent<Rigidbody> ();
		shot_sound = GetComponent<AudioSource> ();

		fire = 0;
		moveVertical = 0;
		moveHorizontal = 0;
		/*
		GameObject serialContollerObject = GameObject.FindWithTag ("SerialController");
		if (serialContollerObject != null) {
			serialController = serialContollerObject.GetComponent <SerialController> ();
		}

		if (serialController == null) {
			Debug.Log ("Cannot find 'SerialController' script");
		}
		*/

		GameObject gameContollerObject = GameObject.FindWithTag ("GameController");
		if (gameContollerObject != null) {
			gameController = gameContollerObject.GetComponent <GameController> ();
		}

		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}

		nextFire = 0.0f;
		gameState = 0;
	}

	void Update () {
		//PlayerRedBear ();
		gameState = gameController.getState(); 
		playerControls = gameController.RedBear ();
		moveHorizontal = playerControls [0];
		moveVertical = playerControls [1];
		fire = playerControls [2];

		if (gameState == 4 && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			// GameObject clone = 
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation); // as GameObject;
			shot_sound.Play ();
		}

	}

	void FixedUpdate() {

		//moveHorizontal = Input.GetAxis ("Horizontal");
		//moveVertical = Input.GetAxis ("Vertical");
		//print("MoverHoziontal: " + moveHorizontal);
		Vector3 movement = new Vector3 (0.0f, 0.0f, 0.0f);
		if (gameState == 1) {
			movement = new Vector3 (moveHorizontal, 0.0f, 0.0f);
		}

		rb.velocity = movement * speed;
		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);

		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, boundary.Xmin, boundary.Xmax),
			0.0f,
			0.0f
		);
	}

	/*
	void PlayerRedBear() {
		string message = serialController.GetMessage();
		if (message != null) {
			string[] serialControl = message.Split (',');
			print ("Message " + message);
			moveHorizontal = Convert.ToSingle (serialControl [0]);
			moveVertical = Convert.ToSingle (serialControl [1]);
			if (Convert.ToInt32 (serialControl [2]) == 1) {
				fire = true;
			} else {
				fire = false;
			}
		}
	}*/

		
}
