using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Anima2D 
{
	public static class BoneUtils
	{	
		public static string GetUniqueBoneName(Bone2D root)
		{
			string boneName = "bone";
			
			Bone2D[] bones = null;
			
			if(root)
			{
				bones = root.GetComponentsInChildren<Bone2D>(true);
				boneName = boneName + " " + (bones.Length + 1).ToString();
			}
			
			return boneName;
		}

		public static void DrawBoneCap(Bone2D bone)
		{
			Color color = bone.color * 0.25f;
			color.a = 1f;
			DrawBoneCap(bone, color);
		}

		public static void DrawBoneCap(Bone2D bone, Color color)
		{
			Handles.matrix = bone.transform.localToWorldMatrix;
			DrawBoneCap(Vector3.zero,GetBoneRadius(bone),color);
		}
		
		public static void DrawBoneCap(Vector3 position, float radius, Color color)
		{
			Handles.color = color;
			HandlesExtra.DrawCircle(position, radius*0.65f);
		}

		public static void DrawBoneBody(Bone2D bone)
		{
			DrawBoneBody(bone,bone.color);
		}

		public static void DrawBoneBody(Bone2D bone, Color color)
		{
			Handles.matrix = bone.transform.localToWorldMatrix;
			DrawBoneBody(Vector3.zero, bone.localEndPosition, GetBoneRadius(bone),color);
		}
		
		public static void DrawBoneBody(Vector3 position, Vector3 endPosition, float radius, Color color)
		{
			Vector3 distance = position - endPosition;
			
			if(distance.magnitude > radius && color.a > 0f)
			{
				HandlesExtra.DrawLine(position,endPosition,Vector3.back,2f*radius,0f,color);
				HandlesExtra.DrawSolidArc(position,Vector3.back,Vector3.Cross(endPosition-position,Vector3.forward),180f,radius,color);
			}
		}

		public static void DrawBoneOutline(Bone2D bone, float outlineSize, Color color)
		{
			Handles.matrix = bone.transform.localToWorldMatrix;
			DrawBoneOutline(Vector3.zero,
			                bone.localEndPosition,
			                GetBoneRadius(bone),
			                outlineSize / Handles.matrix.GetScale().x,
			                color);
		}

		public static void DrawBoneOutline(Vector3 position, Vector3 endPoint, float radius, float outlineSize, Color color)
		{
			Handles.color = color;
			HandlesExtra.DrawLine(position,endPoint,Vector3.back, 2f * (radius + outlineSize), 2f * outlineSize);
			HandlesExtra.DrawSolidArc(position,Vector3.forward,Vector3.Cross(endPoint-position,Vector3.back),180f,radius + outlineSize,color);

			if(outlineSize > 0f)
			{
				HandlesExtra.DrawSolidArc(endPoint,Vector3.back,Vector3.Cross(endPoint-position,Vector3.back),180f,outlineSize,color);
			}
		}

		public static float GetBoneRadius(Bone2D bone)
		{
			return Mathf.Min(bone.localLength / 20f, 0.125f * HandleUtility.GetHandleSize(bone.transform.position));
		}

		public static string GetBonePath(Bone2D bone)
		{
			return GetBonePath(bone.root.transform,bone);
		}

		public static string GetBonePath(Transform root, Bone2D bone)
		{
			return GetPath(root, bone.transform);
		}

		public static string GetPath(Transform root, Transform transform)
		{
			string path = "";

			Transform current = transform;
			
			if(root)
			{
				while(current && current != root)
				{
					path = current.name + path;

					current = current.parent;

					if(current != root)
					{
						path = "/" + path;
					}
				}

				if(!current)
				{
					path = "";
				}
			}

			return path;
		}

		public static Bone2D ReconstructHierarchy(List<Bone2D> bones, List<string> paths)
		{
			Bone2D rootBone = null;

			for (int i = 0; i < bones.Count; i++)
			{
				Bone2D bone = bones[i];
				string path = paths[i];

				for (int j = 0; j < bones.Count; j++)
				{
					Bone2D other = bones [j];
					string otherPath = paths[j];

					if(bone != other && !path.Equals(otherPath) && otherPath.Contains(path))
					{
						other.transform.parent = bone.transform;
						other.transform.localScale = Vector3.one;
					}
				}
			}

			for (int i = 0; i < bones.Count; i++)
			{
				Bone2D bone = bones[i];

				if(bone.parentBone)
				{
					if((bone.transform.position - bone.parentBone.endPosition).sqrMagnitude < 0.00001f)
					{
						bone.parentBone.child = bone;
					}
				}else{
					rootBone = bone;
				}
			}

			return rootBone;
		}

		public static void UpdateLinkedParentBone(Bone2D bone, bool deattachChilren, string undoName, bool recordObject)
		{
			if(!bone) return;
			
			Bone2D linkedParentBone = bone.linkedParentBone;
			
			if(linkedParentBone)
			{
				List<Vector3> childLocalScales = new List<Vector3>(linkedParentBone.transform.childCount);
				List<Transform> children = new List<Transform>(linkedParentBone.transform.childCount);
				
				if(deattachChilren)
				{
					foreach(Transform child in linkedParentBone.transform)
					{
						if(recordObject)
						{
							Undo.RecordObject(child,undoName);
						}else{
							Undo.RegisterCompleteObjectUndo(child,undoName);
						}

						children.Add(child);
						childLocalScales.Add(child.localScale);
					}
					
					linkedParentBone.transform.DetachChildren();
				}else{

					if(recordObject)
					{
						Undo.RecordObject(bone.transform,undoName);
					}else{
						Undo.RegisterCompleteObjectUndo(bone.transform,undoName);
					}

					children.Add(bone.transform);
					childLocalScales.Add(bone.transform.localScale);
					bone.transform.parent = null;
				}
				
				Vector3 localPosition = linkedParentBone.transform.InverseTransformPoint(bone.transform.position);
				
				if(localPosition.sqrMagnitude > 0f)
				{
					float angle = Mathf.Atan2(localPosition.y,localPosition.x) * Mathf.Rad2Deg;

					if(recordObject)
					{
						Undo.RecordObject(linkedParentBone.transform,undoName);
						Undo.RecordObject(linkedParentBone,undoName);
					}else{
						Undo.RegisterCompleteObjectUndo(linkedParentBone.transform,undoName);
						Undo.RegisterCompleteObjectUndo(linkedParentBone,undoName);
					}
					
					linkedParentBone.transform.localRotation *= Quaternion.AngleAxis(angle, Vector3.forward);
					
					EditorUtility.SetDirty(linkedParentBone.transform);
				}

				for (int i = 0; i < children.Count; i++)
				{
					Transform child = children [i];

					child.parent = linkedParentBone.transform;
					child.localScale = childLocalScales[i];

					if(linkedParentBone.child && linkedParentBone.child.transform == child)
					{
						child.position = linkedParentBone.endPosition;
					}

					EditorUtility.SetDirty (child);
				}
			}
		}
	}
}
