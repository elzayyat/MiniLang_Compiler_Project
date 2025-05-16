using System;

namespace MiniLangCompiler
{
    public enum TokenType
    {
        // Keywords
        VAR, IF, ELSE, WHILE, PRINT, READ, TRUE, FALSE,
        
        // Types
        INT, BOOL, STRING,
        
        // Operators
        PLUS, MINUS, MULTIPLY, DIVIDE, ASSIGN,
        
        // Comparison operators
        EQUAL, NOT_EQUAL, LESS, LESS_EQUAL, GREATER, GREATER_EQUAL,
        
        // Logical operators
        NOT,
        
        // Delimiters
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, LEFT_BRACKET, RIGHT_BRACKET, SEMICOLON, COMMA,
        
        // Literals
        IDENTIFIER, NUMBER, STRING_LITERAL,
        
        // Special
        EOF
    }
    
    public class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public object Value { get; }
        public int Line { get; }
        public int Column { get; }
        
        public Token(TokenType type, string lexeme, object value, int line, int column)
        {
            Type = type;
            Lexeme = lexeme;
            Value = value;
            Line = line;
            Column = column;
        }
        
        public override string ToString()
        {
            return $"{Type} {Lexeme} {Value}";
        }
    }
}