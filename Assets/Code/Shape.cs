using UnityEngine;
using System.Linq;

public class Shape : MonoBehaviour
{
    private const float CELL_SIZE = 0.4f;

    [Header("Movement")]
    [SerializeField] private float xMovementDelay = .2f;
    private float actualXMovemnetDelay;

    private int rotationPhase = 1;  // <1, 4>, using to determine rotation

    private void Start()
    {
        transform.position = new Vector3(-0.2f, 3.8f);
        actualXMovemnetDelay = xMovementDelay;
        InvokeRepeating("MoveDown", 1, 1);
    }

    private void Update()
    {
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
        transform.position -= new Vector3(0, CELL_SIZE);
    }

    /// <summary>
    /// Moves the shape horizontally according to the factor which responds for the direction.
    /// </summary>
    /// <param name="dirFactor"></param>
    void MoveHorizontal(float dirFactor)
    {
        Transform edgeChild = GetEdgeChild();

        if (Mathf.Abs(edgeChild.position.x + CELL_SIZE * dirFactor) <= 1.8f)
            transform.position += new Vector3(CELL_SIZE * dirFactor, 0);
    }

    /// <summary>
    /// Returns the child nearest the edge.
    /// </summary>
    /// <returns></returns>
    Transform GetEdgeChild()
    {
        Transform[] children = new Transform[4];
        for (int i = 0; i < 4; i++) { children[i] = transform.GetChild(i); }
        return children.OrderBy(t => Mathf.Abs(t.transform.position.x)).LastOrDefault();
    }

    /*bool IsOnEdge(float nextMoveXChange)
    {
        for (int i = 0; i < 4; i++)
        {
            Transform child = transform.GetChild(i);
            if (Mathf.Abs(child.position.x + nextMoveXChange) > 1.8f)
                return true;
        }

        return false;
    }*/

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
        Transform edgeChild = GetEdgeChild();
        if (Mathf.Abs(edgeChild.position.x) > 1.8f)
        {
            int dirFactor = (edgeChild.position.x > 0) ? -1 : 1;
            transform.position += new Vector3(CELL_SIZE * dirFactor, 0);
        }
    }
}
