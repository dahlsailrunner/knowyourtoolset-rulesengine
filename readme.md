# KnowYourToolest.RulesEngine
This is a very experimental / proof-of-concept rules engine written in C#
and leveraging the [System.Linq.Dynamic.Core NuGet package](https://github.com/zzzprojects/System.Linq.Dynamic.Core).

That package was based on some early work from the Linq team that never 
made it into the framework formally:  https://weblogs.asp.net/scottgu/dynamic-linq-part-1-using-the-linq-dynamic-query-library


## Example Usage
As an example, you might want to evaluate customers to see what 
kind of discount codes are available to them.

You would define some rules that define discount eligibility and then 
the nature of the discount -- rules basically like the following:
* Anyone in Minnesota gets a 10% discount code valid for 10 days
* Anyone named **Erik** in Minnesota gets a 50% discount code valid for 10 days
* Anyone named **Jason** in Minnesota gets a 5% discount code valid for 2 days

To model the discount, you could create a class like this:

```csharp
public class Discount
{
    public decimal DiscountPct { get; set; }
    public short DaysValid { get; set; }
}
```
Then you might have a `Customer` class that looks like this:

```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string State { get; set; }
}
```

Then you define some rules that look like this (abbreviated from actual code):

```csharp
await store.AddRule(
    new Rule
    {
        RuleTypeCd = "DI", // represents "DIscount"
        ConditionExpression = "State == \"MN\"",
        RuleResults = new List<object> { new Discount {DiscountPct = 0.10M, DaysValid = 5}}
    });

await store.AddRule(
    new Rule
    {
        RuleTypeCd = "DI",
        ConditionExpression = "State == \"MN\" AND Name == \"Erik\"",
        RuleResults = new List<object> { new Discount { DiscountPct = 0.50M, DaysValid = 10 } }
    });

await store.AddRule(
    new Rule
    {
        RuleTypeCd = "DI",
        ConditionExpression = "State == \"MN\" AND Name == \"Jason\"",
        RuleResults = new List<object> { new Discount { DiscountPct = 0.05M, DaysValid = 2 } }
    });
```

To get a list of the available discounts, you would then make a call 
to the `RulesEngine` that looks like this:

```csharp
var customer = new Customer
{
    Id = 22,
    Name = "Jason",
    State = "MN"
};
var applicableDiscounts = await rulesEngine.EvaluateRules<Customer, Discount>(customer, "DI");
```

The above example is simply a single example.  This engine could be 
used with any input object and result type.

For documentation about the possible expressions / operators that can 
be used with Dynamic Linq, see [this documentation](https://dynamic-linq.net/overview).


## Database Considerations
The SampleApplication here is completely runnable and uses a SQLite 
database with EntityFramework Core.

You could implement your own `IRuleStore` and use that instead if you 
want.

The real trick with an engine like this is coming up with a way 
to manage the rules themselves.  

Given another meta-table that defines the "RuleType" - 
like the "DI" type in the example above, and then associates an object 
type and result type, you could define a user interface to provide 
some basic validation around the `ConditionExpression` and the list 
of results to return.