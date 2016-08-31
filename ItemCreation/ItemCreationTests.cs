using System;
using FluentAssertions;
using NSubstitute;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Pipelines.InsertRenderings.Processors;
using Sitecore.SecurityModel.License;
using Xunit;
using Version = Sitecore.Data.Version;

namespace ItemCreation
{
	public class ItemCreationTests
    {
		[Fact]
		public void HowToCreateAnItem()
		{
			// Step 1: Create an ID
			ID itemId = ID.NewID;

			// Step 2: Create an ItemDefintion object
			ItemDefinition itemDefinition = new ItemDefinition(itemId, "item name", templateID: ID.NewID, branchId: ID.Null);

			// Step 3: Create some fields
			ID field1 = ID.NewID;
			string value1 = "value1";
			ID field2 = ID.NewID;
			string value2 = "value2";

			// Step 4: Create an ItemData object

			FieldList fields = new FieldList {{field1, value1}, {field2, value2}};
			ItemData itemData = new ItemData(itemDefinition, Language.Current, Version.First, fields);

			// Step 5: Get a Database object. This requires an App.Config entry
			Database database = Factory.GetDatabase("master");

			// Step 6: Create the item
			Item item = new Item(itemId, itemData, database);

			// Step 7: Profit!
			item.Should().NotBeNull();
			item.Name.Should().Be("item name");
			item.Fields.Count.Should().Be(2);
			item[field1].Should().Be(value1);
			item.Fields[field2].Value.Should().Be(value2);

			// But cannot add children...
			Action addChild = () => item.Add("child", new TemplateID(ID.NewID));
			addChild.ShouldThrowExactly<LicenseException>();

			// Access fields by name requires the TemplateManager

		}

		[Fact]
		public void CanWeMakeATemplateManager()
		{
			Item item = GetItem();
			ID someId = ID.NewID;

			// next creating a field
			TemplateManager.GetFieldId("some field", someId, item.Database).Should().BeNull();
		}

		private Item GetItem()
		{ 
				// Step 1: Create an ID
				ID itemId = ID.NewID;

				// Step 2: Create an ItemDefintion object
				ItemDefinition itemDefinition = new ItemDefinition(itemId, "item name", templateID: ID.NewID, branchId: ID.Null);

				// Step 3: Create some fields
				ID field1 = ID.NewID;
				string value1 = "value1";
				ID field2 = ID.NewID;
				string value2 = "value2";

				// Step 4: Create an ItemData object

				FieldList fields = new FieldList { { field1, value1 }, { field2, value2 } };
				ItemData itemData = new ItemData(itemDefinition, Language.Current, Version.First, fields);

				// Step 5: Get a Database object. This requires an App.Config entry
				Database database = Factory.GetDatabase("master");

				// Step 6: Create the item
				Item item = new Item(itemId, itemData, database);
			return item;

		}

        [Fact]
        public void CanCreateItemData()
        {
            ID itemID = ID.NewID;
            string itemName = "name";
            ID templateID = ID.NewID;
            ID branchId = ID.Null;
            ItemDefinition definition = new ItemDefinition(itemID, itemName, templateID, branchId);

            Language language = Language.Invariant;
            Version version = Version.Parse(1);
            FieldList fields = new FieldList();
            var itemData = new ItemData(definition, language, version, fields);


            itemData.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateDatabase()
        {
            Database db;
            // requires App.config
            db = Sitecore.Configuration.Factory.GetDatabase("master");

            db.Should().NotBeNull();
        }



        [Fact]
        public void CanCreateItem()
        {
   
            ID itemId = ID.NewID;
            ItemData itemData = MakeItemData(itemId, new FieldList());

            Database db = Sitecore.Configuration.Factory.GetDatabase("master");

            Item item = new Item(itemId, itemData, db);

            item.Should().NotBeNull();
        }

        [Fact]
        public void CanAccessFields()
        {
            ID itemId = ID.NewID;
            ID f1 = ID.NewID;
            ID f2 = ID.NewID;

            var fieldList = new FieldList { { f1, "value 1" }, { f2, "value 2" } };
            ItemData itemData = MakeItemData(itemId, fieldList);
            Database db = Sitecore.Configuration.Factory.GetDatabase("master");

            Item item = new Item(itemId, itemData, db);

            item.Should().NotBeNull();
            item[f1].Should().Be("value 1");
            item[f2].Should().Be("value 2");
        }

        [Fact]
        public void ButAddingChildrenBlowsUp()
        {
            ID itemID = ID.NewID;
            ItemData itemData = MakeItemData(ID.NewID, new FieldList());
            Database db = Sitecore.Configuration.Factory.GetDatabase("master");

            Item item = new Item(itemID, itemData, db);

            Action addChild = () => item.Add("child", new TemplateID(item.TemplateID));

            addChild.ShouldThrowExactly<Sitecore.SecurityModel.License.LicenseException>();

        }

        private static ItemData MakeItemData(ID itemId, FieldList fieldList)
        {
          string itemName = "name";
            ID templateID = ID.NewID;
            ID branchId = ID.Null;
            ItemDefinition definition = new ItemDefinition(itemId, itemName, templateID, branchId);

            Language language = Language.Invariant;
            Version version = Version.Parse(1);
            return new ItemData(definition, language, version, fieldList);
        }

    #region DB Tests


    [Fact]
    public void CanCreateSubstituteDatabase()
    {
      Database db = Substitute.For<Sitecore.Data.Database>("sub");
      db.Should().NotBeNull("because Sitecore.Kernell exposes internals to Castle.Core proxies");
    }


    [Fact]
    public void DbShouldBeSingleton()
    {
      Database db1 = Sitecore.Configuration.Factory.GetDatabase("master");
      Database db2 = Sitecore.Configuration.Factory.GetDatabase("master");

      db1.Should().BeSameAs(db2);
    }

    #endregion
  }
}
