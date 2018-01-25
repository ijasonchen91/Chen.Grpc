using Grpc.Core;
using System;
using System.Threading;

namespace Chen.Grpc.Core
{
    public interface IService<TSelf> : IServiceDependency
    {
        TSelf WithOptions(CallOptions option);
        TSelf WithHeaders(Metadata headers);
        TSelf WithDeadline(DateTime deadline);
        TSelf WithCancellationToken(CancellationToken cancellationToken);
        TSelf WithHost(string host);
    }
}
