using Microsoft.TeamFoundation.Build.Workflow.Activities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Build.Extensions.Helpers
{
    public static class Filtering
    {
        public static StringList Apply(StringList input, string csvFilterList)
        {
            var output = new StringList();
            if (input == null) { return null; }

            if (string.IsNullOrEmpty(csvFilterList)) { return input; }

            var selectedItemList = new List<string>(csvFilterList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            foreach (var item in input)
            {
                //if (selectedItemList.Any(o => Regex.IsMatch(item, o.Trim(), RegexOptions.IgnoreCase)))
                if (selectedItemList.Any(o => Path.GetFileNameWithoutExtension(item).Equals(o.Trim(), StringComparison.OrdinalIgnoreCase)))
                { output.Add(item); }
            }

            return output;
        }
    }
}
