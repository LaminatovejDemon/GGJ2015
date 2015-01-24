using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Sentence))]
public class SentenceEditor : Editor 
{
	Sentence m_Data;

	void UpdateBegin()
	{
		if ( m_Data == null )
		{
			m_Data = target as Sentence;
		}

		if ( m_Data._NextSentenceList == null )
		{
			m_Data._NextSentenceList = new List<Sentence.NextSentence>();
		}

		GUI.changed = false;
	}

	void UpdateEnd()
	{
		if ( GUI.changed )
		{
			EditorUtility.SetDirty(m_Data);
		}
	}

	public override void OnInspectorGUI ()
	{
		UpdateBegin();
//		base.OnInspectorGUI ();

		for ( int i = 0; i < m_Data._Labels.Count; ++i )
		{
			EditorGUILayout.BeginHorizontal();
			m_Data._LargeLabels[i] = EditorGUILayout.Toggle(m_Data._LargeLabels[i], GUILayout.MaxWidth(10) );
			m_Data._Labels[i] = EditorGUILayout.TextField(m_Data._Labels[i]);
			EditorGUILayout.EndHorizontal();
		}
		ShowLabelsControllers();

		for ( int i = 0; i < m_Data._NextSentenceList.Count; ++i )
		{
			EditorGUILayout.BeginHorizontal();
			m_Data._NextSentenceList[i]._Target = EditorGUILayout.ObjectField(m_Data._NextSentenceList[i]._Target, typeof(Sentence), false) as Sentence;
			if ( m_Data._NextSentenceList[i]._Condition == null )
			{
				m_Data._NextSentenceList[i]._Condition = new Condition();
			}
			m_Data._NextSentenceList[i]._Condition._ConditionType = (Condition.ConditionType)EditorGUILayout.EnumPopup(m_Data._NextSentenceList[i]._Condition._ConditionType);
			EditorGUILayout.EndHorizontal();
		}
		ShowListControllers(m_Data._NextSentenceList);

		UpdateEnd();
	}

	void ShowListControllers<T>(List<T> data) where T : new()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if ( GUILayout.Button("+", GUILayout.Width(20)) )
		{
			data.Add(new T());
		}
		if ( GUILayout.Button("-", GUILayout.Width(20)) )
		{
			data.RemoveAt(data.Count-1);
		}
		EditorGUILayout.EndHorizontal();
	}

	void ShowLabelsControllers()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if ( GUILayout.Button("+", GUILayout.Width(20)) )
		{
			m_Data._Labels.Add("");
			m_Data._LargeLabels.Add(false);
		}
		if ( GUILayout.Button("-", GUILayout.Width(20)) )
		{
			m_Data._Labels.RemoveAt(m_Data._Labels.Count-1);
			m_Data._LargeLabels.RemoveAt(m_Data._LargeLabels.Count-1);
		}
		EditorGUILayout.EndHorizontal();
	}
}
