using System;
using System.Collections.Generic;

namespace MiniLangCompiler
{
    public class CallNode : ASTNode
    {
        public string Name { get; }
        public List<ASTNode> Arguments { get; }
        
        public CallNode(string name, List<ASTNode> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}