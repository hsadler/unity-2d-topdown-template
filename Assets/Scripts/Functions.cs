using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Functions
{


    public static Vector3 RoundVector(Vector3 v)
    {
        return new Vector3(
            (int)Math.Round(v.x, 0),
            (int)Math.Round(v.y, 0),
            (int)Math.Round(v.z, 0)
        );
    }


}