using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser_lib
{
    abstract class Operator : IExpressionMember, ICloneable
    {
        public delegate dynamic OperatorCode(dynamic operands/*be array of values*/);      

        public object Clone()
        {
            if (GetType() == typeof(UnarOperator))          { return new UnarOperator(look, code); }
            else if (GetType() == typeof(BinarOperator))    { return new BinarOperator(look, code); }
            else if (GetType() == typeof(TernarOperator))   { return new TernarOperator(look, code); }

            throw new Exception("UndefinedOperatorType");
        }

        internal abstract (int, int)[] Find(string expression); //returns array of (startindex, endindex) of appropriate look element
                                                                //null if not found

        internal IExpressionMember[] Operands;

        //there must be :
        //...unar_operator...
        //operand|binar_operator...
        //operand|Ternar_operator...operand|Ternar_operator...

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

        internal class TernarOperator : Operator
        {
            internal TernarOperator(string[] _look, OperatorCode _code) : base(_look, _code) { }

            internal override (int, int)[] Find(string expression)
            {
                string temp_expression = expression;

                while (true)
                {
                    int first_part_index = temp_expression.IndexOf(look[0]);

                    if (first_part_index == -1)
                        return null;

                    int second_part_index = GetSecondPartIndex(temp_expression, first_part_index + look[0].Length);

                    if(first_part_index == 0 || second_part_index == 0 || 
                        reserved_values.Contains(expression[first_part_index - 1]) 
                     || reserved_values.Contains(expression[second_part_index - 1]))
                    {
                        temp_expression = temp_expression.Remove(first_part_index, look[0].Length);
                        temp_expression = temp_expression.Insert(first_part_index, "".PadLeft(look[0].Length, ' '));

                        temp_expression = temp_expression.Remove(second_part_index, look[1].Length);
                        temp_expression = temp_expression.Insert(second_part_index, "".PadLeft(look[1].Length, ' '));
                    }
                    else
                    {
                        return new (int, int)[]
                        {(first_part_index, first_part_index + look[0].Length - 1),
                         (second_part_index, second_part_index + look[1].Length - 1)};
                    }
                }
            }

            internal int GetSecondPartIndex(string expression, int first_part_end_pos)
            {
                int balance = 1;
                string temp_expression = expression.Substring(first_part_end_pos + 1);
                int removed = 0;

                while(true)
                {
                    int first_part_index = temp_expression.IndexOf(look[0]);
                    int second_part_index = temp_expression.IndexOf(look[1]);

                    if (first_part_index == -1)
                        first_part_index = int.MaxValue;

                    if (second_part_index == -1)
                        throw new Exception("TernarOperatorPartsAreDisbalancedInExpression");

                    if(first_part_index < second_part_index)
                    {
                        balance++;
                        temp_expression = temp_expression.Remove(first_part_index, look[0].Length);
                        removed += look[0].Length;
                    }
                    else
                    {
                        balance--;
                        temp_expression = temp_expression.Remove(second_part_index, look[1].Length);
                        removed += look[1].Length;

                        if (balance == 0)
                            return first_part_end_pos + removed + second_part_index;
                    }
                }
                
            }
        }
    }
    
}
