using System.Collections.Generic;

namespace cryptscript
{
    public interface ICallable : IObject
    {
        IObject Call(List<IObject> args);
    }

    public class Routine : Object, ICallable
    {
        private string Name { get; set; }
        private List<string> Arguments { get; set; }
        private List<Line> Code { get; set; }

        public Routine(string name, List<Line> code, List<string> args)
        {
            Name = name;
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
                Interpreter.ThrowError(new Error(ErrorType.InvalidArgumentError));
                return null;
            }

            // add arguments to local id group
            IdentifierGroup locals = new IdentifierGroup();
            for(int i = 0; i < Arguments.Count; i++)
            {
                if(args.Count > i)
                {
                    locals.SetReference(Arguments[i], args[i]);
                }
            }

            // add self to local id group
            locals.SetReference(Name, this);

            Parser funcParser = new Parser(locals);
            IObject result = null;
            foreach(Line line in Code)
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

        public override string Repr(bool showQuotes) => Repr();
    }
}