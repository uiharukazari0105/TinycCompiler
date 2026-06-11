using System.Collections.Immutable;
using Compiler.Output;
using Compiler.Tokens.Syntax;
using Compiler.Tokens.Syntax.Expression;
using Compiler.Tokens.Syntax.Statement;

namespace Compiler;

public class Parser
{
    public List<LogDefinition> Diagnostics { get; } = [];
    private readonly SyntaxToken[] _tokens;
    private int _position;
    
    public Parser(string text)
    {
        var tokens = new List<SyntaxToken>();
        var lexer = new Lexer(text);
        List<SyntaxKind> filter = [SyntaxKind.WhiteSpace, SyntaxKind.Bad]; 
        SyntaxToken token;
        do
        {
            token = lexer.NextToken();
            if (filter.Contains(token.Kind)) continue;
            tokens.Add(token);
        } while (token.Kind != SyntaxKind.EndOfFile);
        
        _tokens = tokens.ToArray();
        Diagnostics.AddRange(lexer.Diagnostics);
    }

    public CompilationUnitSyntax ParseCompilationUnit()
    {
        var statement = ParseStatement();
        var endOfFileToken = Match(SyntaxKind.EndOfFile);
        return new CompilationUnitSyntax(statement, endOfFileToken);
    }

    private StatementSyntax ParseStatement(bool enableMatchEndLine = true)
    {
        StatementSyntax result;
        bool matchEndLine = false;
        
        if (Current.Kind == SyntaxKind.Semicolon)
            result = new EmptyStatementSyntax(Match(SyntaxKind.Semicolon));
        else if (Current.Kind == SyntaxKind.OpenBrace)
            result = ParseBlockStatement();
        else if (Current.Kind == SyntaxKind.ShortKeyword ||
                 Current.Kind == SyntaxKind.IntKeyword ||
                 Current.Kind == SyntaxKind.LongKeyword ||
                 Current.Kind == SyntaxKind.FloatKeyword ||
                 Current.Kind == SyntaxKind.DoubleKeyword ||
                 Current.Kind == SyntaxKind.BoolKeyword ||
                 Current.Kind == SyntaxKind.CharKeyword)
        {
            if (Peek(1).Kind == SyntaxKind.Identifier && Peek(2).Kind == SyntaxKind.OpenParenthesis)
                result = ParseFunctionDeclaration();
            else
            {
                result = ParseVariableDeclarationStatement();
                matchEndLine = true;
            }
        }
        else if (Current.Kind == SyntaxKind.IfKeyWord)
            result = ParseIfStatement();
        else if (Current.Kind == SyntaxKind.WhileKeyword)
            result = ParseWhileKeyword();
        else if (Current.Kind == SyntaxKind.ForKeyword)
            result = ParseForKeyword();
        else
        {
            result = ParseExpressionStatement();
            matchEndLine = true;
        }
        if(enableMatchEndLine && matchEndLine)
            Match(SyntaxKind.Semicolon);
        return result;
    }

    private StatementSyntax ParseBlockStatement()
    {
        var statements = ImmutableArray.CreateBuilder<StatementSyntax>(); 
        var openBraceToken = Match(SyntaxKind.OpenBrace);
        while (Current.Kind != SyntaxKind.EndOfFile
               && Current.Kind != SyntaxKind.CloseBrace)
        {
            var statement = ParseStatement();
            statements.Add(statement);
        }
        var closeBraceToken = Match(SyntaxKind.CloseBrace);
        return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
    }

    private ExpressionStatementSyntax ParseExpressionStatement()
    {
        var expression = ParseExpression();
        return new ExpressionStatementSyntax(expression); 
    }

