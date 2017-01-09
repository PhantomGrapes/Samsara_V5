using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Anima2D 
{
	public class BlendShapeEditor : WindowEditorTool
	{
		public SpriteMeshCache spriteMeshCache;

		protected override string GetHeader() { return "Blendshapes"; }
		//public static GUIContent addKeyframeContent = EditorGUIUtility.IconContent("Animation.AddKeyframe", "Add Keyframe.");
		//public static GUIContent keyframeContent = EditorGUIUtility.IconContent("animationkeyframe");

		//Vector2 m_ScrollPosition = Vector2.zero;
		ReorderableList m_FrameList = null;
		SerializedObject m_SerializedObjectCache;

		//bool m_GUIChanged = false;
		bool m_Active = false;
		Rect m_SelectionRect;
		Vector3 m_RectPosition = Vector3.zero;
		Quaternion m_RectRotation = Quaternion.identity;

		List<Vector2> m_NormalizedVertices = new List<Vector2>();

		float windowHeight {
			get {
				float calculatedHeight = 80f;

				/*if(spriteMeshCache.selectedBlendshape != null)
				{
					calculatedHeight += 65f;

					int numLines = Mathf.Clamp(spriteMeshCache.selectedBlendshape.frames.Count-1,0,int.MaxValue);
					calculatedHeight += (EditorGUIUtility.singleLineHeight + 5) * numLines;
				}*/

				return calculatedHeight;
			}
		}

		public BlendShapeEditor()
		{
			windowRect = new Rect(5f, 5f, 250, 50);

			//SetupList();

			Undo.undoRedoPerformed += UndoRedoPerformed;
		}

		void UndoRedoPerformed()
		{
			//SetupList();
		}

		protected override void DoHide()
		{
			spriteMeshCache.selectedBlendshape = null;
			
			base.DoHide();
		}

		public override void OnWindowGUI(Rect viewRect)
		{
			//windowRect.height = Mathf.Min(viewRect.height - 10, windowHeight);
			base.OnWindowGUI(viewRect);
		}

		protected override void DoWindow(int windowId)
		{
			EditorGUIUtility.labelWidth = 50f;


			EditorGUILayout.BeginVertical();

			EditorGUILayout.BeginHorizontal();

			EditorGUI.BeginChangeCheck();

			int blendshapeIndex = spriteMeshCache.blendshapes.IndexOf(spriteMeshCache.selectedBlendshape);

			blendshapeIndex = EditorGUILayout.Popup(blendshapeIndex,GetBlendshapeNames());

			
			if(EditorGUI.EndChangeCheck())
			{
				spriteMeshCache.RegisterUndo("select blendshape");
				spriteMeshCache.selectedBlendshape = spriteMeshCache.blendshapes[blendshapeIndex];

				SetupList();
			}
			
			if(GUILayout.Button("New"))
			{
				CreateBlendshape();
				//ValidateBlendshape();
				//SetupList();
			}
			
			EditorGUI.BeginDisabledGroup(spriteMeshCache.selectedBlendshape == null);
			
			if(GUILayout.Button("Delete"))
			{
				DeleteBlendshape();
				//ValidateBlendshape();
				//SetupList();
			}
			
			EditorGUI.EndDisabledGroup();
			
			
			//if(GUILayout.Button(addKeyframeContent))
			//{
			
			//}
			
			EditorGUILayout.EndHorizontal();


			EditorGUI.BeginDisabledGroup(spriteMeshCache.selectedBlendshape == null);

			EditorGUI.BeginChangeCheck();

			string name = "";

			if(spriteMeshCache.selectedBlendshape != null)
			{
				name = spriteMeshCache.selectedBlendshape.name;
			}

			name = EditorGUILayout.TextField("Name",name);

			if(EditorGUI.EndChangeCheck())
			{
				if(spriteMeshCache.selectedBlendshape != null)
				{
					spriteMeshCache.RegisterUndo("change name");
					spriteMeshCache.selectedBlendshape.name = name;
				}
			}

			EditorGUI.EndDisabledGroup();


			/*
			m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

			if(m_SerializedObjectCache != null && m_FrameList != null)
			{
				//m_SerializedObjectCache.Update();

				//m_FrameList.DoLayoutList();

				//m_SerializedObjectCache.ApplyModifiedProperties();
			}

			EditorGUILayout.EndScrollView();
			*/

			EditorGUILayout.Space();

			Texture2D eventMarker = EditorGUIUtility.IconContent("Animation.EventMarker").image as Texture2D;
			GUIStyle thumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
			GUIStyle slider = new GUIStyle(GUI.skin.horizontalSlider);
			GUI.skin.horizontalSliderThumb.normal.background = eventMarker;
			GUI.skin.horizontalSliderThumb.active.background = eventMarker;
			GUI.skin.horizontalSliderThumb.focused.background = eventMarker;
			GUI.skin.horizontalSliderThumb.hover.background = eventMarker;
			GUI.skin.horizontalSliderThumb.fixedWidth = eventMarker.width-2;
			GUI.skin.horizontalSliderThumb.fixedHeight = eventMarker.height;
			GUI.skin.horizontalSliderThumb.stretchWidth = false;
			GUI.skin.horizontalSlider.padding = new RectOffset(0,0,-10,0);

			EditorGUIExt.SetSliderLablels(new GUIContent(""),new GUIContent("100"));
			v = EditorGUIExt.Slider(v,0f,100f);
			EditorGUILayout.Space();
			//v = EditorGUILayout.Slider(v,0f,100f);

			GUI.skin.horizontalSliderThumb = thumb;
			GUI.skin.horizontalSlider = slider;

			EditorGUILayout.EndVertical();

			EditorGUIUtility.labelWidth = 0f;

			GUIContent keyframeContent = EditorGUIUtility.IconContent("animationkeyframe");
		
			if(Event.current.type == EventType.Repaint)
			{
				Rect rect = new Rect(20f,69f,keyframeContent.image.width,keyframeContent.image.height);
				
				Color color = GUI.color;
				GUI.color = Color.white;
				GUI.DrawTexture(rect,keyframeContent.image as Texture2D);

				rect.x += 15;
				GUI.DrawTexture(rect,keyframeContent.image as Texture2D);
				rect.x += 15;
				GUI.DrawTexture(rect,keyframeContent.image as Texture2D);
				rect.x += 15;
				GUI.DrawTexture(rect,keyframeContent.image as Texture2D);
				GUI.color = color;
			}

			/*
			if(m_GUIChanged)
			{
				m_GUIChanged = false;

				spriteMeshCache.selectedFrameIndex = m_FrameList.index;

				ValidateBlendshape();

				DoGUIChanged();
			}
			*/
		}

		float v = 0f;

		protected override void DoGUI()
		{
			if(m_Active &&
			   spriteMeshCache.selectedNodes.Count > 0)
			{
				if(!EditorGUI.actionKey)
				{
					EditorGUI.BeginChangeCheck();

					RectHandles.Do(ref m_SelectionRect,ref m_RectPosition, ref m_RectRotation);

					if(EditorGUI.EndChangeCheck())
					{
						DenormalizeVertices();
					}
				}else{
					RectHandles.RenderRect(m_SelectionRect,m_RectPosition,m_RectRotation,new Color(0.25f, 0.5f, 1f, 0.8f), 0f, 0.8f);
				}
			}
		}

		public void ActivateRectTool(bool activate)
		{
			if(activate && spriteMeshCache.selectedNodes.Count <= 1)
			{
				m_Active = false;
				return;
			}

			if(activate)
			{
				m_SelectionRect = GetSelectedVerticesRect();

				if(!m_Active)
				{
					m_RectPosition = m_SelectionRect.center;
				}

				m_SelectionRect.min -= (Vector2)m_RectPosition;
				m_SelectionRect.max -= (Vector2)m_RectPosition;
				m_RectRotation = Quaternion.identity;

				NormalizeVertices();
			}

			m_Active = activate;
		}

		void NormalizeVertices()
		{
			m_NormalizedVertices.Clear();

			foreach(Node node in spriteMeshCache.selectedNodes)
			{
				Vector2 v = spriteMeshCache.GetPosition(node);

				v = v - ((Vector2)m_RectPosition + m_SelectionRect.min);
				v.x /= m_SelectionRect.width;
				v.y /= m_SelectionRect.height;

				m_NormalizedVertices.Add(v);
			}
		}

		void DenormalizeVertices()
		{
			for (int i = 0; i < m_NormalizedVertices.Count; i++)
			{
				Vector2 v = m_RectRotation * (Vector2.Scale (m_NormalizedVertices[i], m_SelectionRect.size) + m_SelectionRect.min) + m_RectPosition;
				spriteMeshCache.SetPosition(spriteMeshCache.selectedNodes[i], v);
			}
		}

		Rect GetSelectedVerticesRect()
		{
			Rect rect = new Rect();

			Vector2 min = new Vector2(float.MaxValue,float.MaxValue);
			Vector2 max = new Vector2(float.MinValue,float.MinValue);

			foreach(Node node in spriteMeshCache.selectedNodes)
			{
				Vector2 v = spriteMeshCache.GetPosition(node);
				
				if(v.x < min.x) min.x = v.x;
				if(v.y < min.y) min.y = v.y;
				if(v.x > max.x) max.x = v.x;
				if(v.y > max.y) max.y = v.y;
			}

			Vector2 offset = Vector2.one * 5f;
			rect.min = min - offset;
			rect.max = max + offset;

			return rect;
		}

		void ValidateBlendshape()
		{
			if(spriteMeshCache.selectedBlendshape.frames.Count == 0)
			{
				spriteMeshCache.CreateFrame(spriteMeshCache.selectedBlendshape,100f);
				spriteMeshCache.selectedFrameIndex = 0;
				m_FrameList.index = 0;
			}
		}

		void CreateBlendshape()
		{
			spriteMeshCache.RegisterUndo("new blendshape");

			if(spriteMeshCache)
			{
				spriteMeshCache.selectedBlendshape = spriteMeshCache.CreateBlendshape("New blendshape");
				spriteMeshCache.selectedFrameIndex = 0;
			}
		}

		void DeleteBlendshape()
		{
			spriteMeshCache.RegisterUndo("delete blendshape");
			
			if(spriteMeshCache)
			{
				spriteMeshCache.DeleteBlendshape(spriteMeshCache.selectedBlendshape);
				spriteMeshCache.selectedBlendshape = null;
			}
		}

		void SetupList()
		{
			if(spriteMeshCache)
			{
				m_SerializedObjectCache = null;
				m_FrameList = null;

				int selectedBlendshapeIndex = spriteMeshCache.blendshapes.IndexOf(spriteMeshCache.selectedBlendshape);

				if(selectedBlendshapeIndex >= 0)
				{
					m_SerializedObjectCache = new SerializedObject(spriteMeshCache);
					SerializedProperty selectedBlendshapeProp = m_SerializedObjectCache.FindProperty("blendshapes").GetArrayElementAtIndex(selectedBlendshapeIndex);
					SerializedProperty framesProp = selectedBlendshapeProp.FindPropertyRelative("frames");

					m_FrameList = new ReorderableList(m_SerializedObjectCache,framesProp,true,true,true,true);
						
					m_FrameList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
							
						SerializedProperty shapeFrameProp = m_FrameList.serializedProperty.GetArrayElementAtIndex(index);
						SerializedProperty weightProp = shapeFrameProp.FindPropertyRelative("weight");

						rect.y += 1.5f;

						EditorGUIUtility.fieldWidth = 32f;
						EditorGUI.Slider( new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),weightProp, -100, 100);
						EditorGUIUtility.fieldWidth = 0f;
					};
						
					m_FrameList.drawHeaderCallback = (Rect rect) => {  
						EditorGUI.LabelField(rect, "Frames");
					};

					m_FrameList.onSelectCallback = (ReorderableList list) => {
						//m_GUIChanged = true;
					};

					m_FrameList.onAddCallback = (ReorderableList list) => {
						ReorderableList.defaultBehaviours.DoAddButton(list);
						//m_GUIChanged = true;
					};

					m_FrameList.onRemoveCallback = (ReorderableList list) => {
						ReorderableList.defaultBehaviours.DoRemoveButton(list);
						//m_GUIChanged = true;
					};

					spriteMeshCache.selectedFrameIndex = 0;
					m_FrameList.index = 0;

					//m_GUIChanged = true;
				}
			}
		}
		
		string[] GetBlendshapeNames()
		{
			List<string> names = new List<string>();

			if(spriteMeshCache)
			{
				for (int i = 0; i < spriteMeshCache.blendshapes.Count; i++)
				{
					BlendShape blendshape = spriteMeshCache.blendshapes [i];
					names.Add("" + i + " "+ blendshape.name);
				}
			}

			return names.ToArray();
		}
	}
}
