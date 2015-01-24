using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextManager : MonoBehaviour 
{
	bool _EndGame = false;

	public Choice _ChoiceTemplate;
	public Font _FontTemplate;
	public float _LargeFontSize;
	public float _SmallFontSize;
	public float _TextSpeedMultiplier = 1.0f;
	public List<Choice> _VisibleChoices;
	public List<Transform> _ChoicePositions;

	int _Row = 0;

	Sentence _ActualSentence;
	Sentence _ActualTemplate;
	public Sentence _PreviousInstance {get; private set;}
	int _ActualSentencePartIndex;

	private static TextManager _Instance;

	void CreateSentence(Sentence targetTemplate, int index, bool raiseSpeed, bool changeLine = true)
	{
		if ( targetTemplate == null )
		{
			_EndGame = true;
			return;
		}

		_PreviousInstance = _ActualSentence;
		_ActualTemplate = targetTemplate;
		_ActualSentence = GameObject.Instantiate(targetTemplate) as Sentence;
		TextMesh text_ = _ActualSentence.gameObject.AddComponent<TextMesh>();
		text_.font = _FontTemplate;
		_ActualSentencePartIndex = index;
		text_.text = _ActualSentence._Labels[_ActualSentencePartIndex];
		_ActualSentence.Init();

		bool largeFont_ = targetTemplate._LargeLabels[_ActualSentencePartIndex];
		text_.characterSize = largeFont_ ? _LargeFontSize : _SmallFontSize;
		text_.anchor = TextAnchor.MiddleLeft;
		text_.color = Color.black;

		_ActualSentence.GetComponent<MeshRenderer>().material = _FontTemplate.material;
		_ActualSentence.transform.parent = transform;

		if ( _PreviousInstance == null)
		{
			return;
		}



		if ( raiseSpeed )
		{
			_ActualSentence._MaxSpeed = 2.0f;
		}

		float previousY_ = _PreviousInstance.transform.position.y;
		Vector3 localPosition_ = _ActualSentence.transform.localPosition;

		if ( !changeLine )
		{
			localPosition_.y = previousY_;
			_ActualSentence._MaxSpeed = _PreviousInstance._MaxSpeed;
		}
		else if ( previousY_ <= -2.5f )
		{
			localPosition_.y = previousY_ + 2.5f;
		}
		else if ( previousY_ >= 2.5f )
		{
			localPosition_.y = previousY_ - 2.5f;
		}
		else
		{
			localPosition_.y = previousY_ + ((Random.Range(0,2) * 2.0f)-1) * 2.5f;
		}
		_ActualSentence.transform.localPosition = localPosition_;
		
	}

	void Update()
	{
		if ( _ActualSentence == null && _EndGame != true )
		{
			CreateSentence(ChapterManager.Get().GetEpisode(), 0, false);
		}
	}

	public static TextManager Get()
	{
		if ( _Instance == null )
		{
			_Instance = GameObject.FindGameObjectWithTag("TextManager").GetComponent<TextManager>();
			_Instance._EndGame = false;
		}

		return _Instance;
	}

	public void OnSentenceTrigger(Sentence source)
	{
		List<Sentence.NextSentence> _choicesList = new List<Sentence.NextSentence>(source._NextSentenceList);
		
		ScoreManager.Get().FilterChoices(_choicesList);

		if ( _ActualSentence._Labels.Count > _ActualSentencePartIndex + 1)
		{
			source.Leave();
			if ( _ActualSentencePartIndex == 0 )
			{
				CreateSentence(_ActualTemplate, _ActualSentencePartIndex+1, false, false);
			}
			else
			{
				CreateSentence(_ActualTemplate, _ActualSentencePartIndex+1, false);
			}
		}
		else if ( _choicesList.Count == 0 )
		{
			source.Leave();
			CreateSentence(ChapterManager.Get().GetEpisode(), 0, false);
		}
		else 
		{
			StartCoroutine(DisplayChoices(_choicesList));
		}
	}

	void DisplayChoice(Sentence.NextSentence item, List<Transform> placesList)
	{
		Choice choiceInstance_ = GameObject.Instantiate(_ChoiceTemplate) as Choice;
		choiceInstance_.transform.position = Vector3.zero;
		choiceInstance_.SetText(item._Target.name);
		
		choiceInstance_.transform.parent = GetChoicePosition(placesList);
		choiceInstance_.transform.localPosition = Vector3.zero;
		
		choiceInstance_.gameObject.AddComponent<BoxCollider>();
		choiceInstance_.GetComponent<BoxCollider>().center = Vector3.zero;
		Vector3 bounds_ = choiceInstance_.transform.GetChild(0).renderer.bounds.extents * 2.0f;
		bounds_.z = 1.0f;
		choiceInstance_.GetComponent<BoxCollider>().size = bounds_;
		choiceInstance_._parentSentence = item;

		if ( _VisibleChoices == null )
		{
			_VisibleChoices = new List<Choice>();
		}
		_VisibleChoices.Add(choiceInstance_);
	}

	IEnumerator DisplayChoices(List<Sentence.NextSentence> choices)
	{
		List<Transform> _choicesPlaces = new List<Transform>(_ChoicePositions);

		for ( int i = 0; i < Mathf.Min(3, choices.Count); ++i )
		{
			DisplayChoice(choices[i], _choicesPlaces);
			yield return new WaitForSeconds(0.5f / ((float)i+1));
		}

		yield break;
	}

	IEnumerator HideActualChoicesBut(Choice but)
	{
		for ( int i = 0; i < _VisibleChoices.Count; ++i )
		{
			if ( _VisibleChoices[i] == but )
			{
				continue;
			}

			_VisibleChoices[i].GetComponent<Animator>().SetBool("Hide", true);

//			yield return new WaitForSeconds(0.1f);
		}

		_VisibleChoices.Clear();
		
		yield break;
	}

	Transform GetChoicePosition(List<Transform> choicesList)
	{
		int index_ = Random.Range(0, choicesList.Count / 2) * 2;

		Transform ret_ = choicesList[index_ + Random.Range(0,2) ];
		choicesList.RemoveAt(index_);
		choicesList.RemoveAt(index_);

		return ret_;
	}

	public void ChoiceSelected(Choice target)
	{
		_ActualSentence.Leave();
		ColorManager.Get().SetMood(target._parentSentence._Condition._ScoreType);
		StartCoroutine(HideActualChoicesBut(target));
		target.GetComponent<Animator>().SetBool("Select", true);
		CreateSentence(target._parentSentence._Target, 0, true);
	}

}
