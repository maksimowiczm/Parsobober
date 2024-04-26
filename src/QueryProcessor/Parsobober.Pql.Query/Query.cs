using Parsobober.Pkb.Relations.Abstractions;
using Parsobober.Pkb.Relations.Dto;

namespace Parsobober.Pql.Query
{
    internal class Query : IQuery
    {
        private readonly Dictionary<string, Type> _declarations = new();
        private string _select = string.Empty;
        List<Func<IPkbAccessor, IEnumerable<Statement>>> _actions;
        IPkbAccessor _accesor;
        public Query(Dictionary<string, Type> declarations, string select, List<Func<IPkbAccessor, IEnumerable<Statement>>> actions,IPkbAccessor accesor) 
        {
            _declarations = declarations;
            _select = select;
            _actions = actions;
            _accesor = accesor;

        }
        public string Execute()
        {
            List<Statement> result = new();
            foreach (var action in _actions) 
            {
                result.Concat(action(_accesor));
            }
            return result.ToString();
        }
    }
}
