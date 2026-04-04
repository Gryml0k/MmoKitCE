using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIToggle : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;

		[SerializeField] [Range(10, 200)] private int height = 40;
		[SerializeField] [Range(10, 200)] private int width = 40;
		[SerializeField] [Range(0, 200)] private int labelPadding = 10;
		[SerializeField] private int imageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color color = new Color(0f, 0f, 0f, 0f);

		private RectTransform componentRect;
		private Image backgroundImage;
		private RectTransform backgroundImageRect;
		private RectTransform textRect;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (componentRect == null)
				componentRect = GetComponent<RectTransform>();
			if (backgroundImage == null)
				backgroundImage = GetComponentInChildren<Image>();
			if (backgroundImage != null && backgroundImageRect == null)
				backgroundImageRect = backgroundImage.transform.GetComponent<RectTransform>();
			if (textRect == null)
			{
				ThemeUIText text = GetComponentInChildren<ThemeUIText>();
				if (text != null)
					textRect = text.transform.GetComponent<RectTransform>();
			}
			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;
			Vector2 newSize = Vector2.zero;

			if (componentRect != null)
			{
				newSize = componentRect.sizeDelta;

				if (!overrideTheme && theme != null)
					newSize.y = theme.itemTitleHeight;
				else
					newSize.y = height;

				componentRect.sizeDelta = newSize;
				LayoutRebuilder.ForceRebuildLayoutImmediate(componentRect);
			}

			if (backgroundImageRect != null)
			{
				newSize = backgroundImageRect.sizeDelta;

				if (!overrideTheme && theme != null)
				{
					newSize.x = theme.toggleWidth;
					newSize.y = theme.toggleHeight;
					backgroundImageRect.sizeDelta = newSize;
					backgroundImageRect.anchoredPosition = new Vector2((float)theme.toggleWidth / 2f, (float)theme.toggleHeight / -2f);
				}
				else
				{
					newSize.x = width;
					newSize.y = height;
					backgroundImageRect.sizeDelta = newSize;
					backgroundImageRect.anchoredPosition = new Vector2((float)width / 2f, (float)height / -2f);
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundImageRect);
			}

			if (textRect != null)
			{
				newSize = textRect.sizeDelta;

				if (!overrideTheme && theme != null)
					ApplyTransformStretchPaddingLeft(textRect, 0, theme.toggleWidth + theme.toggleLabelPadding, 0);
				else
					ApplyTransformStretchPaddingLeft(textRect, 0, width + labelPadding, 0);
			}

			if (backgroundImage != null)
			{
				if (!overrideTheme && theme != null)
					ApplySprite(backgroundImage, 0, theme.togglePixelsPerUnitMultiplier, theme.toggleColor, ImageType.Sliced, theme.toggleImage);
				else
					ApplySprite(backgroundImage, imageIndex, pixelsPerUnitMultiplier, color);
			}
		}
#endif
	}
}