using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quartz_example.Jobs
{
    public class CustomJob
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Type JobType { get; set; }
        public ICustomJob Instance { get; set; }
    }
}
