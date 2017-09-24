using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MyHandles))]
public class HandlesInspector : Editor
{

    MyHandles myhandles;


    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        myhandles = (MyHandles)target;

    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    void OnSceneGUI()
    {
        Handles.color = Color.blue;


        // Handles.Label(myhandles.transform.position + new Vector3(0, 1, 0),
        // "myhandles");

        // myhandles.areaRadius = Handles.RadiusHandle(Quaternion.identity, myhandles.transform.position, myhandles.areaRadius);

        // myhandles.size = Handles.ScaleValueHandle(myhandles.size, myhandles.transform.position, Quaternion.identity, myhandles.size, Handles.ArrowCap, 0.5f);

        for (int i = 0; i < myhandles.nodePoints.Length; i++)
        {
            // myhandles.nodePoints[i] = Handles.PositionHandle(myhandles.nodePoints[i], myhandles.nodePointsRotation[i]);
            myhandles.nodePoints[i] = Handles.PositionHandle(myhandles.nodePoints[i], Quaternion.identity);

            if (i + 1 < myhandles.nodePoints.Length)
            {
                // 绘制线。两个点
                Handles.DrawLine(myhandles.nodePoints[i], myhandles.nodePoints[i+1]);
            }


        }
        // for (int i = 0; i < myhandles.nodePointsRotation.Length; i++)
        // {
        //     myhandles.nodePointsRotation[i] = Handles.RotationHandle(myhandles.nodePointsRotation[i], myhandles.nodePoints[i]);
        // }

    }
}
