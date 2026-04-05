using LiteNetLibManager;

namespace MultiplayerARPG
{
    public abstract partial class BaseGameNetworkManager
    {
        /// <summary>
        /// While handling an incoming client-&gt;server RPC, holds the sender's connection id (otherwise -1).
        /// Used for server-owned objects (e.g. monster swarms) that allow <c>canCallByEveryone</c> RPCs.
        /// </summary>
        public static long IncomingClientRpcConnectionId { get; private set; } = -1;

        protected override void HandleClientCallFunction(MessageHandlerData messageHandler)
        {
            IncomingClientRpcConnectionId = messageHandler.ConnectionId;
            try
            {
                base.HandleClientCallFunction(messageHandler);
            }
            finally
            {
                IncomingClientRpcConnectionId = -1;
            }
        }
    }
}
