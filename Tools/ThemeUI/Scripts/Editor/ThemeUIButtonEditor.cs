using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace DenariiGames.ThemeUI
{
	[CustomEditor(typeof(ThemeUIButton))]
	public class ThemeUIButtonEditor : Editor
	{
		private SerializedProperty overrideProp;
		private SerializedProperty styleProp;
		private SerializedProperty widthProp;
		private SerializedProperty heightProp;
		private SerializedProperty indexProp;
		private SerializedProperty ppuProp;
		private SerializedProperty colorProp;
		private SerializedProperty fontSizeProp;
		private SerializedProperty fontColorProp;

		private void OnEnable()
		{
			overrideProp = serializedObject.FindProperty("overrideTheme");
			styleProp = serializedObject.FindProperty("style");
			widthProp = serializedObject.FindProperty("width");
			heightProp = serializedObject.FindProperty("height");
			indexProp = serializedObject.FindProperty("backgroundImageIndex");
			ppuProp = serializedObject.FindProperty("pixelsPerUnitMultiplier");
			colorProp = serializedObject.FindProperty("backgroundColor");
			fontSizeProp = serializedObject.FindProperty("fontSize");
			fontColorProp = serializedObject.FindProperty("fontColor");
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

				var component = (ThemeUIButton)target;
				Image backgroundImage = component.GetComponent<Image>();

				EditorGUILayout.LabelField("Button Dimensions", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(widthProp, new GUIContent("Width"));
				EditorGUILayout.PropertyField(heightProp, new GUIContent("Height"));
				EditorGUILayout.Space(12);

				EditorGUILayout.LabelField("Button Background", EditorStyles.boldLabel);
				var sprites = ThemeUIThemeManager.GetBackgroundSprites();
				string[] options;
				if (sprites.Length > 0)
				{
					options = new string[sprites.Length];
					for (int i = 0; i < sprites.Length; i++)
					{
						Sprite sp = sprites[i];
						options[i] = sp != null ? $"{i}: {sp.name}" : $"{i}: (null)";
					}
					int currentIndex = indexProp.intValue;

					EditorGUILayout.BeginHorizontal();

					EditorGUILayout.PrefixLabel("Image");
					int newIndex = EditorGUILayout.Popup(currentIndex, options, GUILayout.ExpandWidth(true));

					if (GUILayout.Button(new GUIContent("◄", "Previous sprite"), GUILayout.Width(30)))
					{
						newIndex = (currentIndex - 1 + sprites.Length) % sprites.Length;
					}

					if (GUILayout.Button(new GUIContent("►", "Next sprite"), GUILayout.Width(30)))
					{
						newIndex = (currentIndex + 1) % sprites.Length;
					}

					EditorGUILayout.EndHorizontal();

					if (newIndex != currentIndex)
					{
						indexProp.intValue = newIndex;
						serializedObject.ApplyModifiedProperties();
						component.ApplySprite(backgroundImage, newIndex, ppuProp.floatValue, colorProp.colorValue);
						Repaint();
					}
				}
				else
				{
					EditorGUILayout.HelpBox("No background sprites found.", MessageType.Warning);
				}

				EditorGUILayout.PropertyField(colorProp, new GUIContent("Background Color"));
				EditorGUILayout.PropertyField(ppuProp, new GUIContent("Pixels Per Unit Multiplier"));
				EditorGUILayout.Space(12);

				EditorGUILayout.LabelField("Button Font", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(fontSizeProp, new GUIContent("Font Size"));
				EditorGUILayout.PropertyField(fontColorProp, new GUIContent("Font Color"));
				EditorGUILayout.Space(12);

				if (EditorGUI.EndChangeCheck())
				{
					serializedObject.ApplyModifiedProperties();
					component.ApplySprite(backgroundImage, indexProp.intValue, ppuProp.floatValue, colorProp.colorValue);
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

				var sprites = ThemeUIThemeManager.GetBackgroundSprites();
				indexProp.intValue = 0;
				for (int i = 0; i < sprites.Length; i++)
				{
					if (theme.buttonBackgroundImage == sprites[i])
					{
						indexProp.intValue = i;
						break;
					}
				}

				ppuProp.floatValue = theme.buttonPixelsPerUnitMultiplier;
				colorProp.colorValue = theme.buttonBackgroundColor;

				widthProp.intValue = theme.buttonWidth;
				heightProp.intValue = theme.buttonHeight;
				fontSizeProp.intValue = theme.buttonFontSize;
				fontColorProp.colorValue = theme.buttonFontColor;

				serializedObject.ApplyModifiedProperties();
				Repaint();
			}

			GUILayout.EndHorizontal();
		}
		#endregion
	}
}