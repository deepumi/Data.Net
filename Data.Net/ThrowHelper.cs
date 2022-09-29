using System;

namespace Data.Net;

internal class ThrowHelper
{
    internal static void Throw(string name) => throw new ArgumentNullException(name);

    internal static void ThrowException(string message) => throw new Exception(message);
}