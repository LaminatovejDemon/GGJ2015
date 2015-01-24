using UnityEngine;
using System.Collections;

[System.Serializable]
public class ColorManager : MonoBehaviour 
{
	static ColorManager _Instance;

	[SerializeField]
	public Color[] _Moods;
	public float _InitTime;

	public static ColorManager Get()
	{
		if ( _Instance == null )
		{
			_Instance = GameObject.FindGameObjectWithTag("ColorManager").GetComponent<ColorManager>();
		}

		return _Instance;
	}

	ScoreManager.ScoreType _Mood = ScoreManager.ScoreType.Count;
	ScoreManager.ScoreType _TargetMood = ScoreManager.ScoreType.Count;

	public void SetMood(ScoreManager.ScoreType type)
	{
		_TargetMood = type;
		_InitTime = Time.time;
	}

	void Update()
	{
		if ( _TargetMood != _Mood )
		{
			Color lerpedColor_ = Color.Lerp(_Moods[(int)_Mood], _Moods[(int)_TargetMood], Time.time - _InitTime);
			renderer.material.SetColor("_TintColor", lerpedColor_);

			if ( renderer.material.GetColor("_TintColor") == _Moods[(int)_TargetMood] )
			{
				_Mood = _TargetMood;
			}
		}
	}
}
