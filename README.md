# DNN Deployer #

## Release Notes ##
v2.2.1026.2015
- Added logic to ignore errors in SSL certificates

## Automated Deployment ##

I have been working on several DotNetNuke enhancements for several customers in terms of making it easier and **automating deployment** of:

- Skins
- Modules
- Libraries
- Language packs
- Importing sites from remote templates
- Pages on current portal or any portal.
- Adding modules to pages.
- Changing module details on a page (e.g. Module Title).
 
This was a crucial step for customers managing multiple portals and multiple installations of DNN. 

## Publishing Challenges ##

Every time one or more user controls on a few modules were updated, deployment used to take about an entire night doing most of these steps manually on all portals and all servers. While now, this can be done in minutes.

## Flavors ##

This can be done from:

- Command line. A new tool was created called dnncmd.exe
- TFS Build Agent. A set of custom TFS Build Agent activies were created in order to publish modules (or extensions) right after the DNN package is created on the TFS build agent.
- From code. A library was created to allow DevOps applications to publish modules to a DNN site from their own code.

## How it Works ##

On the DNN side, an extension (module) was create to expose a REST interface to accept commands and call DNN API within to process requests. This module was named DNN Deployer.

It will need to be adjusted to DNN code conventions.

## What's Included? ##

**BuildSrc\Main\Build.sln**

This solution contains the following components:


- **TFS Build DNN Extension**

> **BuildSrc\Main\dev\Extensions\Build.Extensions.csproj**

> TFS Build Process Activities allowing to Publishing DNN extensions. These activities are included in the publishing workflow template below.

- **TFS Build Process Template**

> **BuildProcessTemplates\Facture.Build.Template.1.1.xaml**

> TFS Build Process Template (Publishing Workflow) allowing compiling and publishing DNN extensions automatically from Build Server Agent.

- **dnncmd**

> **BuildSrc\Main\dev\DotNetNuke\dnncmd\dnncmd.csproj**

> Tool to publish/update/administer DNN extensions from the command line.

- **Unit Tests**

> **BuildSrc\Main\test\Extensions.Tests\Build.Extensions.Tests.csproj**

> A variety of unit tests

**Deployer\Deployer.sln**

VStudio Solution for DNN Extensions.

- **DNN Deployer**

This is the actual DNN Library Extension that does the magic. It exposes a REST API to allow other components to publish extensions and some other administrative stuffs.

> Deployer\Deployer.csproj

Unit testing for this project is included on the prior VStudio Solution above.

## Credits ##
- Fomenta SAS
>Code has been developed by Fomenta S.A.S., a specialized provider on Software Architecture and Advanced Integrations on Distributed Solutions for several Software Factories and other 3rd-party clients.

- Facture SAS
> This was the client who initially paid for the solution and has donated the code. Facture S.A.S. is a leading Software Factory developing custom-tailored solutions using DNN.  

Code is currently maintained by **Fomenta S.A.S.**