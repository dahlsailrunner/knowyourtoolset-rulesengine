using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnowYourToolset.RulesEngine
{
    public enum RuleResultOptions
    {
        /// <summary>
        /// Return only the first matching result
        /// </summary>
        FirstOnly,
        /// <summary>
        /// Return all matching results
        /// </summary>
        All
    }

    public class RulesEngine
    {
        private readonly IRuleStore _ruleStore;
        private readonly Dictionary<string, List<Rule>> _ruleDictionary = new Dictionary<string, List<Rule>>();

        public RulesEngine(IRuleStore ruleStore)
        {
            _ruleStore = ruleStore;
        }

        /// <summary>
        /// EvaluateRules will return a list of results derived by applying the condition expression of the applicable rules to the input object (objToEvaluate).
        /// </summary>
        /// <typeparam name="TInput">The type of the object being evaluated against the rules.</typeparam>
        /// <typeparam name="TRuleResult">The type of a single result for a rule result for an object that was matched.</typeparam>
        /// <param name="objToEvaluate">The input object that should be evaluated against the rules.</param>
        /// <param name="ruleTypeCd">The RuleTypeCd that defines the set of rules to use from the tRules table in the database.</param>
        /// <param name="ruleOption">Optional parameter.  Used to indicate whether only first matched result should be returned or all matches.  Defaults to RuleResultOptions.All.</param>
        /// <returns>A list of the results based on the rules that matched the input object.  If no matches were found an empty list is returned.</returns>         
        public async Task<List<TRuleResult>> EvaluateRules<TInput, TRuleResult>(TInput objToEvaluate, string ruleTypeCd,
            RuleResultOptions ruleOption = RuleResultOptions.All)
        {
            if (!_ruleDictionary.ContainsKey(ruleTypeCd))
            {
                _ruleDictionary[ruleTypeCd] = await _ruleStore.GetRulesByType(ruleTypeCd);
            }

            var listWithObjectToEvaluate = new List<TInput> { objToEvaluate }.AsQueryable();

            var rulesToApply = _ruleDictionary[ruleTypeCd];

            var resultList = new List<TRuleResult>();

            foreach (var rule in rulesToApply)
            {
                try
                {
                    var match = listWithObjectToEvaluate.Where(rule.ConditionExpression);

                    if (!match.Any())
                        continue; // this item did not match!
                }
                catch (Exception ex)
                {
                    var newEx = new Exception($"Rule evaluation failed!  Check condition expression:  {rule.ConditionExpression}", ex);
                    newEx.Data.Add("Object being evaluated", objToEvaluate);
                    throw newEx;
                }
                // assert -- must have matched!
                resultList.AddRange(GetResultsFromMatchedRule<TRuleResult>(rule));

                if (ruleOption == RuleResultOptions.FirstOnly)
                    return resultList;
            }

            return resultList;
        }

        private List<TRuleResult> GetResultsFromMatchedRule<TRuleResult>(Rule rule)
        {
            try
            {
                var resultsFromRule = JsonSerializer.Deserialize<List<TRuleResult>>(rule.ResultJson);
                return resultsFromRule;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deserializing results into {typeof(TRuleResult).Name}.  Check ResultJson on Rule {rule.Id}", ex);
            }
        }
    }
}
