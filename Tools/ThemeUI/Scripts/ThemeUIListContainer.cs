using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIListContainer : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;

		[SerializeField] [Range(0,100)] private int topPadding = 20;
		[SerializeField] [Range(0,100)] private int horizontalPadding = 20;
		[SerializeField] [Range(0,100)] private int bottomPadding = 20;
		[SerializeField] [Range(0,100)] private int spacing = 20;

		[SerializeField] private int imageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color color = new Color(0.25f, 0.25f, 0.25f, 1f);

		private RectTransform maskRect;
		private VerticalLayoutGroup verticalLayout;
		private Image containerImage;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (maskRect == null)
				maskRect = GetComponentInChildren<Mask>().transform.GetComponent<RectTransform>();
			if (verticalLayout == null)
				verticalLayout = GetComponentInChildren<VerticalLayoutGroup>();
			if (containerImage == null)
				containerImage = GetComponent<Image>();
			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			if (maskRect != null)
			{
				if (!overrideTheme && theme != null)
					ApplyTransformStretchPadding(maskRect, theme.containerTopPadding, theme.containerHorizontalPadding, theme.containerBottomPadding);
				else
					ApplyTransformStretchPadding(maskRect, topPadding, horizontalPadding, bottomPadding);
			}

			if (verticalLayout != null)
			{
				if (!overrideTheme && theme != null)
				{
					verticalLayout.spacing = theme.listSpacing;
				}
				else
				{
					verticalLayout.spacing = spacing;
				}
				verticalLayout.enabled = true;
			}

			if (containerImage != null)
			{
				if (!overrideTheme && theme != null)
					ApplySprite(containerImage, 0, theme.containerPixelsPerUnitMultiplier, theme.containerBackgroundColor, ImageType.Sliced, theme.containerBackgroundImage);
				else
					ApplySprite(containerImage, imageIndex, pixelsPerUnitMultiplier, color);
			}
		}
#endif
	}
}