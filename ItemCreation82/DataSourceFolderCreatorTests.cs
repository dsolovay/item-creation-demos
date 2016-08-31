using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.Exceptions;
using Sitecore;
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
		[Fact]
		public void Process_ItemFolderMissing_CreatesIt()
		{
			var sut = new DataSourceCreatorProcessor();
			var db = Substitute.For<Database>();
			Item renderingItem = MakeItem(db, ID.NewID, "some rendering item");
			ItemPath itemPath = Substitute.For<ItemPath>(renderingItem);
			renderingItem.Paths.Returns(itemPath);
			renderingItem.Paths.FullPath.Returns("/some/path");
		 
			GetRenderingDatasourceArgs args = new GetRenderingDatasourceArgs(renderingItem, db);

			sut.Process(args);

			renderingItem.Received().Add("Items", new TemplateID(TemplateIDs.Folder));

		}

		private Item MakeItem(Database db, ID id, string name)
		{
			ItemData data = new ItemData(new ItemDefinition(id, name, ID.Null, ID.Null), Language.Current, Version.First,
				new FieldList()); 
			return Substitute.For<Item>(id, data, db);
		}
	}

	public class DataSourceCreatorProcessor
	{
		public void Process(GetRenderingDatasourceArgs args)
		{
			args.RenderingItem.Add("Items", new TemplateID(TemplateIDs.Folder));
		}
	}
}

