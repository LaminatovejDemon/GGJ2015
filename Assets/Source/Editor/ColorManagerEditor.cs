using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ColorManager))]
public class ColorManagerEditor : Editor 
{
	ColorManager m_Data;

	void UpdateBegin()
	{
		if ( m_Data == null )
		{
			m_Data = target as ColorManager;
		}

		if ( m_Data._Moods == null || m_Data._Moods.Length < (int)ScoreManager.ScoreType.Count+1 )
		{
			m_Data._Moods = new Color[(int)ScoreManager.ScoreType.Count+1];
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

		for ( int j = 0; j < (int)(ScoreManager.ScoreType.Count); ++j )
		{
			m_Data._Moods[j] = EditorGUILayout.ColorField(((ScoreManager.ScoreType)j).ToString(), m_Data._Moods[j]); 			
		}
		m_Data._Moods[(int)ScoreManager.ScoreType.Count] = EditorGUILayout.ColorField("Default", m_Data._Moods[(int)ScoreManager.ScoreType.Count]); 			

		UpdateEnd();
	}
}
