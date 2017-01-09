using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Anima2D
{
	[Serializable]
	public class BlendShapeFrame
	{
		public float weight;
		public List<Vector2> vertices = new List<Vector2>();

		public static BlendShapeFrame Create(float weight, List<Vector2> vertices)
		{
			BlendShapeFrame frame = new BlendShapeFrame();
			frame.vertices = vertices;
			frame.weight = weight;
			return frame;
		}
	}
}
