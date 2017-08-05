using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTest : MonoBehaviour 
{

	public List<DialogParticipant> people;
	

	void Update () 
	{
		if( Input.GetKeyDown(KeyCode.Space) )
		{
			DialogController.Create( people.ToArray() );
		}
	}
}
