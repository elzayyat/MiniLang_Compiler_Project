using System;

namespace MiniLangCompiler
{
    public class ArrayAccessNode : ASTNode
    {
        public string Name { get; }
        public ASTNode Index { get; }
        
        public ArrayAccessNode(string name, ASTNode index)
        {
            Name = name;
            Index = index;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}