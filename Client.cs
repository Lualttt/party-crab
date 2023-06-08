using EngineIOSharp.Common.Enum;
using SocketIOSharp.Client;
using SocketIOSharp.Common;

namespace party_crab
{
    public class Client {
        public static void Connect()
        {
            Plugin.client = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, Plugin.party_server.Item1, Plugin.party_server.Item2));
            
            Plugin.client.On(SocketIOEvent.CONNECTION, () =>
            {
                Plugin.SendMessage("connected to party sever", 1);
            });
            
            Plugin.client.Connect();
        }
    }
}