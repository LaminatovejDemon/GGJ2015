using UnityEngine;
using System.Collections;

[System.Serializable]
public class Condition
{
	public enum ConditionType
	{
		Invalid,
		Random,
		Agressive,
		Protective,
		Cheerful,
		Questioning,
		Reasonbable,
	}

	public ConditionType _ConditionType;
}
