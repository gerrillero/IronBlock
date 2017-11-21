using System;
using System.Collections.Generic;
using System.Linq;
using IronBlock.Blocks.Math;
using IronBlock.Blocks.Text;
using IronBlock.Blocks.Variables;
using IronBlock.Blocks.Controls;
using IronBlock.Blocks.Logic;

namespace IronBlock.Blocks
{
    public static class Extensions
    {
        public static object Evaluate(this IEnumerable<Value> values, string name, IDictionary<string, object> variables)
        {
            var value = values.FirstOrDefault(x => x.Name == name);
            if (null == value) throw new ArgumentException($"value {name} not found");
            
            return value.Evaluate(variables);
        }

        public static string Evaluate(this IEnumerable<Field> fields, string name)
        {
            var field = fields.FirstOrDefault(x => x.Name == name);
            if (null == field) throw new ArgumentException($"field {name} not found");
            
            return field.Value;
        }

        public static Statement GetStatement(this IEnumerable<Statement> statements, string name)
        {
            var statement = statements.FirstOrDefault(x => x.Name == name);
            if (null == statement) throw new ArgumentException($"statement {name} not found");

            return statement;
        }



        public static void AddStandardBlocks(this Parser parser)
        {
            parser.AddBlock<ControlsRepeatExt>("controls_repeat_ext");
            parser.AddBlock<ControlsIf>("controls_if");
            parser.AddBlock<LogicCompare>("logic_compare");
            parser.AddBlock<MathArithmetic>("math_arithmetic");
            parser.AddBlock<MathNumber>("math_number");
            parser.AddBlock<TextBlock>("text");
            parser.AddBlock<TextPrint>("text_print");
            parser.AddBlock<VariablesGet>("variables_get");
            parser.AddBlock<VariablesSet>("variables_set");
        }


    }

}