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
    public class StackObjectPool<T> where T : IStackObject, new()
    {
        static Stack<T> stackPool = new Stack<T>();

        /// <summary>
        /// Pop对象
        /// </summary>
        /// <returns></returns>
        public static T Get()
        {
            T obj;
            if (stackPool.Count > 0)
            {
                obj = stackPool.Pop();
            }
            else
            {
                obj = new T();
                obj.OnInit();
            }

            obj.OnPop();
            return obj;
        }

        public static void Push(T obj)
        {
            if (obj != null)
            {
                obj.OnPush();
            }
            stackPool.Push(obj);
        }
    }

    #endregion

    public class StackObjectDict
    {
//        public static Dictionary<string, object> GetSODict()
//        {
//            return StackObjectPool<Dictionary<string, object>>.GetObject();
//        }
//
//        public static void PutSODict(Dictionary<string, object> dict)
//        {
//            dict.Clear();
//            StackObjectPool<Dictionary<string, object>>.PutObject(dict);
//        }
    }
}