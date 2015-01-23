using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour 
{
	public delegate void TouchListener(TouchPhase phase, Collider target, Vector3 position);

	Dictionary<int, Collider> _Touches;
	List<TouchListener> _Listeners;
	bool _MouseDown = false;
	
	static InputController instance;

	public static void RegisterListener(TouchListener listener)
	{
		if ( instance == null )
		{
			instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputController>();
		}

		if ( instance._Listeners == null )
		{
			instance._Listeners = new List<TouchListener>();
		}
		instance._Listeners.Add(listener);
	}

	void Update()
	{

#if UNITY_EDITOR
		if ( Input.GetMouseButtonDown(0) )
		{
			TouchBegan(0, Input.mousePosition);
			_MouseDown = true;
		}
		if ( Input.GetMouseButtonUp(0) )
		{
			TouchEnded(0, Input.mousePosition);
			_MouseDown = false;
		}
		if ( _MouseDown )
		{
			TouchMoved(0, Input.mousePosition);
		}
#else
		for ( int i = 0; i < Input.touchCount; ++i )
		{
			switch ( Input.GetTouch(i).phase )
			{
			case TouchPhase.Began:
				TouchBegan(i, Input.GetTouch(i).position);
				break;

			case TouchPhase.Moved:
			case TouchPhase.Stationary:
				TouchMoved(i, Input.GetTouch(i).position);
				break;

			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				TouchEnded(i, Input.GetTouch(i).position);
				break;
			}
		}
#endif
	}

	Ray _Ray;

	void OnDrawGizmos()
	{
		Debug.DrawRay(_Ray.origin, _Ray.direction, Color.blue, 1000);
	}

	Collider GetObjectBelowFinger(Vector2 screenPosition)
	{
		Vector3 origin_ = screenPosition;
		origin_.z = -1;

		_Ray = new Ray(Camera.main.ScreenToWorldPoint(origin_), Vector3.forward);
		RaycastHit info_;
		if ( Physics.Raycast(_Ray, out info_, 100) )
		{
			return info_.collider;
		}

		return null;
	}

	bool RegisterTouch(int index, Collider object_)
	{
		if (_Touches == null )
		{
			_Touches = new Dictionary<int, Collider>();
		}

		if ( object_ == null )
		{
			return false;
		}

		_Touches[index] = object_;
		return true;
	}

	void TouchMoved(int index, Vector3 position)
	{
		if ( _Touches.ContainsKey(index) )
		{
			NotifyListeners(TouchPhase.Moved, _Touches[index], position);
		}
	}

	void TouchBegan(int index, Vector3 position)
	{
		if ( RegisterTouch(index, GetObjectBelowFinger(position)) )
		{
			NotifyListeners(TouchPhase.Began, _Touches[index], position);
		}
	}

	void TouchEnded(int index, Vector3 position)
	{
		if ( _Touches.ContainsKey(index) )
		{
			NotifyListeners(TouchPhase.Ended, _Touches[index], position);
			_Touches.Remove(index);
		}
	}

	void NotifyListeners(TouchPhase phase, Collider target, Vector3 position)
	{
		for ( int i = 0; i < _Listeners.Count; ++i )
		{
			_Listeners[i](phase, target, position);
		}
	}
}
