using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KnowYourToolset.RulesEngine.EntityFramework
{
    public class EntityFrameworkRuleStore : IRuleStore
    {
        private readonly RuleDbContext _context;

        public EntityFrameworkRuleStore(RuleDbContext context)
        {
            _context = context;
        }

        public async Task<List<Rule>> GetRulesByType(string ruleTypeCd)
        {
            return await _context.Rules.AsQueryable().Where(r => r.RuleTypeCd == ruleTypeCd).ToListAsync();
        }

        public async Task AddRule(Rule ruleToAdd)
        {
            await _context.Rules.AddAsync(ruleToAdd);
            await _context.SaveChangesAsync();
        }
    }
}
