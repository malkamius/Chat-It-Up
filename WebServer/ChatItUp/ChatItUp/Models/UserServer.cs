using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class UserServer
{
    public Guid UserId { get; set; }

    public Guid ServerId { get; set; }

    public DateTime JoinedOn { get; set; }

    public virtual Server Server { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
