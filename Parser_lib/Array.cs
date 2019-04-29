using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser_lib
{
    internal class Array : IExpressionMember, ICloneable
    {
        internal IExpressionMember[] parameters;

        internal static List<Array> ArrayList = new List<Array>();

        internal dynamic Evaluate(int[] parameters)
        {
            if(parameters.Length == 1)
            {
                return Data[parameters[0]];
            }
            else if(parameters.Length == 2)
            {
                return Data[parameters[0], parameters[1]];
            }

            throw new Exception($"{parameters.Length}DArrayNotExists");
        }

        public object Clone()
        {
            return new Array() { Name = Name, Data = Data };
        }

        internal dynamic Data;
        internal string Name;
    }
}
