using UnityEngine;
using System.Collections;

public class Choice : MonoBehaviour 
{
	void OnEnable() 
	{
		InputController.RegisterListener(OnInput);	
	}
	
	void OnInput(TouchPhase phase, Collider targetObject, Vector3 position)
	{
		if ( targetObject != transform.GetComponent<BoxCollider>() )
		{
			return;
		}
		
		Debug.Log (gameObject.name + ": " + phase + " to " + position);
	}
}
