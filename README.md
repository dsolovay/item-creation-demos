# item-creation-demos
SitecoreSYM demos of item creation with 8.1, FakeDB, and 8.2

Contents:
* Comparision of item creation in [8.1](ItemCreation/ItemCreationTests.cs) and [8.2](ItemCreation82/ItemCreationTests.cs). 
* Simple demo of [FakeDB](FakeDbDemo/ItemCreation.cs) (not included in talk for reasons of time)
* [Example](ItemCreation82/DataSourceFolderCreatorTests.cs) of building a pipeline processor using NSubstitute versions of 8.2 items
  * Includes [AutoSitecoreSubstitute](ItemCreation82/AutoSitecoreSubstitutesAttribute.cs) customization of AutoFixture
  
## To Build
Make sure to include Sitecore as a NuGet package source, as described here:
https://doc.sitecore.net/sitecore%20experience%20platform/82/developing/developing%20with%20sitecore/sitecore%20public%20nuget%20packages%20faq
