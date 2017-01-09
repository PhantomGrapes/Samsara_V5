using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

namespace Anima2D
{
	public class AnimationWindowExtra
	{
		static Type m_AnimationWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationWindow");
		static Type m_AnimationWindowStateType = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.AnimationWindowState");
		
		static EditorWindow m_AnimationWindow = null;
		public static EditorWindow animationWindow {
			get	{
				if( m_AnimationWindow == null )
				{
					m_AnimationWindow = FindWindowOpen( m_AnimationWindowType );
				}
				return m_AnimationWindow;
			}
		}
		
		static EditorWindow FindWindowOpen(Type windowType)
		{
			UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll( windowType );
			
			foreach( UnityEngine.Object o in objs )
			{
				if( o.GetType() == windowType )
				{
					return (EditorWindow)o;
				}
			}
			
			return null;
		}
		
		#if UNITY_5_0
		
		static object state {
			get {
				return m_AnimationWindowType.GetProperty("state", BindingFlags.Instance | BindingFlags.Public).GetValue(animationWindow,null);
			} 
		}
		
		public static int frame {
			get {
				if(state != null)
				{
					return (int)(m_AnimationWindowStateType.GetField("m_Frame", BindingFlags.Instance | BindingFlags.Public).GetValue(state));
				}
				
				return 0;
			}
			
			set {
				MethodInfo methodInfo = m_AnimationWindowType.GetMethod("PreviewFrame",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				if(methodInfo != null)
				{
					object[] parameters = { value };
					methodInfo.Invoke(animationWindow, parameters);
				}
			}
		}
		
		public static bool recording {
			get {
				if(animationWindow)
				{
					MethodInfo methodInfo = m_AnimationWindowType.GetMethod("GetAutoRecordMode",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
					if(methodInfo != null)
					{
						return (bool)methodInfo.Invoke(animationWindow, null);
					}
				}
				
				return false;
			}
			
			set {
				if(animationWindow)
				{
					MethodInfo methodInfo = m_AnimationWindowType.GetMethod("SetAutoRecordMode",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
					if(methodInfo != null)
					{
						object[] parameters = { value };
						methodInfo.Invoke(animationWindow, parameters);
					}
				}
			}
		}
		
		public static AnimationClip activeAnimationClip {
			get {
				if(state != null)
				{
					return m_AnimationWindowStateType.GetField("m_ActiveAnimationClip", BindingFlags.Instance | BindingFlags.Public).GetValue(state) as AnimationClip;
				}
				
				return null;
			}
		}
		
		public static GameObject activeGameObject {
			get {
				if(state != null)
				{
					return m_AnimationWindowStateType.GetField("m_ActiveGameObject", BindingFlags.Instance | BindingFlags.Public).GetValue(state) as GameObject;
				}
				
				return null;
			}
		}
		
		public static GameObject rootGameObject {
			get {
				if(state != null)
				{
					return m_AnimationWindowStateType.GetField("m_RootGameObject", BindingFlags.Instance | BindingFlags.Public).GetValue(state) as GameObject;
				}
				
				return null;
			}
		}

		public static int refresh {
			get {
			    if(state != null)
				{
					return (int)(m_AnimationWindowStateType.GetField("m_Refresh", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(state));
				}

				return 0;
			}
		}
		
		public static float FrameToTime(int frame)
		{
			if(state != null)
			{
				MethodInfo methodInfo = m_AnimationWindowStateType.GetMethod("FrameToTime",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				if(methodInfo != null)
				{
					object[] parameters = { (float)frame };
					return (float) methodInfo.Invoke(state,parameters);
				}
			}
			return 0f;
		}
		
		public static float currentTime {
			get {
				if(state != null)
				{
					MethodInfo methodInfo = m_AnimationWindowStateType.GetMethod("GetTimeSeconds",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
					if(methodInfo != null)
					{
						return (float) methodInfo.Invoke(state,null);
					}
				}
				return 0f;
			}
		}
		
		#else
		static Type m_AnimEditorType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimEditor");
		
		static ScriptableObject animEditor {
			get {
				if(animationWindow != null)
				{
					return (ScriptableObject)m_AnimationWindowType.GetField( "m_AnimEditor", BindingFlags.Instance | BindingFlags.NonPublic ).GetValue( animationWindow );
				}
				return null;
			}
		}
		
		static object state {
			get {
				if(animEditor)
				{
					return m_AnimEditorType.GetField("m_State", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(animEditor);
				}
				return null;
			}
			
		}
		
		public static int frame {
			get {
				
				if(state != null)
				{
					return (int)m_AnimationWindowStateType.GetProperty("frame",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(state,null);
				}
				
				return 0;
			}
			
			set {
				if(state != null)
				{
					m_AnimationWindowStateType.GetProperty("frame",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).SetValue(state, value, null);
				}
			}
		}
		
		public static bool recording {
			get {
				
				if(state != null)
				{
					return (bool)m_AnimationWindowStateType.GetProperty("recording",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(state,null);
				}
				
				return false;
			}
			
			set {
				if(state != null)
				{
					m_AnimationWindowStateType.GetProperty("recording",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).SetValue(state, value, null);
				}
			}
		}
		
		public static AnimationClip activeAnimationClip {
			get {
				if(state != null)
				{
					return (AnimationClip)m_AnimationWindowStateType.GetProperty("activeAnimationClip", BindingFlags.Instance | BindingFlags.Public).GetValue(state,null);
				}
				return null;
			}
		}
		
		public static GameObject activeGameObject {
			get {
				
				if(state != null)
				{
					return (GameObject)m_AnimationWindowStateType.GetProperty("activeGameObject", BindingFlags.Instance | BindingFlags.Public).GetValue(state,null);
				}
				
				return null;
			}
		}
		
		public static GameObject rootGameObject {
			get {
				
				if(state != null)
				{
					return (GameObject)m_AnimationWindowStateType.GetProperty("activeRootGameObject", BindingFlags.Instance | BindingFlags.Public).GetValue(state,null);
				}
				
				return null;
			}
		}

		public static int refresh {
			get {
				if(state != null)
				{
					return (int)m_AnimationWindowStateType.GetProperty("refresh",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(state,null);
				}
				return 0;
			}
		}
		
		public static float currentTime {
			get {
				if(state != null)
				{
					return (float)m_AnimationWindowStateType.GetProperty("currentTime", BindingFlags.Instance | BindingFlags.Public).GetValue(state,null);
				}
				return 0f;
			}
		}
		
		public static float FrameToTime(int frame)
		{
			if(state != null)
			{
				MethodInfo methodInfo = m_AnimationWindowStateType.GetMethod("FrameToTime",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				if(methodInfo != null)
				{
					object[] parameters = { (float)frame };
					return (float) methodInfo.Invoke(state,parameters);
				}
			}
			return 0f;
		}
		#endif
	}
}
