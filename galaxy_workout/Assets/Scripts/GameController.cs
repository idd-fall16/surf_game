using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public GameObject hazard;
	public Vector3 spawnValues; 
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;

	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;

	private bool gameOver;
	private bool restart;
	private int score;

	private SerialController serialController;

	private float[] playerControls = {0};
	private float fire;
	private float moveHorizontal;
	private float moveVertical;


	private int gameState;

	void Start () {

		gameState = 0;
		gameOver = false;
		restart = false;
		fire = 0;

		restartText.text = "";
		gameOverText.text = "";
		score = 0;
		UpdateScore ();

		GameObject serialContollerObject = GameObject.FindWithTag ("SerialController");
		if (serialContollerObject != null) {
			serialController = serialContollerObject.GetComponent <SerialController> ();
		}

		if (serialController == null) {
			Debug.Log ("Cannot find 'SerialController' script");
		}

		StartCoroutine (SpawnWaves ());
			
	}

	void Update () {
		playerControls = RedBear ();
		fire = playerControls [2];

		if (restart) {
			if (Input.GetKeyDown (KeyCode.R)) {
				SceneManager.LoadScene("Main");
			}
		}
		print ("fire: " + fire);
		print ("State: " + gameState);
		switch (gameState) {
		//idle
		case 0:
			if (fire == 1)
				gameState = 1;
			break;
		//standing
		case 1:
			if (!(fire == 1))
				gameState = 2;
			break;
		//About to do a pushup
		case 2:
			if (fire == 1)
				gameState = 3;
			break;
		//pushup
		case 3:
			if (!(fire == 1))
				gameState = 4;
			break;
		//done with pushup
		case 4:
			if (fire == 1)
				gameState = 1;
			break;
		}
		print ("State: " + gameState);
		
	}



	IEnumerator SpawnWaves() {

		while (true) {
			if (gameState == 1) {
				yield return new WaitForSeconds (startWait);
				for (int i = 0; i < hazardCount; i++) {
					Vector3 spawnPosition = new Vector3 (UnityEngine.Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
					Quaternion spawnRotation = Quaternion.identity;
					Instantiate (hazard, spawnPosition, spawnRotation);
					yield return new WaitForSeconds (spawnWait);
				}
				yield return new WaitForSeconds (waveWait);

			} else {
				yield return null;
			}

			if (gameOver) {
				restartText.text = "Press 'R' for Restart";
				restart = true;
				break;
			}
		}
	}

	public void Addscore (int newScoreValue) {
		
		score += newScoreValue;
		UpdateScore ();
	}

	void UpdateScore () {

		scoreText.text = "Score: " + score;
	}

	public void GameOver () {
	
		gameOverText.text = "GameOver!";
		gameOver = true;
	}


	public float[] RedBear() {
		string message = serialController.GetMessage ();
		float[] playerControls = {0,0,0};
		if (message != null) {
			string[] serialControl = message.Split (',');

			Single.TryParse((serialControl [0]),out playerControls[0]);
			Single.TryParse((serialControl [1]),out playerControls[1]);
			Single.TryParse((serialControl [2]),out playerControls[2]);

			//print (message);
			/*
			playerControls[0] = Convert.ToSingle (serialControl [0]);
			playerControls[1] = Convert.ToSingle (serialControl [1]);
			playerControls [2] = Convert.ToSingle (serialControl [2]);
			*/
		}
		return playerControls;
	}


	public int getState() {
		return gameState;
	}

}
