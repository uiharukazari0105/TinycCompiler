using System.Collections.Immutable;
using Compiler.Output;
using Compiler.Tokens.Binding.Expression;
using Compiler.Tokens.Binding.Operator;
using Compiler.Tokens.Binding.Statement;
using Compiler.Tokens.Syntax;
using Compiler.Tokens.Syntax.Expression;
using Compiler.Tokens.Syntax.Statement;

namespace Compiler.Tokens.Binding;

public sealed class Binder
{
    public List<LogDefinition> Diagnostics { get; } = [];

    private BoundScope _scope;

    public static BoundGlobalScope BindGlobalScope(BoundGlobalScope? previous, CompilationUnitSyntax syntax)
    {
        var parentScope = CreateParentScopes(previous);
        var binder = new Binder(parentScope);
        var expression = binder.BindStatement(syntax.Statement );
        var variables = binder._scope.GetDeclaredVariables();
        var diagnostics = binder.Diagnostics;
        return new BoundGlobalScope(previous, diagnostics, variables, expression);
    }

    private static BoundScope? CreateParentScopes(BoundGlobalScope? previous)
    {
        var stack = new Stack<BoundGlobalScope>();
        while (previous is not null)
        {
            stack.Push(previous);
            previous = previous.Previous;
        }
        
        BoundScope? parent = null;
        while (stack.Count > 0)
        {
            var global = stack.Pop();
            var scope = new BoundScope(parent);
            foreach (var variable in global.Variables)
                scope.TryDeclare(variable);
            
            parent = scope;
        }
        
        return parent;
    }
    
    public Binder(BoundScope? parent)
    {
        _scope = new BoundScope(parent);
    }
    
    private BoundStatement BindStatement(StatementSyntax syntax)
    {
        switch (syntax.Kind)
        {
            case SyntaxKind.BlockStatement:
                return BindBlockStatement((BlockStatementSyntax)syntax);
            case SyntaxKind.ExpressionStatement:
                return BindExpressionStatement((ExpressionStatementSyntax)syntax);
            case SyntaxKind.VariableDeclarationStatement:
                return BindVariableDeclarationStatement((VariableDeclarationStatementSyntax)syntax);
            case SyntaxKind.IfStatement:
                return BindIfStatement((IfStatementSyntax)syntax);
            case SyntaxKind.WhileStatement:
                return BindWhileStatement((WhileStatementSyntax)syntax);
            case SyntaxKind.ForStatement:
                return BindForStatement((ForStatementSyntax)syntax);
            case SyntaxKind.EmptyStatement:
                return new BoundEmptyStatement();
        }
        Diagnostics.Add(new LogDefinition(LogLevel.Error, $"没有这样的表达式 <{syntax.Kind}>", true));
        throw new Exception($"没有这样的表达式 <{syntax.Kind}>");
    }

    private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
    {
        var statements = ImmutableArray.CreateBuilder<BoundStatement>();
        _scope = new BoundScope(_scope);
        
        foreach (var statementSyntax in syntax.Statements)
        {
            var statement = BindStatement(statementSyntax);
            statements.Add(statement);
        }

        _scope = _scope.Parent!;
        
        return new BoundBlockStatement(statements.ToImmutable());
    }
    
    private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
    {
        var expression = BindExpression(syntax.Expression);
        return new BoundExpressionStatement(expression);
    }

