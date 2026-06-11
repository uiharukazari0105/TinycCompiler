using System.Reflection;

namespace Compiler.Tokens.Binding.Expression;

public abstract class BoundNode
{
    public abstract BoundNodeKind Kind { get; }

    public abstract IEnumerable<BoundNode> GetChildren();

    public IEnumerable<(string Name, object Value)> GetProperties()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if(property.Name == nameof(Kind) || property.Name.Contains("Operator") || property.Name.Contains("Variable"))
                continue;
            if(typeof(BoundNode).IsAssignableFrom(property.PropertyType) ||
               typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                continue;
            
            var value = property.GetValue(this);
            if (value is not null)
            {
                if(property.Name == nameof(Type))
                    yield return (property.Name, (value.ToString()??"").Split('.')[^1]);
                else
                    yield return (property.Name, value);
            }
        }
    }
}