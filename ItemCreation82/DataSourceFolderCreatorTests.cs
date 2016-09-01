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
		public void Process_ItemFolderMissing_CreatesIt(DataSourceFolderCreator sut, Database db, Item renderingItem)
		{
			renderingItem.SetChildren(new ItemList());
			var args = new GetRenderingDatasourceArgs(renderingItem, db);

			sut.Process(args);

			renderingItem.Received().Add("Items", new TemplateID(TemplateIDs.Folder));
		}

		[Theory, AutoSitecoreSubstitutes]
		public void Process_ItemFolderPresent_DoesNotCreateNewOne(DataSourceFolderCreator sut, [Substitute]Database db, Item renderingItem, Item childItem)
		{
			childItem.Name.Returns("Items");
			childItem.TemplateID.Returns(TemplateIDs.Folder);
			renderingItem.SetChildren(new ItemList {childItem});
			var args = new GetRenderingDatasourceArgs(renderingItem, db);

			sut.Process(args);

			renderingItem.DidNotReceive().Add("Items", new TemplateID(TemplateIDs.Folder));
		}
	}
}