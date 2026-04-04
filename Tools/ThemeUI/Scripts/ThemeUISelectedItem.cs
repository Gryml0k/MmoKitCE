using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUISelectedItem : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;

		[SerializeField] private int imageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color color = new Color(0f, 0f, 0f, 0f);

		private Image image;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (image == null)
				image = GetComponent<Image>();
			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			if (image != null)
			{
				if (!overrideTheme && theme != null)
					ApplySprite(image, 0, theme.selectedPixelsPerUnitMultiplier, theme.selectedColor, ImageType.Sliced, theme.selectedImage);
				else
					ApplySprite(image, imageIndex, pixelsPerUnitMultiplier, color);
			}
		}
#endif
	}
}