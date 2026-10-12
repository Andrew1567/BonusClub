using System;

namespace FleetMaintenance.Core.Errors;

public class DomainRuleException : Exception
{
    public string Rule { get; }

    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    public DomainRuleException(string rule, string message,
        Exception inner) : base(message, inner)
    {
        Rule = rule;
    }
}
