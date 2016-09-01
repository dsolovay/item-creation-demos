using System.Linq;
using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Pipelines.GetRenderingDatasource;
using Xunit;
using Version = Sitecore.Data.Version;

namespace ItemCreation82
{
	public class DataSourceFolderCreatorTests
	{
		[Theory, AutoNSubData]
		public void Process_ItemFolderMissing_CreatesIt(DataSourceCreatorProcessor sut, [Substitute]Database db)
		{
			Item renderingItem = MakeSubstituteItem(ID.NewID, "some rendering item", ID.NewID, db);
			renderingItem.GetChildren().Returns(new ChildList(renderingItem, new ItemList()));
			var args = new GetRenderingDatasourceArgs(renderingItem, db);

			sut.Process(args);

			renderingItem.Received().Add("Items", new TemplateID(TemplateIDs.Folder));
		}

		[Theory, AutoNSubData]
		public void Process_ItemFolderPresent_DoesNotCreateNewOne(DataSourceCreatorProcessor sut, [Substitute]Database db)
		{
			Item renderingItem = MakeSubstituteItem(ID.NewID, "some rendering item", ID.NewID, db);
			Item childItem = MakeSubstituteItem(ID.NewID, "Items", TemplateIDs.Folder, db);
			renderingItem.GetChildren().Returns(new ChildList(renderingItem, new ItemList {childItem}));
			var args = new GetRenderingDatasourceArgs(renderingItem, db);

			sut.Process(args);

			renderingItem.DidNotReceive().Add("Items", new TemplateID(TemplateIDs.Folder));
		}

		private Item MakeSubstituteItem(ID id, string name, ID templateId, Database db)
		{
			var definition = new ItemDefinition(id, name, templateId, ID.Null);
			var data = new ItemData(definition, Language.Current, Version.First,
				new FieldList()); 
			var item = Substitute.For<Item>(id, data, db);

			item.Name.Returns(name);
			item.TemplateID.Returns(templateId);
			item.Paths.Returns(Substitute.For<ItemPath>(item));
			return item;
		}
	}

	public class AutoNSubDataAttribute : AutoDataAttribute
	{
		public AutoNSubDataAttribute() : base(new Fixture().Customize(new AutoNSubstituteCustomization()))
		{
			
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