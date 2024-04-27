using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void ThrowRay()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
        freezePos = hit.point + new Vector2(0, 0.2f);
    }
}