    private StatementSyntax ParseVariableDeclarationStatement()
    {
        var keyword = NextToken();

        var identifiers = ImmutableArray.CreateBuilder<SyntaxToken>();
        var commas = ImmutableArray.CreateBuilder<SyntaxToken>();

        identifiers.Add(Match(SyntaxKind.Identifier));

        while (Peek(0).Kind == SyntaxKind.Comma)
        {
            commas.Add(Match(SyntaxKind.Comma));
            identifiers.Add(Match(SyntaxKind.Identifier));
        }

        SyntaxToken? equals = null;
        ExpressionSyntax? initializer = null;
        if (Peek(0).Kind != SyntaxKind.Semicolon)
        {
            equals = Match(SyntaxKind.Equals);
            initializer = ParseExpression();
        }

        return new VariableDeclarationStatementSyntax(keyword,
            identifiers.ToImmutable(), commas.ToImmutable(), equals, initializer);
    }

    private StatementSyntax ParseFunctionDeclaration()
    {
        var returnType = NextToken(); // type keyword
        var identifier = Match(SyntaxKind.Identifier); // function name
        var openParen = Match(SyntaxKind.OpenParenthesis); // (
        var closeParen = Match(SyntaxKind.CloseParenthesis); // )
        var body = (BlockStatementSyntax)ParseBlockStatement(); // { ... }
        return new FunctionDeclarationSyntax(returnType, identifier, openParen, closeParen, body);
    }

    private StatementSyntax ParseIfStatement()
    {
        var keyword = Match(SyntaxKind.IfKeyWord);
        var condition = ParseExpression();
        var statement = ParseStatement();
        var elseClause = ParseElseClause();
        return new IfStatementSyntax(keyword, condition, statement, elseClause);
    }

    private ElseClauseSyntax? ParseElseClause()
    {
        if (Current.Kind != SyntaxKind.ElseKeyword)
            return null;
        var keyword = Match(SyntaxKind.ElseKeyword);
        var statement = ParseStatement();
        return  new ElseClauseSyntax(keyword, statement);
    }
    
    private StatementSyntax ParseWhileKeyword()
    {
        var keyword = Match(SyntaxKind.WhileKeyword);
        var condition = ParseExpression();
        var statement = ParseStatement();
        return new WhileStatementSyntax(keyword, condition, statement);
    }
    
    private StatementSyntax ParseForKeyword()
    {
        var keyword = Match(SyntaxKind.ForKeyword);
        var openParenthesis = Match(SyntaxKind.OpenParenthesis);
        
        List<StatementSyntax> initializers = [];
        if (Current.Kind != SyntaxKind.Semicolon)
            do
            {
                if(Current.Kind == SyntaxKind.Comma)
                    Match(SyntaxKind.Comma);
                initializers.Add(ParseStatement(false));
            } while (Current.Kind != SyntaxKind.Semicolon);
        Match(SyntaxKind.Semicolon);
        var condition = Current.Kind == SyntaxKind.Semicolon? null: ParseExpression();
        Match(SyntaxKind.Semicolon);
        List<StatementSyntax> stepStatements = [];
        if(Current.Kind != SyntaxKind.CloseParenthesis)
            do
            {
                if(Current.Kind == SyntaxKind.Comma)
                    Match(SyntaxKind.Comma);
                stepStatements.Add(ParseStatement(false));
            }while (Current.Kind != SyntaxKind.CloseParenthesis);
        var closeParenthesisToken = Match(SyntaxKind.CloseParenthesis);
        var statement =  ParseStatement();
        return new ForStatementSyntax(keyword, openParenthesis, initializers, condition, stepStatements, closeParenthesisToken, statement);
    }

