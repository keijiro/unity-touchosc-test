using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class OscClient : MonoBehaviour
{
    public int listenPort = 6666;
    UdpClient udpClient;
    IPEndPoint endPoint;
    
    void Start ()
    {
        endPoint = new IPEndPoint (IPAddress.Any, listenPort);
        udpClient = new UdpClient (endPoint);
    }

    void Update ()
    {
        while (udpClient.Available > 0) {
            object[] msg = Osc.ToArray (udpClient.Receive (ref endPoint));
            string text = "";
            foreach (object o in msg)
                text += o.ToString () + " ";
            Debug.Log (text);
        }
    }
}