using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OVP.UI
{
	/*
	public class Minimap : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
	{
		private bool shouldPan = false;
		private Camera minimap{ get{ return CameraManager.singleton.minimap; } }
		new private RectTransform transform;

		void Awake()
		{
			transform = GetComponent<RectTransform>();
		}

		/// <summary>
		/// A click started on the minimap. Called from UI.
		/// </summary>
		public void OnPointerDown( PointerEventData data )
		{
			shouldPan = true;
		}

		/// <summary>
		/// Cancels panning the camera because the mouse left our area. Called from UI.
		/// </summary>
		public void OnPointerExit( PointerEventData data )
		{
			shouldPan = false;
		}

		/// <summary>
		/// Pans the main camera to the touched location on the minimap. Called from UI.
		/// </summary>
		public void OnPointerUp( PointerEventData data )
		{
			if( shouldPan == false )
				return;
			shouldPan = false;

			Vector2 halfSize = new Vector2( transform.rect.width / 2f, transform.rect.height / 2f );


			Vector2 localCursor;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, data.position, data.pressEventCamera, out localCursor))
				return;

			//Remove the pivot from our calculation. (Assumes our pivot is 0.5, 0.5)
			Vector2 viewportPoint = new Vector2(
				VectorExtras.ReverseLerp( localCursor.x, -halfSize.x, halfSize.x ),
				VectorExtras.ReverseLerp( localCursor.y, -halfSize.y, halfSize.y )
			);

			RaycastHit hit;
			if(Physics.Raycast(minimap.ViewportPointToRay( new Vector3(viewportPoint.x, viewportPoint.y, 0f) ), out hit) == false)
				return;

			#if UNITY_EDITOR
			DebugExtension.DebugPoint( new Vector3(hit.point.x, hit.point.y, 0f), 1f, 2f );
			#endif

			CameraManager.singleton.target = new Vector3(hit.point.x, hit.point.y, 0f);
		}

	}
	*/
}