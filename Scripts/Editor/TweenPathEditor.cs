using System;
using UnityEngine;
using System.Collections;
using SnakeUnity;
using UnityEditor;
using UnityEditorInternal;
using Random = UnityEngine.Random;

namespace Tween
{
    [CustomEditor(typeof(TweenPath))]
    public class TweenPathEditor : Editor
    {
        private ReorderableList list;
        private GUIStyle gs;
        private GUIStyle richTextGs;

        private Font monoFont;
        //private bool editAllNodes;
        //private bool legacyAppearance;

        private int simulationIndex;
        private float simulateDuration;
        private float simulateTime;
        private DateTime simulateDateTime;

        private bool svLockX;
        private bool svLockY;
        private bool svLockZ;
        private bool svOriginAxisLock;
        private bool svHandlesAxisLock;

        private Vector2 scrollPosition;

        private int editMode;

        private TweenPath tweenPath;
//        public BezierCurveEditor()
//        {
//        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
   
//            tweenPath.ResolutionFactor = (BezierCurve.ResFactor) EditorGUILayout.EnumPopup("Resolution", tweenPath.ResolutionFactor);
//            serializedObject.FindProperty("resolutionFactor").intValue = (int) tweenPath.ResolutionFactor;
            tweenPath.IsClosedLoop = EditorGUILayout.Toggle("Closed Loop", tweenPath.IsClosedLoop);
            serializedObject.FindProperty("isClosedLoop").boolValue = tweenPath.IsClosedLoop;
//            tweenPath.ApplyRootTransform = EditorGUILayout.Toggle("Apply Root Transform", tweenPath.ApplyRootTransform);
//            serializedObject.FindProperty("applyRootTransform").boolValue = tweenPath.ApplyRootTransform;
            serializedObject.ApplyModifiedProperties();
            var bigGs = new GUIStyle(GetRichTextGuiStyle());
            bigGs.fontSize = 16;
            if (list == null)
            {
                Init();
            }

            #region Curve Nodes

            GUILayout.Label("Curve Nodes", bigGs);
            var listHeight = Mathf.Min(200, list.GetHeight());
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(listHeight));
            list.DoLayoutList();
            EditorGUILayout.EndScrollView();
//            if (list.index >= 0 && list.index < tweenPath.NodeCount)
//            {
//                GUILayout.Label("Current Editing Node: <b><color=#ff0000>" + list.index + "</color></b>", bigGs);
//                var n = tweenPath[list.index];
//                n.type = (BezierCurve.ConnectionType) EditorGUILayout.EnumPopup("Connection Type", n.type);
//                var nOrigin = EditorGUILayout.Vector3Field("Origin", n.origin);
//                if (nOrigin != n.origin)
//                {
//                    Undo.RecordObject(tweenPath, "node change origin");
//                    n.origin = nOrigin;
//                }
//
//                var nHandleVal = EditorGUILayout.Vector3Field("Handle 0", n.handle0);
//                if (nHandleVal != n.handle0)
//                {
//                    Undo.RecordObject(tweenPath, "node change handle0");
//                    n.handle0 = nHandleVal;
//                    if (n.type == BezierCurve.ConnectionType.Linked)
//                        n.handle1 = -n.handle0;
//                }
//
//                nHandleVal = EditorGUILayout.Vector3Field("Handle 1", n.handle1);
//                if (nHandleVal != n.handle1)
//                {
//                    Undo.RecordObject(tweenPath, "node change handle1");
//                    n.handle1 = nHandleVal;
//                    if (n.type == BezierCurve.ConnectionType.Linked)
//                        n.handle0 = -n.handle1;
//                }
//
//                GUILayout.BeginHorizontal();
//                if (GUILayout.Button("Reset Origin"))
//                {
//                    Undo.RecordObject(tweenPath, "reset origin");
//                    n.origin = Vector3.zero;
//                }
//
//                if (GUILayout.Button("Reset Handles"))
//                {
//                    Undo.RecordObject(tweenPath, "reset handles");
//                    n.handle0 = Vector3.zero;
//                    n.handle1 = Vector3.zero;
//                }
//
//                GUILayout.EndHorizontal();
//            }

            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            #region Scene View Editor Options

