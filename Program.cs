using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MiniLangCompiler
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CompilerGUI());
        }
        
        // Helper method to print AST structure
        static void PrintAST(ASTNode node, int indent)
        {
            string indentation = new string(' ', indent * 2);
            
            if (node is ProgramNode programNode)
            {
                Console.WriteLine($"{indentation}Program");
                foreach (var decl in programNode.Declarations)
                {
                    PrintAST(decl, indent + 1);
                }
            }
            else if (node is VarDeclarationNode varDeclNode)
            {
                Console.WriteLine($"{indentation}VarDeclaration: {varDeclNode.Type} {varDeclNode.Name}");
            }
            else if (node is AssignmentNode assignNode)
            {
                Console.WriteLine($"{indentation}Assignment: {assignNode.Variable} =");
                PrintAST(assignNode.Expression, indent + 1);
            }
            else if (node is BinaryOpNode binOpNode)
            {
                Console.WriteLine($"{indentation}BinaryOp: {binOpNode.Operator}");
                PrintAST(binOpNode.Left, indent + 1);
                PrintAST(binOpNode.Right, indent + 1);
            }
            else if (node is VariableNode varNode)
            {
                Console.WriteLine($"{indentation}Variable: {varNode.Name}");
            }
            else if (node is NumberNode numNode)
            {
                Console.WriteLine($"{indentation}Number: {numNode.Value}");
            }
            else if (node is BooleanNode boolNode)
            {
                Console.WriteLine($"{indentation}Boolean: {boolNode.Value}");
            }
            else if (node is IfStatementNode ifNode)
            {
                Console.WriteLine($"{indentation}If:");
                Console.WriteLine($"{indentation}  Condition:");
                PrintAST(ifNode.Condition, indent + 2);
                Console.WriteLine($"{indentation}  Then:");
                PrintAST(ifNode.ThenBranch, indent + 2);
                if (ifNode.ElseBranch != null)
                {
                    Console.WriteLine($"{indentation}  Else:");
                    PrintAST(ifNode.ElseBranch, indent + 2);
                }
            }
            else if (node is WhileStatementNode whileNode)
            {
                Console.WriteLine($"{indentation}While:");
                Console.WriteLine($"{indentation}  Condition:");
                PrintAST(whileNode.Condition, indent + 2);
                Console.WriteLine($"{indentation}  Body:");
                PrintAST(whileNode.Body, indent + 2);
            }
            else if (node is BlockNode blockNode)
            {
                Console.WriteLine($"{indentation}Block:");
                foreach (var stmt in blockNode.Statements)
                {
                    PrintAST(stmt, indent + 1);
                }
            }
            else if (node is PrintStatementNode printNode)
            {
                Console.WriteLine($"{indentation}Print:");
                PrintAST(printNode.Expression, indent + 1);
            }
            else if (node is ReadStatementNode readNode)
            {
                Console.WriteLine($"{indentation}Read: {readNode.Variable}");
            }
            else if (node is ComparisonNode compNode)
            {
                Console.WriteLine($"{indentation}Comparison: {compNode.Operator}");
                PrintAST(compNode.Left, indent + 1);
                PrintAST(compNode.Right, indent + 1);
            }
            else if (node is StringNode strNode)
            {
                Console.WriteLine($"{indentation}String: \"{strNode.Value}\"");
            }
            // Add these to your PrintAST method
            else if (node is ArrayDeclarationNode arrayDeclNode)
            {
                Console.WriteLine($"{indentation}ArrayDeclaration: {arrayDeclNode.Type} {arrayDeclNode.Name}[{arrayDeclNode.Size}]");
            }
            else if (node is ArrayAccessNode arrayAccessNode)
            {
                Console.WriteLine($"{indentation}ArrayAccess: {arrayAccessNode.Name}[");
                PrintAST(arrayAccessNode.Index, indent + 1);
                Console.WriteLine($"{indentation}]");
            }
            else if (node is ArrayAssignmentNode arrayAssignNode)
            {
                Console.WriteLine($"{indentation}ArrayAssignment: {arrayAssignNode.Name}[");
                PrintAST(arrayAssignNode.Index, indent + 1);
                Console.WriteLine($"{indentation}] =");
                PrintAST(arrayAssignNode.Value, indent + 1);
            }
            else if (node is CallNode callNode)
            {
                Console.WriteLine($"{indentation}Call: {callNode.Name}(");
                foreach (var arg in callNode.Arguments)
                {
                    PrintAST(arg, indent + 1);
                }
                Console.WriteLine($"{indentation})");
            }
        }
    }
}