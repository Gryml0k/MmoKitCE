using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UINetworkTime : MonoBehaviour
    {
        public Text textRtt;
        public Text textServerTimestamp;

        private void Update()
        {
            if (BaseGameNetworkManager.Singleton.IsClientConnected || 
                BaseGameNetworkManager.Singleton.IsServer)
            {
                if (textRtt)
                    textRtt.text = ZString.Concat("RTT: ", BaseGameNetworkManager.Singleton.Rtt.ToString("N0"));
                if (textServerTimestamp)
                    textServerTimestamp.text = ZString.Concat("ServerTimestamp: ", BaseGameNetworkManager.Singleton.ServerTimestamp.ToString("N0"));
                return;
            }
            if (textRtt)
                textRtt.text = "RTT: N/A";
            if (textServerTimestamp)
                textServerTimestamp.text = "ServerTimestamp: N/A";
        }
    }
}
