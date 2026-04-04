using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace DenariiGames.ThemeUI
{
	[CustomEditor(typeof(ThemeUIScrollbar))]
	public class ThemeUIScrollbarEditor : Editor
	{
		private SerializedProperty overrideProp;
		private SerializedProperty widthProp;
		private SerializedProperty posProp;
		private SerializedProperty paddingProp;
		private SerializedProperty indexProp;
		private SerializedProperty ppuProp;
		private SerializedProperty colorProp;
		private SerializedProperty hIndexProp;
		private SerializedProperty hPpuProp;
		private SerializedProperty hColorProp;

		private void OnEnable()
		{
			overrideProp = serializedObject.FindProperty("overrideTheme");
			widthProp = serializedObject.FindProperty("width");
			posProp = serializedObject.FindProperty("positionX");
			paddingProp = serializedObject.FindProperty("verticalPadding");
			indexProp = serializedObject.FindProperty("imageIndex");
			ppuProp = serializedObject.FindProperty("pixelsPerUnitMultiplier");
			colorProp = serializedObject.FindProperty("color");
			hIndexProp = serializedObject.FindProperty("handleImageIndex");
			hPpuProp = serializedObject.FindProperty("handlePixelsPerUnitMultiplier");
			hColorProp = serializedObject.FindProperty("handleColor");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(overrideProp, new GUIContent("Override Theme"));
			if (overrideProp.boolValue)
			{
				EditorGUI.indentLevel++;

				var component = (ThemeUIScrollbar)target;
				Image image = component.GetComponent<Image>();
				Image handleImage = component.GetComponent<Scrollbar>().handleRect.GetComponent<Image>();

				EditorGUILayout.LabelField("Scrollbar Dimensions", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(widthProp, new GUIContent("Width"));
				EditorGUILayout.PropertyField(posProp, new GUIContent("Horizontal Offset"));
				EditorGUILayout.PropertyField(paddingProp, new GUIContent("Vertical Padding"));
				EditorGUILayout.Space(12);

				EditorGUILayout.LabelField("Scrollbar Background", EditorStyles.boldLabel);
				var sprites = ThemeUIThemeManager.GetBackgroundSprites();
				string[] options = new string[sprites.Length];
				if (sprites.Length > 0)
				{
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
						component.ApplySprite(image, newIndex, ppuProp.floatValue, colorProp.colorValue);
						Repaint();
					}
				}
				else
				{
					EditorGUILayout.HelpBox("No background sprites found.", MessageType.Warning);
				}

				EditorGUILayout.PropertyField(colorProp, new GUIContent("Color"));
				EditorGUILayout.PropertyField(ppuProp, new GUIContent("Pixels Per Unit Multiplier"));
				EditorGUILayout.Space(12);

				EditorGUILayout.LabelField("Scrollbar Handle", EditorStyles.boldLabel);
				if (sprites.Length > 0)
				{
					int hCurrentIndex = hIndexProp.intValue;

					EditorGUILayout.BeginHorizontal();

					EditorGUILayout.PrefixLabel("Image");
					int hNewIndex = EditorGUILayout.Popup(hCurrentIndex, options, GUILayout.ExpandWidth(true));

					if (GUILayout.Button(new GUIContent("◄", "Previous sprite"), GUILayout.Width(30)))
					{
						hNewIndex = (hCurrentIndex - 1 + sprites.Length) % sprites.Length;
					}

					if (GUILayout.Button(new GUIContent("►", "Next sprite"), GUILayout.Width(30)))
					{
						hNewIndex = (hCurrentIndex + 1) % sprites.Length;
					}

					EditorGUILayout.EndHorizontal();

					if (hNewIndex != hCurrentIndex)
					{
						hIndexProp.intValue = hNewIndex;
						serializedObject.ApplyModifiedProperties();
						component.ApplySprite(handleImage, hNewIndex, hPpuProp.floatValue, hColorProp.colorValue);
						Repaint();
					}
				}

				EditorGUILayout.PropertyField(hColorProp, new GUIContent("Handle Color"));
				EditorGUILayout.PropertyField(hPpuProp, new GUIContent("Handle Pixels Per Unit Multiplier"));

				if (EditorGUI.EndChangeCheck())
				{
					serializedObject.ApplyModifiedProperties();
					component.ApplySprite(image, indexProp.intValue, ppuProp.floatValue, colorProp.colorValue);
					component.ApplySprite(handleImage, hIndexProp.intValue, hPpuProp.floatValue, hColorProp.colorValue);
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
					if (theme.scrollbarImage == sprites[i])
					{
						indexProp.intValue = i;
						break;
					}
				}

				hIndexProp.intValue = 0;
				for (int i = 0; i < sprites.Length; i++)
				{
					if (theme.scrollbarHandleImage == sprites[i])
					{
						hIndexProp.intValue = i;
						break;
					}
				}

				ppuProp.floatValue = theme.scrollbarPixelsPerUnitMultiplier;
				colorProp.colorValue = theme.scrollbarColor;
				hPpuProp.floatValue = theme.scrollbarHandlePixelsPerUnitMultiplier;
				hColorProp.colorValue = theme.scrollbarHandleColor;

				widthProp.intValue = theme.scrollbarWidth;
				posProp.intValue = theme.scrollbarPositionX;
				paddingProp.intValue = theme.scrollbarVerticalPadding;

				serializedObject.ApplyModifiedProperties();
				Repaint();
			}

			GUILayout.EndHorizontal();
		}
		#endregion
	}
}