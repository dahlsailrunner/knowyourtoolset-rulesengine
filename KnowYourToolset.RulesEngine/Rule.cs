using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace KnowYourToolset.RulesEngine
{
    public partial class Rule
    {
        private List<object> _ruleResults;
        public int Id { get; set; }
        public string RuleTypeCd { get; set; }
        public int Priority { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ConditionExpression { get; set; }
        public string ResultJson { get; private set; }

        [NotMapped]
        public List<object> RuleResults
        {
            get => _ruleResults;
            set
            {
                _ruleResults = value;
                ResultJson = JsonSerializer.Serialize(value);
            } 
        }
    }
}
