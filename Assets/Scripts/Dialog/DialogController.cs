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
		followTarget.owner = this;
	}

	public int count{ get{ return participants.Count; } }

	public Vector3 avgPosition
	{
		get{
			//List<Vector3> positions = new List<Vector3>();
			Vector3 sum = Vector3.zero;
			for (int i = 0; i < participants.Count; i++) 
			{
				sum += participants[i].transform.position;
			}
			return sum / participants.Count;
		}
	}

	public List<DialogParticipant> participants;
	private DialogParticipant _talking;
	public DialogParticipant talking
	{
		get{ return _talking; }
		set{
			_talking = value;
		}
	}

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
					this.talking = participants[ UnityEngine.Random.Range(0, participants.Count) ];
					this.display.StartDisplay( DialogDefs.singleton.SelectRandom(table).text );
				}
			};
		}
	}

	public void StartConversation()
	{
		if( DialogDefs.singleton == null )
			Debug.Log("u dun goof");

		table = 0;

		display.onComplete = recursive;
		this.talking = participants[ UnityEngine.Random.Range(0, participants.Count) ];
		display.StartDisplay( DialogDefs.singleton.SelectRandom(table).text );
	}

	public bool IsParticipating( DialogParticipant person )
	{
		for (int i = 0; i < participants.Count; i++) 
		{
			if( participants[i] == person )
				return true;
		}
		return false;
	}

	void Update()
	{
		followTarget.targetPos = avgPosition;
	}
}
