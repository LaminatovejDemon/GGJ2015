using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sentence : MonoBehaviour 
{
	bool _Finished;

	[System.Serializable]
	public class NextSentence
	{
		public Sentence _Target;
		public Condition _Condition;
	}

	public string _Label;

	[SerializeField]
	public List<NextSentence> _NextSentenceList;

	[HideInInspector]
	public float _Speed = 1.0f;
	
	void OnEnable()
	{
		Reset();
	}
	
	void Reset()
	{
		Vector3 defaultPos_ = transform.position;
		Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, 0.0f));
		defaultPos_.x = pos.x;
		transform.position = defaultPos_;
		_Finished = false;
	}
	
	void Update()
	{
		if ( _Finished )
		{
			_Speed -= _Speed * Time.deltaTime;
		}
	
		transform.localPosition += Vector3.left * Time.deltaTime * 5.0f * _Speed;
		
		if ( !_Finished && Camera.main.WorldToViewportPoint(transform.position + Vector3.right * ( renderer.bounds.extents.x * 2.0f)).x < 1.0f )
		{
			_Finished = true;
			TextManager.Get().OnSentenceFinished(this);
		}
	}	
}
