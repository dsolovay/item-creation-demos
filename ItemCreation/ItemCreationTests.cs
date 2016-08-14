using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemCreation
{
    using FluentAssertions;
    using NSubstitute;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.Publishing.Explanations;
    using Sitecore.Publishing.Pipelines.PublishItem;
    using Xunit;

    public class ItemCreationTests
    {
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
