using System;
using System.Collections.Generic;

namespace MiniLangCompiler
{
    // Base AST Node class
    public abstract class ASTNode
    {
        public abstract void Accept(IASTVisitor visitor);
    }

    // Interface for visitor pattern
    public interface IASTVisitor
    {
        void Visit(ProgramNode node);
        void Visit(VarDeclarationNode node);
        void Visit(AssignmentNode node);
        void Visit(BinaryOpNode node);
        void Visit(VariableNode node);
        void Visit(NumberNode node);
        void Visit(BooleanNode node);
        void Visit(IfStatementNode node);
        void Visit(WhileStatementNode node);
        void Visit(BlockNode node);
        void Visit(PrintStatementNode node);
        void Visit(ReadStatementNode node);
        void Visit(ComparisonNode node);
        void Visit(StringNode node); // Add this line
        void Visit(ArrayDeclarationNode node);
        void Visit(ArrayAccessNode node);
        void Visit(ArrayAssignmentNode node);
        void Visit(CallNode node);
    }

    // Program node - the root of our AST
    public class ProgramNode : ASTNode
    {
        public List<ASTNode> Declarations { get; set; }

        public ProgramNode()
        {
            Declarations = new List<ASTNode>();
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Variable declaration node
    public class VarDeclarationNode : ASTNode
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public VarDeclarationNode(string type, string name)
        {
            Type = type;
            Name = name;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Assignment node
    public class AssignmentNode : ASTNode
    {
        public string Variable { get; set; }
        public ASTNode Expression { get; set; }

        public AssignmentNode(string variable, ASTNode expression)
        {
            Variable = variable;
            Expression = expression;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Binary operation node
    public class BinaryOpNode : ASTNode
    {
        public string Operator { get; set; }
        public ASTNode Left { get; set; }
        public ASTNode Right { get; set; }

        public BinaryOpNode(string op, ASTNode left, ASTNode right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Variable node
    public class VariableNode : ASTNode
    {
        public string Name { get; set; }

        public VariableNode(string name)
        {
            Name = name;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Number node
    public class NumberNode : ASTNode
    {
        public double Value { get; set; }

        public NumberNode(double value)
        {
            Value = value;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Boolean node
    public class BooleanNode : ASTNode
    {
        public bool Value { get; set; }

        public BooleanNode(bool value)
        {
            Value = value;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // If statement node
    public class IfStatementNode : ASTNode
    {
        public ASTNode Condition { get; set; }
        public ASTNode ThenBranch { get; set; }
        public ASTNode ElseBranch { get; set; }

        public IfStatementNode(ASTNode condition, ASTNode thenBranch, ASTNode elseBranch = null)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // While statement node
    public class WhileStatementNode : ASTNode
    {
        public ASTNode Condition { get; set; }
        public ASTNode Body { get; set; }

        public WhileStatementNode(ASTNode condition, ASTNode body)
        {
            Condition = condition;
            Body = body;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Block node
    public class BlockNode : ASTNode
    {
        public List<ASTNode> Statements { get; set; }

        public BlockNode()
        {
            Statements = new List<ASTNode>();
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Print statement node
    public class PrintStatementNode : ASTNode
    {
        public ASTNode Expression { get; set; }

        public PrintStatementNode(ASTNode expression)
        {
            Expression = expression;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Read statement node
    public class ReadStatementNode : ASTNode
    {
        public string Variable { get; set; }

        public ReadStatementNode(string variable)
        {
            Variable = variable;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Comparison node
    public class ComparisonNode : ASTNode
    {
        public string Operator { get; set; }
        public ASTNode Left { get; set; }
        public ASTNode Right { get; set; }

        public ComparisonNode(string op, ASTNode left, ASTNode right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}