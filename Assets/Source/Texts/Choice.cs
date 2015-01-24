using UnityEngine;
using System.Collections;

public class Choice : MonoBehaviour 
{
	void OnEnable() 
	{
		InputController.RegisterListener(OnInput);	
		animation.PlayQueued("ChoiceIdle");
	}
	
	void OnInput(TouchPhase phase, Collider targetObject, Vector3 position)
	{
		if ( targetObject != transform.GetComponent<BoxCollider>() )
		{
			return;
		}
		
		Debug.Log (gameObject.name + ": " + phase + " to " + position);
	}

	public void SetText(string label)
	{
		for ( int i = 0; i < transform.childCount; ++i )
		{
			transform.GetChild(i).GetComponent<TextMesh>().text = label;
		}
	}
}
