using UnityEngine;
using System.Collections;

public class SentenceRenderer : MonoBehaviour 
{
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
	}

	void Update()
	{
		transform.localPosition += Vector3.left * Time.deltaTime * 5.0f * _Speed;

		if ( Camera.main.WorldToViewportPoint(transform.position + Vector3.right * ( renderer.bounds.extents.x * 2.0f)).x < 0 )
		{
			Reset();
		}
	}


}
