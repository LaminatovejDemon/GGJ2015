using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChapterManager : MonoBehaviour 
{
	int _CurrentChapter;

	[System.Serializable]
	public class Chapter
	{
		[SerializeField]
		public List<Sentence> _Chapter;

		public bool _Random;
	};

	[SerializeField]
	public List< Chapter > _Chapters; 

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
		if ( _CurrentChapter < _Chapters.Count && _Chapters[_CurrentChapter]._Chapter.Count == 0 )
		{
			++_CurrentChapter;
		}

		if ( _CurrentChapter >= _Chapters.Count )
		{
			return null;
		}

		int targetIndex_ = _Chapters[_CurrentChapter]._Random ? Random.Range(0, _Chapters[_CurrentChapter]._Chapter.Count) : 0;

		Sentence ret_ = _Chapters[_CurrentChapter]._Chapter[targetIndex_];

		_Chapters[_CurrentChapter]._Chapter.RemoveAt(targetIndex_);

		return ret_;
	}
}
