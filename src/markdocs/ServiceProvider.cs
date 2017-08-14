using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ITGlobal.MarkDocs.Tools
{
    public sealed class ServiceProvider : IServiceProvider, IDisposable
    {
        private readonly object _sync = new object();

        private readonly Dictionary<Type, object> _instances
            = new Dictionary<Type, object>();

        private readonly Dictionary<Type, Func<IServiceProvider, object>> _factories
            = new Dictionary<Type, Func<IServiceProvider, object>>();

        public static ServiceProvider Factory(Action<IServiceCollection> config)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog();

            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            config(services);

            var provider = new ServiceProvider(services);
            return provider;
        }

        private ServiceProvider(IEnumerable<ServiceDescriptor> services)
        {
            foreach (var service in services)
            {
                var factory = service.ImplementationFactory;

                if (factory == null && service.ImplementationInstance != null)
                {
                    factory = _ => service.ImplementationInstance;
                }

                if (factory == null)
                {
                    factory = GetFactory(service.ImplementationType);
                }

                _factories[service.ServiceType] = factory;
            }
        }

        public object GetService(Type serviceType)
        {
            lock (_sync)
            {
                object instance;
                if (!_instances.TryGetValue(serviceType, out instance))
                {
                    Func<IServiceProvider, object> factory;
                    if (!_factories.TryGetValue(serviceType, out factory))
                    {
                        factory = GetFactory(serviceType);
                        _factories[serviceType] = factory;
                    }

                    instance = factory(this);
                    _instances[serviceType] = instance;
                }

                return instance;
            }
        }

        private static Func<IServiceProvider, object> GetFactory(Type type)
        {
            return sp => CreateInstance(sp, type);
        }

        private static object CreateInstance(IServiceProvider sp, Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsAbstract)
            {
                if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var t = typeInfo.GetGenericArguments()[0];
                    var item = CreateInstance(sp, t);

                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
                    if (item != null)
                    {
                        list.Add(item);
                    }

                    return list;
                }

                return null;
            }

            var ctor = type.GetConstructors().First();
            var args = ctor.GetParameters()
                .Select(p => sp.GetService(p.ParameterType))
                .ToArray();

            var instance = ctor.Invoke(args);
            return instance;
        }

        public void Dispose()
        {
            foreach (var disposable in _instances.Values.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }
    }
}