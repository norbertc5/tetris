using System;
using System.Collections;
using System.Linq;
using UnityEngine;

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
    [HideInInspector] public bool hasStartedFreezing = false;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public char shape;

    [Header("Movement")]
    [SerializeField] private float xMovementDelay = .2f;
    [SerializeField] private float yMovementDelay = 1;
    [SerializeField] private float speedUpFactor = 2;
    private float actualXMovemnetDelay;
    private float actualSpeedUp;

    public Transform[] children = new Transform[4];
    Ghost ghost;

    public delegate void Action();
    public static Action LineDisappeard;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            children[i] = transform.GetChild(i);
        }
        ghost = FindObjectOfType<Ghost>();

        transform.position = new Vector3(-0.2f, 3.8f);
        actualXMovemnetDelay = xMovementDelay;
        StartCoroutine(MoveDown());
        Invoke("FindFloor", .1f);  // on start is must be invoked with small delay
        LineDisappeard += FindFloor;
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
            if (!hasStartedFreezing && GetVerticalEdgeChild().position.y <= GetVerticalEdgeChild().GetComponent<Piece>().freezePos.y)
            {
                StartCoroutine(Freeze());
                hasStartedFreezing = true;
            }
            // moves down the shape
            if (!hasStartedFreezing)
                transform.position -= new Vector3(0, GameManager.CELL_SIZE);


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
        Color firstColor = sp.color;

        while (sp.color != Color.white)
        {
            // changes color of each child
            foreach(Transform s in children)
                s.GetComponent<SpriteRenderer>().color = Color.Lerp(firstColor, Color.white, t);

            t += Time.deltaTime / yMovementDelay;

            // stop freezing when moved
            if (lastChild.position != lastChildPos)
            {
                hasStartedFreezing = false;
                foreach (Transform s in children)
                    s.GetComponent<SpriteRenderer>().color = firstColor;
                yield break;
            }

            yield return null;
        }
        canMove = false;
        GameManager.OnShapeArrived?.Invoke();
        foreach(Transform c in children)
        {
            c.GetComponent<SpriteRenderer>().color = firstColor;
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

        transform.position += new Vector3(GameManager.CELL_SIZE * dirFactor, 0);
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
    public Transform GetVerticalEdgeChild()
    {
        return children.OrderBy(t => t.transform.position.y).FirstOrDefault();
    }

    /// <summary>
    /// Rotates the shape.
    /// </summary>
    void Rotate()
    {
        #region Clamping on shapes

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 10);
        float rightEdge = GameManager.HORIZONTAL_EDGE, leftEdge = -GameManager.HORIZONTAL_EDGE, spaceToRotate = 1.2f;

        if(hit.transform.CompareTag("Piece"))
        {
            rightEdge = hit.point.x;
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


        if (shape == 'I')
            spaceToRotate = 1.6f;

        // if overlapping other shape, return
        if ((Mathf.Abs(rightEdge - leftEdge) <= (GameManager.CELL_SIZE * 2) && Mathf.Abs(rightEdge - leftEdge) != 0)         
            || Mathf.Abs(rightEdge - leftEdge) <= spaceToRotate)
                return;

        Transform rightExtreme = children.OrderBy(t => t.position.x).LastOrDefault();
        Transform leftExtreme = children.OrderBy(t => t.position.x).FirstOrDefault();

        if (rightEdge == 0.6f && leftEdge == -0.6f)
            return;

        #endregion

        transform.eulerAngles += new Vector3(0, 0, 90);

        // check if each shape part is on the board, if not, move the shape
        for (int i = 0; i < 2; i++)
        {
            Transform edgeChild = GetHorizontalEdgeChild();
            if (Mathf.Abs(edgeChild.position.x) > GameManager.HORIZONTAL_EDGE)
            {
                int dirFactor = (edgeChild.position.x > 0) ? -1 : 1;
                transform.position += new Vector3(GameManager.CELL_SIZE * dirFactor, 0);
            }
        }

        // prevents the shape to bog in the bottom edge
        if (GetVerticalEdgeChild().position.y < GameManager.LAST_LINE_Y_POS + .1f)
            transform.position += new Vector3(0, GameManager.CELL_SIZE);

        FindFloor();
    }

    /// <summary>
    /// Finds the 'floor ' for all pieces. Using to stop on obstacels. Also updates ghost.
    /// </summary>
    void FindFloor()
    {
        foreach (Transform child in children)
            child.GetComponent<Piece>().ThrowRayVertical();

        #region Updating ghost

        Piece lastFreezePosPiece = children.OrderBy(t => t.GetComponent<Piece>().freezePos.y).LastOrDefault().GetComponent<Piece>(); 
        ghost.SetGhost(new Vector3(transform.position.x, lastFreezePosPiece.freezePos.y, .5f), transform.localEulerAngles);

        #endregion

    }
}
