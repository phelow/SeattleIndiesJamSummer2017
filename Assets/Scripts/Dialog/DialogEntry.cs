using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class DialogEntry
{
	[Range(0f, 100f)] 
	public float persuasion = 50f;

	[Multiline] 
	public string text;
}

[System.Serializable]
public class DialogTable
{
	public List<DialogEntry> entries;
}