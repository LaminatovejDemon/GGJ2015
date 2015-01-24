using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ScoreManager))]
public class ScoreManagerEditor : Editor 
{
	ScoreManager m_Data;

	void UpdateBegin()
	{
		if ( m_Data == null )
		{
			m_Data = target as ScoreManager;
		}

		if ( m_Data._Thresholds == null || m_Data._Thresholds.Length < (int)ScoreManager.ScoreType.Count )
		{
			m_Data._Thresholds = new int[(int)ScoreManager.ScoreType.Count];
		}

		if ( m_Data._ForbiddenMasks == null || m_Data._ForbiddenMasks.Length < (int)ScoreManager.ScoreType.Count )
		{
			m_Data._ForbiddenMasks = new int[(int)ScoreManager.ScoreType.Count];
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

		for ( int i = 0; i < m_Data._Thresholds.Length; ++i )
		{
			m_Data._Thresholds[i] = EditorGUILayout.IntSlider(((ScoreManager.ScoreType)i).ToString() + " threshold", m_Data._Thresholds[i], 1, 20);
			EditorGUILayout.LabelField("Forbidden choices");

			EditorGUILayout.BeginHorizontal();
			for ( int j = 0; j < (int)(ScoreManager.ScoreType.Count); ++j )
			{
				if ( j == i )
				{
					continue;
				}

				EditorGUILayout.BeginVertical(GUILayout.MaxWidth(75));
				EditorGUILayout.LabelField(((ScoreManager.ScoreType)j).ToString(), GUILayout.MaxWidth(75));
				if ( EditorGUILayout.Toggle((((int)m_Data._ForbiddenMasks[i]) & 1<<j) != 0, GUILayout.MaxWidth(75)) )
				{
					m_Data._ForbiddenMasks[i] |= 1<<j;
				}
				else
				{
					m_Data._ForbiddenMasks[i] &= ~(1 << j);
				}
				EditorGUILayout.EndVertical();
				
			}
			EditorGUILayout.EndHorizontal();
		}


		UpdateEnd();
	}
}
