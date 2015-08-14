using Microsoft.TeamFoundation.Build.Client;
using System;
using System.Activities;

namespace Build.Extensions.Activities
{
    /* http://goo.gl/C0FeJv
     * If a custom assembly uses a dependent assembly (reference) which is needed to run activities, 
     * they will not get deployed properly. If this is the case you will get “unknown type” errors on 
     * build definition initialization:
     * 
     *       TF215097: An error occurred while initializing a build for build definition xxxx: 
     *       The type ‘xxxx’ of property ‘xxxx’ could not be resolved.
     * 
     * To work around this issue, we add a dummy CodeActivity into the dependent assembly with the
     *       class scoped attribute: [BuildActivity(HostEnvironmentOption.All)] 
    */
    [BuildActivity(HostEnvironmentOption.All)]
    public sealed class DummyCodeActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}