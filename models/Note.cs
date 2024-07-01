using System;
using System.Collections.Generic;

namespace NoteBoardServer.models;

public partial class Note
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;
}
