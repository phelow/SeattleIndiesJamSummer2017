using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogParticipant : MonoBehaviour 
{

	public DialogController conversation;

	public bool inConversation{ get{ return conversation != null; } }

	void OnCollisionEnter2D(Collision2D col)
	{
		DialogParticipant other = col.gameObject.GetComponent<DialogParticipant>();
		if( other != null )
		{

			if( other.inConversation == true )
			{

			}

		}
	}
}
