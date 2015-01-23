using UnityEngine;
using System.Collections;

public class ScreenFill : MonoBehaviour 
{
	public bool _Vertical;
	public bool _Horizontal;

	void Awake()
	{
		float vertical_ = _Vertical ? Camera.main.orthographicSize  * 2.0f : transform.localScale.x;
		float horizontal_ = _Horizontal ? Camera.main.orthographicSize * 2.0f * Camera.main.aspect : transform.localScale.y;

		transform.localScale = new Vector3( horizontal_, vertical_, 1.0f);
	}

}
