using System;

namespace MiniLangCompiler
{
    public class ArrayAssignmentNode : ASTNode
    {
        public string Name { get; }
        public ASTNode Index { get; }
        public ASTNode Value { get; }
        
        public ArrayAssignmentNode(string name, ASTNode index, ASTNode value)
        {
            Name = name;
            Index = index;
            Value = value;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}