using Chen.Grpc.Attributes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Grpc.Core
{
    public static class ServerServiceDefinitionBuilder
    {
        /// <summary>
        /// Search  service from all assemblies.
        /// </summary>
        /// <param name="isReturnExceptionStackTraceInErrorDetail">If true, when method body throws exception send to client exception.ToString message. It is useful for debugging.</param>
        /// <returns></returns>
        public static MyServerServiceDefinition Build(bool isReturnExceptionStackTraceInErrorDetail = false)
        {

            return Build(new BuildServerOptions(isReturnExceptionStackTraceInErrorDetail));
        }


#if NET_FRAMEWORK

        /// <summary>
        /// Search MagicOnion service from all assemblies.
        /// </summary>
        public static MyServerServiceDefinition Build(MagicOnionOptions options)
        {
            return BuildServerServiceDefinition(AppDomain.CurrentDomain.GetAssemblies(), options);
        }

#else

        /// <summary>
        /// Search MagicOnion service from entry assembly.
        /// </summary>
        public static MyServerServiceDefinition Build(BuildServerOptions options)
        {
            return Build(new[] { Assembly.GetEntryAssembly() }, options);
        }

#endif

        /// <summary>
        /// Search MagicOnion service from target assemblies. ex: new[]{ typeof(Startup).GetTypeInfo().Assembly }
        /// </summary>
        public static MyServerServiceDefinition Build(Assembly[] searchAssemblies, BuildServerOptions option)
        {
            var types = searchAssemblies
              .SelectMany(x =>
              {
                  try
                  {
                      return x.GetTypes();
                  }
                  catch (ReflectionTypeLoadException ex)
                  {
                      return ex.Types.Where(t => t != null);
                  }
              });
            return Build(types, option);
        }

        public static MyServerServiceDefinition Build(IEnumerable<Type> targetTypes, BuildServerOptions option)
        {
            var builder = ServerServiceDefinition.CreateBuilder();
            var handlers = new HashSet<MethodHandler>();

            var types = targetTypes
              .Where(x => typeof(IServiceDependency).IsAssignableFrom(x))
              .Where(x => !x.GetTypeInfo().IsAbstract)
              .Where(x => x.GetCustomAttribute<IgnoreAttribute>(false) == null)
              .Concat(SupplyEmbeddedServices(option))
              .ToArray();

            //option.MagicOnionLogger.BeginBuildServiceDefinition();
            var sw = Stopwatch.StartNew();

            Parallel.ForEach(types, /* new ParallelOptions { MaxDegreeOfParallelism = 1 },*/ classType =>
            {
                var className = classType.Name;
                if (!classType.GetConstructors().Any(x => x.GetParameters().Length == 0))
                {
                    throw new InvalidOperationException(string.Format("Type needs parameterless constructor, class:{0}", classType.FullName));
                }

                foreach (var methodInfo in classType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (methodInfo.IsSpecialName && (methodInfo.Name.StartsWith("set_") || methodInfo.Name.StartsWith("get_"))) continue;
                    if (methodInfo.GetCustomAttribute<IgnoreAttribute>(false) != null) continue; // ignore

                    var methodName = methodInfo.Name;

                    // ignore default methods
                    if (methodName == "Equals"
                     || methodName == "GetHashCode"
                     || methodName == "GetType"
                     || methodName == "ToString"
                     || methodName == "WithOptions"
                     || methodName == "WithHeaders"
                     || methodName == "WithDeadline"
                     || methodName == "WithCancellationToken"
                     || methodName == "WithHost"
                     )
                    {
                        continue;
                    }

                    // create handler
                    var handler = new MethodHandler(option, classType, methodInfo);
                    lock (builder)
                    {
                        if (!handlers.Add(handler))
                        {
                            throw new InvalidOperationException($"Method does not allow overload, {className}.{methodName}");
                        }
                        handler.RegisterHandler(builder);
                    }
                }
            });

            var result = new MyServerServiceDefinition(builder.Build(), handlers.ToArray());

            sw.Stop();
            //option.MagicOnionLogger.EndBuildServiceDefinition(sw.Elapsed.TotalMilliseconds);

            return result;
        }

        static IEnumerable<Type> SupplyEmbeddedServices(BuildServerOptions options)
        {
            if (options.DisableEmbeddedService) yield break;

            //yield return typeof(MagicOnion.Server.EmbeddedServices.MagicOnionEmbeddedHeartbeat);
            //yield return typeof(MagicOnion.Server.EmbeddedServices.MagicOnionEmbeddedPing);
        }



    }
}
