using UnityEngine;
using UnityEditor;

namespace DenariiGames.ThemeUI
{
	public class ThemeUIWindow : EditorWindow
	{
		private ThemeUITheme currentTheme;
		private const string PREF_KEY_THEME_GUID = "ThemeUI_ActiveTheme_GUID";

		[MenuItem("MMORPG KIT/MmoKitCE/Theme Manager")]
		public static void ShowWindow()
		{
			GetWindow<ThemeUIWindow>("ThemeUI Theme Manager");
		}

		private void OnEnable()
		{
			if (EditorPrefs.HasKey(PREF_KEY_THEME_GUID))
			{
				string guid = EditorPrefs.GetString(PREF_KEY_THEME_GUID);
				if (!string.IsNullOrEmpty(guid))
				{
					string path = AssetDatabase.GUIDToAssetPath(guid);
					if (!string.IsNullOrEmpty(path))
					{
						currentTheme = AssetDatabase.LoadAssetAtPath<ThemeUITheme>(path);
						ThemeUIThemeManager.CurrentTheme = currentTheme;
					}
				}
			}
		}

		private void OnGUI()
		{
			EditorGUILayout.Space(12);

			EditorGUI.BeginChangeCheck();
			currentTheme = (ThemeUITheme)EditorGUILayout.ObjectField(
				"Active Theme",
				currentTheme,
				typeof(ThemeUITheme),
				false
			);

			if (EditorGUI.EndChangeCheck())
			{
				ThemeUIThemeManager.CurrentTheme = currentTheme;
				SavePersistedTheme();
			}

			EditorGUILayout.Space(12);
			if (GUILayout.Button("Apply Theme", GUILayout.Height(32)))
			{
				ThemeUIThemeManager.RefreshAllInScene();
				SavePersistedTheme();
			}
		}

		private void SavePersistedTheme()
		{
			if (currentTheme != null)
			{
				string path = AssetDatabase.GetAssetPath(currentTheme);
				if (!string.IsNullOrEmpty(path))
				{
					string guid = AssetDatabase.AssetPathToGUID(path);
					EditorPrefs.SetString(PREF_KEY_THEME_GUID, guid);
				}
			}
			else
			{
				EditorPrefs.DeleteKey(PREF_KEY_THEME_GUID);
			}
		}
	}
}