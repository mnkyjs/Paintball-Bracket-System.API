using System;

namespace BracketSystem.Core.Models
{
    [Flags]
    public enum Role
    {
        Reader = 1,
        Editor = 2,
        Owner = 4,
        Administrator = 8,
        RootAdmin = 16
    }
}