using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Tween
{
    #region StackObj

    public interface IStackObject
    {
        void OnInit();

        void OnPop();

        void OnPush();
    }

    #endregion

    #region HeapObjectPool

    /// <summary>
    /// 堆对象管理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StackObjectPool<T> where T : new()
    {
        static Stack<T> stackPool = new Stack<T>();

        /// <summary>
        /// Pop对象
        /// </summary>
        /// <returns></returns>
        public static T GetObject()
        {
            T obj;
            IStackObject heapObj;

            if (stackPool.Count > 0)
            {
                obj = stackPool.Pop();
                heapObj = obj as IStackObject;
            }
            else
            {
                obj = new T();
                heapObj = obj as IStackObject;
                if (heapObj != null)
                {
                    heapObj.OnInit();
                }
            }

            if (heapObj != null)
            {
                heapObj.OnPop();
            }

            return obj;
        }

        /// <summary>
        /// Push对象
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void PutObject(T obj)
        {
            IStackObject heapObj = obj as IStackObject;

            if (heapObj != null)
            {
                heapObj.OnPush();
            }

            stackPool.Push(obj);
        }
    }

    #endregion

    public class StackObjectDict
    {
        #region string, object字典

        public static Dictionary<string, object> GetSODict()
        {
            return StackObjectPool<Dictionary<string, object>>.GetObject();
        }

        public static void PutSODict(Dictionary<string, object> dict)
        {
            dict.Clear();
            StackObjectPool<Dictionary<string, object>>.PutObject(dict);
        }

        #endregion
    }
}