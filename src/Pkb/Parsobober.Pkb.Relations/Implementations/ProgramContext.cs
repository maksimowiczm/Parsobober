using Microsoft.Extensions.Logging;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations.Abstractions.Accessors;
using Parsobober.Pkb.Relations.Abstractions.Creators;
using Parsobober.Pkb.Relations.Dto;
using Parsobober.Pkb.Relations.Utilities;

namespace Parsobober.Pkb.Relations.Implementations;

public class ProgramContext(ILogger<ProgramContext> logger)
    : IProgramContextCreator, IProgramContextAccessor, IDtoProgramContextAccessor
{
    private readonly Dictionary<string, TreeNode> _variablesDictionary = new();
    private readonly Dictionary<int, TreeNode> _statementsDictionary = new();
    private readonly Dictionary<string, TreeNode> _proceduresDictionary = new();
    private readonly Dictionary<int, TreeNode> _statementListsDictionary = new();
    private readonly HashSet<int> _constantSet = new();

    public IReadOnlyDictionary<string, TreeNode> VariablesDictionary => _variablesDictionary.AsReadOnly();
    public IReadOnlyDictionary<int, TreeNode> StatementsDictionary => _statementsDictionary.AsReadOnly();
    public IReadOnlyDictionary<string, TreeNode> ProceduresDictionary => _proceduresDictionary.AsReadOnly();
    public IEnumerable<int> ConstantsEnumerable => _constantSet;

    #region IProgramContextCreator

    public bool TryAddVariable(TreeNode variable)
    {
        if (variable.Type != EntityType.Variable)
        {
            logger.LogError("Cannot add node with type different that {type} to variable dictionary. ({node})",
                EntityType.Variable, variable.Type);

            throw new ArgumentException(
                $"Provided node type {variable.Type} is different than required {EntityType.Variable}.");
        }

        if (variable.Attribute is null)
        {
            logger.LogError("Cannot add variable without attribute to dictionary. ({node})", variable);

            throw new ArgumentNullException(variable.Attribute);
        }

        return _variablesDictionary.TryAdd(variable.Attribute, variable);
    }

    public bool TryAddStatement(TreeNode statement)
    {
        if (statement.Type.IsStatement() == false)
        {
            logger.LogError(
                "Cannot add node with type different that {type} or its subtypes to statement dictionary. ({node})",
                EntityType.Statement, statement);

            throw new ArgumentException(
                $"Provided node type {statement.Type} is different than any of required {EntityType.Statement} types.");
        }

        return _statementsDictionary.TryAdd(statement.LineNumber, statement);
    }

    public bool TryAddProcedure(TreeNode procedure)
    {
        if (procedure.Type != EntityType.Procedure)
        {
            logger.LogError("Cannot add node with type different that {type} to procedure dictionary. ({node})",
                EntityType.Procedure, procedure);

            throw new ArgumentException(
                $"Provided node type {procedure.Type} is different than required {EntityType.Procedure}.");
        }

        if (procedure.Attribute is null)
        {
            logger.LogError("Cannot add procedure without attribute to dictionary. ({node})", procedure);

            throw new ArgumentNullException(procedure.Attribute);
        }

        return _proceduresDictionary.TryAdd(procedure.Attribute, procedure);
    }

    public bool TryAddStatementList(TreeNode statementList)
    {
        if (statementList.Type.IsStatementList() == false)
        {
            logger.LogError(
                "Cannot add node with type different that {type} or its subtypes to statement dictionary. ({node})",
                EntityType.StatementsList, statementList);

            throw new ArgumentException(
                $"Provided node type {statementList.Type} is different than any of required {EntityType.StatementsList} types.");
        }

        return _statementListsDictionary.TryAdd(statementList.LineNumber, statementList);
    }

    public bool TryAddConstant(TreeNode constant)
    {
        if (constant.Type.IsConstant() == false)
        {
            logger.LogError(
                "Cannot add node with type different that {type} or its subtypes to statement dictionary. ({node})",
                EntityType.Constant, constant);

            throw new ArgumentException(
                $"Provided node type {constant.Type} is different than any of required {EntityType.Constant} types.");
        }

        return _constantSet.Add(int.Parse(constant.Attribute!));
    }

    #endregion

    #region IDtoProgramContextAccessor

    public IEnumerable<Statement> Statements => StatementsDictionary.Values
        .Select(s => s.ToStatement());

    public IEnumerable<Assign> Assigns => GetStatementsOfType<Assign>();

    public IEnumerable<While> Whiles => GetStatementsOfType<While>();

    public IEnumerable<If> Ifs => GetStatementsOfType<If>();

    public IEnumerable<Call> Calls => GetStatementsOfType<Call>();

    private IEnumerable<T> GetStatementsOfType<T>()
        where T : Statement
        => StatementsDictionary.Values
            .Where(s => s.IsType<T>())
            .Select(s => s.ToStatement() as T)!;

    public IEnumerable<Variable> Variables => VariablesDictionary.Values
        .Select(v => v.ToVariable());

    public IEnumerable<Procedure> Procedures => ProceduresDictionary.Values
        .Select(p => p.ToProcedure());

    public IEnumerable<StatementList> StatementLists => _statementListsDictionary.Values
        .Select(s => s.LineNumber.ToStatementList());

    public IEnumerable<Constant> Constants => ConstantsEnumerable
        .Select(c => c.ToConstant());

    public IEnumerable<ProgramLine> ProgramLines => StatementsDictionary.Keys.Select(p => p.ToProgramLine());

    #endregion
}