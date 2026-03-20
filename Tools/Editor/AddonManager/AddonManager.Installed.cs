/**
 * AddonManager.Installed
 * Author: Denarii Games
 * Version: 1.0-rc1
 *
 * Installed addon functionality.
 */

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;

namespace MmoKtCE.AddonManager
{
    public partial class AddonManagerWindow
	{
		/// <summary>
		/// Iterates over all packages and attempts to find installed version, setting PackageStatus.
		/// </summary>
		private async void GetInstalledAddons()
		{
			//wait for GetPackageJsonUrl to complete
			await Task.Delay(1000);
			//Debug.Log($"[AddonManager {Time.time}] getting installed addons");

			string[] installedGuids;
			string installedGuidPath;
			foreach (PackageInfo package in packages)
			{
				installedGuids = AssetDatabase.FindAssets(package.guid);
				if (installedGuids.Length > 0)
				{
					installedGuidPath = AssetDatabase.GUIDToAssetPath(installedGuids[0]);

					try
					{
						string fileVersion = File.ReadAllText(installedGuidPath).Trim();
						package.installedVersion = fileVersion;
						//Debug.Log($"[AddonManager {Time.time}] found {package.name} v{package.installedVersion}");
					}
					catch (System.Exception e)
					{
						Debug.LogWarning($"[AddonManager] could not read file {installedGuidPath}: {e.Message}");
					}
				}

				UpdatePackageStatus(package);
			}

			Repaint();
		}

		private void UpdatePackageStatus(PackageInfo package)
		{
			if (package.status != PackageStatus.Unknown)
			{
				if (package.installedVersion == null)
					package.status = PackageStatus.NotInstalled;
				else if (package.installedVersion == package.latestVersion)
					package.status = PackageStatus.UpToDate;
				else
					package.status = PackageStatus.Outdated;
			}
		}
	}
}