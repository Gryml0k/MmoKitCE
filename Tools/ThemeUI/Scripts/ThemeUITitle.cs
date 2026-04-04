using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
using TMPro;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUITitle : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;

		[SerializeField] [Range(10, 200)] private int height = 50;
		[SerializeField] private int backgroundImageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0f);
		[SerializeField] private int fontSize = 36;
		[SerializeField] private Color32 fontColor = new Color32(255, 255, 255, 255);
		[SerializeField] [Range(-100, 100)] private int fontMarginTop = 0;
		[SerializeField] private TMP_FontAsset fontAsset;

		private RectTransform targetRect;
		private Image backgroundImage;
		private TMP_Text tmpText;
		private RectTransform textRect;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (targetRect == null)
			targetRect = GetComponent<RectTransform>();
			if (backgroundImage == null)
			backgroundImage = GetComponentInChildren<Image>();
			if (tmpText == null)
				tmpText = GetComponentInChildren<TMP_Text>();
			if (textRect == null && tmpText != null)
				textRect = tmpText.GetComponent<RectTransform>();
			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			if (targetRect != null)
			{
				Vector2 newSize = targetRect.sizeDelta;

				if (!overrideTheme && theme != null)
					newSize.y = theme.titleHeight;
				else
					newSize.y = height;

				targetRect.sizeDelta = newSize;
				LayoutRebuilder.ForceRebuildLayoutImmediate(targetRect);
			}

			if (backgroundImage != null)
			{
				if (!overrideTheme && theme != null)
					ApplySprite(backgroundImage, 0, theme.titlePixelsPerUnitMultiplier, theme.titleBackgroundColor, ImageType.Sliced, theme.titleBackgroundImage);
				else
					ApplySprite(backgroundImage, backgroundImageIndex, pixelsPerUnitMultiplier, backgroundColor);
			}

			if (tmpText != null)
			{
				if (!overrideTheme && theme != null)
				{
					ApplyFont(tmpText, theme.titleFontSize, theme.titleFontColor, true);
					ApplyFontAsset(tmpText, theme.titleFontAsset);
				}
				else
				{
					ApplyFont(tmpText, fontSize, fontColor, true);
					ApplyFontAsset(tmpText, fontAsset);
				}
			}

			if (textRect != null)
			{
				if (!overrideTheme && theme != null)
					ApplyTransformStretchPadding(textRect, theme.titleFontMarginTop, 0, 0);
				else
					ApplyTransformStretchPadding(textRect, fontMarginTop, 0, 0);
			}
		}
#endif
	}
}