    private SyntaxToken Match(SyntaxKind kind)
    {
        if(Current.Kind == kind)
            return NextToken();
        Diagnostics.Add(new LogDefinition(LogLevel.Error,$"不合理的类型 <{Current.Kind}>，应该为 <{kind}>", true));
        return new SyntaxToken(kind, Current.Position, "");
    }
    
    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.OpenParenthesis:
                return ParseParenthesizedExpression();
            case SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword:
                return ParseBooleanLiteral();
            case SyntaxKind.Number:
                return ParseNumberLiteral();
            case SyntaxKind.Character:
                return ParseCharacterLiteral();
            case SyntaxKind.Identifier:
            default:
                return ParseNameExpression();
        }
    }

    private ExpressionSyntax ParseExpression()
    {
        return ParseAssignmentExpression();
    }
    
    private ExpressionSyntax ParseParenthesizedExpression()
    {
        var left = Match(SyntaxKind.OpenParenthesis);
        var expression = ParseExpression();
        var right = Match(SyntaxKind.CloseParenthesis);
        return new ParenthesizedExpressionSyntax(left, expression, right);
    }
    
    private ExpressionSyntax ParseBooleanLiteral()
    {
        var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
        var keywordToken = Match(isTrue? SyntaxKind.TrueKeyword : SyntaxKind.FalseKeyword);
        return new LiteralExpressionSyntax(keywordToken, isTrue);
    }
    
    private ExpressionSyntax ParseNameExpression()
    {
        var identifierToken = Match(SyntaxKind.Identifier);
        return new NameExpressionSyntax(identifierToken);
    }
    
    private ExpressionSyntax ParseNumberLiteral()
    {
        var numberToken = Match(SyntaxKind.Number);
        var numberText = numberToken.Text;
        var castSymbol = numberText[^1];
        if(!char.IsDigit(castSymbol) && castSymbol != '.')
            numberText = numberText[..^1];
            
        dynamic number;
        if(numberToken.Text.Contains("."))
            number = double.Parse(numberText);
        else
            number = int.Parse(numberText);

        if (castSymbol == 'L')
            number = (long)number;
        else if(castSymbol == 'f')
            number = (float)number;
        
        return new LiteralExpressionSyntax(numberToken, number);
    }
    
    private ExpressionSyntax ParseCharacterLiteral()
    {
        var characterToken = Match(SyntaxKind.Character);
        var characterText = characterToken.Text[1..^1];
        var last = characterText.LastIndexOf('\\');
        if(last != -1)
            characterText = characterText[last..];
        if(characterText.Length == 1)
            return new LiteralExpressionSyntax(characterToken, char.Parse(characterText));
        
        if (characterText[^2] == '\\')
        {
            var esc = characterText.Length >= 2 ? characterText[1] : '?';
            var value = esc switch
            {
                '0' => '\0', 'a' => '\a', 'b' => '\b', 'f' => '\f', 'n' => '\n',
                'r' => '\r', 't' => '\t', 'v' => '\v', '\\' => '\\', '\'' => '\'',
                '"' => '\"',
                _ => esc
            };
            return new LiteralExpressionSyntax(characterToken, value);
        }
        
        return new LiteralExpressionSyntax(characterToken, characterText[^1]);
    }
    
    private ExpressionSyntax ParseAssignmentExpression()
    {
        List<SyntaxKind> acceptableOperators = [
            SyntaxKind.Equals,
            SyntaxKind.AddEquals,
            SyntaxKind.MinusEquals,
            SyntaxKind.StarEquals,
            SyntaxKind.SlashEquals,
            SyntaxKind.AmpersandEquals,
            SyntaxKind.PipeEquals,
            SyntaxKind.CaretEquals,
            SyntaxKind.PercentageEquals
        ];
        if (Current.Kind == SyntaxKind.Identifier && acceptableOperators.Contains(Peek(1).Kind))
        {
            var identifierToken = NextToken();
            var operatorToken = NextToken();
            var right = ParseAssignmentExpression();
            return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
        }

        return ParseBinaryExpression();
    }
    
    private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var unaryOperatorPrecedence = Current.Kind.GetUnaryPrecedence();
        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence) //特别应对--1的情况（特别难找）
        {
            var operatorToken = NextToken();
            var operand = ParseBinaryExpression(unaryOperatorPrecedence);
            left = new UnaryExpressionSyntax(operatorToken, operand);
        }
        else
            left = ParsePrimaryExpression();
        
        while (true)
        {
            var precedence = Current.Kind.GetBinaryPrecedence();
            if(precedence == 0 || precedence <= parentPrecedence)
                break;
            var operatorToken = NextToken();
            var right = ParseBinaryExpression(precedence);
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }
        return left;
    }
    
    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }
    
    private SyntaxToken Peek(int offset)
    {
        var index = _position + offset;
        if (index >= _tokens.Length) return _tokens[^1];
        return _tokens[index];
    }
    
    private SyntaxToken Current=>Peek(0);
}