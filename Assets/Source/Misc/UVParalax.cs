using UnityEngine;
using System.Collections;

public class UVParalax : MonoBehaviour 
{
	public float _ParalaxSpeed = 1.0f;
	float _ParalaxValue = 0.0f;
	
	void Start()
	{
		renderer.material.mainTextureScale = new Vector2(Camera.main.aspect * renderer.material.mainTextureScale.x, renderer.material.mainTextureScale.y);
	}
	
	void Update()
	{
		_ParalaxValue += _ParalaxSpeed * Time.deltaTime;
		_ParalaxValue -= (int)_ParalaxValue;
		renderer.material.mainTextureOffset = new Vector2(_ParalaxValue, 0.0f);
	}
}
