using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Ast.Abstractions;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.Simple.Parser.Extractor
{
    internal abstract class SimpleExtractor : ISimpleExtractor
    {
        protected readonly ISimpleExtractor wrappee;

        public SimpleExtractor(ISimpleExtractor wrappee)
        {
            this.wrappee = wrappee;
        }

        virtual public TreeNode Assign()
        {
            return wrappee.Assign();
        }

        virtual public TreeNode Expr()
        {
            return wrappee.Expr();
        }

        virtual public TreeNode Factor()
        {
            return wrappee.Factor();
        }

        public IAst Parse()
        {
            return wrappee.Parse();
        }

        virtual public TreeNode Procedure()
        {
            return wrappee.Procedure();
        }

        virtual public TreeNode Stmt()
        {
            return wrappee.Stmt();
        }

        virtual public TreeNode StmtLst()
        {
            return wrappee.StmtLst();
        }

        virtual public TreeNode While()
        {
            return wrappee.While();
        }
    }
}
