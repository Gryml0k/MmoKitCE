using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DenariiGames.ThemeUI
{
	[CreateAssetMenu(fileName = "ThemeUITheme", menuName = "MmoKitCE/Create Theme", order = -1000)]
	public partial class ThemeUITheme : ScriptableObject
	{
		[Header("Text")]
		[Range(10, 120)] public int fontSize = 30;
		public Color32 fontColor = new Color32(239, 239, 239, 255);
    	public TMP_FontAsset fontAsset;

		[Space(20)]
		[Header("Title")]
		[Range(10, 200)] public int titleHeight = 50;
		public Sprite titleBackgroundImage;
		public float titlePixelsPerUnitMultiplier = 0.5f;
		public Color titleBackgroundColor = new Color(0f, 0f, 0f, 0f);
		[Header("Title Font")]
		[Range(-100, 100)] public int titleFontMarginTop = 0;
		[Range(10, 120)] public int titleFontSize = 36;
		public Color32 titleFontColor = new Color32(255, 255, 255, 255);
    	public TMP_FontAsset titleFontAsset;

		[Space(20)]
		[Header("Item Title")]
		[Range(10, 200)] public int itemTitleHeight = 40;
		[Range(10, 120)] public int itemTitleFontSize = 30;
		public Sprite itemTitleBackgroundImage;
		public float itemTitlePixelsPerUnitMultiplier = 0.5f;
		public Color itemTitleBackgroundColor = new Color(0f, 0f, 0f, 0f);
		[Header("Item Title Font")]
		public Color32 itemTitleFontColor = new Color32(255, 255, 255, 255);
    	public TMP_FontAsset itemTitleFontAsset;

		[Space(20)]
		[Header("Selected Item")]
		public Sprite selectedImage;
		public float selectedPixelsPerUnitMultiplier = 1f;
		public Color selectedColor = new Color(1f, 0.93f, 0f, 0.5f);

		[Space(20)]
		[Header("Buttons")]
		[Range(50, 500)] public int buttonWidth = 200;
		[Range(25, 100)] public int buttonHeight = 60;
		[Range(25, 100)] public int buttonThinHeight = 54;
		[Range(10, 120)] public int buttonFontSize = 36;
		public Color32 buttonFontColor = new Color32(255, 255, 255, 255);
		public Sprite buttonBackgroundImage;
		public float buttonPixelsPerUnitMultiplier = 0.5f;
		public Color buttonBackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f);
		[Header("Primary Button")]
		public Color32 buttonPrimaryFontColor = new Color32(255, 255, 255, 255);
		public Sprite buttonPrimaryImage;
		public float buttonPrimaryPixelsPerUnitMultiplier = 0.5f;
		public Color buttonPrimaryColor = new Color(0.25f, 0.25f, 0.25f, 1f);
		[Header("Close Button")]
		[Range(20, 200)] public int buttonCloseWidth = 60;
		[Range(20, 200)] public int buttonCloseHeight = 60;
		public Color32 buttonCloseFontColor = new Color32(255, 255, 255, 255);
		public Sprite buttonCloseImage;
		public float buttonClosePixelsPerUnitMultiplier = 0.5f;
		public Color buttonCloseColor = new Color(0.25f, 0.25f, 0.25f, 1f);

		[Space(20)]
		[Header("Panels")]
		[Range(-100,100)] public int panelTopPadding = 20;
		[Range(-100,100)] public int panelHorizontalPadding = 20;
		[Range(-100,100)] public int panelBottomPadding = 20;
		public Sprite panelBackgroundImage;
		public float panelPixelsPerUnitMultiplier = 0.5f;
		public Color panelBackgroundColor = new Color(0f, 0f, 0f, 1f);

		[Space(20)]
		[Header("Panel Positions")]
		[Range(0,5000)] public int sidePanelWidth = 700;
		[Range(0,200)] public int sidePanelOffset = 50;
		[Range(0,200)] public int sidePanelTopOffset = 130;
		[Range(0,200)] public int sidePanelBottomOffset = 50;
		[Header("Center Panel")]
		[Range(0,5000)] public int centerPanelWidth = 1000;
		[Range(0,5000)] public int centerPanelHeight = 750;
		[Range(0,200)] public int centerPanelTopOffset = 40;

		[Space(20)]
		[Header("Containers")]
		[Range(0,100)] public int containerTopPadding = 20;
		[Range(0,100)] public int containerHorizontalPadding = 20;
		[Range(0,100)] public int containerBottomPadding = 20;
		[Range(0,100)] public int containerSpacing = 20;
		[Header("Container Background")]
		public Sprite containerBackgroundImage;
		public float containerPixelsPerUnitMultiplier = 0.5f;
		public Color containerBackgroundColor = new Color(0f, 0f, 0f, 0f);

		[Space(20)]
		[Header("List Containers")]
		[Range(0,100)] public int listSpacing = 0;

		[Space(20)]
		[Header("Grid Containers")]
		[Range(0,500)] public int gridCellWidth = 150;
		[Range(0,500)] public int gridCellHeight = 150;
		[Range(0,100)] public int gridSpacingX = 0;
		[Range(0,100)] public int gridSpacingY = 0;
		[Header("Grid Small")]
		[Range(0,500)] public int gridSmallWidth = 100;
		[Range(0,500)] public int gridSmallHeight = 100;
		[Header("Grid Cards")]
		[Range(0,500)] public int gridCardWidth = 250;
		[Range(0,500)] public int gridCardHeight = 325;

		[Space(20)]
		[Header("Scrollbars")]
		[Range(0,100)] public int scrollbarWidth = 20;
		[Range(-100,100)] public int scrollbarPositionX = 0;
		[Range(0,100)] public int scrollbarVerticalPadding = 0;
		public Sprite scrollbarImage;
		public float scrollbarPixelsPerUnitMultiplier = 0.5f;
		public Color scrollbarColor = new Color(0f, 0f, 0f, 1f);
		[Header("Scrollbar Handle")]
		public Sprite scrollbarHandleImage;
		public float scrollbarHandlePixelsPerUnitMultiplier = 0.5f;
		public Color scrollbarHandleColor = new Color(1f, 1f, 1f, 1f);

		[Space(20)]
		[Header("Dividers")]
		[Range(0,500)] public int dividerHeight = 50;
		[Range(0,500)] public int dividerImageHeight = 50;
		[Range(0,500)] public int dividerImageWidth = 300;
		public ImageType dividerImageType = ImageType.Sliced;
		public Sprite dividerImage;
		public float dividerPixelsPerUnitMultiplier = 1f;
		public Color dividerColor = new Color(0f, 0f, 0f, 0f);

		[Space(20)]
		[Header("Input Field")]
		[Range(0,100)] public int inputFieldHeight = 45;
		public Sprite inputFieldImage;
		public float inputFieldPixelsPerUnitMultiplier = 0.5f;
		public Color inputFieldColor = new Color(1f, 1f, 1f, 1f);

		[Space(20)]
		[Header("Toggle Field")]
		[Range(0,100)] public int toggleHeight = 40;
		[Range(0,100)] public int toggleWidth = 40;
		[Range(0,100)] public int toggleLabelPadding = 10;
		public Sprite toggleImage;
		public float togglePixelsPerUnitMultiplier = 1f;
		public Color toggleColor = new Color(1f, 1f, 1f, 1f);
	}
}
