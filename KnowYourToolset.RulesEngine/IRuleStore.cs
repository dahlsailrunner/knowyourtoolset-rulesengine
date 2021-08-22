using System.Collections.Generic;
using System.Threading.Tasks;

namespace KnowYourToolset.RulesEngine
{
    public interface IRuleStore
    {
        Task<List<Rule>> GetRulesByType(string ruleTypeCd);
        Task AddRule(Rule ruleToAdd);
    }
}
