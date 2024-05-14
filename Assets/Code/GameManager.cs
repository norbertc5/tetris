using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public Transform GhostTrans { get { return ghostRef; } private set {ghostRef = value; }}
    public List<Piece> pieces = new List<Piece>();

    [SerializeField] private Transform ghostRef;
    [SerializeField] private GameObject shapePrefab;

    public Dictionary<float, int> piecesAmountInRow = new Dictionary<float, int>();

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
    }


    void SpawnShape()
    {
        GameObject newShape = Instantiate(shapePrefab);
    }

    public void SetGhost(Vector3 pos, Vector3 rot)
    {
        GhostTrans.position = pos;
        GhostTrans.eulerAngles = rot;
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

        float lineYPos = 0;

        // find line with 10 pieces
        foreach (KeyValuePair<float, int> pair in piecesAmountInRow)
        {
            if(pair.Value >= 10)
            {
                lineYPos = pair.Key;
                OnLineCleard?.Invoke(pair.Key);
            }
        }

        // move down pieces over cleared line
        if(lineYPos != 0)
        {
            for (float i = lineYPos; i < 0; i += .4f)
            {
                try
                {
                    piecesAmountInRow[(float)Math.Round(i,1)] = piecesAmountInRow[(float)Math.Round(i+.4f, 1)];
                }
                catch{ }
            }
            lineYPos = 0;
        }
    }
}
