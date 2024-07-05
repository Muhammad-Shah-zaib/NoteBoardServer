﻿using System;
using System.Collections.Generic;

namespace NoteBoardServer.Models;

public partial class User
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string? Lastname { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public bool EmailVerified { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}