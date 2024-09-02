//
// Copyright (C) 2017-2021 Carl Emil Carlsen
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


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace CEC.EditorExtensions
{
	public class ScreenshotMenuItem : ScriptableObject
	{
		[MenuItem( "Tools/Screenshot/PNG #%&s", false, 1 )]
		static void Screenshot()
		{
			Screenshot( 1 );
		}

		[MenuItem ( "Tools/Screenshot/PNG x4", false, 2)]
		static void ScreenshotX4()
		{
			Screenshot( 4 );
		}

		[MenuItem ( "Tools/Screenshot/PNG x8", false, 3)]
		static void ScreenshotX8()
		{
			Screenshot( 8 );
		}

		[MenuItem ( "Tools/Screenshot/PNG x12", false, 4)]
		static void ScreenshotX12()
		{
			Screenshot( 12 );
		}

		[MenuItem ( "Tools/Screenshot/PNG x16", false, 5)]
		static void ScreenshotX16()
		{
			Screenshot( 16 );
		}


		static void Screenshot( int sizeMult )
		{
			string name = SceneManager.GetActiveScene().name + " " + DateTimeCode();
			string desktopPath;
			if( Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer ){
				desktopPath = System.Environment.GetEnvironmentVariable( "HOME" ) + "/Desktop";
			} else if( Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer ){
				desktopPath = System.Environment.GetFolderPath( System.Environment.SpecialFolder.Desktop );
				desktopPath = desktopPath.Replace( "\\", "/" );
			} else {
				Debug.LogWarning( "Screen shot failed. Platform not supported." + System.Environment.NewLine );
				return;
			}

			string path = desktopPath + "/" + name + ".png";

			int previousAnialisationMode = QualitySettings.antiAliasing;
			QualitySettings.antiAliasing = 0;
			ScreenCapture.CaptureScreenshot( path, sizeMult );
			QualitySettings.antiAliasing = previousAnialisationMode;
			Debug.Log( "Saved screenshot at: " + path );
		}


		static string DateTimeCode(){
			return System.DateTime.Now.ToString("yy") + System.DateTime.Now.ToString("MM") + System.DateTime.Now.ToString("dd") + "_" + System.DateTime.Now.ToString("hh") + System.DateTime.Now.ToString("mm") + System.DateTime.Now.ToString("ss");
		}
	}
}