using Compiler.Ir.Instruction;
using Compiler.Tokens;
using Compiler.Tokens.Binding.Expression;
using Compiler.Tokens.Binding.Operator;
using Compiler.Tokens.Binding.Statement;

namespace Compiler.Ir;

public class IrGenerator
{
    private readonly List<IrInstruction> _instructions = [];
    private readonly Dictionary<VariableSymbol, string> _varNames = new();
    private readonly HashSet<string> _usedNames = new();
    private int _tempCounter;
    private int _labelCounter;

    public IReadOnlyList<IrInstruction> Generate(BoundStatement root)
    {
        _instructions.Clear();
        _varNames.Clear();
        _usedNames.Clear();
        _tempCounter = 0;
        _labelCounter = 0;
        EmitStatement(root);
        return _instructions;
    }

    /// <summary>Map a VariableSymbol to a unique IR-level name (handles shadowing).</summary>
    private string VarName(VariableSymbol v)
    {
        if (!_varNames.TryGetValue(v, out var name))
        {
            name = v.Name;
            if (_usedNames.Contains(name))
            {
                var suffix = 1;
                while (_usedNames.Contains($"{v.Name}_{suffix}"))
                    suffix++;
                name = $"{v.Name}_{suffix}";
            }
            _varNames[v] = name;
            _usedNames.Add(name);
        }
        return name;
    }

    private string NewTemp() => $"t{++_tempCounter}";
    private int NewLabel() => ++_labelCounter;
    

    private void EmitStatement(BoundStatement node)
    {
        switch (node)
        {
            case BoundBlockStatement b:
                foreach (var s in b.Statements) EmitStatement(s);
                break;

            case BoundVariableDeclarationStatement v:
                var init = EmitExpression(v.Initializer);
                if (init != "0")
                    _instructions.Add(new AssignIr(VarName(v.Variable), init));
                break;

            case BoundExpressionStatement e:
                EmitExpression(e.Expression);
                break;

            case BoundReturnStatement r:
                var val = r.Expression is null ? null : EmitExpression(r.Expression);
                _instructions.Add(new ReturnIr(val));
                break;

            case BoundIfStatement i:
                EmitIf(i);
                break;

            case BoundWhileStatement w:
                EmitWhile(w);
                break;

            case BoundForStatement f:
                EmitFor(f);
                break;

            case BoundFunctionDeclarationStatement fn:
                EmitStatement(fn.Body);
                break;

            case BoundEmptyStatement:
            case BoundBreakStatement:
            case BoundContinueStatement:
                break;
        }
    }

    private void EmitIf(BoundIfStatement node)
    {
        var cond = EmitExpression(node.Condition);

        if (node.ElseStatement is null)
        {
            var end = NewLabel();
            _instructions.Add(new CondJumpIr(cond, end));
            EmitStatement(node.ThenStatement);
            _instructions.Add(new LabelIr(end));
        }
        else
        {
            var elseLbl = NewLabel();
            var end = NewLabel();
            _instructions.Add(new CondJumpIr(cond, elseLbl));
            EmitStatement(node.ThenStatement);
            _instructions.Add(new JumpIr(end));
            _instructions.Add(new LabelIr(elseLbl));
            EmitStatement(node.ElseStatement);
            _instructions.Add(new LabelIr(end));
        }
    }

    private void EmitWhile(BoundWhileStatement node)
    {
        var start = NewLabel();
        var end = NewLabel();
        _instructions.Add(new LabelIr(start));
        var cond = EmitExpression(node.Condition);
        _instructions.Add(new CondJumpIr(cond, end));
        EmitStatement(node.Statement);
        _instructions.Add(new JumpIr(start));
        _instructions.Add(new LabelIr(end));
    }

    private void EmitFor(BoundForStatement node)
    {
        foreach (var init in node.Initializers)
            EmitStatement(init);

        var start = NewLabel();
        var end = NewLabel();
        _instructions.Add(new LabelIr(start));

        if (node.Condition is not null)
        {
            var cond = EmitExpression(node.Condition);
            _instructions.Add(new CondJumpIr(cond, end));
        }

        EmitStatement(node.Statement);

        if (node.StepExpression is not null)
            EmitExpression(node.StepExpression);

        _instructions.Add(new JumpIr(start));
        _instructions.Add(new LabelIr(end));
    }
    

    private string EmitExpression(BoundExpression node)
    {
        switch (node)
        {
            case BoundLiteralExpression l:
                return l.Value.ToString()!;

            case BoundVariableExpression v:
                return VarName(v.Variable);

            case BoundAssignmentExpression a:
            {
                var val = EmitExpression(a.Expression);
                _instructions.Add(new AssignIr(VarName(a.Variable), val));
                return VarName(a.Variable);
            }

            case BoundSelfOperatorExpression s:
            {
                var val = EmitExpression(s.Expression);
                var op = s.Operator.Kind.OperatorString();
                var temp = NewTemp();
                _instructions.Add(new BinaryIr(temp, VarName(s.Variable), op, val));
                _instructions.Add(new AssignIr(VarName(s.Variable), temp));
                return VarName(s.Variable);
            }

            case BoundBinaryExpression b:
            {
                var left = EmitExpression(b.Left);
                var right = EmitExpression(b.Right);
                var op = b.Operator.Kind.OperatorString();
                var temp = NewTemp();
                _instructions.Add(new BinaryIr(temp, left, op, right));
                return temp;
            }

            case BoundUnaryExpression u:
            {
                var operand = EmitExpression(u.Operand);
                var temp = NewTemp();
                _instructions.Add(new BinaryIr(temp, "", u.Operator.Kind.OperatorString(), operand));
                return temp;
            }

            case BoundConversionExpression c:
                return EmitExpression(c.Expression);

            case BoundCommaExpression c:
                EmitExpression(c.Left);
                return EmitExpression(c.Right);

            case BoundPrefixExpression p:
            {
                var delta = p.OperatorKind == BoundUnaryOperatorKind.PrefixIncrement ? "1" : "-1";
                _instructions.Add(new BinaryIr(VarName(p.Variable), VarName(p.Variable), "+", delta));
                return VarName(p.Variable);
            }

            case BoundPostfixExpression p:
            {
                var old = NewTemp();
                _instructions.Add(new AssignIr(old, VarName(p.Variable)));
                var delta = p.OperatorKind == BoundUnaryOperatorKind.PostfixIncrement ? "1" : "-1";
                _instructions.Add(new BinaryIr(VarName(p.Variable), VarName(p.Variable), "+", delta));
                return old;
            }

            default:
                return "???";
        }
    }
}
