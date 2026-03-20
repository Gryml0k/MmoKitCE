/**
 * AddonManager.Filter
 * Author: Denarii Games
 * Version: 1.0-rc1
 *
 * Filter related functionality.
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MmoKtCE.AddonManager
{
    public partial class AddonManagerWindow
    {
		/// <summary>
		/// Apply filters to addons.
		/// </summary>
		/// <returns></returns>
		private string currentCategoryFilter = "All Categories";
		private string currentStatusFilter = "All Addons";
		private string currentUpdatedFilter = "Anytime";
		private string currentCoreFilter = "All Publishers";
		private List<PackageInfo> cachedFilteredPackages = new List<PackageInfo>();
		private List<PackageInfo> GetFilteredPackages()
		{
			// Invalidate cache if any filter changed
			if (currentCategoryFilter == lastCategoryFilter &&
				currentStatusFilter == lastStatusFilter &&
				currentUpdatedFilter == lastUpdatedFilter &&
				currentCoreFilter == lastCoreFilter)
			{
				return cachedFilteredPackages;
			}

			System.DateTime today = System.DateTime.Today;

			// Start of this month (e.g., 2025-12-01)
			System.DateTime startOfThisMonth = new System.DateTime(today.Year, today.Month, 1);

			// Start of this week
			System.DateTime startOfThisWeek = today.AddDays(-(int)today.DayOfWeek); // Sunday start

			var filtered = packages.FindAll(pkg =>
			{
				// Category filter
				bool categoryMatch = currentCategoryFilter == "All Categories" || pkg.category == currentCategoryFilter;

				// Core filter
				bool coreMatch = currentCoreFilter switch
				{
					"Community" => !pkg.isCore,
					"Core" => pkg.isCore,
					_ => true
				};

				// Status filter
				bool statusMatch = currentStatusFilter switch
				{
					"Installed" => pkg.status == PackageStatus.UpToDate || pkg.status == PackageStatus.Outdated,
					"Update Available" => pkg.status == PackageStatus.Outdated,
					"Not Installed" => pkg.status == PackageStatus.NotInstalled,
					_ => pkg.status != PackageStatus.Unknown
				};

				// Updated filter
				bool dateMatch = true;
				if (currentUpdatedFilter != "Anytime")
				{
					var updateDt = ParseUpdateDate(pkg.updateDate);

					if (updateDt.HasValue)
					{
						dateMatch = currentUpdatedFilter switch
						{
							"This week"  => updateDt.Value >= startOfThisWeek,
							"This month" => updateDt.Value >= startOfThisMonth,
							_ => true
						};
					}
					else
					{
						dateMatch = false; // No updateDate = exclude from time-based filters
					}
				}

				return categoryMatch && coreMatch && statusMatch && dateMatch;
			});

			// Cache
			cachedFilteredPackages = filtered;
			lastCategoryFilter = currentCategoryFilter;
			lastStatusFilter = currentStatusFilter;
			lastUpdatedFilter = currentUpdatedFilter;
			lastCoreFilter = currentCoreFilter;

			return cachedFilteredPackages;
		}

		private System.DateTime? ParseUpdateDate(string dateStr)
		{
			if (string.IsNullOrWhiteSpace(dateStr) || dateStr.Length != 10)
				return null;

			string[] parts = dateStr.Split('-');
			if (parts.Length != 3) return null;

			if (int.TryParse(parts[0], out int year) &&
				int.TryParse(parts[1], out int month) &&
				int.TryParse(parts[2], out int day))
			{
				try
				{
					return new System.DateTime(year, month, day);
				}
				catch
				{
					return null; // Invalid date (e.g., 2025-02-30)
				}
			}

			return null;
		}

	}
}