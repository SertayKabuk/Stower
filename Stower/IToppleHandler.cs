using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stower
{
    public interface IToppleHandler<TStack>
    {
        Task Handle(IEnumerable<TStack> items);
    }
}
