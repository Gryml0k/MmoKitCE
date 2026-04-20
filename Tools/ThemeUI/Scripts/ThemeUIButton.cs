using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
using TMPro;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIButton : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;
		[SerializeField] private ButtonStyle style = ButtonStyle.Primary;

		[SerializeField] [Range(50, 500)] private int width = 200;
		[SerializeField] [Range(25, 100)] private int height = 60;
		[SerializeField] [Range(10, 120)] private int fontSize = 36;
		[SerializeField] private Color32 fontColor = new Color32(255, 255, 255, 255);
		[SerializeField] private int backgroundImageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color backgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f);

		private RectTransform targetRect;
		private Button button;
		private Image backgroundImage;
		private TMP_Text tmpText;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (targetRect == null)
				targetRect = GetComponent<RectTransform>();
			if (button == null)
				button = GetComponent<Button>();
			if (backgroundImage == null)
				backgroundImage = GetComponentInChildren<Image>();
			if (tmpText == null)
				tmpText = GetComponentInChildren<TMP_Text>();
			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			if (backgroundImage != null)
			{
				if (!overrideTheme && theme != null)
					switch (style)
					{
						case ButtonStyle.Primary:
							ApplySprite(backgroundImage, 0, 
								theme.buttonPrimaryPixelsPerUnitMultiplier,
								theme.buttonPrimaryColor,
								ImageType.Sliced,
								theme.buttonPrimaryImage
							);
							break;

						case ButtonStyle.Close:
							ApplySprite(backgroundImage, 0, 
								theme.buttonClosePixelsPerUnitMultiplier,
								theme.buttonCloseColor, 
								ImageType.Sliced,
								theme.buttonCloseImage
							);
							break;

						default:
							ApplySprite(backgroundImage, 0, 
								theme.buttonPixelsPerUnitMultiplier, 
								theme.buttonBackgroundColor,
								ImageType.Sliced,
								theme.buttonBackgroundImage
							);
							break;
					}
				else
					ApplySprite(backgroundImage, backgroundImageIndex, pixelsPerUnitMultiplier, backgroundColor);
			}

			if (targetRect != null)
			{
				if (!overrideTheme && theme != null)
				{
					switch (style)
					{
						case ButtonStyle.Close:
							ApplyTransformWidthHeight(targetRect, theme.buttonCloseWidth, theme.buttonCloseHeight);
							break;

						case ButtonStyle.Thin:
							ApplyTransformWidthHeight(targetRect, theme.buttonWidth, theme.buttonThinHeight);
							break;

						case ButtonStyle.Increase:
							ApplyTransformWidthHeight(targetRect, theme.buttonIncreaseWidth, theme.buttonIncreaseHeight);
							break;

						default:
							ApplyTransformWidthHeight(targetRect, theme.buttonWidth, theme.buttonHeight);
							break;
					}
				}
				else
					ApplyTransformWidthHeight(targetRect, width, height);
			}

			if (button != null)
			{
				ColorBlock newColors = button.colors;
				if (!overrideTheme && theme != null)
					newColors.normalColor = (style == ButtonStyle.Primary) ? theme.buttonPrimaryColor : theme.buttonBackgroundColor;
				else
					newColors.normalColor = backgroundColor;
				button.colors = newColors;
			}

			if (tmpText != null)
			{
				if (!overrideTheme && theme != null)
					switch (style)
					{
						case ButtonStyle.Primary:
							ApplyFont(tmpText, theme.buttonFontSize, theme.buttonPrimaryFontColor, true);
							break;

						case ButtonStyle.Close:
							ApplyFont(tmpText, theme.buttonFontSize, theme.buttonCloseFontColor, true);
							break;

						default:
							ApplyFont(tmpText, theme.buttonFontSize, theme.buttonFontColor, true);
							break;
					}
				else
					ApplyFont(tmpText, fontSize, fontColor, true);
			}
		}
#endif
	}
}