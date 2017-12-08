using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CirclePanelScript : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public ItemScript[] Items;
    public float Radius;

    private float mRotatedAngle;
    private float mAngleUnit = 60;

    private Vector2 mMouseLastDir = Vector2.zero;
    private bool mLerp = false;
    private float mLerpTo = 0;
    private float t = 0;


    public static float STEP = 2f;
    public static float THRESHOLD = 3f;

    public Action<int> OnPanelOnPos;

    void Start()
    {
        if (Items.Length > 0)
        {
            mAngleUnit = 360 / Items.Length;

            for (int i = 0; i < Items.Length; i++)
            {
                var rad = mAngleUnit * i * Mathf.Deg2Rad;
                var x = Radius * Mathf.Cos(rad);
                var y = Radius * Mathf.Sin(rad);
                Items[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }
        }


    }

    private void Update()
    {
        // if moving to target position
        if (mLerp)
        {
            t += Time.deltaTime;
            // prevent overshooting with values greater than 1
            float decelerate = Mathf.Min(STEP * t, 1f);
            //Debug.Log ("------>" + decelerate + "   delteTIme:" + Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, mLerpTo), decelerate);
            // time to stop lerping?
            if ((transform.localRotation.eulerAngles.z - mLerpTo) < THRESHOLD)
            {
                // snap to target and stop lerping
                transform.localRotation = Quaternion.Euler(0, 0, mLerpTo);
                mLerp = false;

                // invoke on page lerp end action
                if (OnPanelOnPos != null)
                {
                    OnPanelOnPos(0);
                }
                t = 0;
            }
        }

    }

    private void LerpTo(float delta)
    {
        mLerp = true;
        mLerpTo = transform.eulerAngles.z + delta;

        Debug.Log("To Rotate:" + mLerpTo + "  delta:" + delta);
    }


    public void OnDrag(PointerEventData data)
    {
        var worldPos = Camera.main.ScreenToWorldPoint(data.position);
        var localPos = worldPos - transform.position;
        Vector2 mouseDir = new Vector2(localPos.x, localPos.y);
        float delta = mMouseLastDir == Vector2.zero ? 0 : Vector2.SignedAngle(mMouseLastDir, mouseDir);

        Debug.Log("" + "   " + mRotatedAngle + "   " + worldPos + "   " + data.position + "   " + delta);


        transform.localRotation = Quaternion.Euler(0, 0, mRotatedAngle + delta);

        mRotatedAngle = (360 + transform.eulerAngles.z) % 360;
        mMouseLastDir = mouseDir;
    }

    private float FindNearestPosition()
    {
        var ang = mRotatedAngle % mAngleUnit;

        //Debug.Log("all: " + mRotatedAngle + "   unit:" + mAngleUnit + "   delta:" +ang);
        if (ang > mAngleUnit / 2)
        {
            return mAngleUnit - ang;
        }
        else
        {
            return -ang;
        }
    }

    #region IPointerUpHandler implementation

    public void OnPointerUp(PointerEventData eventData)
    {
        mMouseLastDir = Vector2.zero;
        if (mAngleUnit != 0 && mRotatedAngle % mAngleUnit != 0)
        {
            var toRotate = FindNearestPosition();


            LerpTo(toRotate);
        }
    }

    #endregion

    #region IPointerDownHandler implementation

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    #endregion



}
