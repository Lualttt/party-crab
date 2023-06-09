using System;
using UnityEngine;
using SocketIOClient;

namespace party_crab
{
    public class Client {
        public static void Connect()
        {
            Plugin.client = new SocketIO($"http://{Plugin.party_server}");
            
            Plugin.client.OnConnected += (sender, e) =>
            {
                Plugin.client.EmitAsync("name", SteamManager.Instance.field_Private_String_0);
                Plugin.client_ready = true;
                Plugin.SendMessage("connected to party sever", 1);
            };

            Plugin.client.OnDisconnected += (sender, e) =>
            {
                Plugin.current_party = null;
                Plugin.SendMessage("disconnected to party sever", 2);
                Plugin.party_chat = false;
                if (Plugin.party_chat)
                    Plugin.SendMessage("you are now in all chat", 1);
            };

            ////////////////////////

            Plugin.client.On("host", data =>
            {
                var response = data.GetValue<HostResponseDTO>();
                if (response.successful)
                {
                    Plugin.SendMessage($"created party; {response.data.party_id}", 1);
                } else
                {
                    Plugin.SendMessage(response.data.error, 2);
                }
            });
            Plugin.client.On("disband", data =>
            {
                var response = data.GetValue<DisbandResponseDTO>();
                if (!response.successful)
                {
                    Plugin.SendMessage(response.data.error, 2);
                }
            });

            Plugin.client.On("join", data =>
            {
                var response = data.GetValue<JoinResponseDTO>();
                if (response.successful)
                {
                    Plugin.SendMessage($"joined {response.data.party_name} ({response.data.party_count}/{response.data.party_max})", 1);

                    // woooow
                    Plugin.current_party = new Party()
                    {
                        party_name = response.data.party_name,
                        party_max = response.data.party_max,
                        party_count = response.data.party_count,
                        party_public = response.data.party_public,
                        party_host = response.data.party_host,
                        party_id = response.data.party_id,
                    };

                    var dataDTO = new JoinedDTO()
                    {
                        username = SteamManager.Instance.field_Private_String_0
                    };
                    Plugin.client.EmitAsync("joined", dataDTO);
                } else
                {
                    Plugin.SendMessage(response.data.error, 2);
                }
            });
            Plugin.client.On("leave", data =>
            {
                var response = data.GetValue<DisbandResponseDTO>();
                if (!response.successful)
                {
                    Plugin.SendMessage(response.data.error, 2);
                } else
                {
                    if (Plugin.party_chat)
                        Plugin.SendMessage("you are now in all chat", 1);
                    Plugin.current_party = null;
                    Plugin.party_chat = false;
                }
            });

            Plugin.client.On("partylist", data =>
            {
                var response = data.GetValue<PartyListResponseDTO>();
                if (!response.successful)
                {
                    Plugin.SendMessage(response.data.error, 2);
                } else
                {
                    Plugin.SendMessage($"-=+# Parties ({response.data.page}/{response.data.max_page}) #+=-", 1);
                    foreach (Party party in response.data.parties)
                    {
                        Plugin.SendMessage($"{party.party_name} ({party.party_count}/{party.party_max}) {party.party_id}", 1);
                    }
                }
            });

            ////////////////////////

            Plugin.client.On("joined", data =>
            {
                Plugin.SendMessage(data.GetValue<ShortDTO>().message, 1);
            });

            Plugin.client.On("left", data =>
            {
                Plugin.SendMessage(data.GetValue<ShortDTO>().message, 1);
            });

            Plugin.client.On("disbanded", data =>
            {
                Plugin.SendMessage(data.GetValue<ShortDTO>().message, 1);
            });

            Plugin.client.On("promoted", data =>
            {
                Plugin.SendMessage(data.GetValue<ShortDTO>().message, 1);
            });

            Plugin.client.On("message", data =>
            {
                Plugin.SendMessage($"{data.GetValue<MessageDTO>().username}: {data.GetValue<MessageDTO>().message}", 0);
            });

            Plugin.client.ConnectAsync();
        }
    }
}
