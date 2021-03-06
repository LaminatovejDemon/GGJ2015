﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ScoreManager : MonoBehaviour 
{
	public enum ConditionType
	{
		Aggresive = 1<<0,
		Cheerful = 1<<1,
		Protective = 1<<2,
		Questioning = 1<<3,
		Reasonbable = 1<<4,
		Random = 1<<5,
		Invalid = 1<<6,
	}

	public enum ScoreType
	{
		Aggresive,
		Cheerful,
		Protective,
		Questioning,
		Reasonable,
		Count,
	};

	[SerializeField]
	public int[] _Thresholds;

	[SerializeField]
	public int[] _ForbiddenMasks;
	
	int _ForbiddenChoicesMask;

	int[] _Scores;

	private static ScoreManager _Instance;

	public void Reset()
	{
		_ForbiddenChoicesMask = 0;
	}

	public static ScoreManager Get()
	{
		if ( _Instance == null )
		{
			_Instance = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
		}
		return _Instance;
	}

	public ScoreType GetHighestScoreType()
	{
		if ( _Scores == null )
		{
			return ScoreType.Protective;
		}

		int max_ = 0;
		ScoreType maxType_ = ScoreType.Protective;

		for ( int i = 0; i < _Scores.Length; ++i )
		{
			if ( _Scores[i] > max_ )
			{
				max_ = _Scores[i];
				maxType_ = (ScoreType)i;
			}
		}
		return maxType_;
	}

	public void AddScore(ConditionType conditionMask)
	{
		if ( conditionMask <= 0 )
		{
			return;
		}

		if ( _Scores == null )
		{
			_Scores = new int[(int)ScoreType.Count];
		}

		int index_ = 0;
		int maskValue = (int)conditionMask;
		while ( (maskValue & 1) == 0 )
		{
			maskValue >>= 1;
			++index_;
		}

		++_Scores[index_];

		if ( _Scores[index_] >= _Thresholds[index_] )
		{
			_ForbiddenChoicesMask |= _ForbiddenMasks[index_];
		}
	}

	public void FilterChoices(List<Sentence.NextSentence> choicesList)
	{
		for ( int i = 0; i < choicesList.Count; ++i )
		{
			if ( (_ForbiddenChoicesMask & (int)choicesList[i]._Condition._ConditionType) > 0 )
			{
				choicesList.RemoveAt(i);
				if ( choicesList.Count == 0 )
				{
					return;
				}

				--i;
			}
		}
	}
}