    private BoundStatement BindVariableDeclarationStatement(VariableDeclarationStatementSyntax syntax)
    {
        var name = syntax.Identifier.Text;
        
        var initializer = BindExpression(syntax.Initializer);
        
        var keywordType = syntax.Keyword.Kind switch
        {
            SyntaxKind.ShortKeyword => typeof(short),
            SyntaxKind.IntKeyword => typeof(int),
            SyntaxKind.LongKeyword => typeof(long),
            SyntaxKind.FloatKeyword => typeof(float),
            SyntaxKind.DoubleKeyword => typeof(double),
            SyntaxKind.BoolKeyword => typeof(bool),
            SyntaxKind.CharKeyword => typeof(char),
            _ => typeof(object)
        };

        if (initializer.Type != keywordType)
            initializer = BindConversion(keywordType, initializer);

        
        var variable = new VariableSymbol(name, initializer.Type);
        
        if(!_scope.TryDeclare(variable))
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"变量 <{name}> 被重复声明", true));
        return new BoundVariableDeclarationStatement(variable, initializer);
    }
    
    private BoundStatement BindIfStatement(IfStatementSyntax syntax)
    {
        var condition = BindExpression(syntax.Condition);
        var thanStatement = BindStatement(syntax.ThenStatement);
        var elseStatement = syntax.ElseClause is null?null:BindStatement(syntax.ElseClause.ElseStatement);
        return new BoundIfStatement(condition, thanStatement, elseStatement);
    }
    
    private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
    {
        var condition = BindExpression(syntax.Condition);
        var statement = BindStatement(syntax.Statement);
        return new BoundWhileStatement(condition, statement);
    }
    
    private BoundStatement BindForStatement(ForStatementSyntax syntax)
    {
        _scope = new BoundScope(_scope);
        List<BoundStatement> initializers = [];
        foreach (var initializer in syntax.Initializers)
            initializers.Add(BindStatement(initializer));
        var condition = syntax.Condition is null?null:BindExpression(syntax.Condition);
        List<BoundStatement> stepStatements = [];
        foreach (var stepStatement in syntax.StepStatements)
            stepStatements.Add(BindStatement(stepStatement));
        var statement = BindStatement(syntax.ThenStatement);
        _scope = _scope.Parent!;
        return new BoundForStatement(initializers, condition, stepStatements, statement);
    }

    private BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        switch (syntax.Kind)
        {
            case SyntaxKind.LiteralExpression:
                return BindLiteralExpression((LiteralExpressionSyntax)syntax);
            case SyntaxKind.UnaryExpression:
                return BindUnaryExpression((UnaryExpressionSyntax)syntax);
            case SyntaxKind.BinaryExpression:
                return BindBinaryExpression((BinaryExpressionSyntax)syntax);
            case SyntaxKind.ParenthesizedExpression:
                return BindExpression(((ParenthesizedExpressionSyntax)syntax).Expression);
            case SyntaxKind.NameExpression:
                return BindNameExpression((NameExpressionSyntax)syntax);
            case SyntaxKind.AssignmentExpression:
                return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
        }

        Diagnostics.Add(new LogDefinition(LogLevel.Error, $"没有这样的表达式类型 <{syntax.Kind}>", true));
        throw new Exception($"没有这样的表达式类型 <{syntax.Kind}>");
    }

    private BoundExpression BindExpression(ExpressionSyntax syntax, Type expectedType)
    {
        var result = BindExpression(syntax);
        if (result.Type != expectedType)
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"表达式 <{syntax.Kind}> 不能利用类型 <{result.Type}> 预期类型 <{expectedType}>", true));
        return result;
    }

    private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }
    
    private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var boundOperand = BindExpression(syntax.Operand);
        var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
        if (boundOperator is null)
        {
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"一元运算符 <{syntax.OperatorToken.Text}> 不能运算 <{boundOperand.Type}>", true));
            return boundOperand;
        }
        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundOperator = BoundBinaryOperator.Bind(boundLeft.Type, syntax.OperatorToken.Kind, boundRight.Type);
        if (boundOperator is null)
        {
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"二元运算符 <{syntax.OperatorToken.Text}> 不能放在 <{boundLeft.Type}> 和 <{boundRight.Type}> 之间", true));
            return boundLeft;
        }
        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }
    
    private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text;
        
        if (!_scope.TryLookup(name, out var variable))
        {
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"变量 <{name}> 在该作用域没有声明", true));
            return new BoundLiteralExpression(0);
        }
        
        return new BoundVariableExpression(variable!);
    }
    
    private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text;
        var boundExpression = BindExpression(syntax.Expression);

        if (!_scope.TryLookup(name, out var variable))
        {
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"变量 <{name}> 在该作用域没有声明", true));
            return new BoundLiteralExpression(0);
        }
     
        if (boundExpression.Type != variable!.Type)
            Diagnostics.Add(new LogDefinition(LogLevel.Error, $"不能隐式转换 <{boundExpression.Type}> 到 <{variable.Type}>", true));
            
        return new BoundAssignmentExpression(variable, boundExpression);
    }

    private static Dictionary<Type, uint> _typeLevels = new Dictionary<Type, uint>
    {
        {typeof(bool), 0},
        {typeof(char), 1},
        {typeof(short), 2},
        {typeof(int), 3},
        {typeof(long), 4},
        {typeof(float), 5},
        {typeof(double), 6},
    };


    private BoundExpression BindConversion(Type targetType, BoundExpression expression)
    {
        if (expression.Type == targetType)
            return expression;
        
        var sourceLevel = _typeLevels[expression.Type];
        var targetLevel = _typeLevels[targetType];
        
        if(sourceLevel > targetLevel)
            Diagnostics.Add(new LogDefinition(LogLevel.Warning, $"<{expression.Type}> 到 <{targetType}> 的转换存在窄化"));

        return new BoundConversionExpression(targetType, expression);
    }
}