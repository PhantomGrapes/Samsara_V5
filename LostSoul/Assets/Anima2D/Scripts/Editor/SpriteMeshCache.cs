using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Anima2D
{
	public class SpriteMeshCache : SerializedCache
	{
		public SpriteMesh spriteMesh;
		public SpriteMeshData spriteMeshData;
		public SpriteMeshInstance spriteMeshInstance;
		
		public List<Node> nodes = new List<Node>();
		
		public List<Vector2> texcoords = new List<Vector2>();
		
		public List<Vector2> uvs {
			get {
				int width = 1;
				int height = 1;
				SpriteMeshUtils.GetSpriteTextureSize(spriteMesh.sprite,ref width, ref height);
				Vector2 t = new Vector2(1f/width,1f/height);
				return texcoords.ConvertAll( v => Vector2.Scale(v,t) );
			}
		}
		
		public List<BoneWeight> boneWeights = new List<BoneWeight>();
		
		public List<Edge> edges = new List<Edge>();
		
		List<IndexedEdge> indexedEdges {
			get {
				return edges.ConvertAll( e => new IndexedEdge(nodes.IndexOf(e.node1), nodes.IndexOf(e.node2)) );
			}
		}
		
		public List<Hole> holes = new List<Hole>();
		
		public List<int> indices = new List<int>();
		
		public List<BindInfo> bindPoses = new List<BindInfo>();
		
		public List<BlendShape> blendshapes = new List<BlendShape>();
		
		public Vector2 pivotPoint = Vector2.zero;
		
		public Rect rect;
		
		public List<Node> selectedNodes = new List<Node>();
		
		public Node selectedNode { get { return selectedNodes.Count == 1? selectedNodes[0] : null; } }
		public bool multiselection { get { return selectedNodes.Count > 1; } }
		
		public Edge selectedEdge = null;
		
		public bool isBound { get { return bindPoses.Count > 0f; } }
		public bool isDirty { get; set; }
		
		[SerializeField]
		int mSelectedHoleIndex = -1;
		public Hole selectedHole {
			get {
				Hole hole = null;
				
				if(mSelectedHoleIndex >= 0 && mSelectedHoleIndex < holes.Count)
				{
					hole = holes[mSelectedHoleIndex];
				}
				
				return hole;
			}
			set {
				mSelectedHoleIndex = holes.IndexOf(value);
			}
		}
		
		public Bone2D selectedBone;
		
		[SerializeField]
		int m_SelectedBindPose = -1;
		public BindInfo selectedBindPose {
			get {
				BindInfo bindPose = null;
				
				if(m_SelectedBindPose >= 0 && m_SelectedBindPose < bindPoses.Count)
				{
					bindPose = bindPoses[m_SelectedBindPose];
				}
				
				return bindPose;
			}
			set {
				m_SelectedBindPose = bindPoses.IndexOf(value);
			}
		}
		
		[SerializeField]
		int mSelectedBlendshapeIndex = -1;
		public BlendShape selectedBlendshape { get; set; }
		
		public int selectedFrameIndex = -1;
		
		protected override void DoOnBeforeSerialize()
		{
			mSelectedBlendshapeIndex = blendshapes.IndexOf(selectedBlendshape);
		}
		
		public string[] GetBoneNames(string noBoneText)
		{
			List<string> names = new List<string>(bindPoses.Count);
			List<int> repetitions = new List<int>(bindPoses.Count);
			
			names.Add(noBoneText);
			repetitions.Add(0);
			
			foreach(BindInfo bindInfo in bindPoses)
			{
				List<string> repetedNames = names.Where( s => s == bindInfo.name ).ToList();
				
				names.Add(bindInfo.name);
				repetitions.Add(repetedNames.Count);
			}
			
			for (int i = 1; i < names.Count; i++)
			{
				string name = names[i];
				int count = repetitions[i] + 1;
				if(count > 1)
				{
					name += " (" + count.ToString() + ")";
					names[i] = name;
				}
			}
			
			return names.ToArray();
		}
		
		protected override void DoOnAfterDeserialize()
		{
			for(int i = 0; i < nodes.Count; ++i)
			{
				nodes[i].index = i;
			}
			
			selectedBlendshape = null;
			if(mSelectedBlendshapeIndex >= 0 && mSelectedBlendshapeIndex < blendshapes.Count)
			{
				selectedBlendshape = blendshapes[mSelectedBlendshapeIndex];
			}
		}
		
		public void SetSpriteMesh(SpriteMesh _spriteMesh, SpriteMeshInstance _spriteMeshInstance)
		{
			spriteMesh = _spriteMesh;
			spriteMeshInstance = _spriteMeshInstance;
			spriteMeshData = SpriteMeshUtils.LoadSpriteMeshData(_spriteMesh);
			RevertChanges();
		}
		
		public void ApplyChanges()
		{
			if(spriteMeshData)
			{
				spriteMeshData.vertices = texcoords.ToArray();
				spriteMeshData.boneWeights = boneWeights.ToArray();
				spriteMeshData.edges = indexedEdges.ToArray();
				spriteMeshData.holes = holes.ConvertAll( h => h.vertex ).ToArray();
				spriteMeshData.indices = indices.ToArray();
				spriteMeshData.bindPoses = bindPoses.ToArray();
				spriteMeshData.pivotPoint = pivotPoint;
				
				EditorUtility.SetDirty(spriteMeshData);
				
				SpriteMeshUtils.UpdateAssets(spriteMesh,spriteMeshData);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				AssetDatabase.StartAssetEditing();
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(spriteMesh.sprite));
				AssetDatabase.StopAssetEditing();
				isDirty = false;
			}
			
			if(spriteMeshInstance)
			{
				SpriteMeshUtils.UpdateRenderer(spriteMeshInstance,false);
			}
		}
		
		public void RevertChanges()
		{
			Clear();
			
			if(spriteMesh && spriteMeshData)
			{
				pivotPoint = spriteMeshData.pivotPoint;
				rect = SpriteMeshUtils.GetRect(spriteMesh.sprite);
				
				texcoords = spriteMeshData.vertices.ToList();
				nodes = texcoords.ConvertAll( v => Node.Create(texcoords.IndexOf(v)) );
				boneWeights = spriteMeshData.boneWeights.ToList();
				edges = spriteMeshData.edges.ToList().ConvertAll( e => Edge.Create(nodes[e.index1],nodes[e.index2]) );
				holes = spriteMeshData.holes.ToList().ConvertAll( h => new Hole(h) );
				indices = spriteMeshData.indices.ToList();
				bindPoses = spriteMeshData.bindPoses.ToList().ConvertAll( b => b.Clone() as BindInfo );
				
				if(boneWeights.Count != nodes.Count)
				{
					boneWeights = nodes.ConvertAll( n => BoneWeight.Create() );
				}
			}
		}
		
		public void Clear()
		{
			selectedBindPose = null;
			selectedBone = null;
			selectedEdge = null;
			
			selectedNodes.Clear();
			nodes.Clear();
			edges.Clear();
			indices.Clear();
			boneWeights.Clear();
			
			isDirty = false;
			
		}
		
		public void SetPivotPoint(Vector2 _pivotPoint)
		{
			pivotPoint = _pivotPoint;
			isDirty = true;
		}
		
		public Node AddNode(Vector2 position)
		{
			return AddNode(position,null);
		}
		
		public Node AddNode(Vector2 position, Edge edge)
		{
			Node node = Node.Create(nodes.Count);
			
			nodes.Add(node);
			
			if(edge)
			{
				edges.Add(Edge.Create(edge.node1,node));
				edges.Add(Edge.Create(edge.node2,node));
				edges.Remove(edge);
			}
			
			texcoords.Add(position);
			boneWeights.Add(BoneWeight.Create());
			
			Triangulate();
			
			return node;
		}
		
		public void DeleteNode(Node node, bool triangulate = true)
		{
			List<Edge> l_edges = new List<Edge>();
			
			for(int i = 0; i < edges.Count; i++)
			{
				Edge edge = edges[i];
				if(edge.ContainsNode(node))
				{
					l_edges.Add(edge);
				}
			}
			
			if(l_edges.Count == 2)
			{
				Node node1 = l_edges[0].node1 != node ? l_edges[0].node1 : l_edges[0].node2;
				Node node2 = l_edges[1].node1 != node ? l_edges[1].node1 : l_edges[1].node2;
				
				edges.Remove(l_edges[0]);
				edges.Remove(l_edges[1]);
				
				AddEdge(node1,node2);
			}else{
				foreach(Edge edge in l_edges)
				{
					edges.Remove(edge);
				}
			}
			
			texcoords.RemoveAt(node.index);
			boneWeights.RemoveAt(node.index);
			
			nodes.Remove(node);
			
			for(int i = 0; i < nodes.Count; ++i)
			{
				nodes[i].index = i;
			}
			
			if(triangulate)
			{
				Triangulate();
			}
		}
		
		public void AddEdge(Node node1, Node node2)
		{
			Edge newEdge = Edge.Create(node1,node2);
			
			if(!edges.Contains(newEdge))
			{
				edges.Add(newEdge);
				Triangulate();
			}
		}
		
		public void DeleteEdge(Edge edge)
		{
			if(edges.Contains(edge))
			{
				edges.Remove(edge);
				Triangulate();
			}
		}
		
		public void AddHole(Vector2 position)
		{
			holes.Add(new Hole(position));
			Triangulate();
		}
		
		public void DeleteHole(Hole hole, bool triangulate = true)
		{
			holes.Remove(hole);
			
			if(triangulate)
			{
				Triangulate();
			}
		}
		
		public BlendShape CreateBlendshape(string name)
		{
			BlendShape blendshape = BlendShape.Create(name,GetPositions());
			blendshapes.Add(blendshape);
			isDirty = true;
			return blendshape;
		}
		
		public void DeleteBlendshape(BlendShape blendshape)
		{
			if(blendshapes.Contains(blendshape))
			{
				blendshapes.Remove(blendshape);
			}
			isDirty = true;
		}
		
		public void CreateFrame(BlendShape blendshape, float weight)
		{
			blendshape.frames.Add(BlendShapeFrame.Create(weight,GetPositions()));
			isDirty = true;
		}
		
		public void Triangulate()
		{
			SpriteMeshUtils.Triangulate(texcoords,indexedEdges,holes,ref indices);
			
			isDirty = true;
		}
		
		public void InitFromOutline(float detail, float alphaTolerance, bool holeDetection, float tessellation)
		{
			Clear();
			
			float pixelsPerUnit = SpriteMeshUtils.GetSpritePixelsPerUnit(spriteMesh.sprite);
			float factor =  pixelsPerUnit / spriteMesh.sprite.pixelsPerUnit;
			Vector2 position = rect.position / factor;
			Vector2 size = rect.size / factor;
			
			Rect l_rect = new Rect(position.x,position.y,size.x,size.y);
			
			Texture2D texture = SpriteUtility.GetSpriteTexture(spriteMesh.sprite,false);
			Rect clampedRect = MathUtils.ClampRect(MathUtils.OrderMinMax(l_rect),new Rect(0f,0f,texture.width,texture.height));
			
			List<Vector2> l_texcoords;
			List<IndexedEdge> l_indexedEdges;
			List<int> l_indices;
			
			SpriteMeshUtils.InitFromOutline(texture,clampedRect,detail,alphaTolerance,holeDetection, out l_texcoords, out l_indexedEdges, out l_indices);
			SpriteMeshUtils.Tessellate(l_texcoords,l_indexedEdges,holes,l_indices,tessellation * 10f);
			
			nodes = l_texcoords.ConvertAll( v => Node.Create(l_texcoords.IndexOf(v)) );
			edges = l_indexedEdges.ConvertAll( e => Edge.Create(nodes[e.index1], nodes[e.index2]) );
			texcoords = l_texcoords.ConvertAll( v => v * factor );
			boneWeights = l_texcoords.ConvertAll( v => BoneWeight.Create() );
			indices = l_indices;
			
			isDirty = true;
		}
		
		bool ContainsVector(Vector3 vectorToFind, List<Vector3> list, float epsilon, out int index)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Vector3 v = list [i];
				if ((v - vectorToFind).sqrMagnitude < epsilon)
				{
					index = i;
					return true;
				}
			}
			
			index = -1;
			return false;
		}
		
		public void DeleteBone(Bone2D bone)
		{
			if(spriteMeshInstance && bone)
			{
				List<Bone2D> bones = spriteMeshInstance.bones;
				
				if(bones.Contains(bone))
				{
					bones.Remove(bone);
					spriteMeshInstance.bones = bones;
					EditorUtility.SetDirty(spriteMeshInstance);
				}
			}
		}
		
		public void DeleteBindPose(BindInfo bindPose)
		{
			if(bindPose)
			{
				if(selectedBindPose == bindPose)
				{
					selectedBindPose = null;
				}
				
				int index = bindPoses.IndexOf(bindPose);
				
				Unassign(bindPose);
				bindPoses.Remove(bindPose);
				
				for(int i = 0; i < boneWeights.Count; i++)
				{
					BoneWeight boneWeight = boneWeights[i];
					boneWeight.DeleteBoneIndex(index);
					SetBoneWeight(nodes[i],boneWeight);
				}
				
				isDirty = true;
			}
		}
		
		public void Unassign(BindInfo bindPose)
		{
			Unassign(nodes,bindPose);
		}
		
		public void Unassign(List<Node> targetNodes, BindInfo bindPose)
		{
			if(bindPose)
			{
				foreach(Node node in targetNodes)
				{
					BoneWeight boneWeight = GetBoneWeight(node);
					boneWeight.Unassign(bindPoses.IndexOf(bindPose));
					SetBoneWeight(node,boneWeight);
				}
			}
		}
		
		public void BindBone(Bone2D bone)
		{
			if(spriteMeshInstance && bone)
			{
				BindInfo bindInfo = new BindInfo();
				bindInfo.bindPose = bone.transform.worldToLocalMatrix * spriteMeshInstance.transform.localToWorldMatrix;
				bindInfo.boneLength = bone.localLength;
				bindInfo.path = BoneUtils.GetBonePath (bone);
				bindInfo.name = bone.name;
				bindInfo.color = ColorRing.GetColor(bindPoses.Count);
				
				if(!bindPoses.Contains(bindInfo))
				{
					bindPoses.Add (bindInfo);
					
					isDirty = true;
				}
			}
		}
		
		public void BindBones()
		{
			selectedBone = null;
			
			if(spriteMeshInstance)
			{
				bindPoses.Clear();
				
				foreach(Bone2D bone in spriteMeshInstance.bones)
				{
					BindBone(bone);
				}
			}
		}
		
		public void CalculateAutomaticWeights()
		{
			CalculateAutomaticWeights(nodes);
		}
		
		public void CalculateAutomaticWeights(List<Node> targetNodes)
		{
			float pixelsPerUnit = SpriteMeshUtils.GetSpritePixelsPerUnit(spriteMesh.sprite);
			
			if(nodes.Count <= 0)
			{
				Debug.Log("Cannot calculate automatic weights from a SpriteMesh with no vertices.");
				return;
			}
			
			if(bindPoses.Count <= 0)
			{
				Debug.Log("Cannot calculate automatic weights. Specify bones to the SpriteMeshInstance.");
				return;
			}
			
			if(spriteMesh && bindPoses.Count > 1)
			{
				List<Vector3> l_vertices = texcoords.ConvertAll( v => (Vector3)v );
				List<Vector3> controlPoints = new List<Vector3>(bindPoses.Count*2);
				List<IndexedEdge> controlPointEdges = new List<IndexedEdge>(bindPoses.Count);
				
				foreach(BindInfo bindInfo in bindPoses)
				{
					Vector3 tip = SpriteMeshUtils.VertexToTexCoord(spriteMesh,pivotPoint,bindInfo.position,pixelsPerUnit);
					Vector3 tail = SpriteMeshUtils.VertexToTexCoord(spriteMesh,pivotPoint,bindInfo.endPoint,pixelsPerUnit);
					
					int index1 = -1;
					
					if(!ContainsVector(tip,controlPoints,0.01f, out index1))
					{
						index1 = controlPoints.Count;
						controlPoints.Add(tip);
					}
					
					int index2 = -1;
					
					if(!ContainsVector(tail,controlPoints,0.01f, out index2))
					{
						index2 = controlPoints.Count;
						controlPoints.Add(tail);
					}
					
					IndexedEdge edge = new IndexedEdge(index1, index2);
					controlPointEdges.Add(edge);
					
				}
				
				float[,] weightArray;
				
				BbwPlugin.CalculateBbw(l_vertices.ToArray(),
				                       indices.ToArray(),
				                       indexedEdges.ToArray(),
				                       controlPoints.ToArray(),
				                       controlPointEdges.ToArray(),
				                       out weightArray);
				
				FillBoneWeights(targetNodes, weightArray);
				
				isDirty = true;
			}else{
				
				BoneWeight boneWeight = BoneWeight.Create();
				boneWeight.boneIndex0 = 0;
				boneWeight.weight0 = 1f;
				
				foreach(Node node in targetNodes)
				{
					SetBoneWeight(node,boneWeight);
				}
			}
		}
		
		void FillBoneWeights(List<Node> targetNodes, float[,] weights)
		{
			List<float> l_weights = new List<float>();
			
			foreach(Node node in targetNodes)
			{
				l_weights.Clear();
				
				for(int i = 0; i < bindPoses.Count; ++i)
				{
					l_weights.Add(weights[i,node.index]);
				}
				
				SetBoneWeight(node,CreateBoneWeightFromWeights(l_weights));
			}
		}
		
		BoneWeight CreateBoneWeightFromWeights(List<float> weights)
		{
			BoneWeight boneWeight = new BoneWeight();
			
			float weight = 0f;
			int index = -1;
			
			weight = weights.Max();
			if(weight < 0.01f) weight = 0f;
			index = weight > 0f? weights.IndexOf(weight) : -1;
			
			boneWeight.weight0 = weight;
			boneWeight.boneIndex0 = index;
			
			if(index >= 0) weights[index] = 0f;
			
			weight = weights.Max();
			if(weight < 0.01f) weight = 0f;
			index = weight > 0f? weights.IndexOf(weight) : -1;
			
			boneWeight.weight1 = weight;
			boneWeight.boneIndex1 = index;
			
			if(index >= 0) weights[index] = 0f;
			
			weight = weights.Max();
			if(weight < 0.01f) weight = 0f;
			index = weight > 0f? weights.IndexOf(weight) : -1;
			
			boneWeight.weight2 = weight;
			boneWeight.boneIndex2 = index;
			
			if(index >= 0) weights[index] = 0f;
			
			weight = weights.Max();
			if(weight < 0.01f) weight = 0f;
			index = weight > 0f? weights.IndexOf(weight) : -1;
			
			boneWeight.weight3 = weight;
			boneWeight.boneIndex3 = index;
			
			float sum = boneWeight.weight0 + 
				boneWeight.weight1 +
					boneWeight.weight2 +
					boneWeight.weight3;
			
			if(sum > 0f)
			{
				boneWeight.weight0 /= sum;
				boneWeight.weight1 /= sum;
				boneWeight.weight2 /= sum;
				boneWeight.weight3 /= sum;
			}
			
			return boneWeight;
		}
		
		public void SmoothWeights(List<Node> targetNodes)
		{
			float[,] weights = new float[nodes.Count,bindPoses.Count];
			Array.Clear(weights,0,weights.Length);
			
			List<int> usedIndices = new List<int>();
			
			for (int i = 0; i < nodes.Count; i++)
			{
				usedIndices.Clear();
				
				BoneWeight weight = boneWeights[i];
				
				if(weight.boneIndex0 >= 0)
				{
					weights[i,weight.boneIndex0] = weight.weight0;
					usedIndices.Add(weight.boneIndex0);
				}
				if(weight.boneIndex1 >= 0 && !usedIndices.Contains(weight.boneIndex1))
				{
					weights[i,weight.boneIndex1] = weight.weight1;
					usedIndices.Add(weight.boneIndex1);
				}
				if(weight.boneIndex2 >= 0 && !usedIndices.Contains(weight.boneIndex2))
				{
					weights[i,weight.boneIndex2] = weight.weight2;
					usedIndices.Add(weight.boneIndex2);
				}
				if(weight.boneIndex3 >= 0 && !usedIndices.Contains(weight.boneIndex3))
				{
					weights[i,weight.boneIndex3] = weight.weight3;
					usedIndices.Add(weight.boneIndex3);
				}
			}
			
			float[] denominator = new float[nodes.Count];
			float[,] smoothedWeights = new float[nodes.Count,bindPoses.Count]; 
			Array.Clear(smoothedWeights,0,smoothedWeights.Length);
			
			for (int i = 0; i < indices.Count / 3; ++i)
			{
				for (int j = 0; j < 3; ++j)
				{
					int j1 = (j + 1) % 3;
					int j2 = (j + 2) % 3;
					
					for(int k = 0; k < bindPoses.Count; ++k)
					{
						smoothedWeights[indices[i*3 + j],k] += weights[indices[i*3 + j1],k] + weights[indices[i*3 + j2],k]; 
					}
					
					denominator[indices[i*3 + j]] += 2;
				}
			}
			
			for (int i = 0; i < nodes.Count; ++i)
			{
				for (int j = 0; j < bindPoses.Count; ++j)
				{
					smoothedWeights[i,j] /= denominator[i];
				}
			}
			
			float[,] smoothedWeightsTransposed = new float[bindPoses.Count,nodes.Count]; 
			
			for (int i = 0; i < nodes.Count; ++i)
			{
				for (int j = 0; j < bindPoses.Count; ++j)
				{
					smoothedWeightsTransposed[j,i] = smoothedWeights[i,j];
				}
			}
			
			FillBoneWeights(targetNodes, smoothedWeightsTransposed);
			
			isDirty = true;
		}
		
		public void ClearWeights()
		{
			bindPoses.Clear();
			
			isDirty = true;
		}
		
		public void Select(Node node, bool append)
		{
			if(!IsSelected(node))
			{
				if(!append)
				{
					selectedNodes.Clear();
				}
				selectedNodes.Add(node);
			}
		}
		
		public bool IsSelected(Node node)
		{
			return selectedNodes.Contains(node);
		}
		
		public void Unselect(Node node)
		{
			if(IsSelected(node))
			{
				selectedNodes.Remove(node);
			}
		}
		
		public void ClearSelection()
		{
			selectedNodes.Clear();
		}
		
		public List<Vector2> GetPositions()
		{
			if(selectedBlendshape && selectedBlendshape.frames.Count > 0)
			{
				return selectedBlendshape.frames[selectedFrameIndex].vertices.ToList();
			}
			
			return texcoords.ToList();
		}
		
		public Vector2 GetPosition(Node node)
		{
			if(selectedBlendshape && selectedBlendshape.frames.Count > 0)
			{
				return selectedBlendshape.frames[selectedFrameIndex].vertices[node.index];
			}
			return texcoords[node.index];
		}
		
		public void SetPosition(Node node, Vector2 position)
		{
			if(selectedBlendshape && selectedBlendshape.frames.Count > 0)
			{
				selectedBlendshape.frames[selectedFrameIndex].vertices[node.index] = position;
			}else{
				texcoords[node.index] = position;
			}
			isDirty = true;
		}
		
		public void SetTexcoord(Node node, Vector2 position)
		{
			texcoords[node.index] = position;
			isDirty = true;
		}
		
		public Vector2 GetTexcoord(Node node)
		{
			return texcoords[node.index];
		}
		
		public BoneWeight GetBoneWeight(Node node)
		{
			return boneWeights[node.index];
		}
		
		public void SetBoneWeight(Node node, BoneWeight boneWeight)
		{
			boneWeights[node.index] = boneWeight;
			isDirty = true;
		}
	}
}
