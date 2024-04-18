using System.Collections;
using System.Linq;
using UnityEngine;

public class Shape : MonoBehaviour
{
    private const float CELL_SIZE = 0.4f;

    [Header("Movement")]
    [SerializeField] private float xMovementDelay = .2f;
    [SerializeField] private float yMovementDelay = 1;
    private float actualXMovemnetDelay;
    bool canMove = true;

    private int rotationPhase = 1;  // <1, 4>, using to determine rotation
    Transform[] children = new Transform[4];

    private void Start()
    {
        transform.position = new Vector3(-0.2f, 3.8f);
        actualXMovemnetDelay = xMovementDelay;
        InvokeRepeating("MoveDown", 1, yMovementDelay);

        for (int i = 0; i < 4; i++)
        {
            children[i] = transform.GetChild(i);
        }
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

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
    }

    /// <summary>
    /// Moves the shape down by 1 cell.
    /// </summary>
    void MoveDown()
    {
        Transform lastChildY = GetVerticalEdgeChild();

        // checks if the shape touches bottom edge
        if (lastChildY.position.y <= -3.8f)
        {
            StartCoroutine(Freeze());
        }
        else
        {
            StopAllCoroutines();

            foreach (Transform s in children)
            {
                s.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            }
        }

        // moves down the shape
        if(lastChildY.position.y > -3.8f)
            transform.position -= new Vector3(0, CELL_SIZE);
    }

    /// <summary>
    /// The procedure of stopping the shape on the bottom edge.
    /// </summary>
    /// <returns></returns>
    IEnumerator Freeze()
    {
        /*
         * First, changes the color of the shape
         * if the color is changed, stops the movement
         * changing color takes one tick time, so player has one tick time to move the shape on the bottom edge before freez
         */

        float t = 0;
        SpriteRenderer sp = GetComponentInChildren<SpriteRenderer>();

        while (sp.color.g > 0)
        {
            // changes color of each child
            foreach(Transform s in children)
                s.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.magenta, t);

            t += Time.deltaTime / yMovementDelay;
            yield return null;
        }
        canMove = false;
    }

    /// <summary>
    /// Moves the shape horizontally according to the factor which responds for the direction.
    /// </summary>
    /// <param name="dirFactor"></param>
    void MoveHorizontal(float dirFactor)
    {
        Transform edgeChild = GetHorizontalEdgeChild();

        if (Mathf.Abs(edgeChild.position.x + CELL_SIZE * dirFactor) <= 1.8f)
            transform.position += new Vector3(CELL_SIZE * dirFactor, 0);
    }

    /// <summary>
    /// Returns the child nearest the edge.
    /// </summary>
    /// <returns></returns>
    Transform GetHorizontalEdgeChild()
    {
        return children.OrderBy(t => Mathf.Abs(t.transform.position.x)).LastOrDefault();
    }

    /// <summary>
    /// Returns the child nearest the bottm edge.
    /// </summary>
    /// <returns></returns>
    Transform GetVerticalEdgeChild()
    {
        return children.OrderBy(t => Mathf.Abs(t.transform.position.y)).LastOrDefault();
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

        // check if each shape part is on the board, if not, move the shape
        Transform edgeChild = GetHorizontalEdgeChild();
        if (Mathf.Abs(edgeChild.position.x) > 1.8f)
        {
            int dirFactor = (edgeChild.position.x > 0) ? -1 : 1;
            transform.position += new Vector3(CELL_SIZE * dirFactor, 0);
        }

        // prevents the shape to bog in the bottom edge
        if(GetVerticalEdgeChild().position.y < -3.81f)
        {
            transform.position += new Vector3(0, CELL_SIZE);
        }
    }
}
