using UnityEngine;
using System.Collections;
public class JohnHandler : MonoBehaviour 
{
	static JohnHandler _Instance;
	public Animator _John;

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

	Action _PendingAction = Action.Stop;

	public void DoAction(Action type)
	{
		switch ( type )
		{
		case Action.Stop:
			_John.SetBool("walk", false);
			_John.SetBool("run", false);
			break;
		case Action.Run:
			_John.SetBool("run", true);
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

	public void SetYPosition(float yPosition)
	{
		Vector3 johnPosition_ = _John.transform.position;

		if ( _PendingAction == Action.JumpUp && yPosition < johnPosition_.y )
		{
			DoAction(Action.JumpUpEnd);
			_John.transform.position = johnPosition_;	
		}
		else if ( _PendingAction == Action.JumpDown && yPosition > johnPosition_.y )
		{
			DoAction(Action.JumpDownEnd);
			_John.transform.position = johnPosition_;	
		}
		else if  ( yPosition > johnPosition_.y + 0.5f )
		{
			DoAction(Action.JumpUp);
		}
		else if ( yPosition < johnPosition_.y - 0.5f )
		{
			DoAction(Action.JumpDown);
		}
	}

	void GetHeightBelowJohn()
	{

	}
}
