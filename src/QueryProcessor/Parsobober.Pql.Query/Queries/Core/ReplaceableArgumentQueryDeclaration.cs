using Parsobober.Pql.Query.Arguments;
using Parsobober.Pql.Query.Queries.Abstractions;
using Parsobober.Pql.Query.Queries.Exceptions;

namespace Parsobober.Pql.Query.Queries.Core;

internal abstract class ReplaceableArgumentQueryDeclaration<TSelf> : IQueryDeclaration
    where TSelf : ReplaceableArgumentQueryDeclaration<TSelf>
{
    public abstract IArgument Left { get; }
    public abstract IArgument Right { get; }
    public abstract IEnumerable<IComparable> Do(IDeclaration select);

    protected abstract TSelf CloneSelf(IArgument left, IArgument right);

    public IQueryDeclaration ReplaceArgument(IDeclaration select, IArgument replacement)
    {
        if (select == Left && Right is IDeclaration right)
        {
            return CloneSelf(replacement, right);
        }

        if (select == Right && Left is IDeclaration left)
        {
            return CloneSelf(left, replacement);
        }

        if (select != Left && select != Right)
        {
            throw new DeclarationNotFoundException(select, this);
        }

        // I don't know if it is true xD but for now it is todo it's not because boolean queries are now supported
        throw new QueryNotSupported(this, "You can't create a query without any declaration in it.");
    }

    #region DEBUG

#if DEBUG
    public override string ToString() => $"{GetType().DeclaringType!.Name}({Left}, {Right})";

    public List<IComparable> LeftResult
    {
        get
        {
            if (Left is IDeclaration declaration)
            {
                return Do(declaration).ToList();
            }

            return [];
        }
    }

    public List<IComparable> RightResult
    {
        get
        {
            if (Right is IDeclaration declaration)
            {
                return Do(declaration).ToList();
            }

            return [];
        }
    }
#endif

    #endregion
}