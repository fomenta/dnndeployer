using Microsoft.TeamFoundation.Build.Client;
using System;
using System.Collections.Generic;

namespace Build.Extensions.Tests.Mocking
{
    public class MockBuildDefinition : IBuildDefinition
    {
        public IRetentionPolicy AddRetentionPolicy(BuildReason reason, BuildStatus status, int numberToKeep, DeleteOptions deleteOptions)
        {
            throw new NotImplementedException();
        }

        public ISchedule AddSchedule()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> AttachedProperties
        {
            get { throw new NotImplementedException(); }
        }

        public int BatchSize
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IBuildController BuildController
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Uri BuildControllerUri
        {
            get { throw new NotImplementedException(); }
        }

        public IBuildServer BuildServer
        {
            get { throw new NotImplementedException(); }
        }

        public int ContinuousIntegrationQuietPeriod
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ContinuousIntegrationType ContinuousIntegrationType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CopyFrom(IBuildDefinition buildDefinition)
        {
            throw new NotImplementedException();
        }

        public IBuildRequest CreateBuildRequest()
        {
            throw new NotImplementedException();
        }

        public IBuildDetail CreateManualBuild(string buildNumber, string dropLocation, BuildStatus buildStatus, IBuildController controller, string requestedFor)
        {
            throw new NotImplementedException();
        }

        public IBuildDetail CreateManualBuild(string buildNumber, string dropLocation)
        {
            throw new NotImplementedException();
        }

        public IBuildDetail CreateManualBuild(string buildNumber)
        {
            throw new NotImplementedException();
        }

        public IBuildDefinitionSpec CreateSpec()
        {
            throw new NotImplementedException();
        }

        public DateTime DateCreated
        {
            get { throw new NotImplementedException(); }
        }

        public string DefaultDropLocation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public string Description
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Enabled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Id
        {
            get { throw new NotImplementedException(); }
        }

        public Uri LastBuildUri
        {
            get { throw new NotImplementedException(); }
        }

        public string LastGoodBuildLabel
        {
            get { throw new NotImplementedException(); }
        }

        public Uri LastGoodBuildUri
        {
            get { throw new NotImplementedException(); }
        }

        public IProcessTemplate Process
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string ProcessParameters
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IBuildDetail[] QueryBuilds()
        {
            throw new NotImplementedException();
        }

        public DefinitionQueueStatus QueueStatus
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Refresh(string[] propertyNameFilters, QueryOptions queryOptions)
        {
            throw new NotImplementedException();
        }

        public List<IRetentionPolicy> RetentionPolicyList
        {
            get { throw new NotImplementedException(); }
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public List<ISchedule> Schedules
        {
            get { throw new NotImplementedException(); }
        }

        public List<IBuildDefinitionSourceProvider> SourceProviders
        {
            get { throw new NotImplementedException(); }
        }

        public DefinitionTriggerType TriggerType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IWorkspaceTemplate Workspace
        {
            get { throw new NotImplementedException(); }
        }

        public string FullPath
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public string TeamProject
        {
            get { throw new NotImplementedException(); }
        }

        public Uri Uri
        {
            get { throw new NotImplementedException(); }
        }
    }
}
