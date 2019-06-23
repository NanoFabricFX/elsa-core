using System;
using System.Linq;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Core.Services.WorkflowBuilders
{
    public class ActivityBuilder : IActivityBuilder
    {
        public ActivityBuilder(WorkflowBuilder workflowBuilder, ActivityBlueprint activity, string id)
        {
            WorkflowBuilder = workflowBuilder;
            Activity = activity;
            Id = id;
        }

        public WorkflowBuilder WorkflowBuilder { get; }
        public ActivityBlueprint Activity { get; }
        public string Id { get; }

        public IActivityBuilder Add<T>(Action<T> setupActivity, string id = null) where T : class, IActivity
        {
            return WorkflowBuilder.Add(setupActivity, id);
        }

        public IOutcomeBuilder When(string outcome)
        {
            return new OutcomeBuilder(WorkflowBuilder, this, outcome);
        }

        public IActivityBuilder Then<T>(Action<T> setup = null, Action<IActivityBuilder> branch = null, string id = null) where T : class, IActivity
        {
            var activityBuilder = When(null).Then(setup, id);
            branch?.Invoke(activityBuilder);
            return activityBuilder;
        }

        public IConnectionBuilder Then(string activityId)
        {
            return WorkflowBuilder.Connect(
                () => this,
                () => WorkflowBuilder.Activities.First(x => x.Id == activityId)
            );
        }

        public ActivityBlueprint BuildActivity()
        {
            Activity.Id = Id;
            return Activity;
        }

        public WorkflowBlueprint Build() => WorkflowBuilder.Build();
    }
}