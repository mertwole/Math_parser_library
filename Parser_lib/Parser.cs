using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser_lib
{
    public static class Parser
    {
        public static dynamic Parse(string expression)//interface
        {
            expression = ReplaceStringsWithIndexes(expression);//because there can be strings like "12 + 8"
            expression = CleanExpression(expression);//clean must be after replacing strings   
            expression = ReplaceBracketsWithIndexes(expression);
            IExpressionMember ExpressionTreeRoot = BuildExpressionTree(expression);
            return SolveExpressionTree(ExpressionTreeRoot);
        }

        static string CleanExpression(string expression)
        {
            return expression.Replace(" ", "");
        }

        internal static List<string> InExpressionStrings = new List<string>();

        static string ReplaceStringsWithIndexes(string expression)
        {
            InExpressionStrings = new List<string>();

            for (int count = 0; count < expression.Length; count++)
            {
                if (expression[count] == '"')
                {
                    int next_quote_pos = count + 1;

                    while (expression[next_quote_pos] != '"')
                    {
                        next_quote_pos++;

                        if (expression[next_quote_pos] == '"' && next_quote_pos != expression.Length - 1 && expression[next_quote_pos + 1] == '"')
                        {
                            next_quote_pos += 2;
                        }
                    }

                    string expression_string = expression.Substring(count + 1, next_quote_pos - count - 1);
                    expression_string = expression_string.Replace("\"\"", "\"");

                    InExpressionStrings.Add(expression_string);



                    expression = expression.Remove(count + 1, next_quote_pos - count - 1);
                    expression = expression.Insert(count + 1, (InExpressionStrings.Count - 1).ToString());

                    count += (InExpressionStrings.Count - 1).ToString().Length + 1;
                }
            }

            return expression;
        }

        static string ReplaceBracketsWithIndexes(string expression)
        {
            for(int i = 0; i < expression.Length; i++)
            {
                if(expression[i] == '(' || expression[i] == '[')
                {
                    int closing = GetClosingBracket(expression, i);
                    InBracketsExpressions.Add(expression.Substring(i + 1, closing - i - 1));
                    expression = expression.Remove(i + 1, closing - i - 1);
                    string bracket_index = (InBracketsExpressions.Count - 1).ToString();
                    expression = expression.Insert(i + 1, bracket_index);
                    i = i + bracket_index.Length + 2;
                }
            }

            return expression;
        }

        static int GetClosingBracket(string expression, int opening_bracket_pos)
        {
            char opening_bracket_symbol = expression[opening_bracket_pos];
            char closing_bracket_symbol = opening_bracket_symbol == '(' ? ')' : ']';

            int balance = 1;

            for(int i = opening_bracket_pos + 1; i < expression.Length; i++)
            {
                if (expression[i] == opening_bracket_symbol)
                    balance++;
                else if (expression[i] == closing_bracket_symbol)
                    balance--;

                if (balance == 0)
                    return i;
            }

            throw new Exception("DisbalancedBracketsInExpression");
        }

        static List<string> InBracketsExpressions = new List<string>();

        static IExpressionMember BuildExpressionTree(string expression)
        {
            IExpressionMember member;

            //operators parsing
            foreach(var oper in Operator.OperatorList)
            {
                var oper_pos = oper.Find(expression);

                if (oper_pos == null)
                    continue;

                member = (Operator)oper.Clone();

                if(oper.GetType() == typeof(Operator.UnarOperator))
                {
                    (member as Operator).Operands = new IExpressionMember[] 
                    {
                        BuildExpressionTree(expression.Substring(oper_pos[0].Item2 + 1))
                    };
                }
                else if(oper.GetType() == typeof(Operator.BinarOperator))
                {
                    (member as Operator).Operands = new IExpressionMember[] 
                    {
                        BuildExpressionTree(expression.Substring(0, oper_pos[0].Item1)),
                        BuildExpressionTree(expression.Substring(oper_pos[0].Item2 + 1))
                    };
                }
                else if(oper.GetType() == typeof(Operator.TrinarOperator))
                {
                    (member as Operator).Operands = new IExpressionMember[]
                    {
                        BuildExpressionTree(expression.Substring(0, oper_pos[0].Item1)),
                        BuildExpressionTree(expression.Substring(oper_pos[0].Item2 + 1, oper_pos[1].Item1 - oper_pos[0].Item2 - 1)),
                        BuildExpressionTree(expression.Substring(oper_pos[1].Item2 + 1))
                    };
                }

                return member;
            }

            if(expression[expression.Length - 1] == ')')//func or brackets
            {
                if(expression[0] == '(')
                {//brackets
                    int.TryParse(expression.Substring(1, expression.Length - 2), out int index);
                    return BuildExpressionTree(InBracketsExpressions[index]);
                }
                else
                {//function
                    string name = expression.Substring(0, expression.IndexOf('('));

                    foreach(var func in Function.FunctionList)
                    {
                        if(func.Name == name)
                        {
                            Function new_member  = (Function)func.Clone();

                            int.TryParse(expression.Substring(expression.IndexOf('(') + 1, expression.Length - expression.IndexOf('(') - 2), out int brackets_value);

                            string parameters = InBracketsExpressions[brackets_value];

                            List<IExpressionMember> parameters_list = new List<IExpressionMember>();

                            while(parameters.Contains(','))
                            {
                                parameters_list.Add(BuildExpressionTree(ReplaceBracketsWithIndexes(parameters.Substring(0, parameters.IndexOf(',')))));
                                parameters = parameters.Remove(0, parameters.IndexOf(',') + 1);
                            }

                            parameters_list.Add(BuildExpressionTree(ReplaceBracketsWithIndexes(parameters)));

                            new_member.parameters = parameters_list.ToArray();

                            return new_member;
                        }
                    }

                    throw new Exception("FunctionNotSpecified");
                }
            }
            else if(expression[expression.Length - 1] == ']')
            {//array

            }

            return new Operand() { value = expression };
        }

        static dynamic SolveExpressionTree(IExpressionMember root)
        {      
            if(root.GetType() == typeof(Function))
            {
                List<dynamic> evaluated_params = new List<dynamic>();

                foreach (var param in (root as Function).parameters)
                    evaluated_params.Add(SolveExpressionTree(param));

                return (root as Function).Evaluate(evaluated_params.ToArray());
            }
            else if(root.GetType() == typeof(Operand))
            {
                return (root as Operand).Evaluate();
            }
            else//if (root.GetType() == typeof(Operator))
            {
                List<dynamic> evaluated_operands = new List<dynamic>();

                foreach (var operand in (root as Operator).Operands)
                    evaluated_operands.Add(SolveExpressionTree(operand));

                return (root as Operator).Evaluate(evaluated_operands.ToArray());
            }
        }

    }
}
