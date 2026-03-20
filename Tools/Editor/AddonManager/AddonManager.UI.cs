/**
 * AddonManager.UI
 * Author: Denarii Games
 * Version: 1.0-rc1
 *
 * UI related functionality.
 */

using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;

namespace MmoKtCE.AddonManager
{
    public partial class AddonManagerWindow
    {
		/// <summary>
		/// Main entry point to UI.
		/// </summary>
		private Vector2 scrollPosition;
        private Vector2 detailScrollPosition;
		private void OnGUI()
		{
			if (AddonAnalytics.ShowWelcome)
			{
				WelcomeDialog();
				return;
			}

			if (uiManifestError != null)
			{
				DrawCenteredBox(() =>
				{
					EditorGUILayout.HelpBox($"Error loading addons list: {uiManifestError}", MessageType.Error);
					GUILayout.Space(20);
					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Retry", GUILayout.Width(150), GUILayout.Height(30)))
					{
						LoadAddonsManifest();
					}
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
				});
				return;
			}

			if (!isManifestLoaded)
			{
				DrawCenteredBox(() =>
				{
					EditorGUILayout.BeginVertical();
					if (logoIcon != null)
					{
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(logoIcon, GUILayout.Width(64), GUILayout.Height(64));
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
					}
					GUILayout.Label("Loading addons list...");
					EditorGUILayout.EndVertical();
				});
				return;
			}

			if (packages.Count == 0)
			{
				DrawCenteredBox(() =>
				{
					EditorGUILayout.HelpBox("No addons available. Check your internet connection.", MessageType.Error);
				});
				return;
			}

			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				// --- Category Filter ---

				if (GUILayout.Button(currentCategoryFilter, EditorStyles.toolbarDropDown, GUILayout.Width(120)))
				{
					GenericMenu menu = new GenericMenu();

					// Always include "All Categories"
					menu.AddItem(new GUIContent("All Categories"), currentCategoryFilter == "All Categories", () =>
					{
						currentCategoryFilter = "All Categories";
						Repaint();
					});

					// Use cached list — rebuilt only when manifest loads
					foreach (string cat in Constants.Categories)
					{
						string catCapture = cat; // Capture for closure
						menu.AddItem(new GUIContent(cat), currentCategoryFilter == cat, () =>
						{
							currentCategoryFilter = catCapture;
							Repaint();
						});
					}

					menu.DropDown(GUILayoutUtility.GetLastRect());
				}

				// --- Core Filter ---
				if (GUILayout.Button(currentCoreFilter, EditorStyles.toolbarDropDown, GUILayout.Width(120)))
				{
					GenericMenu menu = new GenericMenu();

					string[] options = { "All Publishers", "Core", "Community" };
					foreach (string option in options)
					{
						string capture = option;
						menu.AddItem(new GUIContent(option), currentCoreFilter == option, () =>
						{
							currentCoreFilter = capture;
							Repaint();
						});
					}

					menu.DropDown(GUILayoutUtility.GetLastRect());
				}

				// --- Update Filter ---
				if (GUILayout.Button(currentUpdatedFilter, EditorStyles.toolbarDropDown, GUILayout.Width(100)))
				{
					GenericMenu menu = new GenericMenu();

					string[] options = { "Anytime", "This week", "This month" };
					foreach (string option in options)
					{
						string capture = option;
						menu.AddItem(new GUIContent(option), currentUpdatedFilter == option, () =>
						{
							currentUpdatedFilter = capture;
							Repaint();
						});
					}

					menu.DropDown(GUILayoutUtility.GetLastRect());
				}

				// --- Status Filter ---
				if (GUILayout.Button(currentStatusFilter, EditorStyles.toolbarDropDown, GUILayout.Width(140)))
				{
					GenericMenu menu = new GenericMenu();

					menu.AddItem(new GUIContent("All Addons"), currentStatusFilter == "All Addons", () =>
					{
						currentStatusFilter = "All Addons";
						Repaint();
					});

					menu.AddItem(new GUIContent("Installed"), currentStatusFilter == "Installed", () =>
					{
						currentStatusFilter = "Installed";
						Repaint();
					});

					menu.AddItem(new GUIContent("Update Available"), currentStatusFilter == "Update Available", () =>
					{
						currentStatusFilter = "Update Available";
						Repaint();
					});

					menu.AddItem(new GUIContent("Not Installed"), currentStatusFilter == "Not Installed", () =>
					{
						currentStatusFilter = "Not Installed";
						Repaint();
					});

					menu.DropDown(GUILayoutUtility.GetLastRect());
				}

				int visibleCount = GetFilteredPackages().Count;
				GUILayout.Label($"{visibleCount} addon{(visibleCount == 1 ? "" : "s")}", GUILayout.Width(80));

				GUILayout.FlexibleSpace();
				if (reloadIcon != null)
				{
					if (GUILayout.Button(reloadIcon, EditorStyles.toolbarButton))
					{
						LoadAddonsManifest();
					}
				}

			}
			GUILayout.EndHorizontal();


