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
		[SerializeField] private TextStyle style = TextStyle.Default;

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

			if (tmpText != null)
			{
				if (!overrideTheme && theme != null)
				{
					ApplyFont(tmpText, style == TextStyle.Large ? theme.fontLargeSize : theme.fontSize, theme.fontColor);
					ApplyFontAsset(tmpText, style == TextStyle.Large ? theme.fontLargeAsset : theme.fontAsset);
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