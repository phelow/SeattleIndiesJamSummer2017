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

	public float triangleHeightOffset = 0f;

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


        //	Debug.Log( min );
        //	Debug.Log( max );

        float x = 0.0f ;
        try
        {
            x = Mathf.Clamp(Camera.main.WorldToScreenPoint(owner.talking.transform.position).x, min, max);

        }
        catch
        {
            Destroy(this.gameObject);

        }
//		Debug.Log( x );

		speechTriangle.transform.position = new Vector3( x, bounds[0].y + triangleHeightOffset, 0f );

	}
}
