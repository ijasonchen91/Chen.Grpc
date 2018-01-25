using System;
using System.Collections.Generic;
using System.Text;

namespace Chen.Grpc.Core
{
    public interface IMyFormatter
    {
    }

    public interface IMyFormatter<T> : IMyFormatter
    {

    }
}
