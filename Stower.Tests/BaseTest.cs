using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Stower.Tests
{
    public class BaseTest
    {
        public IHost TestHost { get; }

        public BaseTest()
        {
            TestHost = CreateHostBuilder().Build();
            Task.Run(() => TestHost.RunAsync());
        }

        public static IHostBuilder CreateHostBuilder(string[] args = null) =>
            Host.CreateDefaultBuilder(args)
           .ConfigureServices((hostContext, services) =>
           {
               services.AddOptions();
               services.AddLogging();

               services.AddScoped<IDb, Db>();

               services.AddStower(options =>
               {
                   options.AddStack<Foo>(100, 6);
               }, typeof(FooHandler).Assembly);
           });
    }
}
