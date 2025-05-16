using System;
using System.Collections.Generic;

namespace MiniLangCompiler
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;
        
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }
        
        public ASTNode Parse()
        {
            ProgramNode program = new ProgramNode();
            
            while (!IsAtEnd())
            {
                program.Declarations.Add(Declaration());
            }
            
            return program;
        }
        
        private ASTNode Declaration()
        {
            // Check if declaration starts with a type (C++ style)
            if (Match(TokenType.INT, TokenType.BOOL, TokenType.STRING))
            {
                TokenType type = Previous().Type;
                string typeName = TypeTokenToString(type);
                
                Token name = Consume(TokenType.IDENTIFIER, "Expected variable name");
                
                // Check if this is an array declaration
                if (Match(TokenType.LEFT_BRACKET))
                {
                    // Parse array size
                    Token sizeToken = Consume(TokenType.NUMBER, "Expected array size");
                    int size = Convert.ToInt32(sizeToken.Value);
                    Consume(TokenType.RIGHT_BRACKET, "Expected ']' after array size");
                    Consume(TokenType.SEMICOLON, "Expected ';' after array declaration");
                    
                    // Create an array declaration node
                    return new ArrayDeclarationNode(typeName, name.Lexeme, size);
                }
                
                // Handle optional initialization
                ASTNode initializer = null;
                if (Match(TokenType.ASSIGN))
                {
                    initializer = Expression();
                }
                
                Consume(TokenType.SEMICOLON, "Expected ';' after variable declaration");
                
                // Create a variable declaration node
                VarDeclarationNode declaration = new VarDeclarationNode(typeName, name.Lexeme);
                
                // If there was an initializer, also create an assignment node
                if (initializer != null)
                {
                    // Add both the declaration and assignment to a block
                    BlockNode block = new BlockNode();
                    block.Statements.Add(declaration);
                    block.Statements.Add(new AssignmentNode(name.Lexeme, initializer));
                    return block;
                }
                
                return declaration;
            }
            // Also keep the original var-style declarations
            else if (Match(TokenType.VAR))
            {
                return VarDeclaration();
            }
            
            return Statement();
        }
        
        private ASTNode VarDeclaration()
        {
            TokenType type;
            if (Match(TokenType.INT, TokenType.BOOL, TokenType.STRING))
            {
                type = Previous().Type;
            }
            else
            {
                throw new Exception($"Expected type specifier at line {Peek().Line}, column {Peek().Column}");
            }
            
            string typeName = TypeTokenToString(type);
            
            Token name = Consume(TokenType.IDENTIFIER, "Expected variable name");
            
            Consume(TokenType.SEMICOLON, "Expected ';' after variable declaration");
            
            return new VarDeclarationNode(typeName, name.Lexeme);
        }
        
        private string TypeTokenToString(TokenType type)
        {
            switch (type)
            {
                case TokenType.INT: return "int";
                case TokenType.BOOL: return "bool";
                case TokenType.STRING: return "string";
                default: return "unknown";
            }
        }
        
        private ASTNode Statement()
        {
            if (Match(TokenType.IF))
            {
                return IfStatement();
            }
            
            if (Match(TokenType.WHILE))
            {
                return WhileStatement();
            }
            
            if (Match(TokenType.PRINT))
            {
                return PrintStatement();
            }
            
            if (Match(TokenType.READ))
            {
                return ReadStatement();
            }
            
            if (Match(TokenType.LEFT_BRACE))
            {
                return BlockStatement();
            }
            
            return ExpressionStatement();
        }
        
        private ASTNode IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expected '(' after 'if'");
            ASTNode condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after if condition");
            
            ASTNode thenBranch = Statement();
            ASTNode elseBranch = null;
            
            if (Match(TokenType.ELSE))
            {
                elseBranch = Statement();
            }
            
            return new IfStatementNode(condition, thenBranch, elseBranch);
        }
        
        private ASTNode WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expected '(' after 'while'");
            ASTNode condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after while condition");
            
            ASTNode body = Statement();
            
            return new WhileStatementNode(condition, body);
        }
        
        private ASTNode PrintStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expected '(' after 'print'");
            ASTNode value = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after expression");
            Consume(TokenType.SEMICOLON, "Expected ';' after print statement");
            
            return new PrintStatementNode(value);
        }
        
        private ASTNode ReadStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expected '(' after 'read'");
            Token name = Consume(TokenType.IDENTIFIER, "Expected variable name");
            Consume(TokenType.RIGHT_PAREN, "Expected ')' after variable name");
            Consume(TokenType.SEMICOLON, "Expected ';' after read statement");
            
            return new ReadStatementNode(name.Lexeme);
        }
        
        private ASTNode BlockStatement()
        {
            BlockNode block = new BlockNode();
            
            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                block.Statements.Add(Declaration());
            }
            
            Consume(TokenType.RIGHT_BRACE, "Expected '}' after block");
            
            return block;
        }
        
        private ASTNode ExpressionStatement()
        {
            ASTNode expr = Expression();
            Consume(TokenType.SEMICOLON, "Expected ';' after expression");
            return expr;
        }
        
        private ASTNode Expression()
        {
            if (Check(TokenType.IDENTIFIER))
            {
                // Check for array assignment: array[index] = value
                if (CheckNext(TokenType.LEFT_BRACKET))
                {
                    string name = Advance().Lexeme; // Consume identifier
                    Advance(); // Consume '['
                    ASTNode index = Expression();
                    Consume(TokenType.RIGHT_BRACKET, "Expected ']' after array index");
                    
                    if (Match(TokenType.ASSIGN))
                    {
                        ASTNode value = Expression();
                        return new ArrayAssignmentNode(name, index, value);
                    }
                    
                    // If not an assignment, it's an array access expression
                    return new ArrayAccessNode(name, index);
                }
                // Check for regular assignment: variable = value
                else if (CheckNext(TokenType.ASSIGN))
                {
                    Token name = Advance();
                    Advance(); // Consume '='
                    ASTNode value = Expression();
                    return new AssignmentNode(name.Lexeme, value);
                }
            }
            
            return Comparison();
        }
        
        private ASTNode Comparison()
        {
            ASTNode expr = Term();
            
            while (Match(TokenType.LESS, TokenType.LESS_EQUAL, TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.EQUAL, TokenType.NOT_EQUAL))
            {
                string op = OperatorToString(Previous().Type);
                ASTNode right = Term();
                expr = new ComparisonNode(op, expr, right);
            }
            
            return expr;
        }
        
        private ASTNode Term()
        {
            ASTNode expr = Factor();
            
            while (Match(TokenType.PLUS, TokenType.MINUS))
            {
                string op = OperatorToString(Previous().Type);
                ASTNode right = Factor();
                expr = new BinaryOpNode(op, expr, right);
            }
            
            return expr;
        }
        
        private ASTNode Factor()
        {
            ASTNode expr = Primary();
            
            while (Match(TokenType.MULTIPLY, TokenType.DIVIDE))
            {
                string op = OperatorToString(Previous().Type);
                ASTNode right = Primary();
                expr = new BinaryOpNode(op, expr, right);
            }
            
            return expr;
        }
        
        private ASTNode Primary()
        {
            if (Match(TokenType.TRUE))
            {
                return new BooleanNode(true);
            }
            
            if (Match(TokenType.FALSE))
            {
                return new BooleanNode(false);
            }
            
            if (Match(TokenType.NUMBER))
            {
                return new NumberNode((double)Previous().Value);
            }
            
            // Add support for string literals
            if (Match(TokenType.STRING_LITERAL))
            {
                return new StringNode((string)Previous().Value);
            }
            
            if (Match(TokenType.IDENTIFIER))
            {
                string name = Previous().Lexeme;
                
                // Check if this is an array access
                if (Match(TokenType.LEFT_BRACKET))
                {
                    ASTNode index = Expression();
                    Consume(TokenType.RIGHT_BRACKET, "Expected ']' after array index");
                    return new ArrayAccessNode(name, index);
                }
                
                // Check if this is a function call
                if (Match(TokenType.LEFT_PAREN))
                {
                    List<ASTNode> arguments = new List<ASTNode>();
                    
                    // Parse arguments
                    if (!Check(TokenType.RIGHT_PAREN))
                    {
                        do
                        {
                            arguments.Add(Expression());
                        } while (Match(TokenType.COMMA));
                    }
                    
                    Consume(TokenType.RIGHT_PAREN, "Expected ')' after function arguments");
                    return new CallNode(name, arguments);
                }
                
                return new VariableNode(name);
            }
            
            if (Match(TokenType.LEFT_PAREN))
            {
                ASTNode expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expected ')' after expression");
                return expr;
            }
            
            throw new Exception($"Expected expression at line {Peek().Line}, column {Peek().Column}");
        }
        
        private string OperatorToString(TokenType type)
        {
            switch (type)
            {
                case TokenType.PLUS: return "+";
                case TokenType.MINUS: return "-";
                case TokenType.MULTIPLY: return "*";
                case TokenType.DIVIDE: return "/";
                case TokenType.LESS: return "<";
                case TokenType.LESS_EQUAL: return "<=";
                case TokenType.GREATER: return ">";
                case TokenType.GREATER_EQUAL: return ">=";
                case TokenType.EQUAL: return "==";
                case TokenType.NOT_EQUAL: return "!=";
                default: return "unknown";
            }
        }
        
        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            
            return false;
        }
        
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }
        
        private bool CheckNext(TokenType type)
        {
            if (current + 1 >= tokens.Count) return false;
            return tokens[current + 1].Type == type;
        }
        
        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }
        
        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }
        
        private Token Peek()
        {
            return tokens[current];
        }
        
        private Token Previous()
        {
            return tokens[current - 1];
        }
        
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            
            throw new Exception($"{message} at line {Peek().Line}, column {Peek().Column}");
        }
    }
}