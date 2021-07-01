using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Stower.Tests
{
    public interface IDb
    {
        ConcurrentBag<List<Foo>> Result { get; set; }
    }

    public class Db : IDb
    {
        public ConcurrentBag<List<Foo>> Result { get; set; } = new ConcurrentBag<List<Foo>>();
    }

    public class Foo
    {
        public int FooInt { get; set; }
    }

    public class FooHandler : IToppleHandler<Foo>
    {
        private readonly IDb _db;

        public FooHandler(IDb db)
        {
            _db = db;
        }

        public virtual Task Handle(IEnumerable<Foo> items)
        {
            _db.Result.Add(new List<Foo>(items));
            return Task.CompletedTask;
        }
    }

    [Collection("Sequential")]
    public class UnitTest1 : IClassFixture<BaseTest>
    {
        private readonly IHost _host;

        public UnitTest1(BaseTest baseTest)
        {
            _host = baseTest.TestHost;
        }

        [Fact]
        public async Task ToppleCount()
        {
            var stower = _host.Services.GetRequiredService<IStower>();

            var requestCount = 12;

            Parallel.For(0, requestCount, x =>
            {
                stower.Add(new Foo()
                {
                    FooInt = x
                });
            });

            await _host.StopAsync();

            var db = _host.Services.GetRequiredService<IDb>();

            var totalCount = db.Result.Sum(x => x.Count);

            Assert.Equal(requestCount, totalCount);
        }

        //[Fact]
        //public void MissingTopple()
        //{
        //    var optionsMoq = new Mock<IOptions<StowerOptions>>();
        //    optionsMoq.Setup(x => x.Value).Returns(new StowerOptions()
        //    {
        //        MaxStackLenght = 100,
        //        MaxWaitInSecond = 6000,
        //        Stacks = new System.Collections.Generic.List<Base.IStack>()
        //         {
        //             new BaseStack<Foo>()
        //         }
        //    });




        //    var toppleHandlerMoq = new Mock<IToppleHandler>();

        //    BaseStower baseStower = new BaseStower(optionsMoq.Object, toppleHandlerMoq.Object);

        //    Parallel.For(0, 999, async x =>
        //    {
        //        await baseStower.Add<Foo>(new Foo()
        //        {
        //            FooInt = x
        //        });
        //    });

        //    toppleHandlerMoq.Verify(x => x.Handle(It.IsAny<List<object>>()), Times.Exactly(9));
        //}

    }
}
