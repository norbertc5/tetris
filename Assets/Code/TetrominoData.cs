using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoData : MonoBehaviour
{
    public Dictionary<char, Dictionary<int, int[]>> tetromino = new Dictionary<char, Dictionary<int, int[]>>();

    void Awake()
    {
        tetromino.Add('T', new Dictionary<int, int[]> {
            { 0, new int[] { 0, 0 } },
            { 1, new int[] { 1, 0 } },
            { 2, new int[] { -1, 0 } },
            { 3, new int[] { 0, 1 } }
        });

        tetromino.Add('O', new Dictionary<int, int[]> {
            { 0, new int[] { 0, 0 } },
            { 1, new int[] { 0, 1 } },
            { 2, new int[] { 1, 0 } },
            { 3, new int[] { 1, 1 } }
        });
    }
}
