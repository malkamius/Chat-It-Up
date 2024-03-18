using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class ServerChannel
{
    public Guid ServerId { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}