            GUILayout.Label("Scene View Editor Options", bigGs);
            GUILayout.BeginHorizontal();
            svOriginAxisLock = GUILayout.Toggle(svOriginAxisLock, "Origin Axis Lock", "button");
            svHandlesAxisLock = GUILayout.Toggle(svHandlesAxisLock, "Handles Axis Lock", "button");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            svLockX = GUILayout.Toggle(svLockX, "Lock X Axis", "button");
            svLockY = GUILayout.Toggle(svLockY, "Lock Y Axis", "button");
            svLockZ = GUILayout.Toggle(svLockZ, "Lock Z Axis", "button");
            GUILayout.EndHorizontal();

            #endregion

            GUILayout.Label("Node Edit Mode", bigGs);
            editMode = GUILayout.Toolbar(editMode + 1, new[] {"Root", "Single", "All Separate", "All Together"},
                           "button") - 1;

            #region Simulation

            simulationIndex = EditorGUILayout.IntSlider("Demo Progress", simulationIndex, -1, 100);

            GUILayout.BeginHorizontal();
            simulateDuration = EditorGUILayout.FloatField("Duration", simulateDuration);
            if (GUILayout.Button("Demonstrate"))
            {
                simulateTime = 0;
                simulateDateTime = DateTime.Now;
            }

            GUILayout.EndHorizontal();

            #endregion

//            var helpText = "Nodes: " + tweenPath.NodeCount + ", Length: " + tweenPath.CalculateCurveLength(tweenPath.ResolutionFactor);
//            EditorGUILayout.HelpBox(helpText, MessageType.Info);

