using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CirclePanelScript : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler
{
	public ItemScript[] Items;
	public float Radius;

	private float mRotatedAngle;
	private float mAngleUnit = 60;

	public Action<int> OnPanelOnPos;

	void Start ()
	{
		if (Items.Length > 0) {
			mAngleUnit = 360 / Items.Length;

			for (int i = 0; i < Items.Length; i++) {
				var rad = mAngleUnit * i * Mathf.Deg2Rad;
				var x = Radius * Mathf.Cos (rad);
				var y = Radius * Mathf.Sin (rad);
				Items [i].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (x, y);
			}
		}


	}

	public void OnDrag (PointerEventData data)
	{
		var worldPos = Camera.main.ScreenToWorldPoint (data.position);
		var localPos = worldPos - transform.position;
		Vector2 mouseLocalPostiontion = new Vector2 (localPos.x, localPos.y);
		float rotate = Vector2.Angle (Vector2.down, mouseLocalPostiontion); 
//		Vector3 currentPos = new Vector3 (mouseLocalPostiontion.x, mouseLocalPostiontion.y, 0);
//		Vector3 dirc = Vector3.Cross (currentPos, new Vector3 (1, 1, 0));
//		if (dirc.z > 0)
//			rotate = -rotate;


		var delta = Mathf.Abs (rotate - transform.localRotation.eulerAngles.z) % 90;
//		Debug.Log ("" + rotate + "   " + mRotatedAngle + "   " + worldPos + "   " + data.position);

		mRotatedAngle += delta;
		transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, rotate));
	}

	private float FindNearestPosition ()
	{
		var ang = mRotatedAngle % mAngleUnit;
		if (ang > mAngleUnit / 2) {
			return mAngleUnit - ang;
		} else {
			return -ang;
		}
	}

	#region IPointerUpHandler implementation

	public void OnPointerUp (PointerEventData eventData)
	{
		if (mAngleUnit != 0 && mRotatedAngle % mAngleUnit != 0) {
			var toRotate = FindNearestPosition ();
			Debug.Log ("To Rotate:" + toRotate);
			transform.Rotate (0, 0, toRotate); // TODO:use doTween

//			OnPanelOnPos (mRotatedAngle / mAngleUnit % Items.Length);
		}
	}

	#endregion

	#region IPointerDownHandler implementation

	public void OnPointerDown (PointerEventData eventData)
	{
		
	}

	#endregion


	 
}
