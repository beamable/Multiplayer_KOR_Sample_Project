using Beamable.Samples.SampleProjectBase;
using UnityEngine;
using UnityEditor;

namespace Beamable.Samples.Core
{
	/// <summary>
	/// Ping a custom-formatted readme file and force-show in inspector. Parse the
	/// custom format to markdown-like display.
	///
	/// Inspired by Unity's "Learn" Sample Projects
	///
	/// </summary>
	[CustomEditor(typeof(Readme))]
	public class ReadmeEditor : UnityEditor.Editor
	{
		static float kSpace = 16f;

		protected static Readme SelectReadme(string findAssetsFilter, string[] findAssetsFolders)
		{
			var ids = AssetDatabase.FindAssets(findAssetsFilter, findAssetsFolders);

			if (ids.Length == 1)
			{
				var pathToReadme = AssetDatabase.GUIDToAssetPath(ids[0]);
				return SelectReadme(pathToReadme);
			}
			
			if (ids.Length > 1)
			{
				Debug.LogError("SelectReadme() Too many results found for Readme.");
			}

			return null;
		}


		private static Readme SelectReadme(string pathToReadme)
		{
			if (string.IsNullOrWhiteSpace(pathToReadme))
				return null;

			var readmeObject = AssetDatabase.LoadMainAssetAtPath(pathToReadme);

			if (readmeObject == null)
				return null;

			var editorAsm = typeof(UnityEditor.Editor).Assembly;
			var inspectorWindowType = editorAsm.GetType("UnityEditor.InspectorWindow");
			var window = EditorWindow.GetWindow(inspectorWindowType);
			window.Focus();

			Selection.objects = new UnityEngine.Object[] { readmeObject };
			return (Readme)readmeObject;
		}

		protected override void OnHeaderGUI()
		{
			var readme = (Readme)target;
			Init();

			var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

			GUILayout.BeginHorizontal("In BigTitle");
			{
				GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
				GUILayout.Label(readme.title, TitleStyle);
			}
			GUILayout.EndHorizontal();
		}

      public override void OnInspectorGUI()
		{

			var readme = (Readme)target;
			Init();

			foreach (var section in readme.sections)
			{
				if (!string.IsNullOrWhiteSpace(section.heading))
				{
					GUILayout.Label(section.heading, HeadingStyle);
				}
				if (!string.IsNullOrWhiteSpace(section.text))
				{
					GUILayout.Label(section.text, BodyStyle);
				}
				if (!string.IsNullOrWhiteSpace(section.linkText) &&
				    LinkLabel(new GUIContent(section.linkText)))
				{
					Application.OpenURL(section.url);
				}
				GUILayout.Space(kSpace);
			}
		}


		bool _initialized;

		GUIStyle LinkStyle => m_LinkStyle;
		[SerializeField] GUIStyle m_LinkStyle;

		GUIStyle TitleStyle => m_TitleStyle;
		[SerializeField] GUIStyle m_TitleStyle;

		GUIStyle HeadingStyle => m_HeadingStyle;
		[SerializeField] GUIStyle m_HeadingStyle;

		GUIStyle BodyStyle => m_BodyStyle;
		[SerializeField] GUIStyle m_BodyStyle;

		void Init()
		{
			if (_initialized)
				return;
			m_BodyStyle = new GUIStyle(EditorStyles.label)
			{
				wordWrap = true,
				fontSize = 14
			};

			m_TitleStyle = new GUIStyle(m_BodyStyle)
			{
				fontSize = 26
			};

			m_HeadingStyle = new GUIStyle(m_BodyStyle)
			{
				fontSize = 18
			};

			m_LinkStyle = new GUIStyle(m_BodyStyle)
			{
				wordWrap = false,
				normal =
				{
					// Match selection color which works nicely for both light and dark skins
					textColor = new Color(0f, 0.4706f, 0.8549f, 1f)
				},
				stretchWidth = false
			};

			_initialized = true;
		}

		bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
		{
			var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

			Handles.BeginGUI();
			Handles.color = LinkStyle.normal.textColor;
			Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
			Handles.color = Color.white;
			Handles.EndGUI();

			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

			return GUI.Button(position, label, LinkStyle);
		}
	}
}
