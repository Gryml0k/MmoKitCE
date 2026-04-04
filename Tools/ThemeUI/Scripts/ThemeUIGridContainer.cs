using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIGridContainer : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private GridSize gridSize = GridSize.Square;
		[SerializeField] private bool overrideTheme;

		[SerializeField] [Range(0,100)] private int topPadding = 20;
		[SerializeField] [Range(0,100)] private int horizontalPadding = 20;
		[SerializeField] [Range(0,100)] private int bottomPadding = 20;

		[SerializeField] [Range(0,500)] private int cellWidth = 250;
		[SerializeField] [Range(0,500)] private int cellHeight = 250;
		[SerializeField] [Range(0,100)] private int spacingX = 0;
		[SerializeField] [Range(0,100)] private int spacingY = 0;

		[SerializeField] private int imageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color color = new Color(0.25f, 0.25f, 0.25f, 1f);

		private RectTransform maskRect;
		private GridLayoutGroup gridLayout;
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
			if (gridLayout == null)
				gridLayout = GetComponentInChildren<GridLayoutGroup>();
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

			if (gridLayout != null)
			{
				if (!overrideTheme && theme != null)
				{
					switch (gridSize)
					{
						case GridSize.Square:
							gridLayout.cellSize = new Vector2(theme.gridCellWidth, theme.gridCellHeight);
							break;

						case GridSize.SmallSquare:
							gridLayout.cellSize = new Vector2(theme.gridSmallWidth, theme.gridSmallHeight);
							break;

						case GridSize.Card:
							gridLayout.cellSize = new Vector2(theme.gridCardWidth, theme.gridCardHeight);
							break;
					}
					gridLayout.spacing = new Vector2(theme.gridSpacingX, theme.gridSpacingY);

				}
				else
				{
					gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
					gridLayout.spacing = new Vector2(spacingX, spacingY);
				}
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