using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptScript
{
    class Program
    {
        public static bool IsRunning { get; set; } = true;
        public static UserInput Input { get; set; } = new UserInput();

        static void Main(string[] args)
        {
            Command.SubscribeAll(Input);

            while(IsRunning)
            {
                Input.Parse(Console.ReadLine());
            }
        }
    }
}
