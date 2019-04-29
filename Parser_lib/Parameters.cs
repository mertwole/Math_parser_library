using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Parser_lib.Operator;

namespace Parser_lib
{
    public class Parameters
    {
        public static void AddVariable(string name, dynamic value)
        {
            Variable.VariableList.Add(name, value);
        }

        public static void AddArray(string name, dynamic value)
        {
            Array.ArrayList.Add(new Array() { Name = name, Data = value });
        }

        public delegate dynamic FunctionCode(dynamic[] parameters);

        public static void AddFunction(string name, FunctionCode function)
        {
            Function.FunctionList.Add(new Function() { Name = name, Code = function});
        }

        internal static List<Operator> OperatorList = new List<Operator>()
        {
            new TernarOperator(new string[] {"?", ":"}, (x) => x[0] ? x[1] : x[2]),

            new BinarOperator(new string[] {">"}, (x) => x[0] > x[1]),
            new BinarOperator(new string[] {"<"}, (x) => x[0] < x[1]),
            new BinarOperator(new string[] {">="}, (x) => x[0] >= x[1]),
            new BinarOperator(new string[] {"<="}, (x) => x[0] <= x[1]),
            new BinarOperator(new string[] {"=="}, (x) => x[0] == x[1]),
            new BinarOperator(new string[] {"!="}, (x) => x[0] != x[1]),
            new BinarOperator(new string[] {"+"}, (x) => x[0] + x[1]),
            new BinarOperator(new string[] {"-"}, (x) => x[0] - x[1]),
            new BinarOperator(new string[] {"*"}, (x) => x[0] * x[1]),
            new BinarOperator(new string[] {"/"}, (x) => x[0] / x[1]),

            new UnarOperator(new string[] {"-"}, (x) => -x[0]),
            new UnarOperator(new string[] {"!"}, (x) => !x[0]),
        };

        
    }
}
