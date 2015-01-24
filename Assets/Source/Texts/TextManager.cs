using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextManager : MonoBehaviour 
{
	public Choice _ChoiceTemplate;
	public Sentence _FirstSentence;
	public Font _FontTemplate;
	public float _LargeFontSize;
	public float _SmallFontSize;
	public float _TextSpeedMultiplier = 1.0f;
	public List<Choice> _VisibleChoices;
	public List<Transform> _ChoicePositions;

	Sentence _ActualSentence;
	Sentence _ActualTemplate;
	int _ActualSentencePartIndex;

	private static TextManager _Instance;

	void CreateSentence(Sentence targetTemplate, int index = 0)
	{
		_ActualTemplate = targetTemplate;
		_ActualSentence = GameObject.Instantiate(targetTemplate) as Sentence;
		TextMesh text_ = _ActualSentence.gameObject.AddComponent<TextMesh>();
		text_.font = _FontTemplate;
		_ActualSentencePartIndex = index;
		text_.text = _ActualSentence._Labels[_ActualSentencePartIndex];
		text_.characterSize = targetTemplate._LargeLabels[_ActualSentencePartIndex] ? 0.125f : 0.045f;
		text_.anchor = TextAnchor.MiddleLeft;
		text_.color = Color.black;
		_ActualSentence._Speed = 0.2f * _TextSpeedMultiplier;
		_ActualSentence.GetComponent<MeshRenderer>().material = _FontTemplate.material;
		_ActualSentence.transform.parent = transform;
	}

	void Update()
	{
		if ( _ActualSentence == null )
		{
			CreateSentence(_FirstSentence);
		}
	}

	public static TextManager Get()
	{
		if ( _Instance == null )
		{
			_Instance = GameObject.FindGameObjectWithTag("TextManager").GetComponent<TextManager>();
		}

		return _Instance;
	}

	public void OnSentenceTrigger(Sentence source)
	{
		if ( _ActualSentence._Labels.Count > _ActualSentencePartIndex + 1)
		{
			source.Leave();
			CreateSentence(_ActualTemplate, _ActualSentencePartIndex+1);
		}
		else if ( source._NextSentenceList.Count == 0 )
		{
			source.Leave();
			CreateSentence(_FirstSentence);
		}
		else 
		{
			StartCoroutine(DisplayChoices(source._NextSentenceList));
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
		List<Sentence.NextSentence> _choicesList = new List<Sentence.NextSentence>(choices);

		ScoreManager.Get().FilterChoices(_choicesList);

		for ( int i = 0; i < Mathf.Min(3, _choicesList.Count); ++i )
		{
			DisplayChoice(_choicesList[i], _choicesPlaces);
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

		StartCoroutine(HideActualChoicesBut(target));
		target.GetComponent<Animator>().SetBool("Select", true);
		CreateSentence(target._parentSentence._Target);
	}

}
