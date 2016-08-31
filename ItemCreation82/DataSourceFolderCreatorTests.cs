using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Exceptions;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Pipelines.GetRenderingDatasource;
using Xunit;
using Version = Sitecore.Data.Version;

namespace ItemCreation82
{
	public class DataSourceFolderCreatorTests
	{
		[Fact]
		public void Process_ItemFolderMissing_CreatesIt()
		{
			var sut = new DataSourceCreatorProcessor();
			var db = Substitute.For<Database>();
			Item renderingItem = MakeItem(db, ID.NewID, "some rendering item", "/some/path", ID.Null);
			renderingItem.GetChildren().Returns(new ChildList(renderingItem, new ItemList()));
			var args = new GetRenderingDatasourceArgs(renderingItem, db);

			sut.Process(args);

			renderingItem.Received().Add("Items", new TemplateID(TemplateIDs.Folder));

		}

		[Fact]
		public void Process_ItemFolderPresent_DoesNotCreateNewOne()
		{
			var sut = new DataSourceCreatorProcessor();
			var db = Substitute.For<Database>();
			Item renderingItem = MakeItem(db, ID.NewID, "some rendering item", "/some/path", ID.NewID);
			Item childItem = MakeItem(db, ID.NewID, "Items", "/some/path", TemplateIDs.Folder);
			renderingItem.GetChildren().Returns(new ChildList(renderingItem, new ItemList {childItem}));
			renderingItem.GetChildren().Count.Should().Be(1);
			renderingItem.GetChildren().First().Name.Should().Be("Items");

			var args = new GetRenderingDatasourceArgs(renderingItem, db);

			sut.Process(args);

			renderingItem.DidNotReceive().Add("Items", new TemplateID(TemplateIDs.Folder));

		}

		private Item MakeItem(Database db, ID id, string name, string path, ID templateId)
		{
			ItemData data = new ItemData(new ItemDefinition(id, name, templateId, ID.Null), Language.Current, Version.First,
				new FieldList()); 
			var item = Substitute.For<Item>(id, data, db);
			item.Name.Returns(name);
			item.TemplateID.Returns(templateId);
			item.Paths.Returns(Substitute.For<ItemPath>(item));
			item.Paths.FullPath.Returns(path);
			return item;
		}
	}

	public class DataSourceCreatorProcessor
	{
		public void Process(GetRenderingDatasourceArgs args)
		{
			var renderingItem = args.RenderingItem;

			if (!renderingItem.GetChildren().Any(item => item.Name== "Items" && item.TemplateID == TemplateIDs.Folder))
				renderingItem.Add("Items", new TemplateID(TemplateIDs.Folder));
		}
	}
}

