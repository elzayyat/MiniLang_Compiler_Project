using System;
using System.Collections.Generic;

namespace MiniLangCompiler
{
    public class SemanticAnalyzer
    {
        private Dictionary<string, string> symbolTable = new Dictionary<string, string>();
        private List<string> errors = new List<string>();

        public List<string> Analyze(ASTNode ast)
        {
            errors.Clear();
            symbolTable.Clear();
            
            // Start the analysis from the root node
            AnalyzeNode(ast);
            
            return errors;
        }

        private void AnalyzeNode(ASTNode node)
        {
            if (node is ProgramNode programNode)
            {
                foreach (var decl in programNode.Declarations)
                {
                    AnalyzeNode(decl);
                }
            }
            else if (node is VarDeclarationNode varDeclNode)
            {
                // Check if variable is already declared
                if (symbolTable.ContainsKey(varDeclNode.Name))
                {
                    errors.Add($"Error: Variable '{varDeclNode.Name}' is already declared.");
                }
                else
                {
                    // Add to symbol table
                    symbolTable[varDeclNode.Name] = varDeclNode.Type;
                }
            }
            else if (node is AssignmentNode assignNode)
            {
                // Check if variable is declared
                if (!symbolTable.ContainsKey(assignNode.Variable))
                {
                    errors.Add($"Error: Variable '{assignNode.Variable}' is not declared.");
                }
                
                // Check the expression
                AnalyzeNode(assignNode.Expression);
            }
            else if (node is BinaryOpNode binOpNode)
            {
                AnalyzeNode(binOpNode.Left);
                AnalyzeNode(binOpNode.Right);
                
                // Type checking could be added here
            }
            else if (node is VariableNode varNode)
            {
                // Check if variable is declared
                if (!symbolTable.ContainsKey(varNode.Name))
                {
                    errors.Add($"Error: Variable '{varNode.Name}' is not declared.");
                }
            }
            else if (node is IfStatementNode ifNode)
            {
                AnalyzeNode(ifNode.Condition);
                AnalyzeNode(ifNode.ThenBranch);
                
                if (ifNode.ElseBranch != null)
                {
                    AnalyzeNode(ifNode.ElseBranch);
                }
            }
            else if (node is WhileStatementNode whileNode)
            {
                AnalyzeNode(whileNode.Condition);
                AnalyzeNode(whileNode.Body);
            }
            else if (node is BlockNode blockNode)
            {
                foreach (var stmt in blockNode.Statements)
                {
                    AnalyzeNode(stmt);
                }
            }
            else if (node is PrintStatementNode printNode)
            {
                AnalyzeNode(printNode.Expression);
            }
            else if (node is ReadStatementNode readNode)
            {
                // Check if variable is declared
                if (!symbolTable.ContainsKey(readNode.Variable))
                {
                    errors.Add($"Error: Variable '{readNode.Variable}' is not declared.");
                }
            }
            else if (node is ComparisonNode compNode)
            {
                AnalyzeNode(compNode.Left);
                AnalyzeNode(compNode.Right);
            }
        }
    }
}