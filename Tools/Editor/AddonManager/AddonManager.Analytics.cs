/**
 * AddonManager.Analytics
 * Author: Denarii Games
 * Version: 1.0-rc1
 *
 * Analytics related functionality.
 */

using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MmoKtCE.AddonManager
{
	public static class AddonAnalytics
	{
		public const string WelcomeScreen = "AddonManager_WelcomeScreen";
		private const string AnalyticsConsent = "AddonManager_AnalyticsConsent";
		private const string MeasurementId = "G-ZCJ0YR19B4";
		private const string ApiSecret = "3EMmB_13SzWC4KJ23IngEg";

		public static bool ShowWelcome => !EditorPrefs.GetBool(WelcomeScreen);
		public static bool HasConsent => EditorPrefs.GetBool(AnalyticsConsent);
		private static string clientId = null;

		public static void SetConsent(bool consentValue = false)
		{
			EditorPrefs.SetBool(AnalyticsConsent, consentValue);
		}

		/// <summary>
		/// Get or generate a Guid for clientId
		/// </summary>
		private static string ClientId
		{
			get
			{
				if (clientId == null)
				{
					clientId = PlayerPrefs.GetString("GA4_ClientId", System.Guid.NewGuid().ToString());
					PlayerPrefs.SetString("GA4_ClientId", clientId);
					PlayerPrefs.Save();
				}
				return clientId;
			}
		}

		/// <summary>
		/// Post GA4 event
		/// </summary>
		/// <param name="eventName"></param>
		/// <param name="parameters"></param>
		public static void LogEvent(string eventName, params (string key, object value)[] parameters)
		{
			if (!HasConsent)
				return;

			var payload = new Dictionary<string, object>
			{
				["client_id"] = ClientId,
				["events"] = new[]
				{
					new Dictionary<string, object>
					{
						["name"] = eventName,
						["params"] = parameters.Length > 0
							? parameters.ToDictionary(p => p.key, p => p.value ?? "")
							: new Dictionary<string, object>()
					}
				}
			};
			string json = JsonConvert.SerializeObject(payload, Formatting.None);
			string url = $"https://www.google-analytics.com/mp/collect?measurement_id={MeasurementId}&api_secret={ApiSecret}";

			var www = UnityWebRequest.Post(url, json, "application/json");
			www.SendWebRequest().completed += _ =>
			{
				if (www.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError($"[AddonManager {Time.time}] GA4 event failed ({eventName}): {www.error}");
				}
			};
		}
	}
}