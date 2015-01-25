using UnityEngine;
using System.Collections;
using UnityEditor;

public class JohnHandler : MonoBehaviour 
{
	public float _CharacterSpeedMultiplier = 1.0f;
	static JohnHandler _Instance;
	public Animator _John;
	bool _ShouldStop = false;

	public static JohnHandler Get()
	{
		if ( _Instance == null )
		{
			_Instance = GameObject.FindGameObjectWithTag("JohnHandler").GetComponent<JohnHandler>();
		}
		return _Instance;
	}


	public enum Action
	{
		Stop,
		Walk,
		Run,
		JumpDown,
		JumpUp,
		JumpUpEnd,
		JumpDownEnd,
	};

	Action _PendingAction = Action.Walk;

	public void DoStop()
	{
		_ShouldStop = true;
	}

	public void DoAction(Action type)
	{
		switch ( type )
		{
		case Action.Stop:
			_John.SetBool("run", false);
			_John.SetBool("walk", false);
			break;

		case Action.Run:
			_ShouldStop = false;
			_John.SetBool("run", true);
			_John.SetBool("walk", false);
			break;

		case Action.Walk:
			_ShouldStop = false;
			_John.SetBool("run", false);
			_John.SetBool("walk", true);
			break;

		case Action.JumpUp:
			if ( _PendingAction != Action.JumpDown && _PendingAction != Action.JumpUp )
			{
				_John.SetBool("JumpUp", true);
			}
			break;
		case Action.JumpDown:
			if ( _PendingAction != Action.JumpDown && _PendingAction != Action.JumpUp )
			{
				_John.SetBool("JumpDown", true);
			}
			break;

		case Action.JumpUpEnd:
			_John.SetBool("JumpUp", false);
			_John.SetBool("AfterJumpFloorImpact", true);
			break;

		case Action.JumpDownEnd:
			_John.SetBool("JumpDown", false);
			_John.SetBool("AfterJumpFloorImpact", true);
			break;
		}
		_PendingAction = type;
	}

	public void SetYPositionForward(float yPosition, Sentence match)
	{
		if ( match == null )
		{
			if ( _PendingAction == Action.JumpDown )
			{
				DoAction(Action.JumpDownEnd);
			}
			else if (_PendingAction == Action.JumpDownEnd )
			{
				DoAction(Action.Run);
			}
			return;
		}

		if ( _PendingAction == Action.JumpUp )
		{
			return;
		}

		Vector3 johnPosition_ = _John.transform.position;

		if ( _PendingAction != Action.JumpDown && yPosition < johnPosition_.y - 0.5f)
		{
			DoAction(Action.JumpDown);
		}
		if ( _PendingAction == Action.JumpDown && yPosition > johnPosition_.y - 0.5f )
		{
			johnPosition_.y = yPosition;
			DoAction(Action.JumpDownEnd);

			_John.transform.localPosition = johnPosition_;	
			Debug.Log ("_John 3: " + _John.transform.localPosition);
		}
	}

	public void SetYPosition(float yPosition, Sentence match)
	{
		if ( match == null )
		{
			if ( _PendingAction == Action.JumpUp )
			{
				DoAction(Action.JumpUpEnd);
			}
			else if ( _PendingAction == Action.JumpUpEnd )
			{
				DoAction(Action.Run);
			}

			return;
		}

		if ( _PendingAction == Action.JumpDown )
		{
			return;
		}

		Vector3 johnPosition_ = _John.transform.position;

		if ( _PendingAction == Action.JumpUp )
		{
			if ( yPosition < johnPosition_.y + 0.22f )
			{
				johnPosition_.y = yPosition;
				DoAction(Action.JumpUpEnd);
				_John.transform.localPosition = johnPosition_;
				Debug.Log ("_John 1: " + _John.transform.localPosition);
			}
			return;
		}

		if  ( yPosition > johnPosition_.y + 0.22f && (_PendingAction == Action.Run || _PendingAction == Action.Walk)) 
		{
			DoAction(Action.JumpUp);
		}

//		if ( _PendingAction == Action.Walk || _PendingAction == Action.Run || _PendingAction == Action.Stop ) 
		{
			_John.transform.localPosition += Vector3.left * match.GetActualSpeed() * Time.deltaTime * _CharacterSpeedMultiplier;
		}


		if ( Camera.main.WorldToViewportPoint(johnPosition_).x < 0.2f && 
		    (_PendingAction == Action.Walk || _PendingAction == Action.JumpDownEnd || _PendingAction == Action.JumpUpEnd) )
		{
			DoAction(Action.Run);
		}
		else if ( Camera.main.WorldToViewportPoint(johnPosition_).x > 0.8f && 
		         (_PendingAction == Action.Run || _PendingAction == Action.JumpDownEnd || _PendingAction == Action.JumpUpEnd) )
		{
			DoAction(Action.Walk);
		}
		else if ( Camera.main.WorldToViewportPoint(johnPosition_).x > 0.5f && (_ShouldStop || TextManager.Get()._ChoicesAreDisplayed ) && 
		         (_PendingAction == Action.Run || _PendingAction == Action.Walk || _PendingAction == Action.JumpDownEnd || _PendingAction == Action.JumpUpEnd ) )
		{
			DoAction(Action.Stop);
		}

	}

	void GetHeightBelowJohn()
	{

	}
}
