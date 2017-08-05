using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DisplayString : MonoBehaviour 
{
	//private AudioSource dialogAudio;
	public TextMeshProUGUI textMesh;

	void Awake () 
	{
		//dialogAudio = GetComponent<AudioSource>();

		StartCoroutine(Display());
	}

	public float characterDelay = 0.1f;
	private string _fullText = "Seattle Indies Jam August 2017 ";
	public string text
	{
		get{ return _fullText; }
		set{
			charIndex = 0;
			_fullText = value + " ";

			if( routine != null )
				StopCoroutine( routine );
			routine = StartCoroutine( Display() );
		}
	}
	private Coroutine routine;

	public int charIndex = 0;
	public Action onComplete = null;

	public void StartDisplay( string text )
	{
		this.text = text;
	}

	IEnumerator Display()
	{
		yield return null; //Skip a frame so other scripts have time to initalize

		for (int i = 0; i < _fullText.Length; i++) 
		{
			charIndex = i;


			DisplayPartial( _fullText, charIndex );

			yield return new WaitForSeconds( characterDelay );
		}

		yield return new WaitForSeconds( characterDelay );

		if( onComplete != null )
			onComplete();
	}

	void DisplayPartial( string full, int charCount )
	{
		textMesh.text = full.Remove( charCount );
	}
}
