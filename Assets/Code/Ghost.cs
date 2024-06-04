using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Ghost : MonoBehaviour
{
    Transform[] children = new Transform[4];
    [HideInInspector] public List<float> yPosesUsedToLift;  // defineies positions of pieces uesd to lift to prevent too big lift
    [HideInInspector] public Vector3 gfxPos;
    [SerializeField] Transform gfx;
    TetrominoData tetrominoData;

    private void Awake()
    {
        tetrominoData = FindObjectOfType<TetrominoData>();        
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            children[i] = transform.GetChild(i);
        }

        // to avoid issue with blinking gfx, firstly moves collision and next gfx with delay
        InvokeRepeating("SetGFX", 1, 0.02f);
    }

    public void SetGhost(Vector3 pos, Vector3 rot)
    {
        yPosesUsedToLift.Clear();
        gfxPos = pos;
        transform.position = pos;
        transform.eulerAngles = rot;
        gfx.eulerAngles = rot;

        // sometimes Z and S sahpes are floating over pos and it repairs it
        if ((float)Math.Round(children.OrderBy(t => t.transform.position.y).FirstOrDefault().position.y, 1) ==
            (float)Math.Round(pos.y, 1))
        {
            transform.position = pos - new Vector3(0, 0.4f);
            gfxPos = pos - new Vector3(0, 0.4f);
        }
    }

    /// <summary>
    /// Make ghost changes shape according to tetromino.
    /// </summary>
    /// <param name="shape"></param>
    public void AdjustShape(char shape)
    {
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).localPosition = new Vector3(tetrominoData.tetromino[shape][i][0], tetrominoData.tetromino[shape][i][1]);
            gfx.GetChild(i).localPosition = new Vector3(tetrominoData.tetromino[shape][i][0], tetrominoData.tetromino[shape][i][1]);
        }
    }

    void SetGFX()
    {
        gfx.position = gfxPos;
    }
}
