using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class NetClientExample : MonoBehaviour {
	void Start() {
		NetManager.RegisterReceiver(ReceiveColor, "color");
	}

	public void ReceiveColor(string dataStr) {
		// send "color r g b" from Max
		// "color" into Unity triggers this method with the rest of the string
		// Ex: "color 40 80 255" calls this with dataStr = "40 80 255"
		// now parse the data and get the ints
		Debug.Log("Received from Max: color " + dataStr);

		// split at white space into three substrings
		string[] subStrings = dataStr.Split(null, 3);
		if (subStrings.Length < 3) {
			// bail because not at least three args
			Debug.Log("ReceiveColor: wrong # of args!");
			return;
		}

		// parse the strings into ints
		int r, g, b;
		if (!int.TryParse(subStrings[0], out r)) {
			Debug.Log ("ReceiveColor: bad 'r' arg!");
			return;
		}
		if (!int.TryParse(subStrings[1], out g)) {
			Debug.Log ("ReceiveColor: bad 'g' arg!");
			return;
		}
		if (!int.TryParse(subStrings[2], out b)) {
			Debug.Log ("ReceiveColor: bad 'b' arg!");
			return;
		}

		// r, g, b vars now contain the three RGB values
		// do whatever you want with them here
		Debug.Log("ReceiveColor: R=" + r + " G=" + g + " B=" + b);

		// send example: tell Max we got it
		NetManager.Send("Unity says: Got the color!!");
	}
}
