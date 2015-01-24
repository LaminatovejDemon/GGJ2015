using UnityEngine;
using System.Collections;

public class Choice : MonoBehaviour 
{
	bool _ForDestroy = false;
	public Sentence.NextSentence _parentSentence;

	void OnEnable() 
	{
		_ForDestroy = false;
		InputController.RegisterListener(OnInput);	
		animation.PlayQueued("ChoiceIdle");
	}

	void OnDestroy()
	{
		InputController.UnregisterListener(OnInput);
	}
	
	void OnInput(TouchPhase phase, Collider targetObject, Vector3 position)
	{
		if ( targetObject != transform.GetComponent<BoxCollider>() )
		{
			return;
		}
		
		if ( phase == TouchPhase.Ended && !_ForDestroy )
		{
			TextManager.Get().ChoiceSelected(this);
		}
	}

	public void SetText(string label)
	{
		for ( int i = 0; i < transform.childCount; ++i )
		{
			transform.GetChild(i).GetComponent<TextMesh>().text = label;
		}
	}

	public void Update()
	{
		if ( _ForDestroy && !animation.isPlaying )
		{
			Destroy(this.gameObject);
			_ForDestroy = false;
		}
	}

	public void DestroyWhenAnimationDone()
	{
		_ForDestroy = true;
	}

}
