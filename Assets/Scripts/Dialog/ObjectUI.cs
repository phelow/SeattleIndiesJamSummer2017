using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Speech UI
/// </summary>
public class ObjectUI : MonoBehaviour 
{
	public DialogController owner;

	public Vector3 targetPos;
	public Vector3 offset;

	public float padding = 30f;

	private RectTransform speechWindow;
	public RectTransform speechTriangle;

	public float triangleHeightOffset = -55f;

	void Awake()
	{
		speechWindow = GetComponent<RectTransform>();
	}

	void LateUpdate () 
	{

		Vector3 screenPos = Camera.main.WorldToScreenPoint( new Vector3(targetPos.x, targetPos.y, 0f) );
		transform.position = screenPos + offset;

		Vector3[] bounds = new Vector3[4];
		speechWindow.GetWorldCorners( bounds );

		float min = bounds[0].x + padding;
		float max = bounds[3].x - padding;


<<<<<<< HEAD
        //	Debug.Log( min );
        //	Debug.Log( max );
=======
	//	Debug.Log( min );
	//	Debug.Log( max );
>>>>>>> b24a571... Stuff

        float x = 0.0f ;
        try
        {
            x = Mathf.Clamp(Camera.main.WorldToScreenPoint(owner.talking.transform.position).x, min, max);

<<<<<<< HEAD
        }
        catch
        {
            Destroy(this.gameObject);

        }
=======
>>>>>>> b24a571... Stuff
//		Debug.Log( x );

		speechTriangle.transform.position = new Vector3( x, bounds[0].y + triangleHeightOffset, 0f );

	}
}
