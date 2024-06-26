﻿using Parsobober.Pkb.Ast;

namespace Parsobober.Simple.Parser.Abstractions;

public interface ISimpleExtractor
{
    void FinishedParsing();
    void Stmt(TreeNode node);
    void Assign(TreeNode node);
    void Expr(TreeNode node);
    void Procedure(TreeNode node);
    void While(TreeNode node);
    void If(TreeNode node);
    void Call(TreeNode node);
    void Variable(TreeNode node);
    void Factor(TreeNode node);
    void StmtLst(TreeNode node);
    void Constant(TreeNode node);
    void StmtLst();
}
