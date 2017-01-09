using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Anima2D
{
	[Serializable]
	public class BlendShape
	{
		public string name;
		public List<BlendShapeFrame> frames = new List<BlendShapeFrame>();

		public static BlendShape Create(string name, List<Vector2> vertices)
		{
			BlendShape blendShape = new BlendShape();
			blendShape.name = name;
			blendShape.frames.Add(BlendShapeFrame.Create(100f,vertices));
			return blendShape;
		}

		public static implicit operator bool(BlendShape b)
		{
			return b != null;
		}
	}
}
