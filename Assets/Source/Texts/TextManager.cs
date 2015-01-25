using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextManager : MonoBehaviour 
{
	bool _EndGame = false;
	bool _Initialised = false;
	bool _InitCharacterShouldStop = false;

	public ChapterManager _ChapterManagerTemplate;
	public Choice _ChoiceTemplate;
	public Font _FontTemplate;
	public float _LargeFontSize;
	public float _SmallFontSize;
	public float _TextSpeedMultiplier = 1.0f;
	public List<Choice> _VisibleChoices;
	public List<Transform> _ChoicePositions;
	public List<Sentence> _Queue = new List<Sentence>();


	Vector3 _InitCharPostion;

	Sentence _ActualSentence;
	Sentence _ActualTemplate;

	int _ActualSentencePartIndex;

	private static TextManager _Instance;

	public void OnSentenceEnd(Sentence source)
	{
		_Queue.Remove(source);
	}

	void CreateSentence(Sentence targetTemplate, int index, bool raiseSpeed, bool changeLine = true)
	{
		if ( targetTemplate == null )
		{
			_EndGame = true;
			return;
		}

		_ActualTemplate = targetTemplate;
		_ActualSentence = GameObject.Instantiate(targetTemplate) as Sentence;
		_ActualSentence.transform.parent = transform;
		_ActualSentence.transform.localPosition = Vector3.zero;
		_Queue.Add(_ActualSentence);
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


		if ( raiseSpeed )
		{
			_ActualSentence._MaxSpeed = 2.0f;
		}

		Vector3 localPosition_ = _ActualSentence.transform.localPosition;

		if ( _Queue.Count == 1 )
		{
			return;
		}

		Vector3 previousEndWorldPoint_ = _Queue[_Queue.Count-2].transform.localPosition + _Queue[_Queue.Count-2].renderer.bounds.extents * 2.0f;

		localPosition_.y = _Queue[_Queue.Count-2].transform.localPosition.y;
		localPosition_.x = Mathf.Max(localPosition_.x, previousEndWorldPoint_.x + 0.5f);
//		localPosition_.x = _Queue[_Queue.Count-2].transform.localPosition.x + _Queue[_Queue.Count-2].transform.renderer.bounds.extents.x * 2.0f;

		if ( !raiseSpeed )
		{
			_ActualSentence._MaxSpeed = _Queue[_Queue.Count-2]._MaxSpeed;
		}

		if ( changeLine )
		{
	 		if ( _Queue[_Queue.Count-2].transform.localPosition.y <= -2.5f )
			{
				localPosition_.y = _Queue[_Queue.Count-2].transform.localPosition.y + 2.5f;
			}
			else if ( _Queue[_Queue.Count-2].transform.localPosition.y >= 2.5f )
			{
				localPosition_.y = _Queue[_Queue.Count-2].transform.localPosition.y - 2.5f;
			}
			else
			{
				localPosition_.y = _Queue[_Queue.Count-2].transform.localPosition.y + ((Random.Range(0,2) * 2.0f)-1) * 2.5f;
			}
		}
		_ActualSentence.transform.localPosition = localPosition_;
		
	}

	void Initialise()
	{
		if ( _Initialised )
		{
			return;
		}
	

		if ( !_InitCharacterShouldStop )
		{
			_InitCharPostion = JohnHandler.Get()._John.transform.position;
			_InitCharacterShouldStop = true;
		}
		Vector3 charPos_ = _InitCharPostion;
		charPos_.x = Camera.main.ViewportToWorldPoint(Vector3.left * 0.1f).x;
		JohnHandler.Get()._John.transform.position = charPos_;
		JohnHandler.Get().DoAction(JohnHandler.Action.Run);


//		CreateSentence(ChapterManager.Get().GetEpisode(), 0, false);
		_Initialised = true;
	}

	void Update()
	{
		Initialise();
		GetJohnSentence();

		if ( _EndGame && Camera.main.WorldToViewportPoint(JohnHandler.Get()._John.transform.position).x > 2.0f )
		{
			Reset();
		}
		else if ( _InitCharacterShouldStop && JohnHandler.Get()._John.transform.position.x >= _InitCharPostion.x )
		{
			JohnHandler.Get().DoAction(JohnHandler.Action.Stop);
			_InitCharacterShouldStop = false;

			Sentence.NextSentence firstSentence = new Sentence.NextSentence();
			firstSentence._Target = ChapterManager.Get().GetEpisode();
			DisplayChoice(firstSentence);
		}
	}

	void Reset()
	{
		_Initialised = false;
		_EndGame = false;
		Vector3 charPos_ = _InitCharPostion;
		charPos_.x = Camera.main.ViewportToWorldPoint(Vector3.left * 0.1f).x;
		JohnHandler.Get()._John.transform.position = charPos_;
		GameObject.Destroy(ChapterManager.Get().gameObject);
		GameObject.Instantiate(_ChapterManagerTemplate);
		_InitCharacterShouldStop = true;
	}

	public void GetJohnSentence()
	{
		Vector3 _JohnPosition = JohnHandler.Get()._John.transform.position;
		float _HighestPoint = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
		Sentence match_ = null;

		for ( int i = 0; i< _Queue.Count; ++i )
		{
			if ( _Queue[i].transform.position.x > _JohnPosition.x || _Queue[i].transform.position.x + _Queue[i].renderer.bounds.extents.x * 2.0f < _JohnPosition.x )
			{
				continue;
			}

			float altitude_ = _Queue[i].transform.position.y + _Queue[i].renderer.bounds.extents.y * 0.3f;

			if ( altitude_ > _HighestPoint )
			{
				_HighestPoint = altitude_;
				match_ = _Queue[i];
			}
		}

		JohnHandler.Get().SetYPosition(_HighestPoint, match_);
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
			CreateSentence(_ActualTemplate, _ActualSentencePartIndex+1, false, false);
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

	void DisplayChoice(Sentence.NextSentence item, List<Transform> placesList = null)
	{
		Choice choiceInstance_ = GameObject.Instantiate(_ChoiceTemplate) as Choice;

		if ( placesList != null )
		{
			choiceInstance_.transform.parent = GetChoicePosition(placesList);
			choiceInstance_.SetText(item._Target.name);

			choiceInstance_.transform.localPosition = Vector3.zero;
		}
		else
		{
			choiceInstance_.SetText("Mean While");
			choiceInstance_.transform.localPosition = Vector3.up * 1.42f;
		}

		
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
		JohnHandler.Get().DoStop();
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
		JohnHandler.Get().DoAction(JohnHandler.Action.Walk);
		if ( _ActualSentence != null )
		{
			_ActualSentence.Leave();
			if ( target._parentSentence._Condition != null )
			{
				ColorManager.Get().SetMood(target._parentSentence._Condition._ScoreType);
			}
		}

		StartCoroutine(HideActualChoicesBut(target));
		target.GetComponent<Animator>().SetBool("Select", true);
		CreateSentence(target._parentSentence._Target, 0, true);
	}

}
