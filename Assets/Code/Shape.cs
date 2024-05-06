using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class Shape : MonoBehaviour
{
    /*
     * This class contains:
     * -movement for the shape,
     * -horizontal steering,
     * -rotating,
     * -stopping on an obstacle,
     * -clamping to don't bog beyond edges,
     * -clamping on other shapes while moving horizontal and rotating.
     * */
    public bool hasStartedFreezing = false;
    [HideInInspector] public bool canMove = true;

    [Header("Movement")]
    [SerializeField] private float xMovementDelay = .2f;
    [SerializeField] private float yMovementDelay = 1;
    [SerializeField] private float speedUpFactor = 2;
    private float actualXMovemnetDelay;
    private int rotationPhase = 1;  // <1, 4>, using to determine rotation
    private float actualSpeedUp;
    private const float CELL_SIZE = 0.4f;

    Transform[] children = new Transform[4];
    GameManager gameManager;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            children[i] = transform.GetChild(i);
        }
        gameManager = FindObjectOfType<GameManager>();

        transform.position = new Vector3(-0.2f, 3.8f);
        actualXMovemnetDelay = xMovementDelay;
        StartCoroutine(MoveDown());
        Invoke("FindFloor", .1f);  // on start is must be invoked with small delay
    }

    private void Update()
    {
        if (!canMove)
            return;

        #region Horizontal movement

        // get input and move the shape if delay is valid
        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0 && actualXMovemnetDelay <= 0) 
        {
            MoveHorizontal(input);
            actualXMovemnetDelay = xMovementDelay;
        }
        if (actualXMovemnetDelay > 0)
            actualXMovemnetDelay -= Time.deltaTime;

        #endregion

        #region Rotation

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }

        #endregion

        #region Speed up

        if (Input.GetKeyDown(KeyCode.DownArrow))
            actualSpeedUp = speedUpFactor;
        if (Input.GetKeyUp(KeyCode.DownArrow))
            actualSpeedUp = 0;

        #endregion
    }

    /// <summary>
    /// Moves the shape down by 1 cell.
    /// </summary>
    IEnumerator MoveDown()
    {
        while (canMove)
        {
            yield return new WaitForSeconds(yMovementDelay - actualSpeedUp);

            // checks if the shape can move, if not freeze it
            if (!hasStartedFreezing && GetVerticalEdgeChild().position.y <= -3.8f)
            {
                StartCoroutine(Freeze());
                hasStartedFreezing = true;
            }
            // moves down the shape
            if (!hasStartedFreezing)
                transform.position -= new Vector3(0, CELL_SIZE);


            yield return null;
        }
    }

    /// <summary>
    /// The procedure of stopping the shape on the bottom edge.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Freeze()
    {
        /*
         * First, changes the color of the shape
         * if the color is changed, stops the movement
         * if player has moved while freezing, process stops
         */

        float t = 0;
        SpriteRenderer sp = GetComponentInChildren<SpriteRenderer>();
        Transform lastChild = GetVerticalEdgeChild();
        Vector3 lastChildPos = lastChild.position;

        while (sp.color.g > 0)
        {
            // changes color of each child
            foreach(Transform s in children)
                s.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.magenta, t);

            t += Time.deltaTime / yMovementDelay;

            // stop freezing when moved
            if (lastChild.position != lastChildPos)
            {
                hasStartedFreezing = false;
                yield break;
            }

            yield return null;
        }
        canMove = false;
        GameManager.OnShapeArrived?.Invoke();
        foreach(Transform c in children)
        {
            c.gameObject.layer = 0;
        }

    }

    /// <summary>
    /// Moves the shape horizontally according to the factor which responds for the direction.
    /// </summary>
    /// <param name="dirFactor"></param>
    void MoveHorizontal(float dirFactor)
    {
        foreach (Transform child in children)
        {
            if (!child.GetComponent<Piece>().ThrowRayHorizontal(dirFactor))
                return;
        }

        transform.position += new Vector3(CELL_SIZE * dirFactor, 0);
        FindFloor();
    }

    /// <summary>
    /// Returns the child nearest the edge.
    /// </summary>
    /// <returns></returns>
    public Transform GetHorizontalEdgeChild(bool nearestDirectly = true, bool getLeftExtreme = false)
    {
        if(getLeftExtreme)
            return children.OrderBy(t => t.transform.position.x).FirstOrDefault();

        if (nearestDirectly)
            return children.OrderBy(t => Mathf.Abs(t.transform.position.x)).LastOrDefault();
        else
            return children.OrderBy(t => t.transform.position.x).LastOrDefault();
    }

    /// <summary>
    /// Returns the child nearest the bottm edge.
    /// </summary>
    /// <returns></returns>
    Transform GetVerticalEdgeChild()
    {
        return children.OrderBy(t => t.transform.localPosition.y).FirstOrDefault();
    }

    /// <summary>
    /// Rotates the shape.
    /// </summary>
    void Rotate()
    {
        /*
         * We can't simply rotate whole shape because it makes shape goes beyond the edges
         * So it's a bit more sophisticated
         * We change localPositon of each shape part
         */

        #region Clamping on shapes

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 10);
        float rightEdge = 1.8f, leftEdge = -1.8f;

        if(hit.transform.CompareTag("Piece"))
        {
            rightEdge = hit.point.x;
            if (rightEdge > 0)
                rightEdge += .1f;
            rightEdge = (float)Math.Round(rightEdge, 1);
        }

        hit = Physics2D.Raycast(transform.position, Vector2.left, 10);
        if (hit.transform.CompareTag("Piece"))
        {
            leftEdge = hit.point.x;
            if (leftEdge < 0)
                leftEdge -= .1f;
            leftEdge = (float)Math.Round(leftEdge, 1);
        }

        //Debug.Log($"{leftEdge}, {rightEdge}");

        // if overlapping other shape, return
        if ((Mathf.Abs(rightEdge - leftEdge) <= (CELL_SIZE * 2) && (Mathf.Abs(rightEdge - leftEdge) != 0
            || (rightEdge == 0.6f && leftEdge == -0.6f)))
            || Mathf.Abs(rightEdge - leftEdge) == 1.2f)
                return;

        #endregion

        rotationPhase++;

        for (int i = 0; i < 4; i++)
        {
            Transform child = transform.GetChild(i);
            child.localPosition = new Vector3(child.localPosition.y, child.localPosition.x);

            if (rotationPhase == 1 || rotationPhase == 3)
                child.localPosition = -child.localPosition;
            if (rotationPhase > 4)
                rotationPhase = 1;
        }

        if (Mathf.Abs(gameManager.RoundFloat(GetVerticalEdgeChild().position.y, 1)) >= Mathf.Abs(gameManager.RoundFloat(GetVerticalEdgeChild().GetComponent<Piece>().freezePos.y, 1)))
            transform.position += new Vector3(0, CELL_SIZE);

        // check if each shape part is on the board, if not, move the shape
        Transform edgeChild = GetHorizontalEdgeChild();
        if (Mathf.Abs(edgeChild.position.x) > 1.8f)
        {
            int dirFactor = (edgeChild.position.x > 0) ? -1 : 1;
            transform.position += new Vector3(CELL_SIZE * dirFactor, 0);
        }

        // prevents the shape to bog in the bottom edge
        if (GetVerticalEdgeChild().position.y < -3.81f)
            transform.position += new Vector3(0, CELL_SIZE);

        FindFloor();
    }

    /// <summary>
    /// Finds the 'floor ' for all pieces. Using to stop on obstacels. Also updates ghost.
    /// </summary>
    void FindFloor()
    {
        foreach(Transform child in children)
        {
            child.GetComponent<Piece>().ThrowRayVertical();
        }

        #region Updating ghost

        float yPosAddition = 0;
        Piece lastYPosPiece = GetVerticalEdgeChild().GetComponent<Piece>();
        Piece lastFreezePosPiece = children.OrderBy(t => t.GetComponent<Piece>().freezePos.y).LastOrDefault().GetComponent<Piece>();

        // check if lastYPosPiece and lastFreezePosPiece freeze positions are the same. If true lift up the ghost
        if (lastYPosPiece.transform.localPosition.y == -1 && (lastFreezePosPiece.freezePos.y <= -3.8f || 
            (Mathf.Abs(gameManager.RoundFloat(lastYPosPiece.freezePos.y, 1)) - Mathf.Abs(gameManager.RoundFloat(lastFreezePosPiece.freezePos.y, 1)) == 0)))
        {
            yPosAddition = CELL_SIZE;          
        }
        //Debug.Log(lastFreezePosPiece.freezePos.y + yPosAddition);
        gameManager.SetGhost(new Vector2(transform.position.x, lastFreezePosPiece.freezePos.y + yPosAddition), new Vector3(0, 0, -90 * (rotationPhase - 1)));

        #endregion
    }
}
