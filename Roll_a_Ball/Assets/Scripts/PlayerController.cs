using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

	public float speed;
	public Text countText;
	public Text winText;

	private Rigidbody rb;
	private int count;

	void Start() {
		rb = GetComponent<Rigidbody> ();
		count = 0;
		SetCountText();
		winText.text = "";
	}

	void FixedUpdate () {

		/*
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		*/
		string[] move = SerialConnect.GetMessage().Split(','); 

		float moveHorizontal = Convert.ToSingle(move[0]);
		float moveVertical = Convert.ToSingle(move[1]);

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		rb.AddForce (movement * speed);
	}

	void OnTriggerEnter(Collider other) {

		if (other.gameObject.CompareTag ("Pick Up")) {
			other.gameObject.SetActive (false);
			count++;
			SetCountText();
		}

	}

	void SetCountText() { 
		countText.text = "Count: " + count.ToString ();
		if (count >= 12) {
			winText.text = "You Win!!";
		}
	}
}

