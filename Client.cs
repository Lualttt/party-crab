using SocketIOClient;

namespace party_crab
{
    public class Client {
        public static void Connect()
        {
            Plugin.client = new SocketIO($"http://{Plugin.party_server}");
            
            Plugin.client.OnConnected += async (sender, e) =>
            {
                Plugin.SendMessage("connected to party sever", 1);
                var data = new HostDTO
                {
                    party_name = "Lualt's server",
                    party_max = 6,
                    party_public = false
                };
                await Plugin.client.EmitAsync("host", data);
            };

            Plugin.client.On("message", data =>
            {
                Plugin.SendMessage($"{data.GetValue<MessageDTO>().username}: {data.GetValue<MessageDTO>().message}", 0);
            });

            Plugin.client.ConnectAsync();
        }
    }
}