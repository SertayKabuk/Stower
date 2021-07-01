using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Stower
{
    public abstract class StowerHandlerBase
    {
        public abstract void Add(object item);

        protected static THandler GetHandler<THandler>(ServiceFactory factory)
        {
            THandler handler;

            try
            {
                handler = factory.GetInstance<THandler>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error constructing handler for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.", e);
            }

            if (handler == null)
            {
                throw new InvalidOperationException($"Handler was not found for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.");
            }

            return handler;
        }
    }

    public abstract class StowerHandlerBaseWrapper<TClass> : StowerHandlerBase
    {
        public abstract void Add(TClass item);
    }

    public class StowerHandlerBaseWrapperImpl<TClass> : StowerHandlerBaseWrapper<TClass>
    {
        private readonly ILogger<StowerHandlerBaseWrapperImpl<TClass>> _logger;
        private readonly StowerOptions.StackOptions _options;
        private readonly CancellationToken _cancellationToken;
        private readonly ServiceFactory _serviceFactory;

        private static System.Timers.Timer timer;
        private static readonly BaseStack<TClass> stack = new BaseStack<TClass>();

        public StowerHandlerBaseWrapperImpl(ServiceFactory serviceFactory, ILogger<StowerHandlerBaseWrapperImpl<TClass>> logger, IOptions<StowerOptions> options, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceFactory = serviceFactory;
            _logger = logger;
            _options = options.Value.Stacks.SingleOrDefault(x => x.Type == typeof(TClass)) ?? throw new ArgumentNullException($"Stack options was not found for type { typeof(TClass)}");
            _cancellationToken = hostApplicationLifetime.ApplicationStopping;

            _cancellationToken.Register(() => CancellationRequested());

            if (_options.MaxWaitInSecond > 0)
            {
                timer = new System.Timers.Timer(1000 * _options.MaxWaitInSecond)
                {
                    AutoReset = false
                };

                timer.Elapsed += TimerElapsed;
                timer.Start();

                _logger.LogInformation("Timer created for type {Type} with {Intertval} seconds internal.", typeof(TClass), _options.MaxWaitInSecond);
            }
        }

        private void CancellationRequested()
        {
            _logger.LogWarning("CancellationRequested for type {Type}", typeof(TClass));
            timer?.Dispose();
            Emit().GetAwaiter().GetResult();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _logger.LogInformation("Timer elapsed for type {Type}", typeof(TClass));
            Emit().GetAwaiter().GetResult();
            timer.Start();
        }

        private async Task Emit()
        {
            var items = stack.TakeAll().ToList();
            var count = items.Count();

            if (count > 0)
            {
                Task Handler() => GetHandler<IToppleHandler<TClass>>(_serviceFactory).Handle(items);

                await Handler().ConfigureAwait(false);

                _logger.LogInformation("{Count} items emmited for type {Type}", count, typeof(TClass));
            }
        }

        public override void Add(object item) =>
            Add((TClass)item);

        public override void Add(TClass item)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            stack.Add(item);

            if (stack.Count() >= _options.MaxStackLenght)
            {
                Task.Run(() => Emit()).ConfigureAwait(false);
            }
        }
    }
}
