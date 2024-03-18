using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class User
{
    public Guid Id { get; set; }

    public virtual ICollection<UserServer> UserServers { get; set; } = new List<UserServer>();
}
