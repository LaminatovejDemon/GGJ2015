using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChapterManager : MonoBehaviour 
{
	int _CurrentChapter;

	[SerializeField]
	public List< List<Sentence>> _Chapters; 
	public Sentence _FailsafeEpisode;

	static ChapterManager _Instance;

	public static ChapterManager Get()
	{
		if ( _Instance == null )
		{
			_Instance = GameObject.FindGameObjectWithTag("ChapterManager").GetComponent<ChapterManager>();
			_Instance._CurrentChapter = 0;
		}

		return _Instance;
	}

	public Sentence GetEpisode()
	{
		return _FailsafeEpisode;

		if ( _CurrentChapter < _Chapters.Count && _Chapters[_CurrentChapter].Count == 0 )
		{
			++_CurrentChapter;
		}

		if ( _CurrentChapter >= _Chapters.Count )
		{
			return null;
		}

		int targetIndex_ = Random.Range(0, _Chapters[_CurrentChapter].Count);

		Sentence ret_ = _Chapters[_CurrentChapter][targetIndex_];

		_Chapters[_CurrentChapter].RemoveAt(targetIndex_);

		return ret_;
	}
}
