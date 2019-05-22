using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tween
{
	public class TweenPath : MonoBehaviour
	{
		[SerializeField]
		private PathData _data;
		
		public PathData Data
		{
			get { return _data; }
		}
	}

	[Serializable]
	public class PathData
	{
		public List<Vector3> Nodes;
		public Ease Ease;
		public LoopType LoopType;
	}
}