using System;

namespace MiniLangCompiler
{
    public class StringNode : ASTNode
    {
        public string Value { get; }
        
        public StringNode(string value)
        {
            Value = value;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}