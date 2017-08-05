using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUI : MonoBehaviour 
{
	public Transform followObj;
	public Vector3 offset;

	void LateUpdate () 
	{
		if( followObj != null )
			transform.position = Camera.main.WorldToScreenPoint( new Vector3(followObj.position.x, followObj.position.y, 0f) + offset  );
	}
}
