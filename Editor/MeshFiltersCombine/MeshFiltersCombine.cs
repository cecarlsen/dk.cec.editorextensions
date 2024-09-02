//
// Copyright (C) 2017 Carl Emil Carlsen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace CEC.EditorExtensions
{
	public static class MeshFiltersCombine
	{
		[MenuItem("GameObject/Selected MeshFilters/Combine")]
		public static void Copy( MenuCommand menuCommand )
		{
			// Get all meshfilter on selected objects and their children.
			List<MeshFilter> filters = new List<MeshFilter>();
			foreach( GameObject go in Selection.gameObjects ){
				MeshFilter[] childFilters = go.GetComponentsInChildren<MeshFilter>();
				foreach( MeshFilter childFilter in childFilters ){
					if( !filters.Contains( childFilter ) ) filters.Add( childFilter );
				}
			}

			if( filters.Count == 0 ){
				EditorUtility.DisplayDialog( "No MeshFilter were selected", "Please select a bunch of objects in the scene that have a MeshFilter component attatched.", "OK" );
				return;
			}

			// Create a list of CombineInstances.
			List<CombineInstance> combines = new List<CombineInstance>();
			foreach( MeshFilter filter in filters )
			{
				Mesh mesh = filter.sharedMesh;
				for( int s = 0; s <mesh.subMeshCount; s++ ) {
					CombineInstance combine = new CombineInstance();
					combine.mesh = mesh;
					combine.subMeshIndex = s;
					combine.transform = filter.transform.localToWorldMatrix;
					combines.Add( combine );
				}
			}

			// Combine to mesh.
			Mesh combinedMesh = new Mesh();
			combinedMesh.CombineMeshes( combines.ToArray(), true, true, true );

			// Ask for path.
			string path = EditorUtility.SaveFilePanel( "Save Combined Mesh Asset", "Assets/", "Combine", "asset");
			if( string.IsNullOrEmpty( path ) ) return;
			path = FileUtil.GetProjectRelativePath( path );

			// Store in assets folder.
			AssetDatabase.CreateAsset( combinedMesh, path );
			AssetDatabase.SaveAssets();

			// Log feedback.
			Debug.Log( combines.Count + " meshes were combined and stored to '" + path + "'.\n" );
		}
	}
}