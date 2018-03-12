using System;

namespace CryptScript
{
    public class Interpreter
    {

        public static void Main(String[] args)
        {
            // interactive console interpreter

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
                    Token result = parser.Parse(lexer.Tokenize());
                    if(result != null)
                    {
                        Console.WriteLine(result.ToString());
                    }
                }
            }
        }
    }
}