using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour {

	static Rigidbody rb;
	public float tumble;

	void Start() {
		rb = GetComponent<Rigidbody> ();
		rb.angularVelocity = Random.insideUnitCircle * tumble;
	}
}
