using Microsoft.TeamFoundation.Build.Client;
using System;
using System.Collections.Generic;

namespace Build.Extensions.Tests.Mocking
{
    public class MockBuildInformation : IBuildInformation
    {
        public IBuildInformationNode CreateNode()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public IBuildInformationNode GetNode(int id)
        {
            throw new NotImplementedException();
        }

        public List<IBuildInformationNode> GetNodesByType(string type, bool recursive)
        {
            throw new NotImplementedException();
        }

        public List<IBuildInformationNode> GetNodesByType(string type)
        {
            throw new NotImplementedException();
        }

        public List<IBuildInformationNode> GetNodesByTypes(IEnumerable<string> types, bool recursive)
        {
            throw new NotImplementedException();
        }

        public List<IBuildInformationNode> GetNodesByTypes(IEnumerable<string> types)
        {
            throw new NotImplementedException();
        }

        public List<IBuildInformationNode> GetSortedNodes(IComparer<IBuildInformationNode> comparer)
        {
            throw new NotImplementedException();
        }

        public List<IBuildInformationNode> GetSortedNodesByType(string type, IComparer<IBuildInformationNode> comparer)
        {
            throw new NotImplementedException();
        }

        public List<IBuildInformationNode> GetSortedNodesByTypes(IEnumerable<string> types, IComparer<IBuildInformationNode> comparer)
        {
            throw new NotImplementedException();
        }

        public IBuildInformationNode[] Nodes
        {
            get { throw new NotImplementedException(); }
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
