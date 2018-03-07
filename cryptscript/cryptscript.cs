using System;

namespace CryptScript
{
    public class Interpreter
    {
        public static void Main(String[] args)
        {
            Console.WriteLine("Type 'exit' to quit.");

            while(true)
            {
                Console.Write(">> ");

                string input = Console.ReadLine().Trim();
                if(input == "exit")
                {
                    break;
                }

                Lexer lexer = new Lexer(input);

                string output = "";
                foreach(Token token in lexer.Tokenize())
                {
                    output += token.ToString() + " ";
                }

                Console.WriteLine(output);
            }
        }
    }
}
