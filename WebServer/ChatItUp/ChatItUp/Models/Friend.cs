using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class Friend
{
    public Guid UserId { get; set; }

    public Guid FriendUserId { get; set; }

    public DateTime CreatedOn { get; set; }
}
