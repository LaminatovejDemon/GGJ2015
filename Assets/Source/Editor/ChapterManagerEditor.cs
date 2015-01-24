using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ChapterManager))]
public class ChapterManagerEditor : Editor 
{
	ChapterManager m_Data;
	GUIStyle _Style;

	void UpdateBegin()
	{
		if ( m_Data == null )
		{
			m_Data = target as ChapterManager;
		}

		if ( m_Data._Chapters == null )
		{
			m_Data._Chapters = new List< List<Sentence> >();
		}

		if ( _Style == null )
		{
			_Style = new GUIStyle(GUI.skin.box);
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

		base.OnInspectorGUI();

		EditorGUILayout.LabelField("Chapters");
		for ( int i = 0; i < m_Data._Chapters.Count; ++i )
		{
			if ( m_Data._Chapters[i] == null )
			{
				m_Data._Chapters[i] = new List<Sentence>();
			}
			EditorGUILayout.BeginVertical(_Style);
			EditorGUILayout.LabelField("Act " + (i+1));
			for ( int j = 0; j < m_Data._Chapters[i].Count; ++j )
			{
				m_Data._Chapters[i][j] = EditorGUILayout.ObjectField(m_Data._Chapters[i][j], typeof(Sentence), false ) as Sentence;
			}
			ShowListControllers(m_Data._Chapters[i]);
			EditorGUILayout.EndVertical();
		}
		ShowListControllers(m_Data._Chapters);

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
		if ( data.Count > 0 && GUILayout.Button("-", GUILayout.Width(20)) )
		{
			data.RemoveAt(data.Count-1);
		}
		EditorGUILayout.EndHorizontal();
	}
}
