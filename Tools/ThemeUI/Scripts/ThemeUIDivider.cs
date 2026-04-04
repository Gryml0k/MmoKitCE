using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIDivider : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;
		[SerializeField] private ImageType imageType = ImageType.Sliced;

		[SerializeField] [Range(0, 500)] private int height = 100;
		[SerializeField] [Range(0, 500)] private int imageWidth = 300;
		[SerializeField] [Range(0, 500)] private int imageHeight = 50;
		[SerializeField] private int imageIndex = 0;
		[SerializeField] private Color color = new Color(0.25f, 0.25f, 0.25f, 1f);
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;

		private RectTransform targetRect;
		private Image image;
		private RectTransform imageRect;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (targetRect == null)
				targetRect = GetComponent<RectTransform>();
			if (image == null)
				image = GetComponentInChildren<Image>();
			if (imageRect == null)
			{
				if (image != null)
					imageRect = image.GetComponent<RectTransform>();
			}

			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			if (image != null)
			{

				if (!overrideTheme && theme != null)
					ApplySprite(image, 0, theme.dividerPixelsPerUnitMultiplier, theme.dividerColor, theme.dividerImageType, theme.dividerImage);
				else
					ApplySprite(image, imageIndex, pixelsPerUnitMultiplier, color, imageType);
			}

			if (imageRect != null)
			{
				if (!overrideTheme && theme != null)
					ApplyTransformVerticalAnchor(imageRect, VerticalPosition.Center, theme.dividerImageWidth, theme.dividerImageHeight, 0, 0);
				else
					ApplyTransformVerticalAnchor(imageRect, VerticalPosition.Center, imageWidth, imageHeight, 0, 0);
			}

			if (targetRect != null)
			{
				if (!overrideTheme && theme != null)
					ApplyTransformStretchPadding(targetRect, 0, 0, -theme.dividerHeight);
				else
					ApplyTransformStretchPadding(targetRect, 0, 0, -height);
			}
		}
#endif
	}
}