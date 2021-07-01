using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Stower
{
    internal class Stower : IStower
    {
        private static readonly ConcurrentDictionary<Type, Lazy<StowerHandlerBase>> _itemHandlers = new ConcurrentDictionary<Type, Lazy<StowerHandlerBase>>();
        private readonly IServiceProvider _serviceProvider;

        public Stower(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Add(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            var requestType = item.GetType();
            var handler = _itemHandlers.GetOrAdd(requestType,
                 requestTypeKey => new Lazy<StowerHandlerBase>( () =>
                {
                    var wrapperType = typeof(StowerHandlerBaseWrapperImpl<>).MakeGenericType(requestTypeKey);

                    return (StowerHandlerBase)(ActivatorUtilities.CreateInstance(_serviceProvider, wrapperType)
                                                ?? throw new InvalidOperationException($"Could not create wrapper for type {wrapperType}"));
                }));

            handler.Value.Add(item);
        }
    }
}
