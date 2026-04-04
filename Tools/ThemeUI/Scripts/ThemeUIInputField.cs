using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIInputField : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;
		[SerializeField] private int height = 45;
		[SerializeField] private int imageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color color = new Color(1f, 1f, 1f, 1f);

		private RectTransform targetRect;
		private Image backgroundImage;

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
				backgroundImage = GetComponent<Image>();
			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			if (targetRect != null)
			{
				if (!overrideTheme && theme != null)
					ApplyTransformWidthHeight(targetRect, 0, theme.inputFieldHeight);
				else
					ApplyTransformWidthHeight(targetRect, 0, height);
			}

			if (backgroundImage != null)
			{
				if (!overrideTheme && theme != null)
					ApplySprite(backgroundImage, 0, theme.inputFieldPixelsPerUnitMultiplier, theme.inputFieldColor, ImageType.Sliced, theme.inputFieldImage);
				else
					ApplySprite(backgroundImage, imageIndex, pixelsPerUnitMultiplier, color);
			}

		}
#endif
	}
}