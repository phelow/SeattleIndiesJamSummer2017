using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DisplayString), typeof(ObjectUI))]
public class DialogController : MonoBehaviour 
{
	private const string prefabLocation = "Prefabs/UI/Conversation Window";
	public static GameObject prefab;
	public static Transform canvas;

	public static DialogController Create( params DialogParticipant[] participants )
	{
		if( prefab == null )
			prefab = Resources.Load<GameObject>( prefabLocation );
		if( canvas == null )
			canvas = FindObjectOfType<Canvas>().transform;

		//TODO create script for obj positions

		GameObject obj = Instantiate<GameObject>( prefab, canvas, false );
		DialogController dc = obj.GetComponent<DialogController>();

		foreach (var person in participants) 
		{
			dc.Add( person );
		}

		dc.StartConversation();

		return dc;
	}

	private ObjectUI followTarget;
	private DisplayString display;
	void Awake()
	{
		display = GetComponent<DisplayString>();
		followTarget = GetComponent<ObjectUI>();
	}

	public int count{ get{ return participants.Count; } }

	public List<DialogParticipant> participants;

	public void Add( DialogParticipant newParticipant )
	{
		participants.Add( newParticipant );
		newParticipant.conversation = this;
	}

	public int table = 0;

	public Action recursive
	{
		get{
			return delegate() 
			{
				this.table++;

				if( this.table < DialogDefs.singleton.tables.Count )
				{
					this.display.StartDisplay( DialogDefs.singleton.SelectRandom(table).text );
				}
			};
		}
	}

	public void StartConversation()
	{
		table = 0;

		if( DialogDefs.singleton == null )
			Debug.Log("aaa");
		display.onComplete = recursive;
		display.StartDisplay( DialogDefs.singleton.SelectRandom(table).text );
	}

}
