using Microsoft.TeamFoundation.Build.Client;
using System;
using System.Collections.Generic;

namespace Build.Extensions.Tests.Mocking
{
    public class MockBuildController : IBuildController
    {
        public void AddBuildAgent(IBuildAgent agent)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<IBuildAgent> Agents
        {
            get { throw new NotImplementedException(); }
        }

        public IDictionary<string, object> AttachedProperties
        {
            get { throw new NotImplementedException(); }
        }

        public string CustomAssemblyPath
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

        public DateTime DateCreated
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime DateUpdated
        {
            get { throw new NotImplementedException(); }
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

        public List<System.Reflection.Assembly> LoadCustomActivities(string localPath, Microsoft.TeamFoundation.VersionControl.Client.RecursionType recursionType, HostEnvironmentOption environmentOptions, out List<Type> activityTypes, out List<IFailure> failures)
        {
            throw new NotImplementedException();
        }

        public List<System.Reflection.Assembly> LoadCustomActivitiesAndExtensions(string localPath, Microsoft.TeamFoundation.VersionControl.Client.RecursionType recursionType, HostEnvironmentOption environmentOptions, out List<Type> activityTypes, out List<Type> extensionTypes, out List<IFailure> failures)
        {
            throw new NotImplementedException();
        }

        public int MaxConcurrentBuilds
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

        public Uri MessageQueueUrl
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

        public int QueueCount
        {
            get { throw new NotImplementedException(); }
        }

        public void Refresh(string[] propertyNameFilters, bool refreshAgentList)
        {
            throw new NotImplementedException();
        }

        public void Refresh(bool refreshAgentList)
        {
            throw new NotImplementedException();
        }

        public void RemoveBuildAgent(IBuildAgent agent)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public IBuildServiceHost ServiceHost
        {
            get { throw new NotImplementedException(); }
        }

        public ControllerStatus Status
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

        public string StatusMessage
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

        public System.Collections.ObjectModel.ReadOnlyCollection<string> Tags
        {
            get { throw new NotImplementedException(); }
        }

        public Uri Uri
        {
            get { throw new NotImplementedException(); }
        }

        public Uri Url
        {
            get { throw new NotImplementedException(); }
        }
    }
}
