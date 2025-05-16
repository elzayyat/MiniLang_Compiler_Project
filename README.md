Trae

13 context(s) used

ASTNode.cs

ArrayDeclarationNode.cs

ArrayAccessNode.cs

SemanticAnalyzer.cs

Token.cs

CallNode.cs

... More ( 7 +)

I need to check if the README.md file exists and what content it has before enhancing it with more details about the project files.

README.md :( 1 - 100 )

I need to check if the README.md file exists and what its content is. Let me search for it in the codebase.

Search in the workspace

# MiniLang Compiler Project
A comprehensive compiler implementation for the MiniLang programming language, built in C#. This project demonstrates the complete compilation pipeline from lexical analysis to intermediate code generation.

## Project Overview
The MiniLang compiler is designed as an educational tool to demonstrate compiler construction principles. It processes a simple programming language with support for variables, control structures, arrays, and basic I/O operations.

## Language Features
- Variable declarations with types (int, bool, string)
- Array support for declaration, access, and assignment
- Control structures (if-else, while loops)
- Input/output operations (read, print)
- Arithmetic and logical expressions
- Comparison operations
- Function calls
## Compiler Architecture
The compiler follows a traditional multi-phase architecture:

1. Lexical Analysis (Scanner)
2. Syntax Analysis (Parser)
3. Semantic Analysis
4. Intermediate Code Generation
## Project Components
### Core Components Token.cs
Defines the token types and token class used in lexical analysis. Tokens represent the smallest units of meaning in the source code, such as keywords, identifiers, operators, and literals. Each token stores its type, lexeme (the actual text), value, and position information (line and column).
 Scanner.cs
Implements the lexical analyzer that converts source code text into a stream of tokens. It recognizes language elements like keywords, identifiers, literals, and operators. The scanner maintains position information for error reporting and handles special cases like string literals and numbers.
 Parser.cs
Implements recursive descent parsing to analyze the syntactic structure of the program and build an Abstract Syntax Tree (AST). It includes methods for parsing different language constructs such as declarations, statements, expressions, and control structures. The parser also handles error detection and reporting for syntax errors.
 ASTNode.cs
Defines the base class for all AST nodes and implements the visitor pattern interface. Contains definitions for various node types that represent different language constructs:

- Program structure nodes
- Declaration nodes
- Statement nodes (if, while, block, etc.)
- Expression nodes (binary operations, comparisons)
- Literal nodes (numbers, booleans, strings) SemanticAnalyzer.cs
Performs semantic analysis on the AST to check for semantic errors such as undeclared variables or type mismatches. It builds and maintains a symbol table to track variable declarations and their types. The analyzer traverses the AST using the visitor pattern to verify semantic correctness.
 IntermediateCodeGenerator.cs
Generates three-address code as an intermediate representation from the AST. It includes the ThreeAddressCode class that represents individual instructions and the generator that traverses the AST to produce code. The generator handles control flow, expressions, and statement translation.

### Node Types StringNode.cs
Represents string literal values in the AST. Contains the string value and implements the visitor pattern interface.
 ArrayDeclarationNode.cs
Represents array declarations in the AST, storing the type, name, and size of the array.
 ArrayAccessNode.cs
Represents array element access operations, containing the array name and an expression for the index.
 ArrayAssignmentNode.cs
Represents assignments to array elements, containing the array name, index expression, and value expression.
 CallNode.cs
Represents function calls in the AST, storing the function name and a list of argument expressions.

### User Interface Program.cs
The entry point of the application that initializes the GUI. Also contains a helper method for printing the AST structure for debugging purposes.
 CompilerGUI.cs
Implements a Windows Forms-based user interface for the compiler. Features include:

- Text editor for source code input
- File operations (open, save)
- Compilation trigger
- Results display with tabs for:
  - Tokens
  - AST visualization
  - Semantic analysis results
  - Generated intermediate code MiniLangCompiler.csproj
The project file that defines build settings and dependencies. Configured as a Windows Forms application targeting .NET 6.0.

## Compilation Process
1. Scanning : The source code is scanned to produce tokens
2. Parsing : Tokens are parsed to build an Abstract Syntax Tree
3. Semantic Analysis : The AST is analyzed for semantic errors
4. Code Generation : Intermediate code is generated from the AST
## Getting Started
### Prerequisites
- .NET 6.0 SDK or later
- Visual Studio 2022 or compatible IDE with Windows Forms support
### Building and Running
1. Clone the repository
2. Open the solution in Visual Studio
3. Build the solution
4. Run the application
### Sample Code
The application comes with a simple example program:

```
var int x;
x = 5;
if (x > 3) {
    print(x);
} else {
    x = 0;
}
```
## Future Enhancements
- Type checking implementation
- Support for functions and procedures
- Code optimization phase
- Target code generation