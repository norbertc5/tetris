using UnityEngine;
using System;
using System.Collections;

public class Piece : MonoBehaviour
{
    public Vector3 freezePos;
    public bool hasBeenCounted;  // to detect full lines
    Shape shape;
    GameManager gameManager;

    void Start()
    {
        gameObject.layer = 2;
        shape = GetComponentInParent<Shape>();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.pieces.Add(this);
        GameManager.OnLineCleard += ClearLineCheck;
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

    /// <summary>
    /// Checks if this piece is on clearing line.
    /// </summary>
    /// <param name="cleardLinePos"></param>
    void ClearLineCheck(float cleardLinePos)
    {
        if (!enabled)
            return;

        if ((float)Math.Round(transform.position.y, 1) == cleardLinePos)
            StartCoroutine(Disappear());
        else if (transform.position.y > cleardLinePos)
            Invoke("MovePieceDown", 1);
    }

    void MovePieceDown()
    {
        transform.position -= new Vector3(0, .4f);
    }

    /// <summary>
    /// Fade the piece.
    /// </summary>
    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(.1f);  // to setup color properly
        GetComponent<BoxCollider2D>().enabled = false;
        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        Color firstColor = sp.color;
        float t = 0;

        while (sp.color.a > 0)
        {
            sp.color = Color.Lerp(firstColor, new Color(firstColor.r, firstColor.g, firstColor.b, 0), t);
            t += Time.deltaTime;
            yield return null;
        }
        enabled = false;
        gameObject.SetActive(false);
        Shape.LineDisappeard?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // moves the shape when overlapping obstacle
        if(collision.CompareTag("Piece") && shape.canMove)
        {
            if (shape.GetVerticalEdgeChild() == transform)
                transform.parent.position += new Vector3(0, GameManager.CELL_SIZE);
            else
            {
                if(transform.position.x < transform.parent.position.x)
                    transform.parent.position += new Vector3(GameManager.CELL_SIZE, 0);
                else
                    transform.parent.position -= new Vector3(GameManager.CELL_SIZE, 0);
            }

            foreach (Transform child in shape.children)
                child.GetComponent<Piece>().ThrowRayVertical();
        }
    }
}
