using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform GhostTrans { get { return ghostRef; } private set {ghostRef = value; }}

    [SerializeField] private Transform ghostRef;
    [SerializeField] private GameObject shapePrefab;

    public delegate void Action();
    public static Action OnShapeArrived;

    void Start()
    {
        OnShapeArrived += SpawnShape;
    }


    void SpawnShape()
    {
        GameObject newShape = Instantiate(shapePrefab);
    }

    public void SetGhost(Vector2 pos, Vector3 rot)
    {
        GhostTrans.position = pos;
        GhostTrans.eulerAngles = rot;
    }
}
