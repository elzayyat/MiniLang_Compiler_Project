using System;

namespace MiniLangCompiler
{
    public class ArrayDeclarationNode : ASTNode
    {
        public string Type { get; }
        public string Name { get; }
        public int Size { get; }
        
        public ArrayDeclarationNode(string type, string name, int size)
        {
            Type = type;
            Name = name;
            Size = size;
        }
        
        public override void Accept(IASTVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}