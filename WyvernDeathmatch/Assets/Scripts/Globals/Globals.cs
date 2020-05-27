using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public const float delta = 0.00001f;

    public enum DamageType
    {
        Bullet,
        Fire,
        Ice,
        Beam        // Need a better name, but I don't want "light"
    }
}
