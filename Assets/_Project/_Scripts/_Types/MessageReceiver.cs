using System;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

[Serializable]
public struct Receiver
{
}


public class MessageReceiver : MonoBehaviour
{
	public UltEvent onMessageReceived;
}
