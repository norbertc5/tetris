using System;
using System.Collections;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField] float minMove = 10;
    [SerializeField] float passMinMove = 10;
    [SerializeField] float movementSpeedBorder = 5;
    [SerializeField] float timeToTap = 1;

    Vector2 startPos;
    Vector2 endPos;
    bool hasInvokedAction;
    Touch touch;
    Vector2 lastEndPos;
    bool isHolding;

    public delegate void SwipeAction(float dir);
    public static Action OnTap;
    public static SwipeAction OnSwipeHorizontal;
    public static Action OnSwipeDown;
    public static Action OnTouchRelease;
    public static Action OnSwipeDownAndRelase;


    void Update()
    {
        if (Input.touchCount <= 0)
            return;

        touch = Input.GetTouch(0);

        switch(touch.phase)
        {
            case TouchPhase.Began:
                startPos = new Vector2(Mathf.Floor(Camera.main.ScreenToWorldPoint(touch.position).x / GameManager.CELL_SIZE) * GameManager.CELL_SIZE,
                    Mathf.Floor(Camera.main.ScreenToWorldPoint(touch.position).y / GameManager.CELL_SIZE) * GameManager.CELL_SIZE);
                isHolding = true;
                StartCoroutine(PassDetect());
                StartCoroutine(TapDetect());
                break;

            case TouchPhase.Moved:
                endPos = new Vector2(Mathf.Floor(Camera.main.ScreenToWorldPoint(touch.position).x / GameManager.CELL_SIZE) * GameManager.CELL_SIZE,
                    Mathf.Floor(Camera.main.ScreenToWorldPoint(touch.position).y / GameManager.CELL_SIZE) * GameManager.CELL_SIZE);
                SwipeHandle();
                break;

            case TouchPhase.Ended:
                endPos = endPos = new Vector2(Mathf.Floor(Camera.main.ScreenToWorldPoint(touch.position).x / GameManager.CELL_SIZE) * GameManager.CELL_SIZE,
                    Mathf.Floor(Camera.main.ScreenToWorldPoint(touch.position).y / GameManager.CELL_SIZE) * GameManager.CELL_SIZE);
                hasInvokedAction = false;
                OnTouchRelease?.Invoke();
                isHolding = false;
                break;
        }
    }


    void SwipeHandle()
    {
        if(Mathf.Abs(endPos.x - lastEndPos.x) > minMove)
        {
            hasInvokedAction = false;
        }

        if (hasInvokedAction)
            return;

        float distX = startPos.x - endPos.x;

        if (distX < (-minMove/2))
        {
            OnSwipeHorizontal?.Invoke(1);
            hasInvokedAction = true;
            lastEndPos = endPos;
        }
        else if (distX > (minMove/2))
        {
            OnSwipeHorizontal?.Invoke(-1);
            hasInvokedAction = true;
            lastEndPos = endPos;
        }

        if ((startPos.y - endPos.y) > passMinMove)
        {
            OnSwipeDown?.Invoke();
            hasInvokedAction = true;
        }
    }

    IEnumerator TapDetect()
    {
        float t = 0;
        while(t <= timeToTap)
        {
            t += Time.deltaTime;

            if(touch.phase == TouchPhase.Ended && startPos == endPos)
            {
                OnTap?.Invoke();
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator PassDetect()
    {
        float t = 0;
        while(t <= timeToTap)
        {
            t += Time.deltaTime;
            if(!isHolding && (startPos.y - endPos.y) > passMinMove && Mathf.Abs(startPos.x - endPos.x) < minMove)
            {
                OnSwipeDownAndRelase?.Invoke();
                yield break;
            }
            yield return null;
        }
    }
}
