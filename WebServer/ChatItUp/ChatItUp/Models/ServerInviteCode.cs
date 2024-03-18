using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class ServerInviteCode
{
    public Guid ServerId { get; set; }

    public Guid InviteCode { get; set; }

    public DateTime ExpiresOn { get; set; }

    public short ExpiresAfterUses { get; set; }

    public short Uses { get; set; }
}
