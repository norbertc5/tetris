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
        if (collision.gameObject.layer != 2 && !ghost.yPosesUsedToLift.Contains(ypos - GameManager.CELL_SIZE) &&
            (ypos == (float)Math.Round(collision.transform.position.y, 1) || collision.name == "GameManager"))
        {
            ghost.yPosesUsedToLift.Add(ypos);
            //print($"{name}: {ypos}, collide: {collision.name}: {collision.transform.position.y}");
            transform.parent.position += new Vector3(0, GameManager.CELL_SIZE);
            ghost.gfxPos += new Vector3(0, GameManager.CELL_SIZE);
        }
    }
}
