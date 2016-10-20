using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SerialConnect: MonoBehaviour {

	public static SerialPort myPort = new SerialPort("/dev/cu.usbmodem1411", 9600);
	public static string message;
//	float timePassed = 0.0f;

	// Use this for initialization
	void Start () {
		OpenConnection();
	}

	// Update is called once per frame
	void Update () {
		//timePassed+=Time.deltaTime;
		//if(timePassed>=0.2f){

		//print("BytesToRead" +sp.BytesToRead);
		message = myPort.ReadLine();
		print(message);
		//	timePassed = 0.0f;
		//}
	}

	public void OpenConnection() {
		if (myPort != null) {
			if (myPort.IsOpen) {
				myPort.Close();
				print("Closing port, because it was already open!");
			}
			else {
				myPort.Open();  // opens the connection
				myPort.ReadTimeout = 1;  // sets the timeout value before reporting error
				print("Port Opened!");
			}
		}
		else {
			if (myPort.IsOpen) {
				print("Port is already open");
			}
			else {
				print("Port == null");
			}
		}
	}


	public static string GetMessage() {
		return message;
	}

	void OnApplicationQuit() {
		myPort.Close();
	}
}