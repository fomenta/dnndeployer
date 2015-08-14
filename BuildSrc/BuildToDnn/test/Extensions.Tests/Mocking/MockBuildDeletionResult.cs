using Microsoft.TeamFoundation.Build.Client;
using System;

namespace Build.Extensions.Tests.Mocking
{
    public class MockBuildDeletionResult : IBuildDeletionResult
    {
        public IFailure DropLocationFailure
        {
            get { throw new NotImplementedException(); }
        }

        public IFailure LabelFailure
        {
            get { throw new NotImplementedException(); }
        }

        public bool Successful
        {
            get { return true; }
        }

        public IFailure SymbolsFailure
        {
            get { throw new NotImplementedException(); }
        }

        public IFailure TestResultFailure
        {
            get { throw new NotImplementedException(); }
        }
    }
}
