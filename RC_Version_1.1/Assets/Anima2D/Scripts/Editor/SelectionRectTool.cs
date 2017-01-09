using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Anima2D
{
	public class SelectionRectTool
	{
		static Vector2 s_StartPosition;
		static Vector2 s_EndPosition;
		static Rect s_currentRect;

		public static Rect Do(List<Vector2> points, List<int> selectedPoints)
		{
			int controlID = GUIUtility.GetControlID("SelectionRect".GetHashCode(), FocusType.Keyboard);

			return Do(controlID,points,selectedPoints);
		}

		public static Rect Do(int controlID, List<Vector2> points, List<int> selectedPoints)
		{
			EventType eventType = Event.current.GetTypeForControl(controlID);

			if(eventType == EventType.MouseDown)
			{
				s_StartPosition = Handles.inverseMatrix.MultiplyPoint(Event.current.mousePosition);
				s_EndPosition = s_StartPosition;
				s_currentRect.position = s_StartPosition;
				s_currentRect.size = Vector2.zero;
			}

			EditorGUI.BeginChangeCheck();
			
			s_EndPosition = HandlesExtra.Slider2D(controlID, s_EndPosition, null);
			
			if(EditorGUI.EndChangeCheck())
			{
				selectedPoints.Clear();

				s_currentRect.min = s_StartPosition;
				s_currentRect.max = s_EndPosition;

				for (int i = 0; i < points.Count; i++)
				{
					Vector2 p = points [i];
					if(s_currentRect.Contains(p,true))
					{
						selectedPoints.Add(i);
					}
				}
			}

			if(eventType == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(controlID);
			}

			if (eventType == EventType.Repaint)
			{
				if(GUIUtility.hotControl == controlID)
				{
					List<Vector3> verts = new List<Vector3>(4);
					verts.Add(new Vector3(s_currentRect.position.x,s_currentRect.position.y,0f));
					verts.Add(new Vector3(s_currentRect.position.x,s_currentRect.position.y + s_currentRect.height,0f));
					verts.Add(new Vector3(s_currentRect.position.x + s_currentRect.width,s_currentRect.position.y + s_currentRect.height,0f));
					verts.Add(new Vector3(s_currentRect.position.x + s_currentRect.width,s_currentRect.position.y,0f));
					Handles.color = Color.cyan;
					Handles.DrawSolidRectangleWithOutline(verts.ToArray(),new Color(1f,1f,1f,0.1f),new Color(1f,1f,1f,0.8f));
				}
			}
			return s_currentRect;
		}
	}
}
