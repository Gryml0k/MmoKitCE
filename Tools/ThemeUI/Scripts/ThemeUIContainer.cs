using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIContainer : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;
		[SerializeField] private bool verticalEnabled = false;

		[SerializeField] [Range(0,100)] private int topPadding = 20;
		[SerializeField] [Range(0,100)] private int horizontalPadding = 20;
		[SerializeField] [Range(0,100)] private int bottomPadding = 20;
		[SerializeField] [Range(0,100)] private int spacing = 20;

		[SerializeField] private int imageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color color = new Color(0f, 0f, 0f, 0f);

		private VerticalLayoutGroup verticalLayout;
		private Image containerImage;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (verticalLayout == null)
				verticalLayout = GetComponentInChildren<VerticalLayoutGroup>();
			if (containerImage == null)
				containerImage = GetComponentInChildren<Image>();
			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			if (verticalLayout != null)
			{
				if (verticalEnabled)
				{
					if (!overrideTheme && theme != null)
					{
						verticalLayout.padding.top = theme.containerTopPadding;
						verticalLayout.padding.left = theme.containerHorizontalPadding;
						verticalLayout.padding.right = theme.containerHorizontalPadding;
						verticalLayout.padding.bottom = theme.containerBottomPadding;
						verticalLayout.spacing = theme.containerSpacing;
					}
					else
					{
						verticalLayout.padding.top = topPadding;
						verticalLayout.padding.left = horizontalPadding;
						verticalLayout.padding.right = horizontalPadding;
						verticalLayout.padding.bottom = bottomPadding;
						verticalLayout.spacing = spacing;
					}
					verticalLayout.enabled = true;
				}
				else
					verticalLayout.enabled = false;
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