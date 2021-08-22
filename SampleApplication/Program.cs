using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KnowYourToolset.RulesEngine;
using KnowYourToolset.RulesEngine.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SampleApplication
{
    class Program
    {
        private static ServiceProvider _provider;

        static async Task Main(string[] args)
        {
            Startup();
            var ctx = _provider.GetRequiredService<RuleDbContext>();
            await ctx.Database.MigrateAsync();
            await ctx.Database.ExecuteSqlRawAsync("delete from Rules");

            await AddSomeRules();

            var customer = new Customer
            {
                Id = 22,
                Name = "Jason",
                State = "MN"
            };

            var rulesEngine = _provider.GetRequiredService<RulesEngine>();

            var applicableDiscounts = await rulesEngine.EvaluateRules<Customer, Discount>(customer, "DI");

            foreach (var discount in applicableDiscounts)
            {
                Console.WriteLine(discount);
            }
        }

        private static async Task AddSomeRules()
        {
            var store = _provider.GetRequiredService<IRuleStore>();

            var beginDt = DateTime.Now.Date.AddDays(-10);
            await store.AddRule(
                new Rule
                {
                    BeginDate = beginDt, 
                    RuleTypeCd = "DI", 
                    ConditionExpression = "State == \"MN\"",
                    Priority = 1,
                    RuleResults = new List<object> { new Discount {DiscountPct = 0.10M, DaysValid = 5}}
                });

            await store.AddRule(
                new Rule
                {
                    BeginDate = beginDt,
                    RuleTypeCd = "DI",
                    ConditionExpression = "State == \"MN\" AND Name == \"Erik\"",
                    Priority = 1,
                    RuleResults = new List<object> { new Discount { DiscountPct = 0.50M, DaysValid = 10 } }
                });

            await store.AddRule(
                new Rule
                {
                    BeginDate = beginDt,
                    RuleTypeCd = "DI",
                    ConditionExpression = "State == \"MN\" AND Name == \"Jason\"",
                    Priority = 1,
                    RuleResults = new List<object> { new Discount { DiscountPct = 0.05M, DaysValid = 2 } }
                });
        }

        private static void Startup()
        {
            var services = ConfigureServices();

            _provider = services.BuildServiceProvider();
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<RuleDbContext>();
            services.AddScoped<IRuleStore, EntityFrameworkRuleStore>();
            services.AddScoped<RulesEngine>();

            return services;
        }
    }
}
