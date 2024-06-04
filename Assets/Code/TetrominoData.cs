using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoData : MonoBehaviour
{
    public Dictionary<char, Dictionary<int, int[]>> tetromino = new Dictionary<char, Dictionary<int, int[]>>();
    public Dictionary<char, Color> tetrominoColor = new Dictionary<char, Color>();
    public Queue<char> shapesInQueue = new Queue<char>();

    void Awake()
    {
        tetromino.Add('T', new Dictionary<int, int[]> {
            { 0, new int[] { 0, 0 } },
            { 1, new int[] { 1, 0 } },
            { 2, new int[] { -1, 0 } },
            { 3, new int[] { 0, 1 } }
        });

        tetrominoColor.Add('T', new Color32(204, 51, 255, 255));

        tetromino.Add('O', new Dictionary<int, int[]> {
            { 0, new int[] { 0, 0 } },
            { 1, new int[] { 0, 1 } },
            { 2, new int[] { 1, 0 } },
            { 3, new int[] { 1, 1 } }
        });

        tetrominoColor.Add('O', new Color32(255, 255, 0, 255));

        tetromino.Add('S', new Dictionary<int, int[]> {
            { 0, new int[] { -1, -1 } },
            { 1, new int[] { 0, -1 } },
            { 2, new int[] { 0, 0 } },
            { 3, new int[] { 1, 0 } }
        });

        tetrominoColor.Add('S', new Color32(102, 255, 102, 255));

        tetromino.Add('Z', new Dictionary<int, int[]> {
            { 0, new int[] { -1, 0 } },
            { 1, new int[] { 0, 0 } },
            { 2, new int[] { 0, -1 } },
            { 3, new int[] { 1, -1 } }
        });

        tetrominoColor.Add('Z', new Color32(255, 51, 0, 255));

        tetromino.Add('L', new Dictionary<int, int[]> {
            { 0, new int[] { 0, 2 } },
            { 1, new int[] { 0, 1 } },
            { 2, new int[] { 0, 0 } },
            { 3, new int[] { 1, 0 } }
        });

        tetrominoColor.Add('L', new Color32(255, 153, 51, 255));

        tetromino.Add('J', new Dictionary<int, int[]> {
            { 0, new int[] { 0, 2 } },
            { 1, new int[] { 0, 1 } },
            { 2, new int[] { 0, 0 } },
            { 3, new int[] { -1, 0 } }
        });

        tetrominoColor.Add('J', new Color32(51, 153, 255, 255));

        tetromino.Add('I', new Dictionary<int, int[]> {
            { 0, new int[] { -1, 0 } },
            { 1, new int[] { 0, 0 } },
            { 2, new int[] { 1, 0 } },
            { 3, new int[] { 2, 0 } }
        });

        tetrominoColor.Add('I', new Color32(51, 204, 255, 255));
    }
}
