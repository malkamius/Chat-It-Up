using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class Message
{
    public Guid Id { get; set; }

    public Guid? ChannelId { get; set; } = null;

    public string Body { get; set; } = null!;

    public DateTime SentOn { get; set; }

    public Guid SentBy { get; set; }
}
