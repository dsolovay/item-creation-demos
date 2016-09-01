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

		[Theory, AutoSitecoreSubstitutes]
		public void Processor_FolderMissing_CreatesIt(DataSourceFolderCreator sut, Database db, Item renderingItem)
		{
			renderingItem.SetChildren(new ItemList());
			var args = new GetRenderingDatasourceArgs(renderingItem, db);

			sut.Process(args);

			renderingItem.Received().Add("Items", new TemplateID(TemplateIDs.Folder));
		}

		[Theory, AutoSitecoreSubstitutes]
		public void Processor_FolderPresent_DoesNotCreateIt(DataSourceFolderCreator sut, Database db, Item renderingItem, Item folderItem)
		{
			folderItem.Name.Returns("Items");
			folderItem.TemplateID.Returns(TemplateIDs.Folder);
			renderingItem.SetChildren(new ItemList {folderItem});
			var args = new GetRenderingDatasourceArgs(renderingItem, db);

			sut.Process(args);

			renderingItem.DidNotReceive().Add("Items", new TemplateID(TemplateIDs.Folder));
		}
	}
}