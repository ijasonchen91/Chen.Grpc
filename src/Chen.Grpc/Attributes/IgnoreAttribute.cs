using System;
using System.Collections.Generic;
using System.Text;

namespace Chen.Grpc.Attributes
{
    /// <summary>
    /// Don't register on Engine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}
