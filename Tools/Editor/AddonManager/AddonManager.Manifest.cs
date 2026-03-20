/**
 * AddonManager.Manifest
 * Author: Denarii Games
 * Version: 1.0-rc1
 *
 * Manifest related functionality.
 */

using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MmoKtCE.AddonManager
{
	public partial class AddonManagerWindow
	{
		[System.Serializable]
		private class ManifestWrapper
		{
			public List<string> gitUrls;
		}

		/// <summary>
		/// Main entry point retrieves json array of Github urls from central server.
		/// </summary>
		private bool isManifestLoaded = false;
		private string uiManifestError = null;
		private int packagesPendingDetails = 0;
		private bool _manifestLock = false;
		private void LoadAddonsManifest()
		{
			if (_manifestLock) return;

			_manifestLock = true;
			isManifestLoaded = false;
			uiManifestError = null;
			packages.Clear();

			UnityWebRequest www = UnityWebRequest.Get(PACKAGE_MANIFEST_URL);
			var asyncOp = www.SendWebRequest();

			EditorApplication.CallbackFunction callback = null;
			callback = () =>
			{
				if (asyncOp.isDone)
				{
					EditorApplication.update -= callback;

					if (www.result == UnityWebRequest.Result.Success)
					{
						try
						{
							string jsonText = www.downloadHandler.text.Trim();

							//wrap array in object for JsonUtility
							var manifestWrapper = JsonUtility.FromJson<ManifestWrapper>("{\"gitUrls\":" + jsonText + "}");

							if (manifestWrapper?.gitUrls == null || manifestWrapper.gitUrls.Count == 0)
							{
								uiManifestError = "Manifest loaded but contained no git URLs.";
							}
							else
							{
								packagesPendingDetails = 0;
								foreach (string gitUrl in manifestWrapper.gitUrls)
								{
									PackageInfo pkg = new PackageInfo
									{
										gitUrl = gitUrl
									};

									packagesPendingDetails++;
									FetchPackageDetails(pkg);
									packages.Add(pkg);
								}

								if (packages.Count == 0)
								{
									uiManifestError = "Manifest loaded but contained no URLs.";
								}
							}
						}
						catch (System.Exception e)
						{
							uiManifestError = $"Failed to parse manifest: {e.Message}";
							Debug.LogError($"[AddonManager {Time.time}] manifest error: {e}");
						}
					}
					else
					{
						uiManifestError = $"Failed to download: {www.error}";
					}

					_manifestLock = false;
					Repaint();
				}
			};

			EditorApplication.update += callback;
		}

		/// <summary>
		/// Gets the package.json for each manifest entry.
		/// </summary>
		private Dictionary<string, UnityWebRequest> activeRequests = new Dictionary<string, UnityWebRequest>();    
		private void FetchPackageDetails(PackageInfo pkg)
		{
			if (string.IsNullOrEmpty(pkg.gitUrl)) return;

			pkg.latestVersion = "Fetching...";
			pkg.status = PackageStatus.Fetching;

			if (activeRequests.ContainsKey(pkg.gitUrl)) return;

			UnityWebRequest www = UnityWebRequest.Get(pkg.gitUrl);
			var asyncOp = www.SendWebRequest();
			activeRequests[pkg.gitUrl] = www;

			// Define the callback as a local variable so we can unsubscribe properly
			EditorApplication.CallbackFunction updateCallback = null;

			updateCallback = () =>
			{
				if (asyncOp.isDone)
				{
					EditorApplication.update -= updateCallback; // Correctly remove the exact delegate

					if (www.result == UnityWebRequest.Result.Success)
					{
						try
						{
							string jsonText = www.downloadHandler.text;
							JObject json = JObject.Parse(jsonText);

							pkg.name = json["name"]?.ToString();
							pkg.guid = json["guid"]?.ToString();
							pkg.latestVersion = json["version"]?.ToString();
							pkg.updateDate = json["updateDate"]?.ToString();

							pkg.isCore = string.Equals(json["isCore"]?.ToString(), "true");

							//@todo check if file actually exists in repo
							pkg.packageUrl = json["packageUrl"]?.ToString();

							string rawCategory = json["category"]?.ToString();
							pkg.category = Constants.IsCategoryAllowed(rawCategory) ? rawCategory : "Uncategorized";

							pkg.description = json["description"]?.ToString() ?? "No description provided.";

							string authorName = null;
							if (json["author"] is JValue jValue && jValue.Type == JTokenType.String)
								authorName = jValue.ToString();
							else if (json["author"] is JObject authorObj)
								authorName = authorObj["name"]?.ToString();
							pkg.author = string.IsNullOrWhiteSpace(authorName) ? null : authorName.Trim();

							string screenshotName = json["screenshot"]?.ToString();
							if (!string.IsNullOrEmpty(screenshotName))
							{
								pkg.screenshotUrl = pkg.gitUrl.Replace("package.json", screenshotName);
							}

							//check mandatory properties
							if (string.IsNullOrEmpty(pkg.name) || string.IsNullOrEmpty(pkg.guid) || string.IsNullOrEmpty(pkg.packageUrl) || string.IsNullOrEmpty(pkg.latestVersion) || string.IsNullOrEmpty(pkg.updateDate) || string.IsNullOrEmpty(pkg.author))
							{
								Debug.LogError($"[AddonManager {Time.time}] invalid package: {pkg.gitUrl}");
								pkg.status = PackageStatus.Unknown;
							}
						}
						catch
						{
							Debug.LogError($"[AddonManager {Time.time}] error parsing package: {pkg.gitUrl}");
							pkg.status = PackageStatus.Unknown;
						}
					}
					else
					{
						Debug.LogError($"[AddonManager {Time.time}] failed to fetch package: {pkg.gitUrl}");
						pkg.status = PackageStatus.Unknown;
					}

					activeRequests.Remove(pkg.gitUrl);

					packagesPendingDetails--;

					//are all packages loaded?
					if (packagesPendingDetails == 0)
					{
						isManifestLoaded = true;
						GetInstalledAddons();
					}
				}
			};

			EditorApplication.update += updateCallback;
		}
	}
}
