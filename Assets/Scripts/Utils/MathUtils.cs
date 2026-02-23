using UnityEngine;
public static class MathUtils
{
    public static float DirToAngle(float x, float y, float offset=0f)
    {
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg + offset;
    }
}
