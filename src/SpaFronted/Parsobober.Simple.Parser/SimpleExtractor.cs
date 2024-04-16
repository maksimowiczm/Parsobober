using Parsobober.Pkb.Ast;
using Parsobober.Simple.Parser.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsobober.Simple.Parser
{
    internal abstract class SimpleExtractor(ISimpleExtractor wrappee) : ISimpleExtractor
    {
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
