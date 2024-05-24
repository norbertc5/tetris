using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Ghost : MonoBehaviour
{
    Transform[] children = new Transform[4];
    [HideInInspector] public List<float> yPosesUsedToLift;  // defineies positions of pieces uesd to lift to prevent too big lift

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            children[i] = transform.GetChild(i);
        }
    }

    public void SetGhost(Vector3 pos, Vector3 rot)
    {
        yPosesUsedToLift.Clear();
        transform.position = pos;
        transform.eulerAngles = rot;

        // sometimes Z and S sahpes are floating over pos and it repairs it
        if ((float)Math.Round(children.OrderBy(t => t.transform.position.y).FirstOrDefault().position.y, 1) ==
            (float)Math.Round(pos.y, 1))
            transform.position = pos - new Vector3(0, 0.4f);
    }
}
