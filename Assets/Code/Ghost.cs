using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    bool hasAdjustedPos;
    TetrominoData tetrominoData;

    private void Start()
    {
        tetrominoData = FindObjectOfType<TetrominoData>();
    }

    public void SetGhost(Vector3 pos, Vector3 rot)
    {
        transform.position = pos;
        transform.eulerAngles = rot;
        hasAdjustedPos = false;
    }

    public void AdjustToShape(char shape)
    {
        for (int i = 0; i < 4; i++)
            transform.GetChild(i).localPosition = new Vector3(tetrominoData.tetromino[shape][i][0], tetrominoData.tetromino[shape][i][1]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // adjusting ghost position when boggin in ground or other shape
        if (collision.gameObject.layer != 2 && !hasAdjustedPos)
        {
            hasAdjustedPos = true;
            transform.position += new Vector3(0, 0.4f);
        }
    }
}
