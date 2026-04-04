using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
using TMPro;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIItemTitle : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;
		[SerializeField] private bool center = true;

		[SerializeField] [Range(10, 200)] private int height = 50;
		[SerializeField] private int backgroundImageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0f);
		[SerializeField] private int fontSize = 36;
		[SerializeField] private Color32 fontColor = new Color32(255, 255, 255, 255);
		[SerializeField] private TMP_FontAsset fontAsset;

		private RectTransform targetRect;
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

			if (targetRect != null)
			{
				Vector2 newSize = targetRect.sizeDelta;

				if (!overrideTheme && theme != null)
					newSize.y = theme.itemTitleHeight;
				else
					newSize.y = height;

				targetRect.sizeDelta = newSize;
				LayoutRebuilder.ForceRebuildLayoutImmediate(targetRect);
			}

			if (backgroundImage != null)
			{
				if (!overrideTheme && theme != null)
					ApplySprite(backgroundImage, 0, theme.itemTitlePixelsPerUnitMultiplier, theme.itemTitleBackgroundColor, ImageType.Sliced, theme.itemTitleBackgroundImage);
				else
					ApplySprite(backgroundImage, backgroundImageIndex, pixelsPerUnitMultiplier, backgroundColor);
			}

			if (tmpText != null)
			{
				if (!overrideTheme && theme != null)
				{
					ApplyFont(tmpText, theme.itemTitleFontSize, theme.itemTitleFontColor, center);
					ApplyFontAsset(tmpText, theme.itemTitleFontAsset);
				}
				else
				{
					ApplyFont(tmpText, fontSize, fontColor, center);
					ApplyFontAsset(tmpText, fontAsset);
				}
			}
		}
#endif
	}
}