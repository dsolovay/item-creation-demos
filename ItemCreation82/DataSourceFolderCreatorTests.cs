using System.Collections.Generic;
using NSubstitute;
using Ploeh.AutoFixture.AutoNSubstitute;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines.GetRenderingDatasource;
using Xunit;

namespace ItemCreation82
{


  public class DataSourceFolderCreatorTests
  {

    [Theory, AutoSitecoreData]
    public void Processor_ItemsFolderMissing_Creates(DataSourceFolderCreator sut, Database db, Item renderingItem)
    {
      renderingItem.SetChildren(new ItemList());
      var args = new GetRenderingDatasourceArgs(renderingItem, db);

      sut.Process(args);

      renderingItem.Received().Add("Items", new TemplateID(TemplateIDs.Folder));
    }

    [Theory, AutoSitecoreData]
    public void Processor_ItemsFolderPresent_DoesNotCreate(DataSourceFolderCreator sut, Database db, Item renderingItem, Item folderItem)
    {
      folderItem.Name.Returns("Items");
      folderItem.TemplateID.Returns(TemplateIDs.Folder);
      renderingItem.SetChildren(new ItemList { folderItem });
      var args = new GetRenderingDatasourceArgs(renderingItem, db);

      sut.Process(args);

      renderingItem.DidNotReceiveWithAnyArgs().Add("", new TemplateID());
    }
  }
}