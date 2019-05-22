using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace Tween
{
    [CustomEditor(typeof(TweenPath))]
    public class TweenPathEditor : Editor
    {
        private ReorderableList list;
        private GUIStyle gs;
        private GUIStyle richTextGs;

//        private int simulationIndex;    // 样本进度条 。暂时不做

//        private float simulateDuration;
//        private float simulateTime;
//        private DateTime simulateDateTime;

        private bool svLockX;
        private bool svLockY;

        private bool svLockZ;
//        private bool svOriginAxisLock;
//        private bool svHandlesAxisLock;

        private Vector2 scrollPosition;

        private emEditMode editMode = emEditMode.Single;

        private TweenPath tweenPath;

        public enum emEditMode
        {
            Root = 0,
            Single = 1,
            ALL = 2
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            tweenPath.IsClosedLoop = EditorGUILayout.Toggle("Closed Loop", tweenPath.IsClosedLoop);
            serializedObject.FindProperty("isClosedLoop").boolValue = tweenPath.IsClosedLoop;
            serializedObject.ApplyModifiedProperties();

            tweenPath.ApplyRootTransform = EditorGUILayout.Toggle("Apply Root Transform", tweenPath.ApplyRootTransform);
            serializedObject.FindProperty("applyRootTransform").boolValue = tweenPath.ApplyRootTransform;
            serializedObject.ApplyModifiedProperties();

            tweenPath.ExamplePosition = (GameObject) EditorGUILayout.ObjectField("TestPosition",
                tweenPath.ExamplePosition, typeof(GameObject), true);
            var bigGs = new GUIStyle(GetRichTextGuiStyle());
            bigGs.fontSize = 16;
            if (list == null)
            {
                Init();
            }

            #region Curve Nodes

            GUILayout.Label("节点Nodes", bigGs);
            var listHeight = Mathf.Min(200, list.GetHeight());
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(listHeight));
            list.DoLayoutList();
            EditorGUILayout.EndScrollView();

            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            #region Scene View Editor Options

            GUILayout.Label("节点编辑配置", bigGs);
            GUILayout.BeginHorizontal();
//            svOriginAxisLock = GUILayout.Toggle(svOriginAxisLock, "Origin Axis Lock", "button");
//            svHandlesAxisLock = GUILayout.Toggle(svHandlesAxisLock, "Handles Axis Lock", "button");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            svLockX = GUILayout.Toggle(svLockX, "Lock X Axis", "button");
            svLockY = GUILayout.Toggle(svLockY, "Lock Y Axis", "button");
            svLockZ = GUILayout.Toggle(svLockZ, "Lock Z Axis", "button");
            GUILayout.EndHorizontal();

            #endregion

            GUILayout.Label("节点编辑模式", bigGs);
            editMode = (emEditMode) GUILayout.Toolbar((int) editMode,
                new[] {emEditMode.Root.ToString(), emEditMode.Single.ToString(), emEditMode.ALL.ToString()},
                "button");

            #region Simulation

            // 样本目标体进度
//            simulationIndex = EditorGUILayout.IntSlider("Demo Progress", simulationIndex, -1, 100);

//            GUILayout.BeginHorizontal();
//            simulateDuration = EditorGUILayout.FloatField("Duration", simulateDuration);
//            if (GUILayout.Button("Demonstrate"))
//            {
//                simulateTime = 0;
//                simulateDateTime = DateTime.Now;
//            }
//            GUILayout.EndHorizontal();

            #endregion

            EditorUtility.SetDirty(tweenPath);
        }

        private void Init()
        {
//            simulateDuration = 1f;
//            simulationIndex = -1;
//            svOriginAxisLock = true;
//            svHandlesAxisLock = true;

            var curve = target as TweenPath;

            Tools.current = Tool.None;

            var nodes = serializedObject.FindProperty("localNodes");
            list = new ReorderableList(serializedObject, nodes);

            list.headerHeight = 0;
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var rect1 = rect;
                rect1.width = rect.width / 4;
                EditorGUI.LabelField(rect1, "Node " + index + ",");
                rect.x += rect1.width;
                rect.width -= rect1.width;
                curve.LocalNodes[index] = EditorGUI.Vector3Field(rect, "", curve.LocalNodes[index]);
            };
            list.onAddCallback = (l) =>
            {
                Undo.RecordObject(curve, "adding node");
                int indexInsert = l.index >= curve.LocalNodes.Count ? curve.LocalNodes.Count : l.index;
                var tmp = curve.ExamplePosition == null
                    ? curve.LocalNodes[indexInsert]
                    : curve.ExamplePosition.transform.position - curve.transform.position;

                if (indexInsert < 0) indexInsert = 0;
                curve.LocalNodes.Insert(indexInsert, tmp);
                l.index = indexInsert +1;
                scrollPosition = new Vector2(0, float.PositiveInfinity);
            };
            list.onRemoveCallback = (l) =>
            {
                if (l.index >= 0)
                {
                    Undo.RecordObject(curve, "removing node");
                    curve.LocalNodes.RemoveAt(l.index);
                    l.index--;
                }
            };
            list.index = nodes.arraySize - 1;
        }


        private void OnEnable()
        {
            tweenPath = target as TweenPath;
            if (tweenPath.LocalNodes == null) tweenPath.LocalNodes = new List<Vector3>();
            gs = new GUIStyle();
            gs.normal.textColor = gs.hover.textColor = gs.focused.textColor = Color.white;
            gs.fontSize = 12;
            gs.alignment = TextAnchor.MiddleCenter;
            Init();
            EditorApplication.update += SimulationUpdate;
//            var assets = AssetDatabase.FindAssets("BezierCurveFont t:font");
//            if (assets.Length > 0)
//                monoFont = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(assets[0]));
        }

        private void OnDisable()
        {
            EditorApplication.update -= SimulationUpdate;
        }

        // 自动保存/s
        private void SimulationUpdate()
        {
//            var now = DateTime.Now;
//            float sec = (float) ((now - simulateDateTime).TotalMilliseconds) / 1000f;
//            simulateDateTime = now;
//            if (simulateTime >= 0)
//            {
//                if (simulateTime == simulateDuration)
//                    simulateTime = -1;
//                simulateTime = Mathf.Min(simulateDuration, simulateTime + sec);
////                simulationIndex = Mathf.RoundToInt(simulateTime * 100 / simulateDuration);
//                EditorUtility.SetDirty(target);
//            }
        }

        private void OnSceneGUI()
        {
            var curve = target as TweenPath;
            Matrix4x4 mtx = curve.GetCurveMatrix();
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            for (var i = 0; i < curve.LocalNodes.Count; i++)
            {
                var gp = HandleUtility.WorldToGUIPoint(mtx * ToVec4(curve.LocalNodes[i]));
                var text = "Node " + i.ToString();
                var size = gs.CalcSize(new GUIContent(text));
                var rect = new Rect(gp.x - 25, gp.y - 25, size.x + 6, size.y + 6);
                EditorGUI.DrawRect(rect, new Color(0, 0, 0, .35f));
                GUI.Label(rect, text, gs);
            }

            GUILayout.EndArea();
            Handles.EndGUI();

            if (list == null)
            {
                Init();
            }

            // Root 不绘制节点
            if (editMode == emEditMode.Root)
            {
                if (Tools.current == Tool.None)
                    Tools.current = Tool.Move;
            }
            else
            {
                if (Tools.current != Tool.None)
                    Tools.current = Tool.None;
                DrawNodes(curve, mtx);
            }

//            if (simulationIndex >= 0)
//            {
//                var vec = curve.GetVector(simulationIndex / 100f);
//                Handles.color = Color.cyan;
//                Handles.SphereCap(0, vec, Quaternion.identity, HandleUtility.GetHandleSize(vec) * .2f);
//            }
        }

        private Vector3 ConstraintAxis(Vector3 vec, Vector3 oldVec)
        {
            if (svLockX)
                vec.x = oldVec.x;
            if (svLockY)
                vec.y = oldVec.y;
            if (svLockZ)
                vec.z = oldVec.z;
            return vec;
        }

        private GUIStyle GetRichTextGuiStyle()
        {
            if (richTextGs != null)
                return richTextGs;
            else
            {
                if (EditorStyles.label != null)
                {
                    richTextGs = new GUIStyle(EditorStyles.label);
                    richTextGs.richText = true;
//                    if (monoFont != null)
//                    {
//                        richTextGs.font = monoFont;
                    richTextGs.fontSize = 12;
//                    }
                }
            }

            return richTextGs;
        }

        private void DrawNodes(TweenPath tnPath, Matrix4x4 mtx)
        {
            Handles.color = tnPath.DebugColor;
            var inverseMtx = mtx.inverse;
            var handleRotation = tnPath.ApplyRootTransform ? tnPath.transform.rotation : Quaternion.identity;
//            List<Vector3> Nodes = tnPath.Nodes;
            for (var i = 0; i < tnPath.LocalNodes.Count; i++)
            {
//                var item = tnPath.Nodes[i];
                if (i == list.index || editMode == emEditMode.ALL)
                {
                    Vector4 origin = mtx * ToVec4(tnPath.LocalNodes[i]);
                    Vector3 oldOrign = origin;
                    var nOrigin = Handles.DoPositionHandle(origin, handleRotation); // 绘制节点。
                    nOrigin = ConstraintAxis(nOrigin, oldOrign);
                    if (nOrigin != oldOrign)
                    {
                        Undo.RecordObject(tnPath, "node changing origin");
                        Vector3 delta = inverseMtx * (nOrigin - oldOrign);
                        if (editMode == emEditMode.Root)
                        {
                            for (var j = 0; j < tnPath.LocalNodes.Count; j++)
                            {
                                tnPath.LocalNodes[j] += delta;
                            }
                        }
                        else
                        {
                            tnPath.LocalNodes[i] += delta;
                        }
                    }
                }
                else
                {
                    var nodePos = mtx * ToVec4(tnPath.LocalNodes[i]);
                    Handles.color = tnPath.DebugColor;
                    Handles.CubeCap(0, nodePos, handleRotation, HandleUtility.GetHandleSize(nodePos) * .1f);
                }
            }
        }

        private Vector4 ToVec4(Vector3 v)
        {
            return new Vector4(v.x, v.y, v.z, 1);
        }
    }
}