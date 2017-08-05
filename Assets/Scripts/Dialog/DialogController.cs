using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DisplayString))]
public class DialogController : MonoBehaviour 
{
	private DisplayString display;
	void Awake()
	{
		display = GetComponent<DisplayString>();
	}

	void Start()
	{
		StartConversation();
	}

	public List<GameObject> participants;

	public void Add( GameObject newParticipant )
	{
		participants.Add( newParticipant );
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

		display.onComplete = recursive;
		display.StartDisplay( DialogDefs.singleton.SelectRandom(table).text );
	}

}
