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
        #region variables
        public static void AddVariable(string name, dynamic value)
        {
            Variable.VariableList.Add(name, value);
        }

        public static void SetVariable(string name, dynamic value)
        {
            Variable.VariableList[name] = value;
        }

        public static void RemoveVariable(string name)
        {
            if(!Variable.VariableList.Contains(name))
                return;

            Variable.VariableList.Remove(name);
        }
        #endregion

        #region arrays
        public static void AddArray(string name, dynamic value)
        {
            Array.ArrayList.Add(new Array() { Name = name, Data = value });
        }

        public static dynamic GetArray(string name, dynamic value)
        {
            foreach (var array in Array.ArrayList)
                if (array.Name == name)
                    return array.Data;

            return null;
        }

        public static void RemoveArray(string name)
        {
            foreach(var array in Array.ArrayList)                
                if(array.Name == name)
                {
                    Array.ArrayList.Remove(array);
                }

            return;
        }
        #endregion

        #region functions
        public delegate dynamic FunctionCode(dynamic[] parameters);

        public static void AddFunction(string name, FunctionCode function)
        {
            Function.FunctionList.Add(new Function() { Name = name, Code = function});
        }
        #endregion

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
