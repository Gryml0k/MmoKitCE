using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace DenariiGames.ThemeUI
{
	[CustomEditor(typeof(ThemeUIContainer))]
	public class ThemeUIContainerEditor : Editor
	{
		private SerializedProperty overrideProp;
		private SerializedProperty verticalProp;

		private SerializedProperty tPaddingProp;
		private SerializedProperty hPaddingProp;
		private SerializedProperty bPaddingProp;
		private SerializedProperty spacingProp;

		private SerializedProperty indexProp;
		private SerializedProperty ppuProp;
		private SerializedProperty colorProp;

		private void OnEnable()
		{
			overrideProp = serializedObject.FindProperty("overrideTheme");
			verticalProp = serializedObject.FindProperty("verticalEnabled");

			tPaddingProp = serializedObject.FindProperty("topPadding");
			hPaddingProp = serializedObject.FindProperty("horizontalPadding");
			bPaddingProp = serializedObject.FindProperty("bottomPadding");
			spacingProp = serializedObject.FindProperty("spacing");

			indexProp = serializedObject.FindProperty("imageIndex");
			ppuProp = serializedObject.FindProperty("pixelsPerUnitMultiplier");
			colorProp = serializedObject.FindProperty("color");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(verticalProp, new GUIContent("Vertical Enabled"));
			EditorGUILayout.PropertyField(overrideProp, new GUIContent("Override Theme"));
			if (overrideProp.boolValue)
			{
				EditorGUI.indentLevel++;

				var component = (ThemeUIContainer)target;
				Image backgroundImage = component.GetComponent<Image>();

				EditorGUILayout.LabelField("Container Dimensions", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(tPaddingProp, new GUIContent("Top Padding"));
				EditorGUILayout.PropertyField(hPaddingProp, new GUIContent("Horizontal Padding"));
				EditorGUILayout.PropertyField(bPaddingProp, new GUIContent("Bottom Padding"));
				EditorGUILayout.PropertyField(spacingProp, new GUIContent("Spacing"));
				EditorGUILayout.Space(12);

				EditorGUILayout.LabelField("Container Background", EditorStyles.boldLabel);
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

				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}