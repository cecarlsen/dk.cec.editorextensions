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
	public static class TransformCopyPasteRecursive
	{
		const string directotyName = "TransformCopyPasteRecursive";
		const string fileName = "Temp.dat";


		[MenuItem("CONTEXT/Transform/Copy Transform Recursively")]
		public static void Copy( MenuCommand menuCommand )
		{
			// Get root.
			Transform root = menuCommand.context as Transform;

			// Build data.
			TransformData data = new TransformData( root );

			// Ensure directory.
			string directotyPath = Application.temporaryCachePath + "/" + directotyName;
			if( !Directory.Exists( directotyPath ) ) Directory.CreateDirectory( directotyPath );

			// Store data.
			string filePath = directotyPath + "/" + fileName;
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream( filePath, FileMode.Create );
			formatter.Serialize( stream, data );
		}


		[MenuItem("CONTEXT/Transform/Paste Transform Recursively")]
		public static void Paste( MenuCommand menuCommand )
		{
			// Get root.
			Transform root = menuCommand.context as Transform;

			// Get data.
			string filePath = Application.temporaryCachePath + "/" + directotyName + "/" + fileName;
			if( !File.Exists( filePath ) ){
				Debug.Log( "No data to paste.\n" );
				return;
			}
			TransformData data = null;
			try {
				using( Stream fileStream = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read ) ){
					IFormatter formatter = new BinaryFormatter();
					data = (TransformData) formatter.Deserialize( fileStream );
				}
			} catch( IOException e ){
				Debug.LogWarning( e );
			}

			// Paste.
			if( data != null ) data.ApplyTo( root );
		}


		[Serializable]
		class TransformData
		{
			public string name;
			public SerializableVector3 localPosition;
			public SerializableQuaternion localRotation;
			public SerializableVector3 localScale;
			public TransformData[] children;


			public TransformData( Transform transform )
			{
				name = transform.name;
				localPosition = new SerializableVector3( transform.localPosition );
				localRotation = new SerializableQuaternion( transform.localRotation );
				localScale = new SerializableVector3( transform.localScale );
				children = new TransformData[ transform.childCount ];
				int c = 0;
				foreach( Transform child in transform ){
					children[c] = new TransformData( child );
					c++;
				}
			}


			public void ApplyTo( Transform transform )
			{
				if( name != transform.name ) return;

				transform.localPosition = new Vector3( localPosition.x, localPosition.y, localPosition.z );
				transform.localRotation = new Quaternion( localRotation.x, localRotation.y, localRotation.z, localRotation.w );
				transform.localScale = new Vector3( localScale.x, localScale.y, localScale.z );

				foreach( TransformData childData in children ){
					Transform child = transform.Find( childData.name );
					if( child ) childData.ApplyTo( child );
				}
			}


			[Serializable]
			public struct SerializableVector3
			{
				public float x, y, z;

				public SerializableVector3( Vector3 v ){
					x = v.x;
					y = v.y;
					z = v.z;
				}
			}


			[Serializable]
			public struct SerializableQuaternion
			{
				public float x, y, z, w;

				public SerializableQuaternion( Quaternion q ){
					x = q.x;
					y = q.y;
					z = q.z;
					w = q.w;
				}
			}
		}
	}
}