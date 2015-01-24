﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sentence : MonoBehaviour 
{
	bool _WaitingForInteraction;

	enum SentencePhase
	{
		Pending,
		WaitingForInteraction,
		Leaving,
		Finished,
	}

	SentencePhase _SentencePhase = SentencePhase.Pending;

	[System.Serializable]
	public class NextSentence
	{
		public Sentence _Target;
		public Condition _Condition;
	}

	[SerializeField]
	public List<string> _Labels;
	public List<bool> _LargeLabels;

	[SerializeField]
	public List<NextSentence> _NextSentenceList;
	
	public float _Speed;
	public float _MaxSpeed = 1.0f;
	float _RealSpeed = 1.0f;

	public void Leave()
	{
		_SentencePhase = SentencePhase.Leaving;
	}
	
	public void Init()
	{
		Vector3 defaultPos_ = transform.position;
		Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, 0.0f));
		defaultPos_.x = pos.x;
		transform.position = defaultPos_;
		_MaxSpeed = 1.0f;
		_RealSpeed = 1.0f;
		_SentencePhase = SentencePhase.Pending;
	}
	
	void Update()
	{
		if ( _SentencePhase == SentencePhase.WaitingForInteraction && _RealSpeed > 0 )
		{
			_RealSpeed -= Time.deltaTime;
			_RealSpeed = Mathf.Max(_RealSpeed, 0);
		}
		else if ( _SentencePhase == SentencePhase.Leaving && _RealSpeed < _MaxSpeed )
		{
			_RealSpeed += Time.deltaTime;
			_RealSpeed = Mathf.Min(_RealSpeed, _MaxSpeed);
		}
		else if ( _SentencePhase == SentencePhase.Pending )
		{
			_RealSpeed = _MaxSpeed;
		}
	
		transform.localPosition += Vector3.left * Time.deltaTime * 5.0f * _RealSpeed * _Speed;

		float lastSpeedRatio_ = _MaxSpeed;
		Sentence previous_ = TextManager.Get()._PreviousInstance;
		if ( previous_ != null )
		{
			lastSpeedRatio_ = _MaxSpeed / previous_._MaxSpeed;
		}
		
		if ( _SentencePhase == SentencePhase.Pending && Camera.main.WorldToViewportPoint(transform.position + Vector3.right * ( renderer.bounds.extents.x * 2.0f)).x
		    < lastSpeedRatio_ )
		{
			_SentencePhase = SentencePhase.WaitingForInteraction;
			TextManager.Get().OnSentenceTrigger(this);
		}
		else if ( _SentencePhase == SentencePhase.Leaving && Camera.main.WorldToViewportPoint(transform.position + Vector3.right * ( renderer.bounds.extents.x * 2.0f)).x < 0.0f )
		{
			GameObject.Destroy(gameObject);
		}
	}	
}
