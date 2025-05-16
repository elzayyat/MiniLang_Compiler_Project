using System;
using System.Collections.Generic;
using System.Text;

namespace MiniLangCompiler
{
    public class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();
        
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private int column = 1;
        
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            { "var", TokenType.VAR },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "while", TokenType.WHILE },
            { "print", TokenType.PRINT },
            { "read", TokenType.READ },
            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
            { "int", TokenType.INT },
            { "bool", TokenType.BOOL },
            { "string", TokenType.STRING }
        };
        
        public Scanner(string source)
        {
            this.source = source;
        }
        
        public List<Token> ScanAllTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }
            
            tokens.Add(new Token(TokenType.EOF, "", null, line, column));
            return tokens;
        }
        
        private void ScanToken()
        {
            char c = Advance();
            
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case '[':
                    AddToken(TokenType.LEFT_BRACKET);
                    break;
                case ']':
                    AddToken(TokenType.RIGHT_BRACKET);
                    break;
                case ',':
                    AddToken(TokenType.COMMA);
                    break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '+': AddToken(TokenType.PLUS); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '*': AddToken(TokenType.MULTIPLY); break;
                case '/': AddToken(TokenType.DIVIDE); break;
                
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL : TokenType.ASSIGN);
                    break;
                case '!':
                    AddToken(Match('=') ? TokenType.NOT_EQUAL : TokenType.NOT_EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                
                case '"': ScanString(); break;
                
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace
                    break;
                
                case '\n':
                    line++;
                    column = 1;
                    break;
                
                default:
                    if (IsDigit(c))
                    {
                        ScanNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        ScanIdentifier();
                    }
                    else
                    {
                        throw new Exception($"Unexpected character '{c}' at line {line}, column {column}");
                    }
                    break;
            }
        }
        
        private void ScanString()
        {
            StringBuilder value = new StringBuilder();
            
            while (!IsAtEnd() && Peek() != '"')
            {
                if (Peek() == '\n')
                {
                    line++;
                    column = 1;
                }
                value.Append(Advance());
            }
            
            if (IsAtEnd())
            {
                throw new Exception($"Unterminated string at line {line}, column {column}");
            }
            
            // Consume the closing "
            Advance();
            
            AddToken(TokenType.STRING_LITERAL, value.ToString());
        }
        
        private void ScanNumber()
        {
            while (IsDigit(Peek())) Advance();
            
            // Look for a decimal part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();
                
                while (IsDigit(Peek())) Advance();
            }
            
            double value = double.Parse(source.Substring(start, current - start));
            AddToken(TokenType.NUMBER, value);
        }
        
        private void ScanIdentifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();
            
            string text = source.Substring(start, current - start);
            
            TokenType type;
            if (!keywords.TryGetValue(text, out type))
            {
                type = TokenType.IDENTIFIER;
            }
            
            AddToken(type);
        }
        
        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;
            
            current++;
            column++;
            return true;
        }
        
        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }
        
        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }
        
        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }
        
        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
        
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
        
        private char Advance()
        {
            current++;
            column++;
            return source[current - 1];
        }
        
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }
        
        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line, column - (current - start)));
        }
        
        private bool IsAtEnd()
        {
            return current >= source.Length;
        }
    }
}