using Microsoft.AspNetCore.SignalR;
using SignalRCharServerExample.Data;
using SignalRCharServerExample.Models;

namespace SignalRCharServerExample.Hubs
{
    public class ChatHub : Hub
    {
        public async Task GetNickName(string nickName)
        {
            Client client = new()
            {
                NickName = nickName,
                ConnectionId = Context.ConnectionId
            };
            ClientSource.Clients.Add(client);
            await Clients.Others.SendAsync("clientJoined", nickName);
            await Clients.All.SendAsync("clients", ClientSource.Clients);
        }

        public async Task SendMessageAsync(string message, string clientName)
        {
            clientName = clientName.Trim();
            var senderClient = ClientSource.Clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (clientName == "All CLients")
            {
                await Clients.Others.SendAsync("receiveMessage", message, senderClient.NickName);
            }
            else
            {
                var client = ClientSource.Clients.FirstOrDefault(x => x.NickName == clientName);
                await Clients.Client(client.ConnectionId).SendAsync("receiveMessage", message, senderClient.NickName);

            }
        }

        public async Task AddGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Group group = new Group { GroupName = groupName };

            group.Clients.Add(ClientSource.Clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId));

            GroupSource.Groups.Add(group);

            await Clients.All.SendAsync("groups", GroupSource.Groups);
        }

        public async Task AddClientToGroup(IEnumerable<string> groupNames)
        {
            Client client = ClientSource.Clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            foreach (var groupName in groupNames)
            {
                Group _group = GroupSource.Groups.FirstOrDefault(x => x.GroupName == groupName);
                var result = _group.Clients.Any(x => x.ConnectionId == Context.ConnectionId);
                if (!result)
                {
                    _group.Clients.Add(client);
                    Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                }
            }
        }

        public async Task GetClientToGroup(string groupName)
        {
            if(groupName == "-1")
            {
                await Clients.Caller.SendAsync("clients", ClientSource.Clients);

            }
            else
            {
                Group group = GroupSource.Groups.FirstOrDefault(x => x.GroupName == groupName);

                await Clients.Caller.SendAsync("clients", group.Clients);
            }
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {

            await Clients.Group(groupName).SendAsync("receiveMessage", message, 
                ClientSource.Clients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId).NickName);
        }
    }
}
