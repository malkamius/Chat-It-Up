using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class Server
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid ServerOwner { get; set; }

    public byte[]? Image { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual ICollection<UserServer> UserServers { get; set; } = new List<UserServer>();
}
