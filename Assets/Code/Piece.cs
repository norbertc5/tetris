using System.Linq;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class Piece : MonoBehaviour
{
    public Vector3 freezePos;
    Shape shape;

    void Start()
    {
        gameObject.layer = 2;
        shape = GetComponentInParent<Shape>();
    }

    void Update()
    {
        // if in freeze point, freeze the shape
        if(transform.position == freezePos && !shape.hasStartedFreezing)
        {
            StartCoroutine(shape.Freeze());
            shape.hasStartedFreezing = true;
        }
    }

    /// <summary>
    /// Throws ray and finds freezePos i.e. point to stop at.
    /// </summary>
    public void ThrowRayVertical()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
        float addition = 0;
        if (hit.collider.CompareTag("Piece"))
            addition = .1f;
        freezePos = hit.point + new Vector2(0, 0.2f + addition);
    }

    /// <summary>
    /// Throws ray horizontal and finds position of obstacles. To clamp horizontal movement.
    /// </summary>
    /// <param name="dir"></param>
    public bool ThrowRayHorizontal(float dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 10);
        Vector2 rightHitPos = hit.point;
        hit = Physics2D.Raycast(transform.position, Vector2.left, 10);
        Vector2 leftHitPos = hit.point;

        if ((transform.position.x + 0.4f) >= rightHitPos.x && dir == 1)
            return false;
        if ((transform.position.x - 0.4f) <= leftHitPos.x && dir == -1)
            return false;

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // moves the shape when overlapping obstacle
        if(collision.CompareTag("Piece") && shape.canMove)
        {
            float d = transform.localPosition.x;

            if(d == 0)
                d = -shape.GetHorizontalEdgeChild(true, true).localPosition.x;

            if(shape.GetVerticalEdgeChild() == transform)
                transform.parent.position += new Vector3(0, 0.4f);
            else
                transform.parent.position += new Vector3(0.4f * -d, 0);

            foreach (Transform child in shape.children)
                child.GetComponent<Piece>().ThrowRayVertical();
        }
    }
}
