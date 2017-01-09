using UnityEngine;
using UnityEngine.Sprites;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Anima2D 
{
	class SpriteMeshEditorWindow : TextureEditorWindow
	{
		public enum Mode {
			Mesh,
			Blendshapes
		}
		
		WeightEditor weightEditor = new WeightEditor();
		BlendShapeEditor blendShapeEditor = new BlendShapeEditor();
		InspectorEditor inspectorEditor = new InspectorEditor();
		MeshToolEditor meshToolEditor = new MeshToolEditor();
		
		SpriteMeshCache m_SpriteMeshCache;
		
		Mode mode { get; set; }
		
		Material mMeshGuiMaterial;
		Material meshGuiMaterial {
			get {
				if(!mMeshGuiMaterial)
				{
					mMeshGuiMaterial = new Material(Shader.Find("Hidden/Internal-GUITextureClip"));
					mMeshGuiMaterial.hideFlags = HideFlags.DontSave;
				}
				
				return mMeshGuiMaterial;
			}
		}
		
		Texture2D mDotTexture;
		Texture2D dotTexture {
			get {
				if(!mDotTexture)
				{
					mDotTexture = EditorGUIUtility.Load("Anima2D/dot.png") as Texture2D;
					mDotTexture.hideFlags = HideFlags.DontSave;
				}
				
				return mDotTexture;
			}
		}
		
		Node hoveredNode { get; set; }
		Hole hoveredHole { get; set; }
		Edge hoveredEdge { get; set; }
		BindInfo hoveredBindPose { get; set; }
		Bone2D hoveredBone { get; set; }
		
		TextureImporter m_TextureImporter = null;
		Texture2D m_OriginalTexture = null;
		
		List<Color> m_BindPoseColors;
		int m_selectionControlID = 0;
		List<int> m_SelectionIndices = new List<int>();
		
		[MenuItem("Window/Anima2D/SpriteMesh Editor",false,0)]
		static void ContextInitialize()
		{
			GetWindow();
		}
		
		public static SpriteMeshEditorWindow GetWindow()
		{
			return EditorWindow.GetWindow<SpriteMeshEditorWindow>("SpriteMesh Editor");
		}
		
		void OnEnable()
		{
			Undo.undoRedoPerformed -= UndoRedoPerformed;
			Undo.undoRedoPerformed += UndoRedoPerformed;
			
			showBones = true;
			
			weightEditor.canShow = () => {
				return  meshToolEditor.canShow() &&
				m_SpriteMeshCache.isBound;
			};
			
			inspectorEditor.canShow = () => {
				return  meshToolEditor.canShow();
			};
			
			meshToolEditor.canShow = () => {
				return  mode == Mode.Mesh;
			};
			
			blendShapeEditor.canShow = () => {
				return mode == Mode.Blendshapes;
			};
			
			UpdateFromSelection();
			
			EditorApplication.delayCall += () => {
				RefreshTexture(true);
				SetScrollPositionAndZoomFromSprite();
			};
		}
		
		void OnDisable()
		{
			Undo.undoRedoPerformed -= UndoRedoPerformed;
			
			if(mMeshGuiMaterial)
			{
				DestroyImmediate(mMeshGuiMaterial);
			}
			
			if(m_Texture)
			{
				DestroyImmediate(m_Texture);
			}
		}
		
		void UndoRedoPerformed()
		{
			Repaint();
		}
		
		bool showBones { get; set; }
		bool showTriangles { get; set; }
		
		Vector2 mMousePositionWorld;
		
		List<Vector2> m_Points;
		List<Vector2> m_UVs;
		
		Edge m_ClosestEdge = null;
		
		Matrix4x4 m_SpriteMeshMatrix = new Matrix4x4();
		
		float mPointSize = 6f;
		
		Color vertexColor {
			get {
				return Color.cyan;
			}
		}
		
		int m_LastNearestControl = -1;
		
		override protected void OnGUI()
		{
			Matrix4x4 matrix = Handles.matrix;
			
			textureColor = Color.gray;
			
			if (!m_SpriteMeshCache || !m_SpriteMeshCache.spriteMesh)
			{
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Label("No sprite mesh selected");
				EditorGUI.EndDisabledGroup();
			}else{
				autoRepaintOnSceneChange = true;
				wantsMouseMove = true;
				antiAlias = 2;
				
				HotKeys();
				
				if(Event.current.type == EventType.MouseUp && Event.current.button == 1)
				{
					meshToolEditor.tool = MeshToolEditor.MeshTool.None;
				}
				
				base.OnGUI();
				
				GUI.color = Color.white;
				
				GUILayout.BeginArea(m_TextureViewRect);
				
				BeginWindows();
				
				meshToolEditor.spriteMeshCache = m_SpriteMeshCache;
				meshToolEditor.OnWindowGUI(m_TextureViewRect);
				
				weightEditor.spriteMeshCache = m_SpriteMeshCache;
				weightEditor.OnWindowGUI(m_TextureViewRect);
				
				blendShapeEditor.spriteMeshCache = m_SpriteMeshCache;
				blendShapeEditor.OnWindowGUI(m_TextureViewRect);
				
				inspectorEditor.spriteMeshCache = m_SpriteMeshCache;
				inspectorEditor.OnWindowGUI(m_TextureViewRect);
				
				EndWindows();
				
				if(!meshToolEditor.sliceToggle)
				{
					HandleDeleteVertex();
					HandleDeleteHole();
					HandleDeleteEdge();
					HandleDeleteBone();
					HandleDeleteBindPose();
					
				}
				
				GUILayout.EndArea();
			}
			
			Handles.matrix = matrix;
			
			if(GUIUtility.hotControl == 0 && HandleUtility.nearestControl != m_LastNearestControl)
			{
				m_LastNearestControl = HandleUtility.nearestControl;
				Repaint();
			}
		}
		
		override protected void DoTextureGUIExtras()
		{
			m_selectionControlID = GUIUtility.GetControlID("SelectionRect".GetHashCode(), FocusType.Keyboard);
			
			m_Points = m_SpriteMeshCache.GetPositions();
			m_UVs = m_SpriteMeshCache.uvs;
			m_BindPoseColors = m_SpriteMeshCache.bindPoses.ConvertAll( b => b.color );
			
			hoveredNode = null;
			hoveredEdge = null;
			hoveredHole = null;
			hoveredBindPose = null;
			hoveredBone = null;
			m_ClosestEdge = null;
			
			mMousePositionWorld = ScreenToWorld(Event.current.mousePosition - new Vector2(2f,3f));
			
			if(mode == Mode.Mesh)
			{
				m_ClosestEdge = GetClosestEdge(mMousePositionWorld);
			}
			
			if(showBones)
			{
				if(m_SpriteMeshCache.isBound)
				{
					BindPosesGUI(meshToolEditor.sliceToggle ||
					             mode != Mode.Mesh);
				}else{
					BonesGUI(meshToolEditor.sliceToggle ||
					         mode != Mode.Mesh);
				}
			}
			
			EdgesGUI(meshToolEditor.sliceToggle);
			VerticesGUI(meshToolEditor.sliceToggle);
			
			if(mode == Mode.Mesh)
			{
				PivotHandleGUI(meshToolEditor.sliceToggle);
				HolesGUI(meshToolEditor.sliceToggle);
				
				if(!meshToolEditor.sliceToggle)
				{
					HandleAddVertex();
					HandleAddHole();
					HandleAddEdge();
					HandleAddBindPose();
				}
			}
			
			if(!meshToolEditor.sliceToggle)
			{
				HandleSelection();
			}
		}
		
		void HandleAddBindPose()
		{
			if(m_SpriteMeshCache.spriteMeshInstance && DragAndDrop.objectReferences.Length > 0)
			{
				EventType eventType = Event.current.GetTypeForControl(0);
				
				List<GameObject> dragAndDropGameObjects = DragAndDrop.objectReferences.ToList().ConvertAll( o => o as GameObject);
				List<Bone2D> dragAndDropBones = dragAndDropGameObjects.ConvertAll( go => go ? go.GetComponent<Bone2D>() : null );
				
				if(eventType == EventType.DragPerform)
				{
					bool performAutoWeights = m_SpriteMeshCache.bindPoses.Count == 0;
					
					m_SpriteMeshCache.RegisterUndo("add bind pose");
					foreach(Bone2D bone in dragAndDropBones)
					{
						m_SpriteMeshCache.BindBone(bone);
					}
					
					if(performAutoWeights)
					{
						m_SpriteMeshCache.CalculateAutomaticWeights();
					}
					
					//EditorUtility.SetDirty(m_SpriteMeshCache.spriteMeshInstance);
					
					Event.current.Use();
				}
				
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			}
		}
		
		void HandleDeleteBone()
		{
			EventType eventType = Event.current.GetTypeForControl(0);
			
			if(eventType == EventType.KeyDown &&
			   (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete) &&
			   m_SpriteMeshCache.selectedBone &&
			   m_SpriteMeshCache.selectedNodes.Count == 0 &&
			   !m_SpriteMeshCache.selectedHole &&
			   !m_SpriteMeshCache.selectedEdge)
			{
				Undo.RegisterCompleteObjectUndo(m_SpriteMeshCache.spriteMeshInstance,"delete bone");
				m_SpriteMeshCache.DeleteBone(m_SpriteMeshCache.selectedBone);
				
				Event.current.Use();
			}
		}
		
		void HandleDeleteBindPose()
		{
			EventType eventType = Event.current.GetTypeForControl(0);
			
			if(eventType == EventType.KeyDown &&
			   (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete) &&
			   m_SpriteMeshCache.selectedBindPose &&
			   m_SpriteMeshCache.selectedNodes.Count == 0 &&
			   !m_SpriteMeshCache.selectedHole &&
			   !m_SpriteMeshCache.selectedEdge)
			{
				m_SpriteMeshCache.RegisterUndo("delete bind pose");
				m_SpriteMeshCache.DeleteBindPose(m_SpriteMeshCache.selectedBindPose);
				
				Event.current.Use();
			}
		}
		
		void HandleSelection()
		{
			if(Event.current.GetTypeForControl(0) == EventType.MouseDown &&
			   !Event.current.alt &&
			   (Event.current.button == 0 && HandleUtility.nearestControl == m_selectionControlID) ||
			   Event.current.button == 1)
			{
				if((!EditorGUI.actionKey && m_SpriteMeshCache.selectedNodes.Count > 0) ||
				   m_SpriteMeshCache.selectedEdge ||
				   m_SpriteMeshCache.selectedHole ||
				   ((m_SpriteMeshCache.selectedBindPose || m_SpriteMeshCache.selectedBone) && Event.current.button == 1))
				{
					m_SpriteMeshCache.RegisterUndo("selection");
					m_SpriteMeshCache.ClearSelection();
					m_SpriteMeshCache.selectedEdge = null;
					m_SpriteMeshCache.selectedHole = null;
					blendShapeEditor.ActivateRectTool(false);
					
					if(Event.current.button == 1 && (m_SpriteMeshCache.selectedBindPose || m_SpriteMeshCache.selectedBone))
					{
						m_SpriteMeshCache.selectedBindPose = null;
						m_SpriteMeshCache.selectedBone = null;
					}
				}
			}
			
			if(GUIUtility.hotControl == m_selectionControlID &&
			   Event.current.GetTypeForControl(m_selectionControlID) == EventType.MouseUp &&
			   Event.current.button == 0 &&
			   m_SelectionIndices.Count > 0)
			{
				m_SpriteMeshCache.RegisterUndo("selection");
				
				for (int i = 0; i < m_SelectionIndices.Count; i++)
				{
					m_SpriteMeshCache.Select(m_SpriteMeshCache.nodes[m_SelectionIndices[i]],true);
				}
				
				m_SelectionIndices.Clear();
				
				blendShapeEditor.ActivateRectTool(true);
			}
			
			SelectionRectTool.Do(m_selectionControlID,m_Points,m_SelectionIndices);
		}
		
		void BonesGUI(bool disable)
		{
			if(!m_SpriteMeshCache.spriteMeshInstance) return;
			
			Matrix4x4 old = Handles.matrix;
			Handles.matrix = m_SpriteMeshMatrix;
			float radius = 7.5f / m_TextureImporter.spritePixelsPerUnit / m_Zoom;
			
			for (int i = 0; i < m_SpriteMeshCache.spriteMeshInstance.bones.Count; i++)
			{
				int controlID = GUIUtility.GetControlID ("BonesHandle".GetHashCode(), FocusType.Passive);
				EventType eventType = Event.current.GetTypeForControl(controlID);
				
				Bone2D bone = m_SpriteMeshCache.spriteMeshInstance.bones[i];
				
				if(bone)
				{
					Vector3 position = m_SpriteMeshCache.spriteMeshInstance.transform.InverseTransformPoint(bone.transform.position);
					Vector3 endPoint = m_SpriteMeshCache.spriteMeshInstance.transform.InverseTransformPoint(bone.endPosition);
					
					if(!disable)
					{
						if(HandleUtility.nearestControl == controlID &&
						   GUIUtility.hotControl == 0)
						{
							hoveredBone = bone;
							
							if(eventType == EventType.MouseDown &&
							   Event.current.button == 0 &&
							   !Event.current.shift &&
							   !Event.current.alt)
							{
								if(m_SpriteMeshCache.selectedBone != bone)
								{
									m_SpriteMeshCache.RegisterUndo("select bone");
									m_SpriteMeshCache.selectedBone = bone;
								}
								
								Event.current.Use();
							}
						}
						
						if(eventType == EventType.Layout)
						{
							HandleUtility.AddControl(controlID, HandleUtility.DistanceToLine(position,endPoint));
						}
					}
					
					if(bone && eventType == EventType.Repaint)
					{
						if((GUIUtility.hotControl == 0 && HandleUtility.nearestControl == controlID) ||
						   m_SpriteMeshCache.selectedBone == bone)
						{
							BoneUtils.DrawBoneOutline(position,endPoint,radius,radius*0.25f,Color.yellow);
						}
						
						Color color = bone.color;
						color.a = 1f;
						BoneUtils.DrawBoneBody(position,endPoint,radius,color);
						color = bone.color * 0.25f;
						color.a = 1f;
						BoneUtils.DrawBoneCap(position,radius,color);
					}
				}
			}
			
			Handles.matrix = old;
		}
		
		void BindPosesGUI(bool disable)
		{
			Matrix4x4 old = Handles.matrix;
			Handles.matrix = m_SpriteMeshMatrix;
			float radius = 7.5f / m_TextureImporter.spritePixelsPerUnit / m_Zoom;
			
			for (int i = 0; i < m_SpriteMeshCache.bindPoses.Count; i++)
			{
				int controlID = GUIUtility.GetControlID ("BindPoseHandle".GetHashCode(), FocusType.Passive);
				EventType eventType = Event.current.GetTypeForControl(controlID);
				
				BindInfo bindPose = m_SpriteMeshCache.bindPoses[i];
				
				if(!disable)
				{
					if(HandleUtility.nearestControl == controlID &&
					   GUIUtility.hotControl == 0)
					{
						hoveredBindPose = bindPose;
						
						if(eventType == EventType.MouseDown &&
						   Event.current.button == 0 &&
						   !Event.current.shift &&
						   !Event.current.alt)
						{
							if(m_SpriteMeshCache.selectedBindPose != bindPose)
							{
								m_SpriteMeshCache.RegisterUndo("select bind pose");
								m_SpriteMeshCache.selectedBindPose = bindPose;
							}
							
							Event.current.Use();
						}
					}
					
					if(eventType == EventType.Layout)
					{
						HandleUtility.AddControl(controlID, HandleUtility.DistanceToLine(bindPose.position,bindPose.endPoint));
					}
				}
				
				if(eventType == EventType.Repaint)
				{
					Color innerColor = m_BindPoseColors[i] * 0.25f;
					innerColor.a = 1f;
					
					if((GUIUtility.hotControl == 0 && HandleUtility.nearestControl == controlID) ||
					   m_SpriteMeshCache.selectedBindPose == bindPose)
					{
						BoneUtils.DrawBoneOutline(bindPose.position,bindPose.endPoint,radius,radius*0.25f,Color.white);
					}else if(mode == Mode.Mesh && weightEditor.overlayColors)
					{
						Color c = m_BindPoseColors[i] * 0.5f;
						c.a = 1f;
						
						BoneUtils.DrawBoneOutline(bindPose.position,bindPose.endPoint,radius,radius*0.25f,c);
					}
					
					BoneUtils.DrawBoneBody(bindPose.position, bindPose.endPoint, radius, m_BindPoseColors[i]);
					Color color = m_BindPoseColors[i] * 0.25f;
					color.a = 1f;
					BoneUtils.DrawBoneCap(bindPose.position, radius,color);
				}
			}
			
			Handles.matrix = old;
		}
		
		void HolesGUI(bool disable)
		{
			GUI.color = Color.white;
			
			for (int i = 0; i < m_SpriteMeshCache.holes.Count; i++)
			{
				Hole hole = m_SpriteMeshCache.holes[i];
				Vector2 position = hole.vertex;
				
				int controlID = GUIUtility.GetControlID ("HoleHandle".GetHashCode(), FocusType.Passive);
				EventType eventType = Event.current.GetTypeForControl(controlID);
				
				if(!disable)
				{
					if(HandleUtility.nearestControl == controlID &&
					   GUIUtility.hotControl == 0)
					{
						hoveredHole = hole;
						
						if(eventType == EventType.MouseDown &&
						   Event.current.button == 0 &&
						   !Event.current.alt)
						{
							m_SpriteMeshCache.RegisterUndo("select hole");
							m_SpriteMeshCache.selectedHole = hole;
							m_SpriteMeshCache.ClearSelection();
							m_SpriteMeshCache.selectedEdge = null;
							Undo.IncrementCurrentGroup();
						}
					}
					
					EditorGUI.BeginChangeCheck();
					
					Vector2 newPosition = HandlesExtra.Slider2D(controlID, position, null);
					
					if (EditorGUI.EndChangeCheck())
					{
						Vector2 delta = newPosition - position;
						m_SpriteMeshCache.RegisterUndo("move hole");
						hole.vertex += delta;
						m_SpriteMeshCache.Triangulate();
					}
					
					if(eventType == EventType.Layout)
					{
						HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, 2.5f));
					}
				}
				
				if(eventType == EventType.Repaint)
				{
					bool highlight = false;
					
					if((HandleUtility.nearestControl == controlID && GUIUtility.hotControl == 0) ||
					   m_SpriteMeshCache.selectedHole == hole)
					{
						highlight = true;
					}
					
					Handles.color = Color.red;
					
					if(highlight)
					{
						Handles.color = Color.yellow;
					}
					
					DrawVertex(position);
				}
			}
		}
		
		void EdgesGUI(bool disable)
		{
			for (int i = 0; i < m_SpriteMeshCache.edges.Count; i++)
			{
				int controlID = GUIUtility.GetControlID ("EdgeHandle".GetHashCode(), FocusType.Passive);
				EventType eventType = Event.current.GetTypeForControl(controlID);
				
				Edge edge = m_SpriteMeshCache.edges[i];
				Vector2 position = m_SpriteMeshCache.GetPosition(edge.node1);
				
				if(!disable)
				{
					if(HandleUtility.nearestControl == controlID &&
					   GUIUtility.hotControl == 0)
					{
						hoveredEdge = edge;
						
						if(eventType == EventType.MouseDown &&
						   Event.current.button == 0 &&
						   m_SpriteMeshCache.selectedEdge != edge &&
						   !Event.current.shift &&
						   !Event.current.alt)
						{
							m_SpriteMeshCache.RegisterUndo("select edge");
							m_SpriteMeshCache.selectedEdge = edge;
							m_SpriteMeshCache.ClearSelection();
							m_SpriteMeshCache.selectedHole = null;
						}
					}
					
					if(!Event.current.shift)
					{
						EditorGUI.BeginChangeCheck();
						
						Vector2 newPosition = HandlesExtra.Slider2D(controlID, position, null);
						
						if(EditorGUI.EndChangeCheck())
						{
							m_SpriteMeshCache.RegisterUndo("move edge");
							
							Vector2 delta = newPosition - position;
							
							if(m_SpriteMeshCache.IsSelected(edge.node1) && m_SpriteMeshCache.IsSelected(edge.node2))
							{
								foreach(Node node in m_SpriteMeshCache.selectedNodes)
								{
									Vector2 texcoord = m_SpriteMeshCache.GetPosition(node);
									m_SpriteMeshCache.SetPosition(node,texcoord + delta);
								}
							}else{
								Vector2 texcoord1 = m_SpriteMeshCache.GetPosition(edge.node1);
								Vector2 texcoord2 = m_SpriteMeshCache.GetPosition(edge.node2);
								m_SpriteMeshCache.SetPosition(edge.node1,texcoord1 + delta);
								m_SpriteMeshCache.SetPosition(edge.node2,texcoord2 + delta);
							}
							
							m_SpriteMeshCache.Triangulate();
						}
					}
					
					if (eventType == EventType.Layout)
					{
						Vector2 texcoord1 = m_SpriteMeshCache.GetPosition(edge.node1);
						Vector2 texcoord2 = m_SpriteMeshCache.GetPosition(edge.node2);
						HandleUtility.AddControl(controlID, HandleUtility.DistanceToLine(texcoord1,texcoord2));
					}
				}
				
				if(eventType == EventType.Repaint)
				{
					bool drawEdge = true;
					
					if((HandleUtility.nearestControl == m_selectionControlID || HandleUtility.nearestControl == controlID) &&
					   Event.current.shift &&
					   !m_SpriteMeshCache.selectedNode &&
					   edge == m_ClosestEdge)
					{
						drawEdge = false;
					}
					
					if(Event.current.shift &&
					   m_SpriteMeshCache.selectedNode && 
					   HandleUtility.nearestControl == controlID)
					{
						drawEdge = false;
					}
					
					if(drawEdge)
					{
						Handles.color = vertexColor * new Color (1f, 1f, 1f, 0.75f);
						
						if(disable)
						{
							Handles.color = new Color(0.75f,0.75f,0.75f,1f);
						}
						
						if((HandleUtility.nearestControl == controlID && GUIUtility.hotControl == 0) ||
						   m_SpriteMeshCache.selectedEdge == edge)
						{
							Handles.color = Color.yellow * new Color (1f, 1f, 1f, 0.75f);
						}
						
						DrawEdge(edge, 1.5f/ m_Zoom);
					}
				}
			}
		}
		
		void VerticesGUI(bool disable)
		{
			GUI.color = Color.white;
			
			foreach(Node node in m_SpriteMeshCache.nodes)
			{
				Vector2 position = m_SpriteMeshCache.GetPosition(node);
				
				int controlID = GUIUtility.GetControlID ("VertexHandle".GetHashCode(), FocusType.Passive);
				EventType eventType = Event.current.GetTypeForControl(controlID);
				
				if(!disable)
				{
					if(HandleUtility.nearestControl == controlID &&
					   GUIUtility.hotControl == 0)
					{
						hoveredNode = node;
						
						if(eventType == EventType.MouseDown &&
						   Event.current.button == 0 &&
						   !Event.current.shift &&
						   !Event.current.alt)
						{
							m_SpriteMeshCache.RegisterUndo("select vertex");
							
							if(!m_SpriteMeshCache.IsSelected(node))
							{
								m_SpriteMeshCache.Select(node,EditorGUI.actionKey);
								blendShapeEditor.ActivateRectTool(true);
							}else{
								if(EditorGUI.actionKey)
								{
									m_SpriteMeshCache.Unselect(node);
									blendShapeEditor.ActivateRectTool(true);
								}
							}
							
							m_SpriteMeshCache.selectedHole = null;
							m_SpriteMeshCache.selectedEdge = null;
							
							Undo.IncrementCurrentGroup();
						}
					}
					
					if(!Event.current.shift && !EditorGUI.actionKey)
					{
						EditorGUI.BeginChangeCheck();
						
						Vector2 newPosition = HandlesExtra.Slider2D(controlID, position, null);
						
						if(EditorGUI.EndChangeCheck())
						{
							Vector2 delta = newPosition - position;
							
							m_SpriteMeshCache.RegisterUndo("move vertices");
							
							foreach(Node selectedNode in m_SpriteMeshCache.selectedNodes)
							{
								Vector2 l_position = m_SpriteMeshCache.GetPosition(selectedNode);
								m_SpriteMeshCache.SetPosition(selectedNode,l_position + delta);
							}
							
							m_SpriteMeshCache.Triangulate();
						}
					}
					
					if (eventType == EventType.Layout)
					{
						HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, 2.5f));
					}
				}
				
				if(eventType == EventType.Repaint)
				{
					bool highlight = false;
					
					if((HandleUtility.nearestControl == controlID && GUIUtility.hotControl == 0) || 
					   m_SpriteMeshCache.IsSelected(node) ||
					   m_SelectionIndices.Contains(node.index))
					{
						highlight = true;
					}
					
					if(hoveredEdge && hoveredEdge.ContainsNode(node))
					{
						highlight = true;
					}
					
					if(m_SpriteMeshCache.selectedEdge && m_SpriteMeshCache.selectedEdge.ContainsNode(node))
					{
						highlight = true;
					}
					
					if(weightEditor.isShown && weightEditor.showPie)
					{
						BoneWeight boneWeigth = m_SpriteMeshCache.GetBoneWeight(node);
						DrawPie(position,boneWeigth,10f / m_Zoom);
						
						if(highlight)
						{
							Handles.color = Color.yellow;
							HandlesExtra.DrawCircle(position, 12f / m_Zoom, 0.8f);
						}
					}else{
						Handles.color = vertexColor;
						
						if(disable)
						{
							Handles.color = new Color(0.75f,0.75f,0.75f,1f);
						}
						
						if(highlight)
						{
							Handles.color = Color.yellow;
						}
						
						DrawVertex(position);
					}
				}
			}
		}
		
		void PivotHandleGUI(bool disable)
		{
			GUI.color = Color.white;
			
			int controlID = GUIUtility.GetControlID("PivotHandle".GetHashCode(), FocusType.Passive);
			EventType eventType = Event.current.GetTypeForControl(controlID);
			
			if(!disable)
			{
				EditorGUI.BeginChangeCheck();
				
				Vector2 newPivotPoint = HandlesExtra.Slider2D(controlID, m_SpriteMeshCache.pivotPoint, null);
				
				if (EditorGUI.EndChangeCheck())
				{
					if(EditorGUI.actionKey)
					{
						newPivotPoint.x = (int)(newPivotPoint.x + 0.5f);
						newPivotPoint.y = (int)(newPivotPoint.y + 0.5f);
					}
					
					m_SpriteMeshCache.RegisterUndo("move pivot");
					m_SpriteMeshCache.SetPivotPoint(newPivotPoint);
				}
				
				if(eventType == EventType.Layout)
				{
					HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(m_SpriteMeshCache.pivotPoint,5f));
				}
			}
			
			if(eventType == EventType.Repaint)
			{
				HandlesExtra.PivotCap(controlID,m_SpriteMeshCache.pivotPoint);
			}
		}
		
		void HandleAddHole()
		{
			if(meshToolEditor.tool != MeshToolEditor.MeshTool.Hole ||
			   HandleUtility.nearestControl != m_selectionControlID) return;
			
			EventType eventType = Event.current.GetTypeForControl(0);
			
			if(eventType == EventType.MouseDown &&
			   Event.current.button == 0 &&
			   Event.current.clickCount == 2)
			{
				m_SpriteMeshCache.RegisterUndo("add hole");
				m_SpriteMeshCache.AddHole(mMousePositionWorld);
				m_SpriteMeshCache.selectedEdge = null;
				m_SpriteMeshCache.ClearSelection();
				Event.current.Use();
			}
		}
		
		void HandleAddVertex()
		{
			if(meshToolEditor.tool != MeshToolEditor.MeshTool.None ||
			   (HandleUtility.nearestControl != m_selectionControlID && !hoveredEdge)) return;
			
			EventType eventType = Event.current.GetTypeForControl(0);
			
			if(eventType == EventType.MouseDown &&
			   Event.current.button == 0 &&
			   (Event.current.clickCount == 2 || (Event.current.shift && !m_SpriteMeshCache.selectedNode)))
			{
				m_SpriteMeshCache.RegisterUndo("add point");
				
				Edge edge = null;
				
				if(hoveredEdge)
				{
					edge = hoveredEdge;
				}
				
				if(Event.current.shift && meshToolEditor.tool == MeshToolEditor.MeshTool.None)
				{
					edge = m_ClosestEdge;
				}
				
				m_SpriteMeshCache.AddNode(mMousePositionWorld,edge);
				
				m_SpriteMeshCache.selectedEdge = null;
				
				Event.current.Use();
			}
			
			if(!m_SpriteMeshCache.selectedNode && 
			   !hoveredNode &&
			   meshToolEditor.tool == MeshToolEditor.MeshTool.None &&
			   Event.current.shift &&
			   m_ClosestEdge)
			{
				if(eventType == EventType.Repaint)
				{
					DrawSplitEdge(m_ClosestEdge,mMousePositionWorld);
				}
				
				if(eventType == EventType.MouseMove || eventType == EventType.MouseDrag)
				{
					Repaint();
				}
			}
		}
		
		void HandleDeleteVertex()
		{
			if(Event.current.GetTypeForControl(0) == EventType.KeyDown &&
			   m_SpriteMeshCache.selectedNodes.Count > 0 &&
			   (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete))
			{
				m_SpriteMeshCache.RegisterUndo("delete vertex");
				
				foreach(Node node in m_SpriteMeshCache.selectedNodes)
				{
					m_SpriteMeshCache.DeleteNode(node,false);
				}
				
				m_SpriteMeshCache.ClearSelection();
				
				m_SpriteMeshCache.Triangulate();
				
				Event.current.Use();
			}
		}
		
		void HandleDeleteEdge()
		{
			if(Event.current.GetTypeForControl(0) == EventType.KeyDown &&
			   m_SpriteMeshCache.selectedEdge != null &&
			   (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete))
			{
				m_SpriteMeshCache.RegisterUndo("delete edge");
				
				m_SpriteMeshCache.DeleteEdge(m_SpriteMeshCache.selectedEdge);
				m_SpriteMeshCache.selectedEdge = null;
				m_SpriteMeshCache.Triangulate();
				
				Event.current.Use();
			}
		}
		
		void HandleAddEdge()
		{
			if(GUIUtility.hotControl == 0 &&
			   m_SpriteMeshCache.selectedNode&&
			   Event.current.shift)
			{
				switch(Event.current.type)
				{
				case EventType.MouseDown:
					if(Event.current.button == 0 &&
					   m_SpriteMeshCache.selectedNode &&
					   m_SpriteMeshCache.selectedNode != hoveredNode)
					{
						m_SpriteMeshCache.RegisterUndo("add edge");
						
						Node targetVertex = hoveredNode;
						
						if(!targetVertex)
						{
							targetVertex = m_SpriteMeshCache.AddNode(mMousePositionWorld,hoveredEdge);
						}
						
						if(targetVertex)
						{
							m_SpriteMeshCache.AddEdge(m_SpriteMeshCache.selectedNode,targetVertex);
							m_SpriteMeshCache.Select(targetVertex,false);
						}
						
						Event.current.Use();
						Repaint();
					}
					break;
				case EventType.MouseMove:
					Event.current.Use();
					break;
				case EventType.Repaint:
					Vector3 targetPosition = mMousePositionWorld;
					
					if(hoveredNode)
					{
						targetPosition = m_SpriteMeshCache.GetPosition(hoveredNode);
					}else{
						DrawSplitEdge(hoveredEdge, mMousePositionWorld);
					}
					
					Handles.color = Color.yellow;
					DrawEdge(m_SpriteMeshCache.GetPosition(m_SpriteMeshCache.selectedNode), targetPosition, 2f / m_Zoom);
					
					break;
				}
			}
		}
		
		void HandleDeleteHole()
		{
			switch(Event.current.type)
			{
			case EventType.KeyDown:
				if(GUIUtility.hotControl == 0 &&
				   m_SpriteMeshCache.selectedHole != null &&
				   (Event.current.keyCode == KeyCode.Backspace ||
				 Event.current.keyCode == KeyCode.Delete))
				{
					m_SpriteMeshCache.RegisterUndo("delete hole");
					
					m_SpriteMeshCache.DeleteHole(m_SpriteMeshCache.selectedHole);
					
					m_SpriteMeshCache.selectedHole = null;
					
					Event.current.Use();
				}
				break;
			}
		}
		
		override protected void HandleEvents()
		{
			SetupSpriteMeshMatrix();
		}
		
		override protected void DrawGizmos()
		{
			DrawSpriteMesh();
			
			if(showTriangles)
			{
				DrawTriangles();
			}
		}
		
		Vector2 ScreenToWorld(Vector2 position)
		{
			return Handles.inverseMatrix.MultiplyPoint(position);
		}
		
		Edge GetClosestEdge(Vector2 position)
		{
			float minSqrtDistance = float.MaxValue;
			Edge closestEdge = null;
			
			for(int i = 0; i < m_SpriteMeshCache.edges.Count; ++i)
			{
				Edge edge = m_SpriteMeshCache.edges[i];
				if(edge && edge.node1 && edge.node2)
				{
					float sqrtDistance = MathUtils.SegmentSqrtDistance((Vector3)position,
					                                                   m_SpriteMeshCache.GetPosition(edge.node1),
					                                                   m_SpriteMeshCache.GetPosition(edge.node2));
					if(sqrtDistance < minSqrtDistance)
					{
						closestEdge = edge;
						minSqrtDistance = sqrtDistance;
					}
				}
			}
			
			return closestEdge;
		}
		
		void DoApply()
		{
			m_SpriteMeshCache.ApplyChanges();
		}
		
		void InvalidateCache()
		{
			if(m_SpriteMeshCache)
			{
				DestroyImmediate(m_SpriteMeshCache);
			}
			
			m_SpriteMeshCache = ScriptableObject.CreateInstance<SpriteMeshCache>();
			m_SpriteMeshCache.hideFlags = HideFlags.DontSave;
		}
		
		void DoRevert()
		{
			m_SpriteMeshCache.RevertChanges();
		}
		
		override protected void DoToolbarGUI()
		{
			EditorGUILayout.BeginHorizontal();
			
			EditorGUI.BeginDisabledGroup(m_SpriteMeshCache.spriteMesh == null);
			
			showBones = GUILayout.Toggle(showBones, s_Styles.showBonesIcon, EditorStyles.toolbarButton,GUILayout.Width(32));
			showTriangles = true;
			
			if(GUILayout.Toggle(mode == Mode.Mesh, new GUIContent("Mesh", "Edit the mesh's geometry"), EditorStyles.toolbarButton))
			{
				mode = Mode.Mesh;
			}
			
			/*
			if(GUILayout.Toggle(mode == Mode.Blendshapes, new GUIContent("Blendshapes", "Blendshapes"), EditorStyles.toolbarButton))
			{
				mode = Mode.Blendshapes;
			}
			*/
			
			GUILayout.FlexibleSpace();
			
			if(GUILayout.Button(new GUIContent("Revert", "Revert changes"), EditorStyles.toolbarButton))
			{
				m_SpriteMeshCache.RegisterUndo("Revert");
				
				DoRevert();
			}
			
			if(GUILayout.Button(new GUIContent("Apply", "Apply changes"), EditorStyles.toolbarButton))
			{
				DoApply();
			}
			
			DoAlphaZoomToolbarGUI();
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.EndDisabledGroup();
		}
		
		void HotKeys()
		{
			if(GUIUtility.hotControl == 0 &&
			   Event.current.type == EventType.KeyDown)
			{
				if(Event.current.keyCode == KeyCode.H)
				{
					meshToolEditor.tool = MeshToolEditor.MeshTool.Hole;
					Event.current.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
					Repaint();
				}
			}
		}
		
		void SetupSpriteMeshMatrix()
		{
			Rect textureRect = m_TextureRect;
			textureRect.position -= m_ScrollPosition;
			
			Vector3 invertY = new Vector3(1f, -1f, 0f);
			
			m_SpriteMeshMatrix.SetTRS(
				new Vector3(textureRect.x, textureRect.y + textureRect.height, 0f) +
				Vector3.Scale(m_SpriteMeshCache.pivotPoint,invertY) * m_Zoom,
				Quaternion.Euler(0f, 0f, 0f),
				invertY * m_Zoom * m_TextureImporter.spritePixelsPerUnit);
		}
		
		void DrawMesh(Vector3[] vertices, Vector2[] uvs, Color[] colors, int[] triangles, Material material)
		{
			Mesh mesh = new Mesh();
			
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.colors = colors;
			mesh.triangles = triangles;
			
			material.SetPass(0);
			
			Graphics.DrawMeshNow(mesh, Handles.matrix * GUI.matrix);
			
			DestroyImmediate(mesh);
		}
		
		void DrawSpriteMesh()
		{
			if(m_SpriteMeshCache)
			{
				Texture mainTexture = m_Texture;
				
				List<Color> colors = new List<Color>(m_SpriteMeshCache.nodes.Count);
				
				if(mode == Mode.Mesh &&
				   m_SpriteMeshCache.isBound &&
				   weightEditor.overlayColors)
				{
					foreach(Node node in m_SpriteMeshCache.nodes)
					{
						BoneWeight boneWeight = m_SpriteMeshCache.GetBoneWeight(node);
						int boneIndex0 = boneWeight.boneIndex0;
						int boneIndex1 = boneWeight.boneIndex1;
						int boneIndex2 = boneWeight.boneIndex2;
						int boneIndex3 = boneWeight.boneIndex3;
						float weight0 = boneIndex0 < 0 ? 0f : boneWeight.weight0;
						float weight1 = boneIndex1 < 0 ? 0f : boneWeight.weight1;
						float weight2 = boneIndex2 < 0 ? 0f : boneWeight.weight2;
						float weight3 = boneIndex3 < 0 ? 0f : boneWeight.weight3;
						
						Color vertexColor = m_BindPoseColors[Mathf.Max(0,boneIndex0)] * weight0 +
							m_BindPoseColors[Mathf.Max(0,boneIndex1)] * weight1 +
								m_BindPoseColors[Mathf.Max(0,boneIndex2)] * weight2 +
								m_BindPoseColors[Mathf.Max(0,boneIndex3)] * weight3;
						
						colors.Add (vertexColor);
					}
					
					mainTexture = null;
				}
				
				meshGuiMaterial.mainTexture = mainTexture;
				DrawMesh(m_Points.ConvertAll(v => (Vector3)v).ToArray(),
				         m_UVs.ToArray(),
				         colors.ToArray(),
				         m_SpriteMeshCache.indices.ToArray(),
				         meshGuiMaterial);
			}
		}
		
		
		void DrawTriangles()
		{
			Handles.color = Color.white * new Color (1f, 1f, 1f, 0.25f);
			
			for (int i = 0; i < m_SpriteMeshCache.indices.Count; i+=3)
			{
				int index = m_SpriteMeshCache.indices[i];
				int index1 = m_SpriteMeshCache.indices[i+1];
				int index2 = m_SpriteMeshCache.indices[i+2];
				Vector3 v1 = m_Points[index];
				Vector3 v2 = m_Points[index1];
				Vector3 v3 = m_Points[index2];
				
				Handles.DrawLine(v1,v2);
				Handles.DrawLine(v2,v3);
				Handles.DrawLine(v1,v3);
			}
			
		}
		
		void DrawEdge(Edge edge, float width)
		{
			DrawEdge(edge,width,0f);
		}
		
		void DrawEdge(Edge edge, float width, float vertexSize)
		{
			if(edge && edge.node1 && edge.node2)
			{
				DrawEdge(m_SpriteMeshCache.GetPosition(edge.node1),
				         m_SpriteMeshCache.GetPosition(edge.node2),width);
			}
		}
		
		void DrawEdge(Vector2 p1, Vector2 p2, float width)
		{
			HandlesExtra.DrawLine(p1, p2, Vector3.forward, width);
		}
		
		void DrawVertex(Vector2 position, float size)
		{
			Rect rect = new Rect(0f,0f,size,size);
			rect.center = Handles.matrix.MultiplyPoint(position);
			
			Color color = GUI.color;
			GUI.color = Handles.color;
			GUI.DrawTexture(rect,dotTexture);
			GUI.color = color;
		}
		
		void DrawVertex(Vector2 position)
		{
			DrawVertex(position, mPointSize);
		}
		
		void DrawPie(Vector3 position, BoneWeight boneWeight, float pieSize)
		{
			Handles.color = Color.black;
			DrawVertex(position,22.5f);
			
			int boneIndex = boneWeight.boneIndex0;
			float angleStart = 0f;
			float angle = 0f;
			
			if(boneIndex >= 0)
			{
				angleStart = 0f;
				angle = Mathf.Lerp(0f,360f,boneWeight.weight0);
				Handles.color = m_BindPoseColors[boneWeight.boneIndex0];
				Handles.DrawSolidArc(position, Vector3.forward,Vector3.up,angle, pieSize);
			}
			
			boneIndex = boneWeight.boneIndex1;
			
			if(boneIndex >= 0)
			{
				angleStart += angle;
				angle = Mathf.Lerp(0f,360f,boneWeight.weight1);
				Handles.color = m_BindPoseColors[boneWeight.boneIndex1];
				Handles.DrawSolidArc(position, Vector3.forward,Quaternion.AngleAxis(angleStart,Vector3.forward) * Vector3.up,angle, pieSize);
			}
			
			boneIndex = boneWeight.boneIndex2;
			
			if(boneIndex >= 0)
			{
				angleStart += angle;
				angle = Mathf.Lerp(0f,360f,boneWeight.weight2);
				Handles.color = m_BindPoseColors[boneWeight.boneIndex2];
				Handles.DrawSolidArc(position, Vector3.forward,Quaternion.AngleAxis(angleStart,Vector3.forward) * Vector3.up,angle, pieSize);
			}
			
			boneIndex = boneWeight.boneIndex3;
			
			if(boneIndex >= 0)
			{
				angleStart += angle;
				angle = Mathf.Lerp(0f,360f,boneWeight.weight3);
				Handles.color = m_BindPoseColors[boneWeight.boneIndex3];
				Handles.DrawSolidArc(position, Vector3.forward,Quaternion.AngleAxis(angleStart,Vector3.forward) * Vector3.up,angle, pieSize);
			}
		}
		
		void DrawSplitEdge(Edge edge, Vector2 vertexPosition)
		{
			if(edge != null)
			{
				Vector3 p1 = m_SpriteMeshCache.GetPosition(edge.node1);
				Vector3 p2 = m_SpriteMeshCache.GetPosition(edge.node2);
				Handles.color = Color.yellow;
				DrawEdge(p1,vertexPosition, 2f/m_Zoom);
				DrawEdge(p2,vertexPosition, 2f/m_Zoom);
				Handles.color = Color.yellow;
				DrawVertex(p1);
				DrawVertex(p2);
			}
			
			DrawVertex(vertexPosition);
		}
		
		public void UpdateFromSelection()
		{
			if(!m_SpriteMeshCache)
			{
				InvalidateCache();
			}
			
			SpriteMesh l_spriteMesh = null;
			SpriteMeshInstance l_spriteMeshInstance = null;
			
			if(Selection.activeGameObject)
			{
				l_spriteMeshInstance = Selection.activeGameObject.GetComponent<SpriteMeshInstance>();
			}
			
			if(l_spriteMeshInstance)
			{
				l_spriteMesh = l_spriteMeshInstance.spriteMesh;
				
			}else{
				if (Selection.activeObject is SpriteMesh)
				{
					l_spriteMesh = Selection.activeObject as SpriteMesh;
				}else if(Selection.activeGameObject)
				{
					GameObject activeGameObject = Selection.activeGameObject;
					GameObject prefab = null;
					
					if(PrefabUtility.GetPrefabType(activeGameObject) == PrefabType.Prefab)
					{
						prefab = Selection.activeGameObject;
					}else if(PrefabUtility.GetPrefabType(activeGameObject) == PrefabType.PrefabInstance ||
					         PrefabUtility.GetPrefabType(activeGameObject) == PrefabType.DisconnectedPrefabInstance)
					{
						prefab = PrefabUtility.GetPrefabParent(activeGameObject) as GameObject;
					}
					
					if(prefab)
					{
						l_spriteMesh = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(prefab), typeof(SpriteMesh)) as SpriteMesh;
					}
				}
			}
			
			if(l_spriteMeshInstance || l_spriteMesh)
			{
				m_SpriteMeshCache.spriteMeshInstance = l_spriteMeshInstance;
			}
			
			if(l_spriteMesh && l_spriteMesh != m_SpriteMeshCache.spriteMesh)
			{
				HandleApplyRevertDialog();
				
				InvalidateCache();
				
				if(l_spriteMesh && l_spriteMesh.sprite)
				{
					m_SpriteMeshCache.SetSpriteMesh(l_spriteMesh,l_spriteMeshInstance);
					
					RefreshTexture();
					
					SetScrollPositionAndZoomFromSprite();
				}
			}
		}
		
		void SetScrollPositionAndZoomFromSprite()
		{
			if(!m_SpriteMeshCache || !m_Texture)
			{
				return;
			}
			
			float newZoom = Mathf.Min(this.m_TextureViewRect.width / (float)m_SpriteMeshCache.rect.width, this.m_TextureViewRect.height / (float)m_SpriteMeshCache.rect.height) * 0.9f;
			if(m_Zoom > newZoom)
			{
				m_Zoom = newZoom;
			}
			
			int width = 1;
			int height = 1;
			
			SpriteMeshUtils.GetWidthAndHeight(m_TextureImporter,ref width,ref height);
			
			m_ScrollPosition = Vector2.Scale((m_SpriteMeshCache.rect.center - new Vector2(width,height) * 0.5f),new Vector2(1f,-1f)) * m_Zoom;
		}
		
		void RefreshTexture(bool force = false)
		{
			if(!m_SpriteMeshCache || !m_SpriteMeshCache.spriteMesh || !m_SpriteMeshCache.spriteMesh.sprite)
			{
				return;
			}
			
			Texture2D spriteTexture = SpriteUtility.GetSpriteTexture(m_SpriteMeshCache.spriteMesh.sprite,false);
			
			if(force || spriteTexture != m_OriginalTexture)
			{
				m_OriginalTexture = spriteTexture;
				m_TextureImporter = (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(m_OriginalTexture)) as TextureImporter);
				
				if(m_Texture)
				{
					DestroyImmediate(m_Texture);
				}
				
				if(m_OriginalTexture)
				{
					int width = 0;
					int height = 0;
					
					SpriteMeshUtils.GetWidthAndHeight(m_TextureImporter,ref width, ref height);
					
					m_Texture = CreateTemporaryDuplicate(m_OriginalTexture,width,height);
					m_Texture.filterMode = UnityEngine.FilterMode.Point;
					m_Texture.hideFlags = HideFlags.DontSave;
				}
			}
		}
		
		Texture2D CreateTemporaryDuplicate(Texture2D original, int width, int height)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture || !original)
			{
				return null;
			}
			
			RenderTexture active = RenderTexture.active;
			bool flag1 = !GetLinearSampled(original);
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, !flag1 ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
			GL.sRGBWrite = flag1 && QualitySettings.activeColorSpace == ColorSpace.Linear;
			Graphics.Blit((Texture) original, temporary);
			GL.sRGBWrite = false;
			RenderTexture.active = temporary;
			bool flag2 = width >= SystemInfo.maxTextureSize || height >= SystemInfo.maxTextureSize;
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, original.mipmapCount > 1 || flag2);
			texture2D.ReadPixels(new Rect(0.0f, 0.0f, (float) width, (float) height), 0, 0);
			texture2D.Apply();
			RenderTexture.ReleaseTemporary(temporary);
			SetRenderTextureNoViewport(active);
			texture2D.alphaIsTransparency = original.alphaIsTransparency;
			return texture2D;
		}
		
		bool GetLinearSampled(Texture2D texture)
		{
			bool result = false;
			
			MethodInfo methodInfo = typeof(Editor).Assembly.GetType("UnityEditor.TextureUtil").GetMethod("GetLinearSampled", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			
			if(methodInfo != null)
			{
				object[] parameters = new object[] { texture };
				result = (bool)methodInfo.Invoke(null,parameters);
			}
			
			return result;
		}
		
		void SetRenderTextureNoViewport(RenderTexture rt)
		{
			MethodInfo methodInfo = typeof(EditorGUIUtility).GetMethod("SetRenderTextureNoViewport", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			
			if(methodInfo != null)
			{
				object[] parameters = new object[] { rt };
				methodInfo.Invoke(null,parameters);
			}
		}
		
		void HandleApplyRevertDialog()
		{
			if(m_SpriteMeshCache && m_SpriteMeshCache.isDirty && m_SpriteMeshCache.spriteMesh)
			{
				if (EditorUtility.DisplayDialog("Unapplied changes", "Unapplied changes for '" + m_SpriteMeshCache.spriteMesh.name + "'", "Apply", "Revert"))
				{
					DoApply();
				}
				else
				{
					DoRevert();
				}
			}
		}
		
		private void OnSelectionChange()
		{
			UpdateFromSelection();
			Repaint();
		}
		
	}
}
