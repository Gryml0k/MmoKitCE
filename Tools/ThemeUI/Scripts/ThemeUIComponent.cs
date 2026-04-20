using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DenariiGames.ThemeUI
{
	public enum ImageType
	{
		Simple,
		Sliced,
		Tiled,
		Filled
	}

	public enum VerticalPosition
	{
		Left,
		Center,
		Right
	}

	public enum HorizontalPosition
	{
		Top,
		Bottom
	}

	public enum GridSize
	{
		Square,
		SmallSquare,
		Card
	}

	public enum TextStyle
	{
		Default,
		Large,
	}

	public enum ButtonStyle
	{
		Default,
		Primary,
		Thin,
		Close,
		Increase,
	}


	public class ThemeUIComponent : MonoBehaviour
	{
#if UNITY_EDITOR
		public virtual void RefreshFromTheme()
		{
		}

		/// <summary>
		/// Set width and weight of RectTransform , e.g. button
		/// </summary>
		public void ApplyTransformWidthHeight(RectTransform obj, int width, int height)
		{
			if (obj == null)
				return;

			Vector2 newSize = obj.sizeDelta;
			newSize.x = width;
			newSize.y = height;
			obj.sizeDelta = newSize;
			LayoutRebuilder.ForceRebuildLayoutImmediate(obj);
		}

		/// <summary>
		/// Set padding on stretched RectTransform, e.g. panel or container
		/// </summary>
		public void ApplyTransformStretchPadding(RectTransform obj, int topPadding, int horizontalPadding, int bottomPadding)
		{
			if (obj == null)
				return;

			obj.offsetMin = new Vector2(
				horizontalPadding,
				bottomPadding
			);
			obj.offsetMax = new Vector2(
				-horizontalPadding,
				-topPadding
			);
			LayoutRebuilder.MarkLayoutForRebuild(obj);
		}

		/// <summary>
		/// Set padding on stretched RectTransform, e.g. panel or container
		/// </summary>
		public void ApplyTransformStretchPaddingLeft(RectTransform obj, int topPadding, int leftPadding, int bottomPadding)
		{
			if (obj == null)
				return;

			obj.offsetMin = new Vector2(
				leftPadding,
				bottomPadding
			);
			obj.offsetMax = new Vector2(
				0,
				-topPadding
			);
			LayoutRebuilder.MarkLayoutForRebuild(obj);
		}

		/// <summary>
		/// Set width, PosX and top and bottom padding on left or right RectTransform, e.g. scrollbar
		/// or width and height (PosX) with top padding on center RectTransform, e.g. dialog
		/// </summary>
		public void ApplyTransformVerticalAnchor(RectTransform obj, VerticalPosition position, int width, int positionX, int topOffset, int bottomOffset)
		{
			if (obj == null)
				return;

			switch (position)
			{
				case VerticalPosition.Left:
					obj.anchorMin = new Vector2(0f, 0f);
					obj.anchorMax = new Vector2(0f, 1f);
					obj.pivot     = new Vector2(0f, 0.5f);

					obj.sizeDelta = new Vector2(width, obj.sizeDelta.y);
					obj.anchoredPosition = new Vector2(positionX, obj.anchoredPosition.y);
					obj.offsetMin     = new Vector2(obj.offsetMin.x, bottomOffset);
					obj.offsetMax     = new Vector2(obj.offsetMax.x, -topOffset);
					break;

				case VerticalPosition.Right:
					obj.anchorMin = new Vector2(1f, 0f);
					obj.anchorMax = new Vector2(1f, 1f);
					obj.pivot     = new Vector2(1f, 0.5f);

					obj.sizeDelta     = new Vector2(width, obj.sizeDelta.y);
					obj.anchoredPosition = new Vector2(-positionX, obj.anchoredPosition.y);
					obj.offsetMin     = new Vector2(obj.offsetMin.x, bottomOffset);
					obj.offsetMax     = new Vector2(obj.offsetMax.x, -topOffset);
					break;

				case VerticalPosition.Center:
					obj.anchorMin = new Vector2(0.5f, 0.5f);
					obj.anchorMax = new Vector2(0.5f, 0.5f);
					obj.pivot     = new Vector2(0.5f, 0.5f);

					obj.anchoredPosition = new Vector2(0f, -topOffset);
					obj.sizeDelta = new Vector2(width, positionX);
					break;
			}
			LayoutRebuilder.MarkLayoutForRebuild(obj);
		}

		/// <summary>
		/// Set anchor on RectTransform, e.g. menubar
		/// </summary>
		public void ApplyTransformHorizontalAnchor(RectTransform obj, HorizontalPosition position, int height)
		{
			if (obj == null)
				return;

			switch (position)
			{
				case HorizontalPosition.Top:
					obj.anchorMin = new Vector2(0f, 1f);
					obj.anchorMax = new Vector2(1f, 1f);
					obj.pivot     = new Vector2(0.5f, 1f);
					obj.sizeDelta = new Vector2(obj.sizeDelta.x, height);
					break;

				case HorizontalPosition.Bottom:
					obj.anchorMin = new Vector2(0f, 0f);
					obj.anchorMax = new Vector2(1f, 0f);
					obj.pivot     = new Vector2(0.5f, 0f);
					obj.sizeDelta = new Vector2(obj.sizeDelta.x, height);
					break;
			}
			LayoutRebuilder.MarkLayoutForRebuild(obj);
		}

		public void ApplyFont(TMP_Text tmpText, int fontSize, Color32 fontColor, bool center = false)
		{
			if (Mathf.Abs(tmpText.fontSize - (float)fontSize) > 0.01f)
				tmpText.fontSize = fontSize;
			tmpText.color = fontColor;
			tmpText.alignment = center ? TMPro.TextAlignmentOptions.Center : TMPro.TextAlignmentOptions.Left;
			tmpText.ForceMeshUpdate();
		}

		public void ApplyFontAsset(TMP_Text tmpText, TMP_FontAsset fontAsset)
		{
			tmpText.font = fontAsset;
			tmpText.ForceMeshUpdate();
		}

		public void ApplySprite(Image image, int imageIndex, float pixelsPerUnitMultiplier, Color backgroundColor, ImageType imageType = ImageType.Sliced, Sprite sprite = null)
		{
			if (image == null)
				return;

			if (sprite == null)
				sprite = ThemeUIThemeManager.GetBackgroundSprite(imageIndex);

			image.sprite = sprite;
			switch (imageType)
			{
				case ImageType.Simple:
					image.type = Image.Type.Simple;
					break;
				case ImageType.Sliced:
					image.type = Image.Type.Sliced;
					break;
				case ImageType.Tiled:
					image.type = Image.Type.Tiled;
					break;
				case ImageType.Filled:
					image.type = Image.Type.Filled;
					break;
			}
			image.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
			image.color = backgroundColor;
		}
#endif
	}
}