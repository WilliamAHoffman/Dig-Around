using System;
using UnityEngine;

[Serializable]
public class RangedFloat
{
    [SerializeField] [Range(-1f, 1f)] public float val = 0;
}
