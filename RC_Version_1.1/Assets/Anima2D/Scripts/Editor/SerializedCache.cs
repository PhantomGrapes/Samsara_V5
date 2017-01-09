using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Anima2D
{
	public class SerializedCache : ScriptableObject, ISerializationCallbackReceiver
	{
		public void OnBeforeSerialize() { DoOnBeforeSerialize(); }
		public void OnAfterDeserialize() { DoOnAfterDeserialize(); }

		protected virtual void OnEnable() {}
		protected virtual void DoOnBeforeSerialize() {}
		protected virtual void DoOnAfterDeserialize() {}

		public void RegisterUndo(string name)
		{
			Undo.RegisterCompleteObjectUndo(this,name);
		}
	}
}
