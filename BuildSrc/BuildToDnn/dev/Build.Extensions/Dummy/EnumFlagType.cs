using System;

namespace Build.Extensions.Dummy
{
    [Flags]
    public enum EnumFlagType
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        All = 16
    }

}
