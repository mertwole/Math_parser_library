using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser_lib
{
    internal abstract class Operator : IExpressionMember, ICloneable
    {
        internal delegate dynamic OperatorCode(dynamic operands/*be array of values*/);

        internal static List<Operator> OperatorList = new List<Operator>()
        {
            new BinarOperator(new string[] {"+"}, (x) => x[0] + x[1]),
            new BinarOperator(new string[] {"-"}, (x) => x[0] - x[1]),
            new BinarOperator(new string[] {"*"}, (x) => x[0] * x[1]),
            new BinarOperator(new string[] {"/"}, (x) => x[0] / x[1]),

            new UnarOperator(new string[] {"-"}, (x) => -x[0]),
        };

        internal abstract (int, int)[] Find(string expression);//returns array of (startindex, endindex) of appropriate look element

        public object Clone()
        {
            if (GetType() == typeof(UnarOperator))          { return new UnarOperator(look, code); }
            else if (GetType() == typeof(BinarOperator))    { return new BinarOperator(look, code); }
            else if (GetType() == typeof(TrinarOperator))   { return new TrinarOperator(look, code); }

            throw new Exception("UndefinedOperatorType");
        }

        //null if not found

        internal IExpressionMember[] Operands;

        //there must be :
        //...unar_operator...
        //operand|binar_operator...
        //operand|trinar_operator...operator|trinar_operator...

        string[] look;
        OperatorCode code;

        internal dynamic Evaluate(dynamic[] operands)
        {
            return code(operands);
        }

        internal static List<char> reserved_values = new List<char>{ '+', '-', '*', '/', '>', '<', '=', '!', ':', '?'};

        protected Operator(string[] _look, OperatorCode _code)
        {
            code = _code;
            look = _look;
        }


        internal class UnarOperator : Operator
        {
            internal UnarOperator(string[] _look, OperatorCode _code) : base(_look, _code) { }

            internal override (int, int)[] Find(string expression)
            {
                int pos = expression.IndexOf(look[0]);
                return pos == -1 ? null : new (int, int)[] { (pos, pos + look[0].Length - 1) };
            }
        }

        internal class BinarOperator : Operator
        {
            internal BinarOperator(string[] _look, OperatorCode _code) : base(_look, _code) { }

            internal override (int, int)[] Find(string expression)
            {
                string temp_expression = expression;
                int cuted_length = 0;

                while(true)
                {
                    int pos = temp_expression.IndexOf(look[0]);

                    if (pos == -1)
                        return null;

                    if(pos == 0)
                    {
                        temp_expression = temp_expression.Remove(0, look[0].Length);
                        cuted_length += look[0].Length;
                        continue;
                    }

                    if(reserved_values.Contains(temp_expression[pos - 1]))
                    {//operator leftward
                        temp_expression = temp_expression.Remove(pos, look[0].Length);
                        cuted_length += look[0].Length;
                    }
                    else
                    {//operand leftward
                        return new (int, int)[] { (pos + cuted_length, pos + cuted_length + look[0].Length - 1) };
                    }
                }
            }
        }

        internal class TrinarOperator : Operator
        {
            internal TrinarOperator(string[] _look, OperatorCode _code) : base(_look, _code) { }

            internal override (int, int)[] Find(string expression)
            {
                return null;
            }
        }
    }
    
}
