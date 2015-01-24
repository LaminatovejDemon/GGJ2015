using UnityEngine;
using System.Collections;

public class Choice : MonoBehaviour 
{
	public Sentence.NextSentence _parentSentence;

	void OnEnable() 
	{
		InputController.RegisterListener(OnInput);	
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
		
		if ( phase == TouchPhase.Ended && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ChoiceIdle") )
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
		if ( GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Finished") )
		{
			Destroy(this.gameObject);
		}
	}

}
