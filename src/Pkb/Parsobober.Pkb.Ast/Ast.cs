using Parsobober.Pkb.Ast.Abstractions;

namespace Parsobober.Pkb.Ast;

public class Ast : IAst
{
    public TreeNode Root { get; } = new(0, EntityType.Program);
}