using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static bool Contain(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }
}

public static class LookAtExtension
{
    public static void LookAt2DLerp(this Transform transform, Vector2 dir, float lerpPercent = 0.05f)
    {
        float rotationZ = Mathf.Acos(dir.x / dir.magnitude)
            * 180 / Mathf.PI
            * Mathf.Sign(dir.y);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0, 0, rotationZ),
            lerpPercent
        );
    }
}