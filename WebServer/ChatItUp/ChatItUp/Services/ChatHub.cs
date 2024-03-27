using ChatItUp.Models;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Crypto.IO;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using OggVorbisEncoder;
using MimeKit.Encodings;
using NuGet.Protocol;
using ChatItUp.webm;
using System.Collections.Concurrent;
using static ChatItUp.Services.ChatService;
using SQLitePCL;
using System.Text.Encodings.Web;

namespace ChatItUp.Services
{

    public class ChatHub : Hub
    {
        
        ChatService _chatService;
        private HtmlEncoder _htmlEncoder;
        private static readonly ConcurrentDictionary<Guid, int> UserConnectionCounts = new ConcurrentDictionary<Guid, int>();

        public ChatHub(ChatService chatService, HtmlEncoder htmlEncoder)
        {
            _chatService = chatService;
            _htmlEncoder = htmlEncoder;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdString = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
            {
                var userCount = 0;
                lock (UserConnectionCounts)
                {
                    userCount = UserConnectionCounts.AddOrUpdate(userId, 1, (id, count) => count + 1);
                }
                Console.WriteLine($"User {userId} connected. Connection count: {UserConnectionCounts[userId]}");
                // Set user status to online if needed
                if (userCount == 1)
                {
                    var user = await _chatService.GetUserAsync(userId);
                    await _chatService.SetStatusAsync(userId, "Online");
                    var servers = await _chatService.GetServers(userId);
                    foreach (var server in servers)
                    {
                        var groupName = "server_" + server.Id.ToString();
                        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                        await Clients.Group(groupName).SendAsync("UserConnected", server.Id, userId, user.DisplayName ?? user.EmailAddress, server.IsOwner);
                    }
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdString = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
            {
                var newCount = 0;
                lock (UserConnectionCounts)
                {
                    newCount = UserConnectionCounts.AddOrUpdate(userId, 0, (id, count) => Math.Max(count - 1, 0));
                }
                Console.WriteLine($"User {userId} disconnected. Remaining connections: {newCount}");

                if (newCount == 0)
                {
                    UserConnectionCounts.TryRemove(userId, out _);
                    Console.WriteLine($"User {userId} set to offline.");
                    var user = await _chatService.GetUserAsync(userId);
                    await _chatService.SetStatusAsync(userId, "Offline");
                    
                    var servers = await _chatService.GetServers(userId);
                    foreach (var server in servers)
                    {
                        var groupName = "server_" + server.Id.ToString();
                        await Clients.Group(groupName).SendAsync("UserDisconnected", server.Id, userId, user.DisplayName ?? user.EmailAddress, server.IsOwner);
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                    }
                }
                
            }

            await base.OnDisconnectedAsync(exception);
        }

        //public async Task NotifyRemoveServer(Guid serverId)
        //{
        //    await Clients.Users(from usid in (await _chatService.GetUsersForServer(serverId)) select usid.ToString()).SendAsync("UpdateServers", serverId);
        //}

        //public async Task NotifyRemoveChannel(Guid channelId)
        //{
        //    await Clients.Users(from usid in (await _chatService.GetUsersForChannel(channelId)) select usid.ToString()).SendAsync("RemoveChannel", channelId);
        //}

        public async Task SendMessage(string channelId, string message)
        {
            Guid userId = Guid.Empty;
            if(string.IsNullOrEmpty(message) || string.IsNullOrEmpty((message = message.Trim())))
            {
                return;
            }
            if (Context.User == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", channelId, "System", "Authorization failed.");
            }
            else
            {
                Guid.TryParse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
                Guid.TryParse(channelId, out var channelIdGuid);

                if (!await _chatService.HasAccessToChannelAsync(userId, channelIdGuid))
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", channelId, "System", "No access to channel.");
                }
                else
                {
                    message = _htmlEncoder.Encode(message);
                    var channel = await _chatService.GetChannelAsync(channelIdGuid);

                    await Clients.Group("server_" + channel.ServerId.ToString()).SendAsync("ReceiveMessage", channelId, userId.ToString(), message);
                    await _chatService.SendMessageAsync(userId, channelIdGuid, message);
                }
            }
        }



        public async Task SendAudioChunk(string base64AudioChunk)
        {
            if (!base64AudioChunk.StartsWith("Gk"))
            {
                var data = Convert.FromBase64String(base64AudioChunk);

                var oggBytes = ParseAudio.ParseWebMChunk(data);

                base64AudioChunk = Convert.ToBase64String(oggBytes);
            }

            await Clients.All.SendAsync("ReceiveAudioChunk", base64AudioChunk);
        }

        


    }
}