			// === Package List ===
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			List<PackageInfo> filteredPackages = GetFilteredPackages();

			foreach (var pkg in filteredPackages)
			{
				EditorGUILayout.BeginHorizontal();

				// --- Clickable entire row ---
				Rect rowRect = EditorGUILayout.BeginHorizontal("Button"); // "Button" style gives hover effect
				{
					if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
					{
						selectedPackage = pkg;
						uiDetailMessage = "";
						FetchScreenshot(pkg); // Fetch screenshot if not loaded
						Repaint();
					}

					GUILayout.Space(10);

					// Core
					if (pkg.isCore)
					{
						if (coreBadge != null)
						{
							GUILayout.Label(coreBadge, GUILayout.Width(16), GUILayout.Height(16));
						}
					}

					// --- "New" Badge (if updated this week) ---
					if (IsNewThisWeek(pkg.updateDate))
					{
						if (newBadge != null)
						{
							GUILayout.Label(newBadge, GUILayout.Width(16), GUILayout.Height(16));
						}
					}

					// --- Package Name (expands to fill space) ---
					EditorGUILayout.LabelField(pkg.name, EditorStyles.largeLabel, GUILayout.ExpandWidth(true));

					// --- Version (fixed 50px, right-aligned) ---
					string versionDisplay = pkg.latestVersion == "Fetching..." ? "—" : pkg.latestVersion;

					GUIStyle versionStyle = new GUIStyle(EditorStyles.miniLabel)
					{
						alignment = TextAnchor.MiddleRight,
						fontSize = 11
					};

					EditorGUILayout.LabelField(versionDisplay, versionStyle, GUILayout.Width(50));

					// --- Status Icon (16x16) ---
					Texture2D statusIcon = null;
					if (pkg.status == PackageStatus.UpToDate)
						statusIcon = checkIcon;
					else if (pkg.status == PackageStatus.Outdated)
						statusIcon = updateIcon;

					if (statusIcon != null)
					{
						GUILayout.Label(statusIcon, GUILayout.Width(16), GUILayout.Height(16));
					}
					else
					{
						// Reserve space so alignment stays consistent
						GUILayout.Space(18);
					}

					GUILayout.Space(10);
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndScrollView();

			EditorGUILayout.Space(20);

			if (selectedPackage != null)
				AddonDetailView();
			else
				EditorGUILayout.HelpBox("Select a package to view details.", MessageType.Info);
		}

		/// <summary>
		/// Detail view for selected addon
		/// </summary>
		private string uiDetailMessage = "";
		private void AddonDetailView()
		{
			EditorGUILayout.BeginVertical("box");

			// Use scroll view for detail with fixed height
			detailScrollPosition = EditorGUILayout.BeginScrollView(detailScrollPosition, GUILayout.Height(380));

			// Installation feedback
			if (!string.IsNullOrEmpty(uiDetailMessage))
			{
				MessageType msgType = uiDetailMessage.ToLower().Contains("error") ? MessageType.Error : MessageType.None;
				EditorGUILayout.HelpBox(uiDetailMessage, msgType);
			}

			// Package Name
			GUILayout.Label(selectedPackage.name, HeaderStyle);

			// Category
			GUILayout.Label($"{selectedPackage.category}");

			// Version
			string latestColor = selectedPackage.status == PackageStatus.UpToDate ? "#05714B" : 
								 selectedPackage.status == PackageStatus.Outdated ? "#DD2235" : 
								 EditorGUIUtility.isProSkin ? "#E5E5E5" : "#212121";
			GUILayout.Label($"<color={latestColor}>{selectedPackage.latestVersion}</color> • {selectedPackage.updateDate}", new GUIStyle(EditorStyles.label) { richText = true });

			// Author
			GUILayout.Label($"by {selectedPackage.author}");

			// Guid (for testing)
			//GUILayout.Label($"GUID: {selectedPackage.guid}");

			// Link to GitHub Repo
			Rect linkRect = EditorGUILayout.GetControlRect(false, 20);
			EditorGUI.LabelField(linkRect, $"<u>Github</u>", LinkStyle);

			// Detect click on the link
			if (Event.current.type == EventType.MouseDown && linkRect.Contains(Event.current.mousePosition))
			{
				Application.OpenURL(GetRepositoryBaseUrl(selectedPackage.gitUrl));
			}

			EditorGUILayout.Space(10);

			GUILayout.BeginHorizontal();
			{
				// Install / Update Button
				GUI.enabled = !AddonInstallState.HasPending;

				string buttonLabel = selectedPackage.status switch
				{
					PackageStatus.NotInstalled => "Install",
					PackageStatus.Outdated     => "Update",
					_                          => "Reinstall"
				};

				if (GUILayout.Button(buttonLabel, GUILayout.Width(150), GUILayout.Height(24)))
				{
					InstallOrUpdatePackage();
				}

				if (selectedPackage.status != PackageStatus.NotInstalled)
				{
					if (GUILayout.Button("Delete", GUILayout.Width(150), GUILayout.Height(24)))
					{
						DeleteExistingAddonFolder();
					}
				}

				GUI.enabled = true;
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.Space(15);

			// Featured Image (if available, max 240px high)
			if (selectedPackage.screenshot != null)
			{
				GUILayout.Label(selectedPackage.screenshot, GUILayout.MaxHeight(240));
				EditorGUILayout.Space(10);
			}

			// Description
			// Convert \n to actual newlines for proper line breaks
			string formattedDescription = selectedPackage.description
				.Replace("\\n\\n", "\n\n")  // Double newline for paragraph spacing
				.Replace("\\n", "\n");      // Single newline

			GUILayout.Label(formattedDescription, ParagraphStyle);

			GUILayout.Space(8);
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		/// <summary>
		/// Draws welcome dialog to collect analytics consent
		/// </summary>
	    private bool analyticsToggle;
		private void WelcomeDialog()
		{
			DrawCenteredBox(() =>
			{
				EditorGUILayout.BeginVertical();
				if (logoIcon != null)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label(logoIcon, GUILayout.Width(64), GUILayout.Height(64));
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
				}
				GUILayout.Label("Welcome to Addon Manager", HeaderStyle);

				string welcomeMessage = 
					"MmoKitCE Addon Manager collects completely anonymous usage statistics (addon downloads) to highlight the most popular addons in the community.\n\n" +
					"No personal or project data is ever collected or transmitted.\n\n" +
					"Help the community discover the best addons!";
				welcomeMessage.Replace("\\n\\n", "\n\n");
				GUILayout.Label(welcomeMessage, ParagraphStyle);

				GUILayout.Space(10);

				EditorGUILayout.BeginHorizontal();
				bool newToggle = EditorGUILayout.Toggle(analyticsToggle, GUILayout.Width(20));
				EditorGUILayout.LabelField(
					"Help improve MmoKitCE Addon Manager by sending anonymous usage data",
					EditorStyles.wordWrappedLabel,
					GUILayout.ExpandWidth(true)
				);
				EditorGUILayout.EndHorizontal();

				if (newToggle != analyticsToggle)
				{
					analyticsToggle = newToggle;
					AddonAnalytics.SetConsent(newToggle);
				}

				GUILayout.Space(20);

				if (GUILayout.Button("Continue", GUILayout.Height(30)))
				{
					EditorPrefs.SetBool(AddonAnalytics.WelcomeScreen, true);
					Repaint();
				}

				EditorGUILayout.EndVertical();
			}, 350f, 350f);
		}

		/// <summary>
		/// Utility to convert uri to package.json to clickable url
		/// </summary>
		/// <param name="fullUrl"></param>
		/// <returns></returns>
		public static string GetRepositoryBaseUrl(string fullUrl)
		{
			Uri uri = new Uri(fullUrl);

			// Get everything up to and including the repo name
			string path = uri.AbsolutePath;

			// Split the path and take only the owner and repo parts
			string[] parts = path.Trim('/').Split('/');

			if (parts.Length >= 2)
			{
				string owner = parts[0];
				string repo = parts[1];

				// Reconstruct the base URL
				return $"https://github.com/{owner}/{repo}";
			}

			// Fallback (shouldn't happen with valid GitHub URLs)
			return fullUrl;
		}

		/// <summary>
		/// Caches pkg.screenshot from pkg.screenshotUrl
		/// </summary>
		/// <param name="pkg"></param>
		private void FetchScreenshot(PackageInfo pkg)
		{
			if (pkg.screenshot != null || string.IsNullOrEmpty(pkg.screenshotUrl)) return;

			UnityWebRequest www = UnityWebRequestTexture.GetTexture(pkg.screenshotUrl);
			var asyncOp = www.SendWebRequest();

			EditorApplication.CallbackFunction updateCallback = null;

			updateCallback = () =>
			{
				if (asyncOp.isDone)
				{
					EditorApplication.update -= updateCallback;

					if (www.result == UnityWebRequest.Result.Success)
					{
						pkg.screenshot = ((DownloadHandlerTexture)www.downloadHandler).texture;
					}
					else
					{
						Debug.LogWarning($"[AddonManager {Time.time}] failed to fetch screenshot for {pkg.name}: {www.error}");
					}

					Repaint();
				}
			};

			EditorApplication.update += updateCallback;
		}

		private bool IsNewThisWeek(string updateDateStr)
		{
			var updateDt = ParseUpdateDate(updateDateStr);
			if (!updateDt.HasValue) return false;

			System.DateTime today = System.DateTime.Today;
			int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7; // Monday = 0
			System.DateTime startOfThisWeek = today.AddDays(-daysSinceMonday);

			return updateDt.Value >= startOfThisWeek;
		}

		private void DrawCenteredBox(Action content, float dialogWidth = 350f, float dialogHeight = 180f)
		{
			// Full window rect
			Rect fullRect = new Rect(0, 0, position.width, position.height);

			Rect centeredRect = new Rect(
				(fullRect.width - dialogWidth) / 2,
				(fullRect.height - dialogHeight) / 2,
				dialogWidth,
				dialogHeight
			);

			GUILayout.BeginArea(centeredRect);
			content?.Invoke();
			GUILayout.EndArea();
		}

		private static GUIStyle headerStyle;
		private static GUIStyle HeaderStyle
		{
			get
			{
				if (headerStyle == null)
				{
					headerStyle = new GUIStyle(EditorStyles.label)
					{
						fontSize = 20,
						richText = true
					};
				}

				return headerStyle;
			}
		}

		private static GUIStyle paragraphStyle;
		private static GUIStyle ParagraphStyle
		{
			get
			{
				if (paragraphStyle == null)
				{
					paragraphStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
					{
						fontSize = 13,
						richText = true
					};
				}

				// Update color in case user switched theme
				Color desiredColor = EditorGUIUtility.isProSkin 
					? new Color(0.898f, 0.898f, 0.898f)  // #E5E5E5
					: new Color(0.129f, 0.129f, 0.129f); // #212121

				if (paragraphStyle.normal.textColor != desiredColor)
				{
					paragraphStyle.normal.textColor = desiredColor;
				}

				return paragraphStyle;
			}
		}

		private static GUIStyle linkStyle;
		private static GUIStyle LinkStyle
		{
			get
			{
				if (linkStyle == null)
				{
					linkStyle = new GUIStyle(EditorStyles.label)
					{
						normal = { textColor = new Color(0.3f, 0.6f, 1f, 1f) }, // Classic blue link color
						hover = { textColor = new Color(0.4f, 0.7f, 1f, 1f) },
						fontSize = 12,
						richText = true
					};
				}

				return linkStyle;
			}
		}
	}
}