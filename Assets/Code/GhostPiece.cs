using UnityEngine;
using System;

public class GhostPiece : MonoBehaviour
{
    Ghost ghost;

    void Start()
    {
        ghost = GetComponentInParent<Ghost>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // move the ghost up when bogging and adds it's y pos to list to prevent too big lift
        float ypos = (float)Math.Round(transform.position.y, 1);
        if (collision.gameObject.layer != 2 && !ghost.yPosesUsedToLift.Contains(ypos - 0.4f))
        {
            transform.parent.position += new Vector3(0, 0.4f);
            ghost.gfxPos += new Vector3(0, 0.4f);
            ghost.yPosesUsedToLift.Add(ypos);
        }
    }
}
