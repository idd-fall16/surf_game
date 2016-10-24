using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	static Rigidbody rb;
	public float speed;
	void Start() {
		rb = GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * speed;
	}
}
