using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser_lib
{
    class Function : IExpressionMember, ICloneable
    {
        internal IExpressionMember[] parameters;

        internal dynamic Evaluate(dynamic[] parameters)
        {
            return Code(parameters);
        }

        public object Clone()
        {
            return new Function() { Name = Name, Code = Code};
        }

        delegate dynamic FunctionCode(dynamic[] parameters);
        FunctionCode Code;
        internal string Name;

        internal static List<Function> FunctionList = new List<Function>()
        {
            new Function(){ Name = "Summ", Code = (x) => x[0] + x[1]}
        };
    }
}
