using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatItUp.Data;
using ChatItUp.Context;
using ChatItUp.Models;
using System.Security.Claims;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ChatItUp.Services
{


    public class ChatService
    {
        private readonly CIUDataDbContext _context;
        private readonly ILogger<ChatService> _logger;

        public ChatService(CIUDataDbContext context, ILogger<ChatService> logger)
        {
            _context = context;
            _logger = logger;
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

            public string ImageUrl { get; set; }

            public bool IsOwner { get; set; }
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
            try
            {
                ///api/chat/GetRecentMessages?channelId=
                return await _context.UserServers.Include(us => us.Server)
                    .Where(us => us.UserId == userId && (us.Server.DeletedOn == null || us.Server.DeletedOn == DateTime.MinValue))
                    .Select(us => new Server { Id = us.Server.Id, Name = us.Server.Name, ImageUrl = "api/ServerImage/GetServerImage?ServerId=" + us.ServerId.ToString(), IsOwner = us.Server.ServerOwner == userId })
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<Server>();
            }
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

        internal async Task<Byte[]> GetServerImageAsync(Guid userId, Guid serverId)
        {
            var server = _context.UserServers.Include(us => us.Server) // Ensure Server is eagerly loaded
                .FirstOrDefault(us => us.UserId == userId && us.ServerId == serverId);

            
            // Create an image in memory
            using (var bitmap = new Bitmap(64, 64))
            using (var g = Graphics.FromImage(bitmap))
            using (var ms = new MemoryStream())
            {
                Image? srcImage = null;
                try
                {

                    if (server != null && server.Server != null && server.Server.Image != null && server.Server.Image.Length > 0)
                    {
                        using (var instream = new MemoryStream(server.Server.Image))
                        {
                            srcImage = Image.FromStream(instream);
                        }

                    }

                    if (srcImage != null)
                    {
                        g.DrawImage(srcImage, 0, 0, 64, 64);
                    }
                    else
                    {
                        var firstLetter = server != null && server.Server != null? server.Server.Name.FirstOrDefault('@').ToString() : "@";
                        // Customize your image here
                        g.Clear(Color.Gray); // Background color
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        var font = new Font("Arial", 24, FontStyle.Bold, GraphicsUnit.Pixel);
                        var textSize = g.MeasureString(firstLetter, font);
                        g.DrawString(firstLetter, font, Brushes.White, (64 - textSize.Width) / 2, (64 - textSize.Height) / 2);
                    }
                }
                finally 
                {
                    if (srcImage != null) srcImage.Dispose();
                }
                // Save the image to a memory stream in PNG format
                bitmap.Save(ms, ImageFormat.Png);

                // Set the position to the beginning of the stream
                ms.Position = 0;

                // Return the stream as a file
                return ms.ToArray();
            }
        }

        internal async Task<IEnumerable<Guid>> GetUsersForServer(Guid serverId)
        {
            return await (from us in _context.UserServers where us.ServerId == serverId select us.UserId).ToListAsync();
        }

        internal async Task<IEnumerable<Guid>> GetUsersForChannel(Guid channelId)
        {
            var channel = _context.ServerChannels.FirstOrDefault(c => c.Id == channelId);

            if (channel != null)
            {
                return await (from us in _context.UserServers where us.ServerId == channel.ServerId select us.UserId).ToListAsync();
            } 
            else
            {
                return Enumerable.Empty<Guid>();
            }
        }
    }
}
