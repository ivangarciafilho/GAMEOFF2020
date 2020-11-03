using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMessageBroadcaster : MonoBehaviour
{
	public string callbackName;
	public SendMessageOptions receiverRequirement = SendMessageOptions.DontRequireReceiver;

	[SerializeField] private DataPackage _currentPackage;
	public DataPackage currentPackage => _currentPackage;

	public void SendMessage ( Component target)
		=>target.BroadcastMessage (callbackName, _currentPackage, receiverRequirement );
}
