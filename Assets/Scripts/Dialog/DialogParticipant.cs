using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogParticipant : MonoBehaviour 
{

	public DialogController conversation;

	public bool inConversation{ get{ return conversation != null; } }

	//public bool isTalking{ get{ return conversation.

	void OnCollisionEnter2D(Collision2D col)
	{
		if( this.inConversation )
			return;

		DialogParticipant other = col.gameObject.GetComponent<DialogParticipant>();
		if( other != null )
		{

			if( other.inConversation == true )
			{
				other.conversation.Add( this );
			}
			else
			{
				DialogController.Create( new DialogParticipant[]{ this, other } );
			}

		}
	}
}
