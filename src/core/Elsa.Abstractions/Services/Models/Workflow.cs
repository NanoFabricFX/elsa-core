﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elsa.Models;
using Elsa.Serialization.Models;
using Newtonsoft.Json.Linq;
using NodaTime;

namespace Elsa.Services.Models
{
    public class Workflow
    {
        public Workflow(
            string definitionId,
            IEnumerable<IActivity> activities,
            Variables input = null,
            WorkflowInstance workflowInstance = null) : this()
        {
            DefinitionId = definitionId;
            Activities = activities.ToList();
            Input = new Variables(input ?? Variables.Empty);
            Initialize(workflowInstance);
        }

        public Workflow()
        {
            Scopes = new Stack<WorkflowExecutionScope>(new[] { new WorkflowExecutionScope() });
            BlockingActivities = new HashSet<IActivity>();
            ExecutionLog = new List<LogEntry>();
        }

        public string Id { get; set; }
        public string DefinitionId { get; }
        public WorkflowStatus Status { get; set; }
        public Instant CreatedAt { get; set; }
        public Instant? StartedAt { get; set; }
        public Instant? HaltedAt { get; set; }
        public Instant? FinishedAt { get; set; }
        public ICollection<IActivity> Activities { get; } = new List<IActivity>();
        public IList<Connection> Connections { get; } = new List<Connection>();
        public Stack<WorkflowExecutionScope> Scopes { get; }
        public HashSet<IActivity> BlockingActivities { get; set; }
        public IList<LogEntry> ExecutionLog { get; set; }
        public WorkflowFault Fault { get; set; }
        public Variables Input { get; set; }

        public WorkflowInstance ToInstance()
        {
            var activities = Activities.ToDictionary(x => x.Id, x => x.ToInstance());

            return new WorkflowInstance
            {
                Id = Guid.NewGuid().ToString("N"),
                DefinitionId = Id,
                Status = Status,
                CreatedAt = CreatedAt,
                StartedAt = StartedAt,
                HaltedAt = HaltedAt,
                FinishedAt = FinishedAt,
                Activities = activities,
                Scopes = new Stack<WorkflowExecutionScope>(Scopes),
                BlockingActivities = new HashSet<string>(BlockingActivities.Select(x => x.Id)),
                ExecutionLog = ExecutionLog.ToList(),
                Fault = Fault?.ToInstance() 
            };
        }

        private void Initialize(WorkflowInstance instance)
        {
            if(instance == null)
                return;
            
            Id = instance.Id;
            Status = instance.Status;
            CreatedAt = instance.CreatedAt;
            StartedAt = instance.StartedAt;
            HaltedAt = instance.HaltedAt;
            FinishedAt = instance.FinishedAt;

            foreach (var activity in Activities)
            {
                activity.State = new JObject(instance.Activities[activity.Id]);
            }
        }
    }
}