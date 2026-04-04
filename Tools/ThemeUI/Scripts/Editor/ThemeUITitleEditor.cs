using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace DenariiGames.ThemeUI
{
	[CustomEditor(typeof(ThemeUITitle))]
	public class ThemeUITitleEditor : Editor
	{
		private SerializedProperty overrideProp;
		private SerializedProperty indexProp;
		private SerializedProperty ppuProp;
		private SerializedProperty colorProp;
		private SerializedProperty heightProp;
		private SerializedProperty fontSizeProp;
		private SerializedProperty fontColorProp;
		private SerializedProperty marginTopProp;
		private SerializedProperty fontAssetProp;

		private void OnEnable()
		{
			overrideProp = serializedObject.FindProperty("overrideTheme");
			indexProp = serializedObject.FindProperty("backgroundImageIndex");
			ppuProp = serializedObject.FindProperty("pixelsPerUnitMultiplier");
			colorProp = serializedObject.FindProperty("backgroundColor");
			heightProp = serializedObject.FindProperty("height");
			fontSizeProp = serializedObject.FindProperty("fontSize");
			fontColorProp = serializedObject.FindProperty("fontColor");
			marginTopProp = serializedObject.FindProperty("fontMarginTop");
			fontAssetProp = serializedObject.FindProperty("fontAsset");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(overrideProp, new GUIContent("Override Theme"));
			if (overrideProp.boolValue)
			{
				EditorGUI.indentLevel++;

				var component = (ThemeUITitle)target;
				Image backgroundImage = component.GetComponent<Image>();

				EditorGUILayout.LabelField("Title", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(heightProp, new GUIContent("Height"));
				EditorGUILayout.PropertyField(marginTopProp, new GUIContent("Font Margin Top"));
				EditorGUILayout.PropertyField(fontSizeProp, new GUIContent("Font Size"));
				EditorGUILayout.PropertyField(fontColorProp, new GUIContent("Font Color"));
				EditorGUILayout.PropertyField(fontAssetProp, new GUIContent("Font Asset"));
				EditorGUILayout.Space(12);

				EditorGUILayout.LabelField("Title Background", EditorStyles.boldLabel);
				var sprites = ThemeUIThemeManager.GetBackgroundSprites();
				if (sprites.Length > 0)
				{
					string[] options = new string[sprites.Length];
					for (int i = 0; i < sprites.Length; i++)
					{
						Sprite sp = sprites[i];
						options[i] = sp != null ? $"{i}: {sp.name}" : $"{i}: (null)";
					}
					int currentIndex = indexProp.intValue;

					EditorGUILayout.BeginHorizontal();

					EditorGUILayout.PrefixLabel("Background Image");
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
					if (theme.titleBackgroundImage == sprites[i])
					{
						indexProp.intValue = i;
						break;
					}
				}

				ppuProp.floatValue = theme.titlePixelsPerUnitMultiplier;
				colorProp.colorValue = theme.titleBackgroundColor;

				heightProp.intValue = theme.titleHeight;
				marginTopProp.intValue = theme.titleFontMarginTop;

				fontSizeProp.intValue = theme.titleFontSize;
				fontColorProp.colorValue = theme.titleFontColor;
				fontAssetProp.objectReferenceValue = theme.titleFontAsset;

				serializedObject.ApplyModifiedProperties();
				Repaint();
			}

			GUILayout.EndHorizontal();
		}
		#endregion
	}
}