using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetManager : MonoBehaviour {
	public string sendAddress;
	public int sendPort;
	public int receivePort;

    private static Thread receiveThread;

	private static IPEndPoint sendEndPoint;
	private static UdpClient sendClient;

	private UdpClient receiveClient;

	private static Dictionary<string, System.Action<string>> receivers = new Dictionary<string, System.Action<string>>();

	private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

	void Start () {
		// init transport layer with no arguments (default settings)
		NetworkTransport.Init();

		sendEndPoint = new IPEndPoint(IPAddress.Parse(sendAddress), sendPort);
		sendClient = new UdpClient();

		// invoke async receiver
		receiveClient = new UdpClient(receivePort);
		receiveClient.BeginReceive(new System.AsyncCallback(Receiver), null);
	}

	public static void RegisterReceiver(System.Action<string> receiver, string matchString) {
		Debug.Log("registering receiver: " + receiver + " and matchstring: " + matchString);
		receivers.Add(matchString, receiver);
	}
	
	public static void Send(string sendStr) {
		byte[] sendBytes = Encoding.UTF8.GetBytes (sendStr);
		sendClient.Send (sendBytes, sendBytes.Length, sendEndPoint);
	}

	private void Receiver(System.IAsyncResult receiveResult) {
		// get asynchronously received data
		IPEndPoint receiveEndpoint = new IPEndPoint(IPAddress.Any, 0);
		byte[] receiveBytes = receiveClient.EndReceive(receiveResult, ref receiveEndpoint);
		string receiveString = Encoding.UTF8.GetString(receiveBytes);
		Debug.Log("UDP in: " + receiveString);
		messageQueue.Enqueue(receiveString);
	}
	
	void Update() {
		while (messageQueue.Count != 0) {
			// get queued message
			string message = messageQueue.Dequeue();

			// break into cmd and data at first space
			int firstSpaceIndex = message.IndexOf(" ");
			if (firstSpaceIndex == -1 || firstSpaceIndex == 0) {
				// do nothing
			}
			else {
				string command = message.Substring(0 , firstSpaceIndex);
				string data = message.Substring(firstSpaceIndex + 1);
				receivers[command](data);
			}
		
			// invoke a new async receiver
			receiveClient.BeginReceive(new System.AsyncCallback(Receiver), null);
		}
	}
}


