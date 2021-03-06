﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogParticipant : MonoBehaviour 
{

	public DialogController conversation;

	public bool inConversation{ get{ return conversation != null; } }

	public AudioClip conversationJoin;
	public AudioClip collisionAudio;

	//public bool isTalking{ get{ return conversation.

	void OnCollisionEnter2D(Collision2D col)
	{
		if( this.inConversation )
			return;

		DialogParticipant other = col.gameObject.GetComponent<DialogParticipant>();
        if (other != null)
        {

            if (!other.GetComponent<FervorBucket>().IsConverted() && !this.GetComponent<FervorBucket>().IsConverted())
            {
                return;
            }

			if( other.inConversation == true )
			{
				//other.conversation.Add( this );

				//AudioHelper.PlayClipAtPoint( collisionAudio, transform.position, 0.5f, SoundType.Effect, transform );
			}
			else
			{
				DialogController.Create( new DialogParticipant[]{ this, other } );
				//AudioHelper.PlayClipAtPoint( conversationJoin, transform.position, 0.5f, SoundType.Effect, transform );
			}

		}
	}
}
