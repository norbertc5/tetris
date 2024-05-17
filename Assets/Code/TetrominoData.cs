using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoData : MonoBehaviour
{
    public Dictionary<char, Dictionary<int, int[]>> tetromino = new Dictionary<char, Dictionary<int, int[]>>();
    public Dictionary<char, Color> tetrominoColor = new Dictionary<char, Color>();

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
    }
}
