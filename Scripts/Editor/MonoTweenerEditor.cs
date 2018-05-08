using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TN
{
    [CustomEditor(typeof(MonoTweener))]
    public class MonoTweenerEditor : Editor
    {
        private MonoTweener monoTweener;
        private ReorderableList list;
        private Vector2 scrollPosition;

        public TweenScript Tmp;

        private void OnEnable()
        {

            monoTweener = target as MonoTweener;
            if (monoTweener != null && monoTweener.Tweeners == null) monoTweener.Tweeners = new List<TweenScript>();
            InitList();
#if UNITY_EDITOR
            EditorApplication.update += monoTweener.Update;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= monoTweener.Update;
#endif
        }

        private void InitList()
        {
            Tools.current = Tool.None;

            var nodes = serializedObject.FindProperty("tweeners");
            if (nodes == null)
                Debug.LogError("null");
            list = new ReorderableList(serializedObject, nodes)
            {
                headerHeight = 0,
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var rect1 = rect;
                    rect1.width = rect.width / 4;
                    EditorGUI.LabelField(rect1, "Tweener " + index + ",");
                    rect.x += rect1.width;
                    rect.width -= rect1.width;
                    monoTweener.Tweeners[index].animType =
                        (AnimType) EditorGUI.EnumPopup(rect, monoTweener.Tweeners[index].animType);
                },
                onAddCallback = (l) =>
                {
                    Undo.RecordObject(monoTweener, "adding node");
//                    int indexInsert = l.index >= monoTweener.Tweeners.Count ? monoTweener.Tweeners.Count : l.index;
                    var tmp = new TweenScript();
                    tmp.isPause = true;
                    tmp.animGameObject = monoTweener.gameObject;
                    monoTweener.Tweeners.Add(tmp);
                    scrollPosition = new Vector2(0, float.PositiveInfinity);
                },
                onRemoveCallback = (l) =>
                {
                    if (l.index >= 0)
                    {
                        Undo.RecordObject(monoTweener, "removing node");
                        monoTweener.Tweeners.RemoveAt(l.index);
                        l.index--;
                    }
                },
                index = nodes.arraySize - 1
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (list == null)
            {
                InitList();
            }

            #region Nodes

            GUILayout.Label("Nodes");
            var listHeight = Mathf.Min(200, list.GetHeight());
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(listHeight));
            list.DoLayoutList();
            EditorGUILayout.EndScrollView();

            if (list.index >= 0 && list.index < list.count)
            {
                monoTweener.Tweeners[list.index].fromV3 = EditorGUILayout.Vector3Field("FromV3", monoTweener.Tweeners[list.index].fromV3);
                monoTweener.Tweeners[list.index].toV3 = EditorGUILayout.Vector3Field("ToV3", monoTweener.Tweeners[list.index].toV3);
                monoTweener.Tweeners[list.index].totalTime = EditorGUILayout.FloatField("Time", monoTweener.Tweeners[list.index].totalTime);
            }

            if (GUILayout.Button("Play"))
            {
                monoTweener.Play();
            }
            #endregion
        }
    }
}