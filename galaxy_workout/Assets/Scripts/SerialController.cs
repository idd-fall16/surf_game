using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System;

public class SerialController: MonoBehaviour {

	public static SerialPort myPort = new SerialPort("/dev/cu.usbmodem1411", 9600);
	private string message;
	public float dataRate;
	public int timeOut;

	// Use this for initialization
	void Start () {
		OpenConnection();
	}


	void Update () {
		StartCoroutine (
			ReadSerialPortCo (
				(string data) => message = data,
				() => print("Error!"),
				timeOut
			)
		);
	}



	void ReadSerialPort () {
			message = myPort.ReadLine ();
			print (message);
	}

	// Update is called once per frame	
	/*
	IEnumerator ReadSerialPortCo () {
		while (true) {
			message = myPort.ReadLine ();
			//print (message);
			yield return new WaitForSeconds (dataRate);
		}

	}
	*/

	public void OpenConnection() {
		
		if (myPort != null) {
			if (myPort.IsOpen) {
				//myPort.Close();
				print("Port already open!");
			}
			else {
				myPort.Open();  // opens the connection
				myPort.ReadTimeout = timeOut;  // sets the timeout value before reporting error
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


	public string GetMessage() {
		return message;
	}

	void OnApplicationQuit() {
		myPort.Close();
	}
		
	public IEnumerator ReadSerialPortCo(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity) {
		DateTime initialTime = DateTime.Now;
		DateTime nowTime;
		TimeSpan diff = default(TimeSpan);

		string dataString = null;
			do {
				try {
					dataString = myPort.ReadLine ();
				} catch (TimeoutException) {
					dataString = null;
				}

				if (dataString != null) {
					callback (dataString);
					yield return null;
				} else {
					yield return new WaitForSeconds (dataRate);
				}

				nowTime = DateTime.Now;
				diff = nowTime - initialTime;

			} while (diff.Milliseconds < timeout);

			if (fail != null)
				fail ();
			yield return null;
		}
}