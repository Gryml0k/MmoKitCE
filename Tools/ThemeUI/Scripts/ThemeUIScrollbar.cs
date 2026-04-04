using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIScrollbar : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;

		[SerializeField] [Range(0,100)] private int width = 15;
		[SerializeField] [Range(-100,100)] private int positionX = -10;
		[SerializeField] [Range(0,100)] private int verticalPadding = 35;
		[SerializeField] private int imageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color color = new Color(0f, 0f, 0f, 0f);

		//scrollbar handle
		[SerializeField] private int handleImageIndex = 0;
		[SerializeField] private float handlePixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color handleColor = new Color(1f, 1f, 1f, 1f);

		private RectTransform rect;
		private Image image;
		private Image handleImage;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (rect == null)
				rect = GetComponent<RectTransform>();
			if (image == null)
				image = GetComponent<Image>();
			if (handleImage == null)
				handleImage = GetComponent<Scrollbar>().handleRect.GetComponent<Image>();
			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			if (image != null)
			{
				if (!overrideTheme && theme != null)
					ApplySprite(image, 0, theme.scrollbarPixelsPerUnitMultiplier, theme.scrollbarColor, ImageType.Sliced, theme.scrollbarImage);
				else
					ApplySprite(image, imageIndex, pixelsPerUnitMultiplier, color);
			}

			if (handleImage != null)
			{
				if (!overrideTheme && theme != null)
					ApplySprite(handleImage, 0, theme.scrollbarHandlePixelsPerUnitMultiplier, theme.scrollbarHandleColor, ImageType.Sliced, theme.scrollbarHandleImage);
				else
					ApplySprite(handleImage, handleImageIndex, handlePixelsPerUnitMultiplier, handleColor);
			}

			if (rect != null)
			{
				if (!overrideTheme && theme != null)
					ApplyTransformVerticalAnchor(rect, VerticalPosition.Right, theme.scrollbarWidth, theme.scrollbarPositionX, theme.scrollbarVerticalPadding, theme.scrollbarVerticalPadding);
				else
					ApplyTransformVerticalAnchor(rect, VerticalPosition.Right, width, positionX, verticalPadding, verticalPadding);
			}
		}
#endif
	}
}