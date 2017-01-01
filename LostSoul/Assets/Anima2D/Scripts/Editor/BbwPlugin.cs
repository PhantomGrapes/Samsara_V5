using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Anima2D 
{
	public class BbwPlugin
	{
		[DllImport ("Anima2D")]
		private static extern int Bbw([In,Out] IntPtr vertices, int vertexCount, 
		                               [In,Out] IntPtr indices, int indexCount,
		                               [In,Out] IntPtr edges, int edgesCount,
		                               [In,Out] IntPtr controlPoints, int controlPointsCount,
		                               [In,Out] IntPtr boneEdges, int boneEdgesCount,
		                               [In,Out] IntPtr weights
		);

		[DllImport ("Anima2D")]
		private static extern void SaveData([In,Out] IntPtr vertices, int vertexCount, 
			                              [In,Out] IntPtr indices, int indexCount,
			                              [In,Out] IntPtr edges, int edgesCount,
			                              [In,Out] IntPtr controlPoints, int controlPointsCount,
			                              [In,Out] IntPtr boneEdges, int boneEdgesCount);

		public static void CalculateBbw(Vector3[] vertices, int[] indices, IndexedEdge[] edges, Vector3[] controlPoints, IndexedEdge[] controlPointEdges, out float[,] weights)
		{
			weights = new float[controlPointEdges.Length,vertices.Length];

			GCHandle verticesHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
			GCHandle indicesHandle = GCHandle.Alloc(indices, GCHandleType.Pinned);
			GCHandle edgesHandle = GCHandle.Alloc(edges, GCHandleType.Pinned);
			GCHandle controlPointsHandle = GCHandle.Alloc(controlPoints, GCHandleType.Pinned);
			GCHandle boneEdgesHandle = GCHandle.Alloc(controlPointEdges, GCHandleType.Pinned);
			GCHandle weightsHandle = GCHandle.Alloc(weights, GCHandleType.Pinned);

			/*
			SaveData(verticesHandle.AddrOfPinnedObject(), vertices.Length,
	                 indicesHandle.AddrOfPinnedObject(), indices.Length,
	                 edgesHandle.AddrOfPinnedObject(),edges.Length,
	                 controlPointsHandle.AddrOfPinnedObject(), controlPoints.Length,
			         boneEdgesHandle.AddrOfPinnedObject(), controlPointEdges.Length);
			 */

			Bbw(verticesHandle.AddrOfPinnedObject(), vertices.Length,
			    indicesHandle.AddrOfPinnedObject(), indices.Length,
			    edgesHandle.AddrOfPinnedObject(), edges.Length,
			    controlPointsHandle.AddrOfPinnedObject(), controlPoints.Length,
			    boneEdgesHandle.AddrOfPinnedObject(), controlPointEdges.Length,
			    weightsHandle.AddrOfPinnedObject());

			verticesHandle.Free();
			indicesHandle.Free();
			edgesHandle.Free();
			controlPointsHandle.Free();
			boneEdgesHandle.Free();
			weightsHandle.Free();
		}
	}
}
