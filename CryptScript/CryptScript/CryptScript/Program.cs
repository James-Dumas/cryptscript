using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptScript
{
    class Program
    {
        public static bool IsRunning { get; set; } = true;

        static void Main(string[] args)
        {
            Display.Update(DisplayMode.Main);

            while(IsRunning)
            {
                Display.Input.Parse(Util.PromptForString(">>> "));
            }
        }                
    }
}
