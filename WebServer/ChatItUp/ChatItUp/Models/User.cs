using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class User
{
    public Guid Id { get; set; }
    public string EmailAddress { get; set; } = "@";
    public string? DisplayName { get; set; } = null;

    public string? Status { get; set; } = "Offline";
    public virtual ICollection<UserServer> UserServers { get; set; } = new List<UserServer>();
}
