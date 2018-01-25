using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chen.Grpc.Core
{
    public class MyServerServiceDefinition
    {
        public ServerServiceDefinition ServerServiceDefinition { get; private set; }
        //public IReadOnlyList<MethodHandler> MethodHandlers { get; private set; }

        //public MyServerServiceDefinition(ServerServiceDefinition definition, IReadOnlyList<MethodHandler> handlers)
        //{
        //    this.ServerServiceDefinition = definition;
        //    this.MethodHandlers = handlers;
        //}

        public static implicit operator ServerServiceDefinition(MyServerServiceDefinition self)
        {
            return self.ServerServiceDefinition;
        }

    }
}
