using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
using TMPro;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIText : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;

		[SerializeField] [Range(10, 120)] private int fontSize = 36;
		[SerializeField] private Color32 fontColor = new Color32(255, 255, 255, 255);
		[SerializeField] private TMP_FontAsset fontAsset;

		private TMP_Text tmpText;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (tmpText == null)
				tmpText = GetComponent<TMP_Text>();

			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			if (this == null)
				return;

			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			ContentSizeFitter fitter = GetComponent<ContentSizeFitter>();
			if (fitter == null)
			{
				fitter = gameObject.AddComponent<ContentSizeFitter>();
				fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			}

			if (tmpText != null)
			{
				if (!overrideTheme && theme != null)
				{
					ApplyFont(tmpText, theme.fontSize, theme.fontColor);
					ApplyFontAsset(tmpText, theme.fontAsset);
				}
				else
				{
					ApplyFont(tmpText, fontSize, fontColor);
					ApplyFontAsset(tmpText, fontAsset);
				}
			}
		}
#endif
	}
}