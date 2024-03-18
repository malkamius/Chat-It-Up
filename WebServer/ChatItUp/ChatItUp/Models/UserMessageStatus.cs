using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class UserMessageStatus
{
    public Guid UserId { get; set; }

    public Guid MessageId { get; set; }

    public DateTime? ReadOn { get; set; }

    public DateTime? EditedOn { get; set; }
}
