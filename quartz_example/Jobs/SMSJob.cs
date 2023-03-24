using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quartz_example.Jobs
{
    public class SMSJob : ICustomJob
    {
        public string Name => "SMSJob";

        public int Id => 2;

        public void Execute(UserJob userJob)
        {
            Console.WriteLine($"{userJob.UserName} SMS has sent {DateTime.Now}");
        }
    }
}
