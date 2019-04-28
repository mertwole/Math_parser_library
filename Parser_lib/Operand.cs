using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser_lib
{
    class Operand : IExpressionMember
    {
        public string value;

        public dynamic Evaluate()
        {
            if (value.ToLower() == "true")//bool
                return true;

            if (value.ToLower() == "false")//bool
                return false;

            if (value[0] == '"')//string
            {
                int string_index;
                int.TryParse(value.Substring(1, value.Length - 2), out string_index);
                return Parser.InExpressionStrings[string_index];
            }

            if (int.TryParse(value, out int ival))//int
                return ival;

            if (float.TryParse(value, out float fval))//float
                return fval;

            return Variables.VariableList[value]; //variable
        }
    }
}
