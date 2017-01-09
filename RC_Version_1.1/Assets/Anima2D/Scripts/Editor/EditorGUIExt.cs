using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace Anima2D
{
	public class EditorGUIExt
	{
		struct SliderLabels
		{
			public GUIContent leftLabel;
			public GUIContent rightLabel;
			public void SetLabels(GUIContent leftLabel, GUIContent rightLabel)
			{
				if (Event.current.type == EventType.Repaint)
				{
					this.leftLabel = leftLabel;
					this.rightLabel = rightLabel;
				}
			}
			public bool HasLabels()
			{
				return Event.current.type == EventType.Repaint && this.leftLabel != null && this.rightLabel != null;
			}
		}

		static string kFloatFieldFormatString = "g7";
		static string kIntFieldFormatString = "#######0";

		static SliderLabels sliderLabels;

		static EditorGUIExt()
		{
			sliderLabels = default(SliderLabels);
		}

		public static BoneWeight Weight(BoneWeight boneWeight,int weightIndex, string[] names, bool mixedBoneIndex = false, bool mixedWeight = false)
		{
			int boneIndex = 0;
			float weight = 0f;
			
			boneWeight.GetWeight(weightIndex,out boneIndex,out weight);
			
			EditorGUIUtility.labelWidth = 30f;
			
			EditorGUILayout.BeginHorizontal();

			EditorGUI.BeginChangeCheck();

			EditorGUI.showMixedValue = mixedBoneIndex;
			int newBoneIndex = EditorGUILayout.Popup(boneIndex + 1,names,GUILayout.Width(100f)) - 1;

			EditorGUI.BeginDisabledGroup(newBoneIndex == -1);

			EditorGUI.showMixedValue = mixedWeight;
			weight = EditorGUILayout.Slider(weight,0f,1f);
			
			EditorGUI.EndDisabledGroup();
			
			EditorGUILayout.EndHorizontal();

			if(EditorGUI.EndChangeCheck())
			{
				if(newBoneIndex == -1)
				{
					boneWeight.Unassign(boneIndex);
				}
				boneWeight.SetWeight(weightIndex,newBoneIndex,weight);
			}
			
			return boneWeight;
		}

		public static void SetSliderLablels(GUIContent leftLabel, GUIContent rightLabel)
		{
			sliderLabels.SetLabels(leftLabel,rightLabel);
		}

		public static float Slider(float value, float leftValue, float rightValue, params GUILayoutOption[] options)
		{
			return Slider(GUILayoutUtility.GetRect(GUIContent.none,GUI.skin.horizontalSlider), value, leftValue, rightValue);
		}

		public static float Slider(Rect position, float value, float leftValue, float rightValue)
		{
			int controlID = GUIUtility.GetControlID("EditorSlider".GetHashCode(), EditorGUIUtility.native, position);
			return DoSlider(EditorGUI.IndentedRect(position), DragZoneRect(position), controlID, value, leftValue, rightValue, kFloatFieldFormatString);
		}

		static float DoSlider(Rect position, Rect dragZonePosition, int id, float value, float left, float right, string formatString)
		{
			float num = position.width;
			if (num >= 65f + EditorGUIUtility.fieldWidth)
			{
				float num2 = num - 5f - EditorGUIUtility.fieldWidth;
				EditorGUI.BeginChangeCheck();
				int controlID = GUIUtility.GetControlID("EditorSliderKnob".GetHashCode(), FocusType.Native, position);
				if (GUIUtility.keyboardControl == id && !RecycledEditorIsEditingControl(id))
				{
					GUIUtility.keyboardControl = controlID;
				}
				float start = left;
				float end = right;

				Rect position2 = new Rect(position.x, position.y, num2, position.height);

				value = GUI.Slider(position2, value, 0f, start, end, GUI.skin.horizontalSlider, (!EditorGUI.showMixedValue) ? GUI.skin.horizontalSliderThumb : "SliderMixed", true, controlID);

				if(sliderLabels.HasLabels() && start <= 0f && end >= 0f)
				{
					Color color = GUI.color;
					GUI.color *= new Color(1f, 1f, 1f, 0.75f);
					float x = -start * (position2.width - 10f) / (end - start);
					Rect rect = new Rect(position2);
					rect.x += x;
					rect.y += 10f;
					rect.height = position2.height;
					DoLabel(rect,new GUIContent("0"),TextAnchor.UpperLeft,EditorStyles.miniLabel);
					GUI.color = color;
				}

				if(sliderLabels.HasLabels())
				{
					Color color = GUI.color;
					GUI.color *= new Color(1f, 1f, 1f, 0.75f);
					Rect rect = new Rect(position2.x, position2.y + 10f, position2.width, position2.height);
					DoLeftRightLabels(rect, sliderLabels.leftLabel, sliderLabels.rightLabel, EditorStyles.miniLabel);
					GUI.color = color;
					sliderLabels.SetLabels(null, null);
				}
				if (GUIUtility.keyboardControl == controlID || GUIUtility.hotControl == controlID)
				{
					GUIUtility.keyboardControl = id;
				}
				if (GUIUtility.keyboardControl == id && Event.current.type == EventType.KeyDown && !RecycledEditorIsEditingControl(id) && (Event.current.keyCode == KeyCode.LeftArrow || Event.current.keyCode == KeyCode.RightArrow))
				{
					float num3 = MathUtils.GetClosestPowerOfTen(Mathf.Abs((right - left) * 0.01f));
					if (formatString == kIntFieldFormatString && num3 < 1f)
					{
						num3 = 1f;
					}
					if (Event.current.shift)
					{
						num3 *= 10f;
					}
					if (Event.current.keyCode == KeyCode.LeftArrow)
					{
						value -= num3 * 0.5001f;
					}
					else
					{
						value += num3 * 0.5001f;
					}
					value = MathUtils.RoundToMultipleOf(value, num3);
					GUI.changed = true;
					Event.current.Use();
				}
				if (EditorGUI.EndChangeCheck())
				{
					float f = (right - left) / (num2 - (float)GUI.skin.horizontalSlider.padding.horizontal - GUI.skin.horizontalSliderThumb.fixedWidth);
					value = MathUtils.RoundBasedOnMinimumDifference(value, Mathf.Abs(f));
					if(RecycledEditorIsEditingControl(id))
					{
						RecycledEditorEndEditing();
					}
				}
				value = DoFloatField(new Rect(position.x + num2 + 5f, position.y, EditorGUIUtility.fieldWidth, position.height), dragZonePosition, id, value, formatString, EditorStyles.numberField, true);
			}
			else
			{
				num = Mathf.Min(EditorGUIUtility.fieldWidth, num);
				position.x = position.xMax - num;
				position.width = num;
				value = DoFloatField(position, dragZonePosition, id, value, formatString, EditorStyles.numberField, true);
			}
			value = Mathf.Clamp(value, Mathf.Min(left, right), Mathf.Max(left, right));
			return value;
		}

		static void DoLabel(Rect rect, GUIContent label, TextAnchor alignment, GUIStyle labelStyle)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
						
			TextAnchor l_alignment = labelStyle.alignment;
			labelStyle.alignment = alignment;
			GUI.Label(rect, label, labelStyle);
			labelStyle.alignment = l_alignment;
		}

		static void DoLeftRightLabels(Rect rect, GUIContent leftLabel, GUIContent rightLabel, GUIStyle labelStyle)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			rect.x -= 5f;
			rect.xMax += 10f;

			DoLabel(rect,leftLabel,TextAnchor.UpperLeft,labelStyle);
			DoLabel(rect,rightLabel,TextAnchor.UpperRight,labelStyle);
		}

		static Rect DragZoneRect(Rect position)
		{
			return new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
		}

		static bool RecycledEditorIsEditingControl(int id)
		{
			bool result = false;

			FieldInfo fieldInfo = typeof(EditorGUI).GetField("s_RecycledEditor", BindingFlags.Static | BindingFlags.NonPublic);
			
			if(fieldInfo != null)
			{
				object recycleEditor = fieldInfo.GetValue(null);

				System.Type recycleEditorType = fieldInfo.FieldType;

				MethodInfo methodInfo = recycleEditorType.GetMethod("IsEditingControl", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if(recycleEditor != null && methodInfo != null)
				{
					object[] parameters = new object[] { id };
					result = (bool)methodInfo.Invoke(recycleEditor,parameters);
				}

			}

			return result;
		}

		static void RecycledEditorEndEditing()
		{
			FieldInfo fieldInfo = typeof(EditorGUI).GetField("s_RecycledEditor", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
			
			if(fieldInfo != null)
			{
				object recycleEditor = fieldInfo.GetValue(null);

				MethodInfo methodInfo = fieldInfo.FieldType.GetMethod("EndEditing", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				
				if(recycleEditor != null && methodInfo != null)
				{
					methodInfo.Invoke(recycleEditor,null);
				}
			}
		}

		static float DoFloatField(Rect position, Rect dragHotZone, int id, float value, string formatString, GUIStyle style, bool draggable)
		{
			float result = 0f;

			FieldInfo recycleEditorFieldInfo = typeof(EditorGUI).GetField("s_RecycledEditor",BindingFlags.Static | BindingFlags.NonPublic);
			object recycleEditor = recycleEditorFieldInfo.GetValue(null);
			
			MethodInfo methodInfo = typeof(EditorGUI).GetMethod("DoFloatField", BindingFlags.Static | BindingFlags.NonPublic, null, new [] { recycleEditorFieldInfo.FieldType,typeof(Rect),typeof(Rect),typeof(int),typeof(float),typeof(string),typeof(GUIStyle),typeof(bool) },null);

			if(methodInfo != null)
			{
				if(recycleEditor != null)
				{
					object[] parameters = new object[] { recycleEditor,position,dragHotZone,id,value,formatString,style,draggable };
					result = (float)methodInfo.Invoke(null,parameters);
				}
			}

			return result;
		}

		public static void SortingLayerField(GUIContent label, SerializedProperty layerID, GUIStyle style, GUIStyle labelStyle)
		{
			MethodInfo methodInfo = typeof(EditorGUILayout).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, new [] { typeof(GUIContent),typeof(SerializedProperty),typeof(GUIStyle),typeof(GUIStyle) },null);
			
			if(methodInfo != null)
			{
				object[] parameters = new object[] { label, layerID, style, labelStyle };
				methodInfo.Invoke(null,parameters);
			}
		}
	}
}
