using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Hazard
{
    public float X;
    public float Y;
    public HazardType Type;
}
[Serializable]
public enum HazardType : byte
{
    Asteroid, Enemy
}
