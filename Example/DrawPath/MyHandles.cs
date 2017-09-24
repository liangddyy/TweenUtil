using UnityEngine;
using System.Collections;
using UnityEditor;
using Tween;

public class MyHandles : MonoBehaviour
{
    // public float areaRadius;
    // public float size;

    public Vector3[] nodePoints;
    // public Quaternion[] nodePointsRotation;
    void OnDrawGizmos()//画线
    {
        Vector3 oldVector3 = Vector3.zero, nowVector3 = Vector3.zero;
        float part = 20;
        for (int i = 0; i < nodePoints.Length - 1; i++)
        {
            oldVector3 = TweenMath.CatmullRomPoint(nodePoints[LimitRangeInt(i - 1, nodePoints.Length - 1)], nodePoints[LimitRangeInt(i, nodePoints.Length - 1)], nodePoints[LimitRangeInt(i + 1, nodePoints.Length - 1)], nodePoints[LimitRangeInt(i + 2, nodePoints.Length - 1)], 0);
            for (int j = 1; j <= part; j++)
            {
                nowVector3 = TweenMath.CatmullRomPoint(nodePoints[LimitRangeInt(i - 1, nodePoints.Length - 1)], nodePoints[LimitRangeInt(i, nodePoints.Length - 1)],
                    nodePoints[LimitRangeInt(i + 1, nodePoints.Length - 1)], nodePoints[LimitRangeInt(i + 2, nodePoints.Length - 1)], j / part);
                Gizmos.DrawLine(oldVector3, nowVector3);
                oldVector3 = nowVector3;
            }
        }
    }

    // 防止越界
    private int LimitRangeInt(int t, int max, int min = 0)
    {
        if (t < 0 | max <= 0)
        {
            return 0;
        }
        else if (t > max)
        {
            return max;
        }
        else
        {
            return t;
        }
    }
}
