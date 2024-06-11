using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Piece> pieces = new List<Piece>();

    [SerializeField] private GameObject shapePrefab;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject newScoreDisplay;
    Ghost ghost;
    int score = 0;

    private Dictionary<float, int> piecesAmountInRow = new Dictionary<float, int>();
    TetrominoData tetrominoData;

    public delegate void Action();
    public static Action OnShapeArrived;
    public static Action<float> OnLineCleard;

    public const float CELL_SIZE = 0.4f;
    public const float LAST_LINE_Y_POS = -3.8f;
    public const float HORIZONTAL_EDGE = 1.8f;
    public const float SHAPES_START_Y_POS = 3.4f;
    const int SCORE_FOR_LINE = 10;

    void Start()
    {
        OnShapeArrived += SpawnShape;
        OnShapeArrived += SearchLines;
        OnLineCleard += UpdateScore;

        for (float i = LAST_LINE_Y_POS; i < SHAPES_START_Y_POS + 2 * CELL_SIZE; i += .4f)
        {
            piecesAmountInRow.Add((float)Math.Round(i, 1), 0);
        }
        tetrominoData = FindObjectOfType<TetrominoData>();
        ghost = FindObjectOfType<Ghost>();
        SpawnShape();
    }


    void SpawnShape()
    {
        // when queue is empty draw new one
        if(tetrominoData.shapesInQueue.Count == 0)
        {
            List<int> addedShapesIndexes = new List<int>();
            for(int i=0; i<7; i++)
            {
                // prevent same shapes in one draw
                int r = -1;
                do
                {
                    r = UnityEngine.Random.Range(0, 7);
                }
                while (addedShapesIndexes.Contains(r));

                char shape = ' ';
                switch (r)
                {
                    case 0: shape = 'T'; break;
                    case 1: shape = 'O'; break;
                    case 2: shape = 'S'; break;
                    case 3: shape = 'Z'; break;
                    case 4: shape = 'L'; break;
                    case 5: shape = 'J'; break;
                    case 6: shape = 'I'; break;
                }
                tetrominoData.shapesInQueue.Enqueue(shape);
                addedShapesIndexes.Add(r);
            }
        }
        GameObject newShape = Instantiate(shapePrefab);
        char actualShape = tetrominoData.shapesInQueue.Dequeue();
        newShape.GetComponent<Shape>().shape = actualShape;

        for (int i = 0; i < 4; i++)
        {
            Transform newShapeChild = newShape.transform.GetChild(i);
            newShapeChild.localPosition = new Vector3(tetrominoData.tetromino[actualShape][i][0], tetrominoData.tetromino[actualShape][i][1]);
            newShapeChild.GetComponent<SpriteRenderer>().color = tetrominoData.tetrominoColor[actualShape];
            ghost.AdjustShape(actualShape);
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
        if(fullLines.Count > 0)
        {
            for (float i = fullLines[0]; i < 0; i += CELL_SIZE)
            {
                float thisKey = (float)Math.Round(i, 1);
                float nextKey = (float)Math.Round(i + CELL_SIZE * fullLines.Count, 1);

                if(nextKey < 0)
                    piecesAmountInRow[thisKey] = piecesAmountInRow[nextKey];
            }
        }
    }

    void UpdateScore(float line)
    {
        score += SCORE_FOR_LINE;
        scoreText.text = score.ToString();
        var newScoreDsp = Instantiate(newScoreDisplay);
        newScoreDsp.transform.position = new Vector3(0, line);
        newScoreDsp.GetComponentInChildren<TextMeshProUGUI>().text = $"+{SCORE_FOR_LINE}";
        newScoreDsp.transform.SetParent(newScoreDisplay.transform.parent);
        newScoreDsp.transform.localScale = Vector3.one;
    }
}
