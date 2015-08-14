using Microsoft.TeamFoundation.Build.Client;
using System;
using System.Collections.ObjectModel;

namespace Build.Extensions.Tests.Mocking
{
    public class MockBuildDetail : IBuildDetail
    {
        #region Private
        private IBuildController buildController = new MockBuildController();
        private IBuildDefinition buildDefinition = new MockBuildDefinition();
        private IBuildInformation information = new MockBuildInformation();
        private IBuildServer buildServer = new MockBuildServer();
        #endregion

        public IBuildController BuildController
        {
            get { return buildController; }
        }

        public Uri BuildControllerUri
        {
            get { return new Uri(""); }
        }

        public IBuildDefinition BuildDefinition
        {
            get { return buildDefinition; }
        }

        public Uri BuildDefinitionUri
        {
            get { return new Uri(""); }
        }

        public bool BuildFinished
        {
            get { return false; }
        }

        public string BuildNumber { get; set; }

        public IBuildServer BuildServer
        {
            get { return buildServer; }
        }

        public BuildPhaseStatus CompilationStatus { get; set; }

        public void Connect()
        {
        }

        public void Connect(int pollingInterval, System.ComponentModel.ISynchronizeInvoke synchronizingObject)
        {
        }

        public void Connect(int pollingInterval, int timeout, System.ComponentModel.ISynchronizeInvoke synchronizingObject)
        {
        }

        public IBuildDeletionResult Delete(DeleteOptions options)
        {
            return new MockBuildDeletionResult();
        }

        public IBuildDeletionResult Delete()
        {
            return new MockBuildDeletionResult();
        }

        public void Disconnect()
        {
        }

        public string DropLocation { get; set; }

        public string DropLocationRoot { get; private set; }

        public void FinalizeStatus(BuildStatus status)
        {
        }

        public void FinalizeStatus()
        {
        }

        public DateTime FinishTime
        {
            get { return DateTime.Now; }
        }

        public IBuildInformation Information
        {
            get { return information; }
        }

        public bool IsDeleted { get; private set; }

        public bool KeepForever { get; set; }

        public string LabelName { get; set; }

        public string LastChangedBy { get; private set; }

        public string LastChangedByDisplayName { get; private set; }

        public DateTime LastChangedOn { get; private set; }

        public string LogLocation { get; set; }

        public event PollingCompletedEventHandler PollingCompleted;

        public string ProcessParameters { get; private set; }

        public string Quality { get; set; }

        public BuildReason Reason { get; private set; }

        public void Refresh(string[] informationTypes, QueryOptions queryOptions)
        {
        }

        public void RefreshAllDetails()
        {
        }

        public void RefreshMinimalDetails()
        {
        }

        public ReadOnlyCollection<int> RequestIds { get; private set; }

        public Guid RequestIntermediateLogs()
        {
            return Guid.NewGuid();
        }

        public string RequestedBy { get; private set; }

        public string RequestedFor { get; private set; }

        public ReadOnlyCollection<IQueuedBuild> Requests { get; private set; }

        public void Save()
        {
        }

        public string ShelvesetName { get; private set; }

        public string SourceGetVersion { get; set; }

        public DateTime StartTime
        {
            get { return DateTime.Now.Subtract(TimeSpan.FromMinutes(10)); }
        }

        public BuildStatus Status { get; set; }

        public event StatusChangedEventHandler StatusChanged;

        public event StatusChangedEventHandler StatusChanging;

        public void Stop()
        {
        }

        public string TeamProject { get; private set; }

        public BuildPhaseStatus TestStatus { get; set; }

        public Uri Uri
        {
            get { return new Uri(""); }
        }

        public bool Wait(TimeSpan pollingInterval, TimeSpan timeout, System.ComponentModel.ISynchronizeInvoke synchronizingObject)
        {
            return true;
        }

        public bool Wait(TimeSpan pollingInterval, TimeSpan timeout)
        {
            return true;
        }

        public void Wait()
        {
        }
    }
}
