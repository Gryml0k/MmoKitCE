/**
 * AddonManager.Delete
 * Author: Denarii Games
 * Version: 1.0-rc1
 *
 * Delete related functionality.
 */

using UnityEngine;
using UnityEditor;

namespace MmoKitCE.AddonManager
{
    public partial class AddonManagerWindow
    {
		/// <summary>
		/// Callback from button deletes folder
		/// </summary>
		private bool DeleteExistingAddonFolder()
		{
			uiDetailMessage = "Deleting addon...";

			string[] guids = AssetDatabase.FindAssets(selectedPackage.guid);
			if (guids.Length == 0)
			{
				Debug.Log($"[AddonManager] No existing assets found for {selectedPackage.guid}");
				return false;
			}

		    string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
			if (string.IsNullOrEmpty(assetPath))
			{
				Debug.LogWarning($"[AddonManager] Found {selectedPackage.guid} but got empty path.");
				return false;
			}

			string folderPath = System.IO.Path.GetDirectoryName(assetPath).Replace("\\", "/").TrimEnd('/'); // Unity uses forward slashes
			if (!AssetDatabase.IsValidFolder(folderPath))
			{
				Debug.LogError($"[AddonManager] Derived path is not a valid folder: {folderPath}");
				return false;
			}

			bool success = AssetDatabase.MoveAssetToTrash(folderPath);
			if (!success)
			{
				Debug.LogError($"[AddonManager] Failed to delete folder: {folderPath}");
			}

			return success;
		}
	}
}