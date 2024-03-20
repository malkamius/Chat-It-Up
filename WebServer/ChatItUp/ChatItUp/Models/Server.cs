using System;
using System.Collections.Generic;
using System.Drawing;

namespace ChatItUp.Models;

public partial class Server
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; } = null;

    public DateTime CreatedOn { get; set; }

    public Guid ServerOwner { get; set; }

    public byte[]? Image { get; set; } = null;

    public DateTime? DeletedOn { get; set; } = null;

    public virtual ICollection<UserServer> UserServers { get; set; } = new List<UserServer>();
}
