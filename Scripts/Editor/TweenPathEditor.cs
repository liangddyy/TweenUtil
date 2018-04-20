using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Tween
{
	[CustomEditor(typeof(TweenPath))]
	public class TweenPathEditor : Editor
	{
		private TweenPath tweenPath;
		private void OnEnable()
		{
			tweenPath = target as TweenPath;
		}

		private void OnDisable()
		{ 

		}

		public override void OnInspectorGUI()
		{
			GUILayout.BeginVertical();
			for (int i = 0; i < tweenPath.Data.Nodes.Count; i++)
			{
				EditorGUILayout.Vector3Field("",tweenPath.Data.Nodes[i]);
			}
			
			GUILayout.EndVertical();
		}

		private void OnSceneGUI()
		{
			throw new System.NotImplementedException();
		}
	}
}