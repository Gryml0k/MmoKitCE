using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace DenariiGames.ThemeUI
{
	[CustomEditor(typeof(ThemeUIText))]
	public class ThemeUITextEditor : Editor
	{
		private SerializedProperty overrideProp;
		private SerializedProperty styleProp;
		private SerializedProperty fontSizeProp;
		private SerializedProperty fontColorProp;
		private SerializedProperty fontAssetProp;

		private void OnEnable()
		{
			overrideProp = serializedObject.FindProperty("overrideTheme");
			styleProp = serializedObject.FindProperty("style");
			fontSizeProp = serializedObject.FindProperty("fontSize");
			fontColorProp = serializedObject.FindProperty("fontColor");
			fontAssetProp = serializedObject.FindProperty("fontAsset");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(styleProp, new GUIContent("Style"));
			EditorGUILayout.PropertyField(overrideProp, new GUIContent("Override Theme"));
			if (overrideProp.boolValue)
			{
				EditorGUI.indentLevel++;

				var component = (ThemeUIText)target;

				EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(fontSizeProp, new GUIContent("Font Size"));
				EditorGUILayout.PropertyField(fontColorProp, new GUIContent("Font Color"));
				EditorGUILayout.PropertyField(fontAssetProp, new GUIContent("Font"));
				EditorGUILayout.Space(12);

				if (EditorGUI.EndChangeCheck())
				{
					serializedObject.ApplyModifiedProperties();
				}

				DrawRevertThemeButton();
				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();
		}

		#region GUI Draw Methods
		void DrawRevertThemeButton()
		{
			EditorGUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.Space(EditorGUIUtility.labelWidth);

			if (GUILayout.Button ("Revert to Theme"))
			{
				ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;
				if (theme == null)
					return;

				fontSizeProp.intValue = theme.fontSize;
				fontColorProp.colorValue = theme.fontColor;
				fontAssetProp.objectReferenceValue = theme.fontAsset;

				serializedObject.ApplyModifiedProperties();
				Repaint();
			}

			GUILayout.EndHorizontal();
		}
		#endregion
	}
}