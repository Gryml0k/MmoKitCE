using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace DenariiGames.ThemeUI
{
	[CustomEditor(typeof(ThemeUIPanel))]
	public class ThemeUIPanelEditor : Editor
	{
		private SerializedProperty overrideProp;
		private SerializedProperty autoProp;
		private SerializedProperty posProp;
		private SerializedProperty indexProp;
		private SerializedProperty ppuProp;
		private SerializedProperty colorProp;
		private SerializedProperty tPaddingProp;
		private SerializedProperty hPaddingProp;
		private SerializedProperty bPaddingProp;
		ThemeUIPanel _target;

		private void OnEnable()
		{
			_target = target as ThemeUIPanel;
			overrideProp = serializedObject.FindProperty("overrideTheme");
			autoProp = serializedObject.FindProperty("automaticPositioning");
			posProp = serializedObject.FindProperty("position");
			indexProp = serializedObject.FindProperty("backgroundImageIndex");
			ppuProp = serializedObject.FindProperty("pixelsPerUnitMultiplier");
			colorProp = serializedObject.FindProperty("backgroundColor");
			tPaddingProp = serializedObject.FindProperty("topPadding");
			hPaddingProp = serializedObject.FindProperty("horizontalPadding");
			bPaddingProp = serializedObject.FindProperty("bottomPadding");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(autoProp, new GUIContent("Automatic Positioning"));
			if (autoProp.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(posProp, new GUIContent("Position"));
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.PropertyField(overrideProp, new GUIContent("Override Theme"));
			if (overrideProp.boolValue)
			{
				EditorGUI.indentLevel++;

				var component = (ThemeUIPanel)target;
				Image backgroundImage = component.GetComponent<Image>();

				EditorGUILayout.LabelField("Panel Dimensions", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(tPaddingProp, new GUIContent("Top Padding"));
				EditorGUILayout.PropertyField(hPaddingProp, new GUIContent("Horizontal Padding"));
				EditorGUILayout.PropertyField(bPaddingProp, new GUIContent("Bottom Padding"));
				EditorGUILayout.Space(12);

				EditorGUILayout.LabelField("Panel Background", EditorStyles.boldLabel);
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

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}

			serializedObject.ApplyModifiedProperties();

			if (!Application.isPlaying && _target.StateMachine != null)
			{
				DrawShowPanelButton();
			}
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
					if (theme.panelBackgroundImage == sprites[i])
					{
						indexProp.intValue = i;
						break;
					}
				}

				ppuProp.floatValue = theme.panelPixelsPerUnitMultiplier;
				colorProp.colorValue = theme.panelBackgroundColor;

				tPaddingProp.intValue = theme.panelTopPadding;
				hPaddingProp.intValue = theme.panelHorizontalPadding;
				bPaddingProp.intValue = theme.panelBottomPadding;
				serializedObject.ApplyModifiedProperties();
				Repaint();
			}

			GUILayout.EndHorizontal();
		}

		void DrawShowPanelButton()
		{
			EditorGUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Show Only this Panel", GUILayout.Width(240)))
			{
				foreach (Transform item in _target.transform.parent.transform) 
				{
					if (item != _target.transform) item.gameObject.SetActive (false);
					Undo.RegisterCompleteObjectUndo (_target, "Solo");
					_target.gameObject.SetActive (true);
				}
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		#endregion
	}
}