            EditorUtility.SetDirty(tweenPath);
        }

        private void Init()
        {
            simulateDuration = 1f;
            simulationIndex = -1;
            svOriginAxisLock = true;
            svHandlesAxisLock = true;

            var t = target as BezierCurve;

            Tools.current = Tool.None;

            var nodes = serializedObject.FindProperty("nodes");
            list = new ReorderableList(serializedObject, nodes);
            list.headerHeight = 0;
            //list.drawHeaderCallback = (rect) => GUI.Label(rect, "Nodes");
            //list.elementHeight = (EditorGUIUtility.singleLineHeight + 4) * 4 + 4;
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                //if (index == list.index && !legacyAppearance)
                //{
                //    //Debug.Log(rect.height);
                //    var n = nodes.GetArrayElementAtIndex(index);
                //    var labelWidth = EditorGUIUtility.labelWidth;
                //    EditorGUIUtility.labelWidth /= 3;
                //    var labelRect = new Rect(rect.x, rect.y + 2, rect.width * .33f, EditorGUIUtility.singleLineHeight);
                //    GUI.Label(labelRect, "Node " + index, EditorStyles.boldLabel);
                //    var typeRect = new Rect(rect.x + rect.width * .33f, rect.y + 2, rect.width * .67f, EditorGUIUtility.singleLineHeight);
                //    var type = n.FindPropertyRelative("type");
                //    EditorGUI.PropertyField(typeRect, type);
                //    rect.y += EditorGUIUtility.singleLineHeight + 4;
                //    var origin = n.FindPropertyRelative("origin");
                //    origin.vector3Value = EditorGUI.Vector3Field(rect, "origin", origin.vector3Value);
                //    if (type.enumValueIndex > 0)
                //    {
                //        rect.y += EditorGUIUtility.singleLineHeight + 6;
                //        var handle0 = n.FindPropertyRelative("handle0");
                //        handle0.vector3Value = EditorGUI.Vector3Field(rect, "handle0", handle0.vector3Value);
                //        rect.y += EditorGUIUtility.singleLineHeight + 6;
                //        var handle1 = n.FindPropertyRelative("handle1");
                //        handle1.vector3Value = EditorGUI.Vector3Field(rect, "handle1", handle1.vector3Value);
                //    }
                //    serializedObject.ApplyModifiedProperties();
                //    EditorGUIUtility.labelWidth = labelWidth;
                //}
                //else
                //{
                var n = t[index];
                var str = "<b>Node " + index + ",  ";
                switch (n.type)
                {
                    case BezierCurve.ConnectionType.None:
                        str += "<color=#EB3FFF>  None  </color>";
                        break;
                    case BezierCurve.ConnectionType.Linked:
                        str += "<color=#11BF04> Linked </color>";
                        break;
                    case BezierCurve.ConnectionType.UnLinked:
                        str += "<color=#D00000>UnLinked</color>";
                        break;
                }

                str += "</b>";
                var str1 = "(x: " + n.origin.x.ToString("0.00") + ", y: " + n.origin.y.ToString("0.00") + ", z: " +
                           n.origin.z.ToString("0.00") + ")";

                var richGs = GetRichTextGuiStyle();
                var size = richGs.CalcSize(new GUIContent(str));
                GUI.Label(rect, str, richGs);
                var btnRect = new Rect(rect);
                btnRect.x += size.x + 4;
                btnRect.width = 20;

                var btnGs = EditorStyles.miniButton;
                if (btnGs == null)
                    btnGs = "button";
                btnGs.richText = true;
                if (GUI.Button(btnRect, "<color=#EB3FFF>N</color>", btnGs))
                {
                    Undo.RecordObject(t, "change type to N");
                    n.type = BezierCurve.ConnectionType.None;
                }

                btnRect.x += 24;
                if (GUI.Button(btnRect, "<color=#11BF04>L</color>", btnGs))
                {
                    Undo.RecordObject(t, "change type to L");
                    n.type = BezierCurve.ConnectionType.Linked;
                }

                btnRect.x += 24;
                if (GUI.Button(btnRect, "<color=#D00000>U</color>", btnGs))
                {
                    Undo.RecordObject(t, "change type to U");
                    n.type = BezierCurve.ConnectionType.UnLinked;
                }

                var str1Size = richGs.CalcSize(new GUIContent(str1));
                var vecRect = new Rect(rect);
                vecRect.x = btnRect.x + 24;
                vecRect.width = str1Size.x;

                GUI.Label(vecRect, str1, richGs);
                //}
            };
            //list.elementHeightCallback = GetElementHeight;
            list.onAddCallback = (l) =>
            {
                Undo.RecordObject(t, "adding node");
                var lastNode = t.NodeCount > 0 ? t[t.NodeCount - 1] : null;
                var n = t.AddNode();
                if (lastNode != null)
                    n.origin = lastNode.origin + new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f),
                                   Random.Range(-.1f, .1f));
                l.index = t.NodeCount - 1;
                scrollPosition = new Vector2(0, float.PositiveInfinity);
                //ArrayUtility.Add(ref t.nodes, new BezierCurve.BezierCurveNode());
                //l.index = t.Nodes.Length - 1;
            };
            list.onRemoveCallback = (l) =>
            {
                if (l.index >= 0)
                {
                    Undo.RecordObject(t, "removing node");
                    t.RemoveNode(l.index);
                    //ArrayUtility.RemoveAt(ref t.nodes, l.index);
                    l.index--;
                }
            };
            list.index = nodes.arraySize - 1;
        }

        private void OnEnable()
        {
            tweenPath = target as TweenPath;
            gs = new GUIStyle();
            gs.normal.textColor = gs.hover.textColor = gs.focused.textColor = Color.white;
            gs.fontSize = 12;
            gs.alignment = TextAnchor.MiddleCenter;
            Init();
            EditorApplication.update += SimulationUpdate;
            var assets = AssetDatabase.FindAssets("BezierCurveFont t:font");
            if (assets.Length > 0)
                monoFont = AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(assets[0]));
        }

        private void OnDisable()
        {
            EditorApplication.update -= SimulationUpdate;
        }

        private void SimulationUpdate()
        {
            var now = DateTime.Now;
            float sec = (float) ((now - simulateDateTime).TotalMilliseconds) / 1000f;
            simulateDateTime = now;
            if (simulateTime >= 0)
            {
                if (simulateTime == simulateDuration)
                    simulateTime = -1;
                simulateTime = Mathf.Min(simulateDuration, simulateTime + sec);
                simulationIndex = Mathf.RoundToInt(simulateTime * 100 / simulateDuration);
                EditorUtility.SetDirty(target);
            }
        }

        //private float GetElementHeight(int index)
        //{
        //    if (list.index == index)
        //    {
        //        return (EditorGUIUtility.singleLineHeight + 4) * 4 + 4;
        //    }
        //    return EditorGUIUtility.singleLineHeight;
        //}

        private void OnSceneGUI()
        {
//            var t = target as TweenPath;
//            Matrix4x4 mtx = t.GetCurveMatrix();
//            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
//            for (var i = 0; i < t.NodeCount; i++)
//            {
//                var gp = HandleUtility.WorldToGUIPoint(mtx * ToVec4(t[i].origin));
//                var text = "Node " + i.ToString();
//                var size = gs.CalcSize(new GUIContent(text));
//                var rect = new Rect(gp.x - 25, gp.y - 25, size.x + 6, size.y + 6);
//                EditorGUI.DrawRect(rect, new Color(0, 0, 0, .35f));
//                GUI.Label(rect, text, gs);
//            }
//
//            GUILayout.EndArea();
//            Handles.EndGUI();
//
//            //if (list.index < 0 || list.index > t.nodes.Length)
//            //    return;
//            if (list == null)
//            {
//                Init();
//            }
//
//            #region drawing node handles
//
//            if (editMode == -1)
//            {
//                if (Tools.current == Tool.None)
//                    Tools.current = Tool.Move;
//            }
//            else
//            {
//                if (Tools.current != Tool.None)
//                    Tools.current = Tool.None;
//                DrawNodes(t, mtx);
//            }
//
//            #endregion
//
//            if (simulationIndex >= 0)
//            {
//                var vec = t.GetVector(simulationIndex / 100f);
//                Handles.color = Color.cyan;
//                Handles.SphereCap(0, vec, Quaternion.identity, HandleUtility.GetHandleSize(vec) * .2f);
//            }
        }

        private Vector3 ConstraintAxis(Vector3 vec, Vector3 oldVec, bool isHandle)
        {
            if ((isHandle && svHandlesAxisLock) || (!isHandle && svOriginAxisLock))
            {
                if (svLockX)
                    vec.x = oldVec.x;
                if (svLockY)
                    vec.y = oldVec.y;
                if (svLockZ)
                    vec.z = oldVec.z;
            }

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
                    if (monoFont != null)
                    {
                        richTextGs.font = monoFont;
                        richTextGs.fontSize = 12;
                    }
                }
            }

            return richTextGs;
        }

        private void DrawNodes(BezierCurve t, Matrix4x4 mtx)
        {
            var inverseMtx = mtx.inverse;
            var handleRotation = t.ApplyRootTransform ? t.transform.rotation : Quaternion.identity;
            for (var i = 0; i < t.NodeCount; i++)
            {
                var n = t[i];
                if (i == list.index || editMode > 0)
                {
                    Vector4 origin = mtx * ToVec4(n.origin);
                    Vector3 oldOrign = origin;
                    var nOrigin = Handles.DoPositionHandle(origin, handleRotation);
                    nOrigin = ConstraintAxis(nOrigin, oldOrign, false);
                    if (nOrigin != oldOrign)
                    {
                        Undo.RecordObject(t, "node changing origin");
                        Vector3 delta = inverseMtx * (nOrigin - oldOrign);
                        if (editMode == 2)
                        {
                            for (var j = 0; j < t.NodeCount; j++)
                            {
                                t[j].origin += delta;
                            }
                        }
                        else
                        {
                            n.origin += delta;
                        }
                    }

                    if (editMode < 2 && n.type != BezierCurve.ConnectionType.None)
                    {
                        Handles.color = Color.yellow;
                        Vector4 hPos = mtx * ToVec4(n.origin + n.handle0);
                        Vector3 oldHPos = hPos;
                        Handles.DrawLine(origin, hPos);
                        var nhPos = Handles.FreeMoveHandle(hPos, handleRotation,
                            HandleUtility.GetHandleSize(hPos) * .1f, Vector3.zero, Handles.CircleCap);
                        nhPos = ConstraintAxis(nhPos, hPos, true);
                        if (nhPos != oldHPos)
                        {
                            Undo.RecordObject(t, "node changing handle0");
                            Vector3 delta = inverseMtx * (nhPos - oldHPos);
                            n.handle0 += delta;
                            if (n.type == BezierCurve.ConnectionType.Linked)
                                n.handle1 = -n.handle0;
                        }

                        hPos = mtx * ToVec4(n.origin + n.handle1);
                        oldHPos = hPos;
                        Handles.DrawLine(origin, hPos);
                        nhPos = Handles.FreeMoveHandle(hPos, handleRotation, HandleUtility.GetHandleSize(hPos) * .1f,
                            Vector3.zero, Handles.CircleCap);
                        nhPos = ConstraintAxis(nhPos, hPos, true);
                        if (nhPos != oldHPos)
                        {
                            Undo.RecordObject(t, "node changing handle1");
                            Vector3 delta = inverseMtx * (nhPos - oldHPos);
                            n.handle1 += delta;
                            if (n.type == BezierCurve.ConnectionType.Linked)
                                n.handle0 = -n.handle1;
                        }
                    }
                }
                else
                {
                    var nodePos = mtx * ToVec4(n.origin);
                    Handles.color = t.debugColor;
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