using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace DenariiGames.ThemeUI
{
	public static class ThemeUIThemeManager
	{
#if UNITY_EDITOR
		private static ThemeUITheme _currentTheme;

		public static ThemeUITheme CurrentTheme
		{
			get => _currentTheme;
			set
			{
				if (_currentTheme != value)
				{
					_currentTheme = value;
					RefreshAllInScene();
				}
			}
		}

		public static void RefreshAllInScene()
		{
			var stage = UnityEditor.SceneManagement.StageUtility.GetCurrentStageHandle();
			var components = stage.FindComponentsOfType<ThemeUIComponent>();

			Debug.Log($"[ThemeUIThemeManager] refreshing <color=green>{components.Length} components</color> in scene");
			foreach (var comp in components)
			{
				comp.RefreshFromTheme();
			}
		}

		private static Sprite[] _cachedBackgroundSprites;

		public static Sprite[] GetBackgroundSprites()
		{
			if (_cachedBackgroundSprites == null || _cachedBackgroundSprites.Length == 0)
			{
				LoadSharedAssets();
			}
			return _cachedBackgroundSprites;
		}

		private static void LoadSharedAssets()
		{
			_cachedBackgroundSprites = Resources.LoadAll<Sprite>("ThemeUI");

			if (_cachedBackgroundSprites == null || _cachedBackgroundSprites.Length == 0)
			{
				Debug.LogWarning("[ThemeUIThemeManager] No sprites found");
			}
		}

		public static Sprite GetBackgroundSprite(int index, bool wrap = true)
		{
			var sprites = GetBackgroundSprites();
			if (sprites == null || sprites.Length == 0) return null;

			if (wrap)
			{
				index = (index % sprites.Length + sprites.Length) % sprites.Length;
			}
			else
			{
				index = Mathf.Clamp(index, 0, sprites.Length - 1);
			}

			return sprites[index];
		}
#endif
	}
}