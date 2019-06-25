using System;
using Elsa.Models;
using Elsa.Services.Models;

namespace Elsa.Services
{
    public interface IOutcomeBuilder
    {
        IActivityBuilder Source { get; }
        string Outcome { get; }
        IActivityBuilder Then<T>(Action<T> setup, string id = null) where T : class, IActivity;
        WorkflowDefinition Build();
        IConnectionBuilder Then(string activityId);
    }
}