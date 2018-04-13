using System.Collections.Generic;

namespace cryptscript
{
    public interface ICallable : IObject
    {
        IObject Call(List<IObject> args);
    }

    public class Routine : Object, ICallable
    {
        private List<string> Arguments { get; set; }
        private List<List<Token>> Code { get; set; }

        public Routine(List<List<Token>> code, List<string> args)
        {
            Arguments = args;
            Code = code;
        }

        public IObject Call(List<IObject> args)
        {
            if(Interpreter.StopExecution)
            {
                return null;
            }

            if(args.Count > Arguments.Count)
            {
                return new Error(ErrorType.InvalidArgumentError);
            }

            // add arguments to local id group
            IdentifierGroup locals = new IdentifierGroup();
            for(int i = 0; i < Arguments.Count; i++)
            {
                locals.AddID(Arguments[i]);
                if(args.Count > i)
                {
                    locals.SetReference(Arguments[i], args[i]);
                }
            }

            Parser funcParser = new Parser(locals);
            IObject result = null;
            foreach(List<Token> line in Code)
            {
                result = funcParser.Parse(line, true);
                if(result != null || Interpreter.StopExecution) { break; }
            }

            if(result == null)
            {
                result = new Zilch();
            }

            return result;
        }

        public override string Repr()
        {
            return "<user-defined routine>";
        }
    }
}