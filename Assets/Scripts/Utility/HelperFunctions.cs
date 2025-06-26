using UnityEngine;

public static class HelperFunctions
{
    public static Vector3 Vector3Lerp(Vector3 a, Vector3 b, float t) 
    {
        a.x = Mathf.Lerp(a.x, b.x, t);
        a.y = Mathf.Lerp(a.y, b.y, t);
        a.z = Mathf.Lerp(a.z, b.z, t);
        return a;
    }
}
