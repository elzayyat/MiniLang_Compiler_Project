using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MiniLangCompiler
{
    public class CompilerGUI : Form
    {
        private TextBox sourceCodeTextBox;
        private Button compileButton;
        private TabControl resultTabControl;
        private TabPage tokensTabPage;
        private TabPage astTabPage;
        private TabPage semanticTabPage;
        private TabPage codeTabPage;
        private RichTextBox tokensTextBox;
        private TreeView astTreeView;
        private RichTextBox semanticTextBox;
        private RichTextBox codeTextBox;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem openMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem exitMenuItem;

        public CompilerGUI()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Form settings
            this.Text = "MiniLang Compiler";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Menu
            menuStrip = new MenuStrip();
            fileMenu = new ToolStripMenuItem("File");
            openMenuItem = new ToolStripMenuItem("Open", null, OpenFile);
            saveMenuItem = new ToolStripMenuItem("Save", null, SaveFile);
            exitMenuItem = new ToolStripMenuItem("Exit", null, (s, e) => this.Close());
            
            fileMenu.DropDownItems.Add(openMenuItem);
            fileMenu.DropDownItems.Add(saveMenuItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(exitMenuItem);
            menuStrip.Items.Add(fileMenu);
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;

            // Source code text box
            sourceCodeTextBox = new TextBox();
            sourceCodeTextBox.Multiline = true;
            sourceCodeTextBox.ScrollBars = ScrollBars.Both;
            sourceCodeTextBox.Font = new Font("Consolas", 12);
            sourceCodeTextBox.Location = new Point(10, 30);
            sourceCodeTextBox.Size = new Size(980, 300);
            sourceCodeTextBox.Text = "var int x;\nx = 5;\nif (x > 3) {\n    print(x);\n} else {\n    x = 0;\n}";
            this.Controls.Add(sourceCodeTextBox);

            // Compile button
            compileButton = new Button();
            compileButton.Text = "Compile";
            compileButton.Location = new Point(10, 340);
            compileButton.Size = new Size(100, 30);
            compileButton.Click += CompileButtonClick;
            this.Controls.Add(compileButton);

            // Result tab control
            resultTabControl = new TabControl();
            resultTabControl.Location = new Point(10, 380);
            resultTabControl.Size = new Size(980, 280);
            
            // Tokens tab
            tokensTabPage = new TabPage("Tokens");
            tokensTextBox = new RichTextBox();
            tokensTextBox.ReadOnly = true;
            tokensTextBox.Font = new Font("Consolas", 10);
            tokensTextBox.Dock = DockStyle.Fill;
            tokensTabPage.Controls.Add(tokensTextBox);
            
            // AST tab
            astTabPage = new TabPage("AST");
            astTreeView = new TreeView();
            astTreeView.Dock = DockStyle.Fill;
            astTreeView.Font = new Font("Consolas", 10);
            astTabPage.Controls.Add(astTreeView);
            
            // Semantic tab
            semanticTabPage = new TabPage("Semantic Analysis");
            semanticTextBox = new RichTextBox();
            semanticTextBox.ReadOnly = true;
            semanticTextBox.Font = new Font("Consolas", 10);
            semanticTextBox.Dock = DockStyle.Fill;
            semanticTabPage.Controls.Add(semanticTextBox);
            
            // Code tab
            codeTabPage = new TabPage("Intermediate Code");
            codeTextBox = new RichTextBox();
            codeTextBox.ReadOnly = true;
            codeTextBox.Font = new Font("Consolas", 10);
            codeTextBox.Dock = DockStyle.Fill;
            codeTabPage.Controls.Add(codeTextBox);
            
            resultTabControl.TabPages.Add(tokensTabPage);
            resultTabControl.TabPages.Add(astTabPage);
            resultTabControl.TabPages.Add(semanticTabPage);
            resultTabControl.TabPages.Add(codeTabPage);
            this.Controls.Add(resultTabControl);
        }

        private void CompileButtonClick(object sender, EventArgs e)
        {
            try
            {
                string sourceCode = sourceCodeTextBox.Text;
                
                // Clear previous results
                tokensTextBox.Clear();
                astTreeView.Nodes.Clear();
                semanticTextBox.Clear();
                codeTextBox.Clear();
                
                // Scan tokens
                Scanner scanner = new Scanner(sourceCode);
                List<Token> tokens = scanner.ScanAllTokens();
                
                // Display tokens
                tokensTextBox.AppendText($"Total Tokens: {tokens.Count}\n\n");
                foreach (var token in tokens)
                {
                    tokensTextBox.AppendText(token.ToString() + Environment.NewLine);
                }
                
                // Parse tokens
                Parser parser = new Parser(tokens);
                ASTNode ast = parser.Parse();
                
                // Display AST
                BuildASTTreeView(ast, null);
                
                // Semantic analysis
                SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer();
                List<string> semanticErrors = semanticAnalyzer.Analyze(ast);
                
                if (semanticErrors.Count > 0)
                {
                    semanticTextBox.ForeColor = Color.Red;
                    foreach (var error in semanticErrors)
                    {
                        semanticTextBox.AppendText(error + Environment.NewLine);
                    }
                }
                else
                {
                    semanticTextBox.ForeColor = Color.Green;
                    semanticTextBox.AppendText("No semantic errors found.");
                }
                
                // Generate intermediate code
                IntermediateCodeGenerator codeGenerator = new IntermediateCodeGenerator();
                List<ThreeAddressCode> code = codeGenerator.Generate(ast);
                
                foreach (var line in code)
                {
                    codeTextBox.AppendText(line.ToString() + Environment.NewLine);
                }
                
                MessageBox.Show("Compilation successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Compilation error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BuildASTTreeView(ASTNode node, TreeNode parentNode)
        {
            TreeNode newNode;
            
            if (parentNode == null)
            {
                newNode = astTreeView.Nodes.Add("AST Root");
            }
            else
            {
                newNode = parentNode;
            }
            
            if (node is ProgramNode programNode)
            {
                newNode.Text = "Program";
                foreach (var decl in programNode.Declarations)
                {
                    BuildASTTreeView(decl, newNode);
                }
            }
            else if (node is VarDeclarationNode varDeclNode)
            {
                newNode = parentNode.Nodes.Add($"VarDeclaration: {varDeclNode.Type} {varDeclNode.Name}");
            }
            else if (node is AssignmentNode assignNode)
            {
                newNode = parentNode.Nodes.Add($"Assignment: {assignNode.Variable} =");
                BuildASTTreeView(assignNode.Expression, newNode);
            }
            else if (node is BinaryOpNode binOpNode)
            {
                newNode = parentNode.Nodes.Add($"BinaryOp: {binOpNode.Operator}");
                BuildASTTreeView(binOpNode.Left, newNode);
                BuildASTTreeView(binOpNode.Right, newNode);
            }
            else if (node is VariableNode varNode)
            {
                newNode = parentNode.Nodes.Add($"Variable: {varNode.Name}");
            }
            else if (node is NumberNode numNode)
            {
                newNode = parentNode.Nodes.Add($"Number: {numNode.Value}");
            }
            else if (node is BooleanNode boolNode)
            {
                newNode = parentNode.Nodes.Add($"Boolean: {boolNode.Value}");
            }
            else if (node is IfStatementNode ifNode)
            {
                newNode = parentNode.Nodes.Add("If");
                TreeNode condNode = newNode.Nodes.Add("Condition");
                BuildASTTreeView(ifNode.Condition, condNode);
                
                TreeNode thenNode = newNode.Nodes.Add("Then");
                BuildASTTreeView(ifNode.ThenBranch, thenNode);
                
                if (ifNode.ElseBranch != null)
                {
                    TreeNode elseNode = newNode.Nodes.Add("Else");
                    BuildASTTreeView(ifNode.ElseBranch, elseNode);
                }
            }
            else if (node is WhileStatementNode whileNode)
            {
                newNode = parentNode.Nodes.Add("While");
                TreeNode condNode = newNode.Nodes.Add("Condition");
                BuildASTTreeView(whileNode.Condition, condNode);
                
                TreeNode bodyNode = newNode.Nodes.Add("Body");
                BuildASTTreeView(whileNode.Body, bodyNode);
            }
            else if (node is BlockNode blockNode)
            {
                newNode = parentNode.Nodes.Add("Block");
                foreach (var stmt in blockNode.Statements)
                {
                    BuildASTTreeView(stmt, newNode);
                }
            }
            else if (node is PrintStatementNode printNode)
            {
                newNode = parentNode.Nodes.Add("Print");
                BuildASTTreeView(printNode.Expression, newNode);
            }
            else if (node is ReadStatementNode readNode)
            {
                newNode = parentNode.Nodes.Add($"Read: {readNode.Variable}");
            }
            else if (node is ComparisonNode compNode)
            {
                newNode = parentNode.Nodes.Add($"Comparison: {compNode.Operator}");
                BuildASTTreeView(compNode.Left, newNode);
                BuildASTTreeView(compNode.Right, newNode);
            }
        }
        
        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    sourceCodeTextBox.Text = File.ReadAllText(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void SaveFile(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, sourceCodeTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}