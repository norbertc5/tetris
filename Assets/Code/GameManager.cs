using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public List<Piece> pieces = new List<Piece>();

    [SerializeField] private GameObject shapePrefab;
    Ghost ghost;

    private Dictionary<float, int> piecesAmountInRow = new Dictionary<float, int>();
    TetrominoData tetrominoData;

    public delegate void Action();
    public static Action OnShapeArrived;
    public static Action<float> OnLineCleard;

    void Start()
    {
        OnShapeArrived += SpawnShape;
        OnShapeArrived += SearchLines;

        for (float i = -3.8f; i < 0; i += .4f)
        {
            piecesAmountInRow.Add((float)Math.Round(i, 1), 0);
        }
        tetrominoData = FindObjectOfType<TetrominoData>();
        ghost = FindObjectOfType<Ghost>();
        SpawnShape();
    }


    void SpawnShape()
    {
        GameObject newShape = Instantiate(shapePrefab);

        int r = UnityEngine.Random.Range(0, 4);
        char shape = ' ';
        switch (r)
        {
            case 0: shape = 'T'; break;
            case 1: shape = 'O'; break;
            case 2: shape = 'S'; break;
            case 3: shape = 'Z'; break;
        }
        //shape = 'T';

        for (int i = 0; i < 4; i++)
        {
            Transform newShapeChild = newShape.transform.GetChild(i);
            newShapeChild.localPosition = new Vector3(tetrominoData.tetromino[shape][i][0], tetrominoData.tetromino[shape][i][1]);
            newShapeChild.GetComponent<SpriteRenderer>().color = tetrominoData.tetrominoColor[shape];
            ghost.AdjustToShape(shape);
        }
    }

    /// <summary>
    /// Searchs if there is any completed line.
    /// </summary>
    void SearchLines()
    {
        // count all pieces according to line
        pieces.ForEach((Piece p) => { 
            if(!p.hasBeenCounted)
            {
                piecesAmountInRow[(float)Math.Round(p.transform.position.y, 1)] += 1;
                p.hasBeenCounted = true;
            }
        });

        List<float> fullLines = new List<float>();

        // find line with 10 pieces
        foreach (KeyValuePair<float, int> pair in piecesAmountInRow)
        {
            if(pair.Value >= 10)
            {
                fullLines.Add(pair.Key);
                OnLineCleard?.Invoke(pair.Key);
            }
        }

        // move down amount of pieces in dict for pieces over cleared line
        if (fullLines.Count > 0)
        {
            for (float i = -3.8f; i < 0; i += .4f)
            {
                try
                {
                    int piecesInLine = piecesAmountInRow[(float)Math.Round(i + .4f, 1)];
                    if (piecesInLine >= 10)
                        piecesInLine = 0;
                    piecesAmountInRow[(float)Math.Round(i, 1)] = piecesInLine;
                }
                catch
                {

                }
            }

            fullLines.Clear();
        }
    }
}
