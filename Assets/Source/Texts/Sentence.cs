using UnityEngine;
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

	SentencePhase _SentencePhase;

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
	public float _MaxSpeed;
	float _RealSpeed;
	
	void OnEnable()
	{
		Reset();
	}

	public void Leave()
	{
		_SentencePhase = SentencePhase.Leaving;
	}
	
	void Reset()
	{
		Vector3 defaultPos_ = transform.position;
		Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, 0.0f));
		defaultPos_.x = pos.x;
		transform.position = defaultPos_;
		_SentencePhase = SentencePhase.Pending;
		_RealSpeed = _MaxSpeed;
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
		
		if ( _SentencePhase == SentencePhase.Pending && Camera.main.WorldToViewportPoint(transform.position + Vector3.right * ( renderer.bounds.extents.x * 2.0f)).x < 1.0f + (_MaxSpeed - TextManager.Get()._PreviousSpeed) )
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
