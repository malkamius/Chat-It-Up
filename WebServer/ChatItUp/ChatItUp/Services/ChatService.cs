using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatItUp.Data;
using ChatItUp.Context;
using ChatItUp.Models;
using System.Security.Claims;

namespace ChatItUp.Services
{


    public class ChatService
    {
        private readonly CIUDataDbContext _context;

        public ChatService(CIUDataDbContext context)
        {
            _context = context;
        }

        public class Message
        {
            public Guid Channel { get; set; }
            public Guid Id { get; set; }
            public string User { get; set; }
            public string Body { get; set; }
            public DateTime SentOn { get; set; }

        }

        public class Server
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

        }

        public class Channel
        {
            public Guid ServerId { get; set; }
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
        public async Task<List<Message>> GetRecentMessagesAsync(Guid userId, Guid channelId, int skip = 0)
        {


            return await _context.Messages
                                .Where(m => m.ChannelId == channelId)
                                .OrderByDescending(m => m.SentOn)
                                .Skip(skip)
                                .Take(3)
                                .Select(item => new Message { Channel = item.ChannelId, User = item.SentBy.ToString(), Body = item.Body, SentOn = item.SentOn })
                          .ToListAsync();
        }

        public async Task<List<Server>> GetServers(Guid userId)
        {
            return await _context.UserServers
                .Where(us => us.UserId == userId)
                .Select(us => new Server { Id = us.Server.Id, Name = us.Server.Name })
                .ToListAsync();
        }

        public async Task<List<Channel>> GetChannels(Guid userId, Guid serverId)
        {
            // Check if the user is a member of the server
            bool isMember = await _context.UserServers
                .AnyAsync(us => us.UserId == userId && us.ServerId == serverId);

            if (!isMember)
            {
                // The user is not a member of the server, return an empty list or handle as needed
                return new List<Channel>();
            }

            // User is a member, proceed to fetch and return channels for the server
            return await _context.ServerChannels
                .Where(sc => sc.ServerId == serverId)
                .Select(sc => new Channel { ServerId = sc.ServerId, Id = sc.Id, Name = sc.Name })
                .ToListAsync();
        }

        internal async Task<bool> HasAccessToChannelAsync(Guid userId, Guid channelId)
        {
            var channel = await _context.ServerChannels.FirstOrDefaultAsync(i => i.Id == channelId);
            if (channel == null) return false;

            return await _context.UserServers.AnyAsync(i => i.UserId == userId && i.ServerId == channel.ServerId);
        }

        internal async Task SendMessageAsync(Guid userId, Guid channelId, string message)
        {
            var newMessage = new Models.Message
            {
                ChannelId = channelId,
                SentBy = userId,
                Id = Guid.NewGuid(),
                Body = message,
                SentOn = DateTime.Now
            };

            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();
        }
    }
}
