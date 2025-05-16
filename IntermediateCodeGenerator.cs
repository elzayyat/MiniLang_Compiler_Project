using System;
using System.Collections.Generic;

namespace MiniLangCompiler
{
    public class ThreeAddressCode
    {
        public string Result { get; set; }
        public string Operator { get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }

        public ThreeAddressCode(string result, string op, string arg1, string arg2 = null)
        {
            Result = result;
            Operator = op;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public override string ToString()
        {
            if (Operator == "=")
            {
                return $"{Result} = {Arg1}";
            }
            else if (Operator == "if")
            {
                return $"if {Arg1} goto {Result}";
            }
            else if (Operator == "goto")
            {
                return $"goto {Result}";
            }
            else if (Operator == "label")
            {
                return $"{Result}:";
            }
            else if (Operator == "print")
            {
                return $"print {Arg1}";
            }
            else if (Operator == "read")
            {
                return $"read {Result}";
            }
            else
            {
                return $"{Result} = {Arg1} {Operator} {Arg2}";
            }
        }
    }

    public class IntermediateCodeGenerator
    {
        private List<ThreeAddressCode> code = new List<ThreeAddressCode>();
        private int tempCounter = 0;
        private int labelCounter = 0;

        public List<ThreeAddressCode> Generate(ASTNode ast)
        {
            code.Clear();
            tempCounter = 0;
            labelCounter = 0;
            
            GenerateCode(ast);
            
            return code;
        }

        private string GenerateCode(ASTNode node, string target = null)
        {
            if (node is ProgramNode programNode)
            {
                foreach (var decl in programNode.Declarations)
                {
                    GenerateCode(decl);
                }
                return null;
            }
            else if (node is VarDeclarationNode)
            {
                // No code generation needed for declarations
                return null;
            }
            else if (node is AssignmentNode assignNode)
            {
                string exprResult = GenerateCode(assignNode.Expression);
                code.Add(new ThreeAddressCode(assignNode.Variable, "=", exprResult));
                return assignNode.Variable;
            }
            else if (node is BinaryOpNode binOpNode)
            {
                string leftResult = GenerateCode(binOpNode.Left);
                string rightResult = GenerateCode(binOpNode.Right);
                string temp = GenerateTemp();
                code.Add(new ThreeAddressCode(temp, binOpNode.Operator, leftResult, rightResult));
                return temp;
            }
            else if (node is VariableNode varNode)
            {
                return varNode.Name;
            }
            else if (node is NumberNode numNode)
            {
                return numNode.Value.ToString();
            }
            else if (node is BooleanNode boolNode)
            {
                return boolNode.Value.ToString().ToLower();
            }
            else if (node is IfStatementNode ifNode)
            {
                string condResult = GenerateCode(ifNode.Condition);
                string elseLabel = GenerateLabel();
                string endLabel = GenerateLabel();
                
                // If condition is false, jump to else branch
                code.Add(new ThreeAddressCode(elseLabel, "if", "!" + condResult));
                
                // Then branch
                GenerateCode(ifNode.ThenBranch);
                code.Add(new ThreeAddressCode(endLabel, "goto", null));
                
                // Else branch
                code.Add(new ThreeAddressCode(elseLabel, "label", null));
                if (ifNode.ElseBranch != null)
                {
                    GenerateCode(ifNode.ElseBranch);
                }
                
                // End of if statement
                code.Add(new ThreeAddressCode(endLabel, "label", null));
                return null;
            }
            else if (node is WhileStatementNode whileNode)
            {
                string startLabel = GenerateLabel();
                string endLabel = GenerateLabel();
                
                // Start of loop
                code.Add(new ThreeAddressCode(startLabel, "label", null));
                
                // Condition
                string condResult = GenerateCode(whileNode.Condition);
                code.Add(new ThreeAddressCode(endLabel, "if", "!" + condResult));
                
                // Body
                GenerateCode(whileNode.Body);
                code.Add(new ThreeAddressCode(startLabel, "goto", null));
                
                // End of loop
                code.Add(new ThreeAddressCode(endLabel, "label", null));
                return null;
            }
            else if (node is BlockNode blockNode)
            {
                foreach (var stmt in blockNode.Statements)
                {
                    GenerateCode(stmt);
                }
                return null;
            }
            else if (node is PrintStatementNode printNode)
            {
                string exprResult = GenerateCode(printNode.Expression);
                code.Add(new ThreeAddressCode(null, "print", exprResult));
                return null;
            }
            else if (node is ReadStatementNode readNode)
            {
                code.Add(new ThreeAddressCode(readNode.Variable, "read", null));
                return null;
            }
            else if (node is ComparisonNode compNode)
            {
                string leftResult = GenerateCode(compNode.Left);
                string rightResult = GenerateCode(compNode.Right);
                string temp = GenerateTemp();
                code.Add(new ThreeAddressCode(temp, compNode.Operator, leftResult, rightResult));
                return temp;
            }
            
            return null;
        }

        private string GenerateTemp()
        {
            return $"t{tempCounter++}";
        }

        private string GenerateLabel()
        {
            return $"L{labelCounter++}";
        }
    }
}