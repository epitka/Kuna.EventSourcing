using System;

namespace Carts;

public class Fingerprint
{
    public const string Default = "default-fingerprint";

    public string Calculate() => Guid.NewGuid().ToString("N");
}
