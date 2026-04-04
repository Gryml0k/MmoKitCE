//statemachine logic from Surge Framework by PixelPlacement

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
#endif

namespace DenariiGames.ThemeUI
{
	public class ThemeUIPanel : ThemeUIComponent
	{
#if UNITY_EDITOR
		[SerializeField] private bool overrideTheme;
		[SerializeField] private bool automaticPositioning = true;
		[SerializeField] private VerticalPosition position = VerticalPosition.Left;

		[SerializeField] [Range(-100,100)] private int topPadding = 20;
		[SerializeField] [Range(-100,100)] private int horizontalPadding = 20;
		[SerializeField] [Range(-100,100)] private int bottomPadding = 20;
		[SerializeField] private int backgroundImageIndex = 0;
		[SerializeField] private float pixelsPerUnitMultiplier = 0.5f;
		[SerializeField] private Color backgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f);

		private RectTransform panelRect;
		private Image backgroundImage;
		private RectTransform contentRect;

		public override void RefreshFromTheme()
		{
			base.RefreshFromTheme();
			ApplyStyles();
		}

		private void OnValidate()
		{
			if (panelRect == null)
				panelRect = GetComponent<RectTransform>();
			if (backgroundImage == null)
				backgroundImage = GetComponentInChildren<Image>();
			if (contentRect == null)
				contentRect = GetComponentInChildren<VerticalLayoutGroup>().transform.GetComponent<RectTransform>();
			EditorApplication.delayCall += ApplyStyles;
		}

		private void ApplyStyles()
		{
			EditorApplication.delayCall -= ApplyStyles;
			ThemeUITheme theme = ThemeUIThemeManager.CurrentTheme;

			if (backgroundImage != null)
			{
				if (!overrideTheme && theme != null)
					ApplySprite(backgroundImage, 0, theme.panelPixelsPerUnitMultiplier, theme.panelBackgroundColor, ImageType.Sliced, theme.panelBackgroundImage);
				else
					ApplySprite(backgroundImage, backgroundImageIndex, pixelsPerUnitMultiplier, backgroundColor);
			}

			if (contentRect != null)
			{
				if (!overrideTheme && theme != null)
					ApplyTransformStretchPadding(contentRect, theme.panelTopPadding, theme.panelHorizontalPadding, theme.panelBottomPadding);
				else
					ApplyTransformStretchPadding(contentRect, topPadding, horizontalPadding, bottomPadding);
			}

			if (panelRect != null && theme != null && !automaticPositioning)
			{
				if (position == VerticalPosition.Center)
					ApplyTransformVerticalAnchor(panelRect, position, theme.centerPanelWidth, theme.centerPanelHeight, theme.centerPanelTopOffset, 0);
				else
					ApplyTransformVerticalAnchor(panelRect, position, theme.sidePanelWidth, theme.sidePanelOffset, theme.sidePanelTopOffset, theme.sidePanelBottomOffset);
			}
		}
#endif

		/// <summary>
		/// Gets a value indicating whether this instance is the first state in this state machine.
		/// </summary>
		public bool IsFirst
		{
			get
			{
				return transform.GetSiblingIndex () == 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is the last state in this state machine.
		/// </summary>
		public bool IsLast
		{
			get
			{
				return transform.GetSiblingIndex () == transform.parent.childCount - 1;
			}
		}

		/// <summary>
		/// Gets or sets the state machine.
		/// </summary>
		public ThemeUIPanelGroup StateMachine
		{
			get
			{
				if (_stateMachine == null)
				{
					_stateMachine = transform.parent.GetComponent<ThemeUIPanelGroup>();
					if (_stateMachine == null)
						return null;
				}

				return _stateMachine;
			}
		}

		//Private Variables:
		ThemeUIPanelGroup _stateMachine;

		//Public Methods
		/// <summary>
		/// Changes the state.
		/// </summary>
		public void ChangeState(int childIndex)
		{
			StateMachine.ChangeState(childIndex);
		}

		/// <summary>
		/// Changes the state.
		/// </summary>
		public void ChangeState(string state)
		{
			StateMachine.ChangeState (state);
		}

		/// <summary>
		/// DG mod: Change to the next state if possible.
		/// </summary>
		public void NextState(bool exitIfLast = false)
		{
			StateMachine.Next (exitIfLast);
		}

		/// <summary>
		/// DG mod: Change to the previous state if possible.
		/// </summary>
		public void PreviousState(bool exitIfFirst = false)
		{
			StateMachine.Previous (exitIfFirst);
		}

		/// <summary>
		/// Exit the current state.
		/// </summary>
		public void Exit()
		{
			StateMachine.Exit ();
		}
	}
}