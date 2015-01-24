using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextManager : MonoBehaviour 
{
	public Choice _ChoiceTemplate;
	public Sentence _FirstSentence;
	public Font _FontTemplate;
	public float _TextSpeedMultiplier = 1.0f;

	public List<Transform> _ChoicePositions;

	Sentence _ActualSentence;

	private static TextManager _Instance;

	void Update()
	{
		if ( _ActualSentence == null )
		{
			_ActualSentence = GameObject.Instantiate(_FirstSentence) as Sentence;
			TextMesh text_ = _ActualSentence.gameObject.AddComponent<TextMesh>();
			text_.font = _FontTemplate;
			text_.text = _ActualSentence._Label;
			text_.characterSize = 0.125f;
			text_.anchor = TextAnchor.MiddleLeft;
			text_.color = Color.black;
			_ActualSentence._Speed = 0.2f * _TextSpeedMultiplier;
			_ActualSentence.GetComponent<MeshRenderer>().material = _FontTemplate.material;
			_ActualSentence.transform.parent = transform;

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

	public void OnSentenceFinished(Sentence source)
	{
		StartCoroutine(DisplayChoices(source._NextSentenceList));
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
	}

	IEnumerator DisplayChoices(List<Sentence.NextSentence> choices)
	{
	
		List<Transform> _choicesList = new List<Transform>(_ChoicePositions);

		for ( int i = 0; i < Mathf.Min(3, choices.Count); ++i )
		{
			DisplayChoice(choices[i], _choicesList);
			yield return new WaitForSeconds(0.5f / ((float)i+1));
		}

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

}
