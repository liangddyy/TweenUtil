using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tween
{
    public class TweenPath : MonoBehaviour
    {
        private bool dirty;
        public Color DebugColor = Color.white;

        [HideInInspector] [SerializeField] private GameObject curExample;
        [HideInInspector] [SerializeField] private List<Vector3> localNodes;

        [HideInInspector] [SerializeField] private bool isClosedLoop;

        [HideInInspector] [SerializeField] private bool applyRootTransform = true;

        public GameObject ExamplePosition
        {
            get
            {
                if (curExample == null)
                    curExample = GetComponent<GameObject>();
                return curExample;
            }
            set { curExample = value; }
        }

        [SerializeField]
        public bool IsClosedLoop
        {
            get { return isClosedLoop; }
            set
            {
                isClosedLoop = value;
                dirty = true;
            }
        }

        public bool ApplyRootTransform
        {
            get { return applyRootTransform; }
            set { applyRootTransform = value; /*dirty = true;*/ }
        }

        public List<Vector3> LocalNodes
        {
            get { return localNodes; }
            set { localNodes = value; }
        }

        public List<Vector3> Nodes
        {
            get
            {
                Matrix4x4 mtx = GetCurveMatrix();
                List<Vector3> list = new List<Vector3>();
                foreach (var x in localNodes) list.Add(mtx * ToVec4(x));
                return list;
            }
        }

        public Vector3 GetNode(int index)
        {
            if (index < 0)
                index = 0;
            if (index >= localNodes.Count - 1)
                index = localNodes.Count - 1;
            Matrix4x4 mtx = GetCurveMatrix();
            return mtx * ToVec4(localNodes[index]);
        }

        void OnDrawGizmos() //画线
        {
#if UNITY_EDITOR
            Handles.color = DebugColor;
            Matrix4x4 mtx = GetCurveMatrix();
            Vector3 oldVector3 = Vector3.zero, nowVector3 = Vector3.zero;
            float part = 20;
            for (int i = 0; i < localNodes.Count; i++)
            {
                if (i != localNodes.Count - 1 || isClosedLoop)
                {
                    oldVector3 = TweenMath.CatmullRomPoint(localNodes[LimitRangeInt(i - 1, localNodes.Count - 1)],
                        localNodes[LimitRangeInt(i, localNodes.Count - 1)],
                        localNodes[LimitRangeInt(i + 1, localNodes.Count - 1)],
                        localNodes[LimitRangeInt(i + 2, localNodes.Count - 1)], 0);
                    oldVector3 = mtx * ToVec4(oldVector3);
                    for (int j = 1; j <= part; j++)
                    {
                        nowVector3 = TweenMath.CatmullRomPoint(localNodes[LimitRangeInt(i - 1, localNodes.Count - 1)],
                            localNodes[LimitRangeInt(i, localNodes.Count - 1)],
                            localNodes[LimitRangeInt(i + 1, localNodes.Count - 1)],
                            localNodes[LimitRangeInt(i + 2, localNodes.Count - 1)], j / part);
                        nowVector3 = mtx * ToVec4(nowVector3);
                        Handles.DrawLine(oldVector3, nowVector3);
                        oldVector3 = nowVector3;
                    }
                }
            }
#endif
        }

        // 防止越界
        private int LimitRangeInt(int t, int max, int min = 0)
        {
            if (max <= 0 || max < min)
            {
                return 0;
            }
            else if (t > max)
            {
                return isClosedLoop ? LimitRangeInt(t - max - 1, max, min) : max;
            }
            else if (t < min)
            {
                return isClosedLoop ? LimitRangeInt(max - min + t + 1, max, min) : min;
            }

            return t;
        }

        private Vector4 ToVec4(Vector3 v)
        {
            return new Vector4(v.x, v.y, v.z, 1);
        }

        public Matrix4x4 GetCurveMatrix()
        {
            if (ApplyRootTransform)
                return Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            else
                return Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        }
    }

    // [Serializable]
    // public class PathData
    // {
    //     public List<Vector3> Nodes;
    //     public Ease Ease;
    //     public LoopType LoopType;
    // }
}