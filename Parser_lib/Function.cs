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

        public static List<Function> FunctionList = new List<Function>();

        internal dynamic Evaluate(dynamic[] parameters)
        {
            return Code(parameters);
        }

        public object Clone()
        {
            return new Function() { Name = Name, Code = Code};
        }

        internal Parameters.FunctionCode Code;
        internal string Name;
    }
}
