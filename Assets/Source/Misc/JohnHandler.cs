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
	};

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
		}
	}

	void GetHeightBelowJohn()
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

	}
}
