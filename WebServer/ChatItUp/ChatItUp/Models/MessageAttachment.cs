using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class MessageAttachment
{
    public Guid AttachmentId { get; set; }

    public Guid MessageId { get; set; }

    public string AttachmentName { get; set; } = null!;

    public string? AttachmentPath { get; set; }

    public byte[]? AttachmentBody { get; set; }

    public virtual Message Message { get; set; } = null!;
}
