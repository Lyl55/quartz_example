using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quartz_example.Jobs
{
    class EmailJob : ICustomJob
    {
        public void Execute(UserJob userjob)
        {
            Console.WriteLine($"{userjob.UserName} Email has sent {DateTime.Now}");
        }

        public string Name => "EmailJob";
        public int Id => 1;
    }
}
