using UnityEngine;
using System.Collections;

public class DestroyMyBoundary : MonoBehaviour {

	void OnTriggerExit(Collider other) {
		Destroy (other.gameObject);
	}
}
