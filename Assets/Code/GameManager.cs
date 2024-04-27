using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
}
