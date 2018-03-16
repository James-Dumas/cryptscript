using System;

namespace CryptScript
{
    public class Interpreter
    {
        public static bool IsInteractive { get; set; }
        public static int LineNumber { get; set; } = 0;

        public static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                // interactive console interpreter

                IsInteractive = true;

                Console.WriteLine("Type 'exit' to quit.");

                Parser parser = new Parser();
                while(true)
                {
                    Console.Write(">> ");

                    string input = Console.ReadLine().Trim();
                    if(input == "exit")
                    {
                        break;
                    }

                    if(input.Length > 0)
                    {
                        Lexer lexer = new Lexer(input);
                        IObject result = parser.Parse(lexer.Tokenize());
                        if(result != null)
                        {
                            Console.WriteLine(result.Value.ToString());
                        }
                    }
                }
            }
            else
            {
                // run script file

                IsInteractive = false;
            }
        }
    }
}