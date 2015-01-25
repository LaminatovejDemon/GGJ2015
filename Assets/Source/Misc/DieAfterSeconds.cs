using UnityEngine;
using System.Collections;

public class DieAfterSeconds : MonoBehaviour 
{
	public float _DieInSecs;
	float _Time;

	void Start()
	{
		_Time = Time.time;
	}

	void Update()
	{
		if ( Time.time - _Time  > _DieInSecs )
		{
			GameObject.Destroy(gameObject);
		}
	}

}
