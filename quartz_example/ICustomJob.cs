using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quartz_example
{
    public interface ICustomJob
    {
        void Execute(UserJob userjob);
        string Name { get; }
        int Id { get; }
    }
}
