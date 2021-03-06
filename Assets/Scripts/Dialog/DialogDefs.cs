﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton for dialog system data.
/// </summary>
[System.Serializable]
public class DialogDefs : MonoBehaviour 
{
	#region singleton
	private static DialogDefs _instance;
	public static DialogDefs singleton
	{
		get{ return _instance; }
	}

	void Awake()
	{
		if( _instance == null )
			_instance = this;
		else
			Debug.LogError("There is already a DialogDefs in the scene!");
	}
	#endregion

	[SerializeField] public List<DialogTable> tables;

	public DialogEntry SelectRandom( int table )
	{
		return tables[table].entries[ Random.Range( 0, tables[table].entries.Count ) ];
	}


}

