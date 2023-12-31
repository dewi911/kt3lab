using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

public class Lexeme
{
    public string Value { get; set; }
    public string Type { get; set; }

    public Lexeme(string value, string type)
    {
        Value = value;
        Type = type;
    }
}

public class Lexer
{
    private readonly string inputText;
    private readonly List<Lexeme> lexemeTable;
    List<string> lexemes = new List<string>();
    List<string> types = new List<string>();
    public Lexer(string inputText)
    {
        this.inputText = inputText;
        lexemeTable = new List<Lexeme>();

    }

    public List<Lexeme> Analyze()
    {
        string[] lines = inputText.Split('\n');
        string fullText = string.Join(" ", lines);

        
        string idPattern = @"[a-zA-Z_][a-zA-Z0-9_]*";
        string charConstPattern = @"'[^']*'";
        string commentPattern = @"\/\/.*";

        string lexemePattern = $"({idPattern})|({charConstPattern})|({commentPattern})";

       
        MatchCollection matches = Regex.Matches(fullText, lexemePattern);
       
        foreach (Match match in matches)
        {
            string value = match.Value;
            string type = "a"; 
            lexemeTable.Add(new Lexeme(value, type));
            lexemes.Add(value);
            types.Add(type);
        }

        return lexemeTable;
    }
}

public class SyntaxAnalyzer
{
    private List<Lexeme> lexemeTable;
    public int currentIndex;
    

    public SyntaxAnalyzer(List<Lexeme> lexemeTable)
    {
        
            this.lexemeTable = lexemeTable;
            currentIndex = 0;
        
    }

    public bool Parse()
    {
        try
        {
            S();
            Console.WriteLine(currentIndex);
            return currentIndex == lexemeTable.Count;
        }
        catch
        {
            return false;
        }
    }

    private void S()
    {
        Match("a:=");
        Console.WriteLine(Match(";"));
        F();
        Match(";");
    }

    private void F()
    {
        T();
        if (Match("+"))
        {
            F();
        }
    }

    private void T()
    {
        E();
        if (Match("."))
        {
            E();
        }
        else if (Match("I"))
        {
            E();
        }
    }

    private void E()
    {
        if (Match("("))
        {
            F();
            Match(")");
        }
        else if (Match("-"))
        {
            Match("(");
            F();
            Match(")");
        }
        else
        {
            Match("a");
        }
    }

    private bool Match(string expected)
    {
        if (currentIndex < lexemeTable.Count && lexemeTable[currentIndex].Type == expected)
        {
            currentIndex++;
            return true;
        }
        return false;
    }
}

public class SyntaxTree
{
    private readonly StringBuilder tree;
    private int depth;

    public SyntaxTree()
    {
        tree = new StringBuilder();
        depth = 0;
    }

    public void AddNode(string name)
    {
        tree.AppendLine(new string(' ', depth * 2) + name);
        depth++;
    }

    public void ReturnToParent()
    {
        depth--;
    }

    public override string ToString()
    {
        return tree.ToString();
    }
}

public class SyntaxTreeBuilder
{
    private List<Lexeme> lexemeTable;
    private SyntaxTree syntaxTree;
    private int currentIndex;

    public SyntaxTreeBuilder(List<Lexeme> lexemeTable, SyntaxTree syntaxTree)
    {
        this.lexemeTable = lexemeTable;
        this.syntaxTree = syntaxTree;
        currentIndex = 0;
    }

    public void BuildTree()
    {
        try
        {
            S();
        }
        catch
        {
            Console.WriteLine("Syntax analysis failed.");
            return;
        }

        Console.WriteLine("Syntax analysis successful. Syntax Tree:");
        Console.WriteLine(syntaxTree);
    }

    private void S()
    {
        syntaxTree.AddNode("S");
        Match("a:=");
        F();
        Match(";");
        syntaxTree.ReturnToParent();
    }

    private void F()
    {
        syntaxTree.AddNode("F");
        T();
        if (Match("+"))
        {
            F();
        }
        syntaxTree.ReturnToParent();
    }

    private void T()
    {
        syntaxTree.AddNode("T");
        E();
        if (Match("."))
        {
            E();
        }
        else if (Match("I"))
        {
            E();
        }
        syntaxTree.ReturnToParent();
    }

    private void E()
    {
        syntaxTree.AddNode("E");
        if (Match("("))
        {
            F();
            Match(")");
        }
        else if (Match("-"))
        {
            Match("(");
            F();
            Match(")");
        }
        else
        {
            Match("a");
        }
        syntaxTree.ReturnToParent();
    }

    public bool Match(string expected)
    {
        if (currentIndex < lexemeTable.Count && lexemeTable[currentIndex].Type == expected)
        {
            syntaxTree.AddNode(lexemeTable[currentIndex].Value);
            currentIndex++;
            return true;
        }
        return false;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        string inputText = "a:=b+c;";
        Lexer lexer = new Lexer(inputText);
        List<Lexeme> lexemeTable = lexer.Analyze();

        Console.WriteLine("Lexeme Table:");
        foreach (var lexeme in lexemeTable)
        {
            Console.WriteLine($"{lexeme.Value} : {lexeme.Type}");
        }

        SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(lexemeTable);
        bool asdad = syntaxAnalyzer.Parse();
        if (syntaxAnalyzer.Parse()) 
        {
            SyntaxTree syntaxTree = new SyntaxTree(); 
            
            SyntaxTreeBuilder treeBuilder = new SyntaxTreeBuilder(lexemeTable, syntaxTree);
            
       
            treeBuilder.BuildTree(); 
        }
        else 
        {
            Console.WriteLine($"неуспешно{asdad}");
        }
    }
}
