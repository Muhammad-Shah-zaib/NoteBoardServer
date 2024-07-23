using System;
using System.Collections.Generic;

namespace NoteBoardServer.Models;

public partial class AuthToken
{
    public int TokenId { get; set; }

    public string TokenType { get; set; } = null!;

    public string Token { get; set; } = null!;

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
