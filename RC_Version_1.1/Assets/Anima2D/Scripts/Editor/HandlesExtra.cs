using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Anima2D 
{
	public class HandlesExtra
	{
		public delegate void DrawCapFunction(int controlID, Vector3 position);

		static Material s_HandleWireMaterial;
		static Material s_HandleWireMaterial2D;

		private static Vector2 s_CurrentMousePosition;
		private static Vector2 s_DragStartScreenPosition;
		private static Vector2 s_DragScreenOffset;

		static Vector3[] s_circleArray;

		public static Vector2 Slider2D(int id, Vector2 position, DrawCapFunction drawCapFunction)
		{
			EventType type = Event.current.GetTypeForControl(id);
			
			switch(type)
			{
			case EventType.MouseDown:
				if (Event.current.button == 0 && HandleUtility.nearestControl == id && !Event.current.alt)
				{
					GUIUtility.keyboardControl = id;
					GUIUtility.hotControl = id;
					s_CurrentMousePosition = Event.current.mousePosition;
					s_DragStartScreenPosition = Event.current.mousePosition;
					Vector2 b = Handles.matrix.MultiplyPoint(position);
					s_DragScreenOffset = s_CurrentMousePosition - b;
					EditorGUIUtility.SetWantsMouseJumping(1);
					
					Event.current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == id && (Event.current.button == 0 || Event.current.button == 2))
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					s_CurrentMousePosition = Event.current.mousePosition;
					Vector2 center = position;
					position = Handles.inverseMatrix.MultiplyPoint(s_CurrentMousePosition - s_DragScreenOffset);
					if (!Mathf.Approximately((center - position).magnitude, 0f))
					{
						GUI.changed = true;
					}
					Event.current.Use();
				}
				break;
			case EventType.KeyDown:
				if (GUIUtility.hotControl == id && Event.current.keyCode == KeyCode.Escape)
				{
					position = Handles.inverseMatrix.MultiplyPoint(s_DragStartScreenPosition - s_DragScreenOffset);
					GUIUtility.hotControl = 0;
					GUI.changed = true;
					Event.current.Use();
				}
				break;
			case EventType.Repaint:
				if(drawCapFunction != null)
				{
					drawCapFunction(id,position);
				}
				break;
			}
			
			return position;
		}

		public static void PivotCap(int controlID, Vector3 position)
		{
			GUIStyle pivotdot = "U2D.pivotDot";
			GUIStyle pivotdotactive = "U2D.pivotDotActive";
			
			DrawImageBasedCap(controlID, position, pivotdot, pivotdotactive);
		}
		
		public static void RectScalingCap(int controlID, Vector3 position)
		{
			GUIStyle dragdot = "U2D.dragDot";
			GUIStyle dragdotactive = "U2D.dragDotActive";
			
			DrawImageBasedCap(controlID, position, dragdot, dragdotactive);
		}
		
		static void DrawImageBasedCap(int controlID, Vector3 position, GUIStyle normal, GUIStyle active)
		{
			Vector3 vector = Handles.matrix.MultiplyPoint(position);
			Handles.BeginGUI();
			float fixedWidth = normal.fixedWidth;
			float fixedHeight = normal.fixedHeight;
			Rect position2 = new Rect(vector.x - fixedWidth / 2f, vector.y - fixedHeight / 2f, fixedWidth, fixedHeight);
			if (GUIUtility.hotControl == controlID)
			{
				active.Draw(position2, GUIContent.none, controlID);
			}
			else
			{
				normal.Draw(position2, GUIContent.none, controlID);
			}
			Handles.EndGUI();
		}

		static Material handleWireMaterial
		{
			get
			{
				if (!s_HandleWireMaterial)
				{
					s_HandleWireMaterial = (Material)EditorGUIUtility.LoadRequired("SceneView/HandleLines.mat");
					s_HandleWireMaterial2D = (Material)EditorGUIUtility.LoadRequired("SceneView/2DHandleLines.mat");
				}
				return (!Camera.current) ? s_HandleWireMaterial2D : s_HandleWireMaterial;
			}
		}

		static void SetDiscSectionPoints(Vector3[] dest, int count, Vector3 normal, Vector3 from, float angle)
		{
			from.Normalize();
			Quaternion rotation = Quaternion.AngleAxis(angle / (float)(count - 1), normal);
			Vector3 vector = from;
			for (int i = 0; i < count; i++)
			{
				dest[i] = vector;
				vector = rotation * vector;
			}
		}

		public static void DrawCircle(Vector3 center, float radius)
		{
			DrawCircle(center,radius,0f);
		}

		public static void DrawCircle(Vector3 center, float radius, float innerRadius)
		{
			if (Event.current.type != EventType.Repaint || Handles.color.a == 0f)
			{
				return;
			}
			
			innerRadius = Mathf.Clamp01(innerRadius);
			
			if(s_circleArray == null)
			{
				s_circleArray = new Vector3[12];
				SetDiscSectionPoints(s_circleArray, 12, Vector3.forward, Vector3.right, 360f);
			}

			Shader.SetGlobalColor("_HandleColor", Handles.color * new Color(1f, 1f, 1f, 0.5f));
			Shader.SetGlobalFloat("_HandleSize", 1f);
			handleWireMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(4);
			for (int i = 1; i < s_circleArray.Length; i++)
			{
				GL.Color(Handles.color);
				GL.Vertex(center + s_circleArray[i - 1]* radius * innerRadius);
				GL.Vertex(center + s_circleArray[i - 1]*radius);
				GL.Vertex(center + s_circleArray[i]*radius);
				GL.Vertex(center + s_circleArray[i - 1]* radius * innerRadius);
				GL.Vertex(center + s_circleArray[i]*radius);
				GL.Vertex(center + s_circleArray[i]* radius * innerRadius);
			}
			GL.End();
			GL.PopMatrix();
		}

		public static void DrawTriangle(Vector3 center, Vector3 normal, float radius, float innerRadius)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			
			innerRadius = Mathf.Clamp01(innerRadius);
			
			Vector3[] array = new Vector3[4];
			SetDiscSectionPoints(array, 4, normal, Vector3.up, 360f);
			Shader.SetGlobalColor("_HandleColor", Handles.color * new Color(1f, 1f, 1f, 0.5f));
			Shader.SetGlobalFloat("_HandleSize", 1f);
			handleWireMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(4);
			for (int i = 1; i < array.Length; i++)
			{
				GL.Color(Handles.color);
				GL.Vertex(center + array[i - 1]*innerRadius);
				GL.Vertex(center + array[i - 1]*radius);
				GL.Vertex(center + array[i]*radius);
				GL.Vertex(center + array[i - 1]*innerRadius);
				GL.Vertex(center + array[i]*radius);
				GL.Vertex(center + array[i]*innerRadius);
			}
			GL.End();
			GL.PopMatrix();
		}

		public static void DrawSquare(Vector3 center, Vector3 normal, float radius, float innerRadius)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			
			innerRadius = Mathf.Clamp01(innerRadius);
			
			Vector3[] array = new Vector3[5];
			SetDiscSectionPoints(array, 5, normal, Vector3.left + Vector3.up, 360f);
			Shader.SetGlobalColor("_HandleColor", Handles.color * new Color(1f, 1f, 1f, 0.5f));
			Shader.SetGlobalFloat("_HandleSize", 1f);
			handleWireMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(4);
			for (int i = 1; i < array.Length; i++)
			{
				GL.Color(Handles.color);
				GL.Vertex(center + array[i - 1]*innerRadius);
				GL.Vertex(center + array[i - 1]*radius);
				GL.Vertex(center + array[i]*radius);
			}
			GL.End();
			GL.PopMatrix();
		}

		public static void DrawLine (Vector3 p1, Vector3 p2, Vector3 normal, float width)
		{
			DrawLine(p1,p2,normal,width,width);
		}

		public static void DrawLine (Vector3 p1, Vector3 p2, Vector3 normal, float widthP1, float widthP2)
		{
			DrawLine(p1,p2,normal,widthP1,widthP2,Handles.color);
		}

		public static void DrawLine (Vector3 p1, Vector3 p2, Vector3 normal, float widthP1, float widthP2, Color color)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			Vector3 right = Vector3.Cross(normal,p2-p1).normalized;
			handleWireMaterial.SetPass(0);
			GL.PushMatrix ();
			GL.MultMatrix (Handles.matrix);
			GL.Begin (4);
			GL.Color (color);
			GL.Vertex (p1 + right * widthP1 * 0.5f);
			GL.Vertex (p1 - right * widthP1 * 0.5f);
			GL.Vertex (p2 - right * widthP2 * 0.5f);
			GL.Vertex (p1 + right * widthP1 * 0.5f);
			GL.Vertex (p2 - right * widthP2 * 0.5f);
			GL.Vertex (p2 + right * widthP2 * 0.5f);
			GL.End ();
			GL.PopMatrix ();
		}

		static Vector3[] s_array;
		public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			if(s_array == null)
			{
				s_array = new Vector3[60];
			}

			SetDiscSectionPoints(s_array, 60, normal, from, angle);
			handleWireMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(4);
			for (int i = 1; i < s_array.Length; i++)
			{
				GL.Color(color);
				GL.Vertex(center);
				GL.Vertex(center + s_array[i - 1]*radius);
				GL.Vertex(center + s_array[i]*radius);
			}
			GL.End();
			GL.PopMatrix();
		}
	}
}
