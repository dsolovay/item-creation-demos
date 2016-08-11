using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemCreation
{
  using FluentAssertions;
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
      ID itemID=ID.NewID;
      string itemName="name";
      ID templateID=ID.NewID;
      ID branchId = ID.Null;
      ItemDefinition definition = new ItemDefinition(itemID, itemName,templateID, branchId);

      Language language = Language.Invariant;
      Version version = Version.Parse(1);
      FieldList fields = new FieldList();
      var itemData = new ItemData(definition, language, version, fields);


      itemData.Should().NotBeNull();
    }

    [Fact]
    public void CanCreateDatabase()
    {
      //Database db = new Database(); Private constructor
      Database db = Sitecore.Configuration.Factory.GetDatabase("master");

      db.Should().NotBeNull();
    }

    [Fact]
    public void DbShouldBeSingleton()
    {
      Database db1 = Sitecore.Configuration.Factory.GetDatabase("master");
      Database db2 = Sitecore.Configuration.Factory.GetDatabase("master");

      db1.Should().BeSameAs(db2);
    }

    [Fact]
    public void CanCreateItem()
    {
      ItemData itemData;
      var itemID = MakeItemData(out itemData, new FieldList());

      Database db = Sitecore.Configuration.Factory.GetDatabase("master");

      Item item = new Item(itemID, itemData, db);

      item.Should().NotBeNull();
    }

    [Fact]
    public void CanAccessFields()
    {
      ItemData itemData;
      ID field1 = ID.NewID;
      ID field2= ID.NewID;
      var fieldList = new FieldList {{field1, "value 1"}, { field2, "value 2"}};
      var itemID = MakeItemData(out itemData, fieldList);

      Database db = Sitecore.Configuration.Factory.GetDatabase("master");

      Item item = new Item(itemID, itemData, db);

      item.Should().NotBeNull();
      item[field1].Should().Be("value 1");
      item[field2].Should().Be("value 2");
    }

    [Fact]
    public void Add_CalledOnSyntheticItem_ThrowsLicenseException()
    {
      ItemData itemData;
      var itemID = MakeItemData(out itemData, new FieldList());

      Database db = Sitecore.Configuration.Factory.GetDatabase("master");

      Item item = new Item(itemID, itemData, db);

      Action addChild = () => item.Add("child", new TemplateID(item.TemplateID));

      addChild.ShouldThrowExactly<Sitecore.SecurityModel.License.LicenseException>();

    }

    private static ID MakeItemData(out ItemData itemData, FieldList fieldList)
    {
      ID itemID = ID.NewID;
      string itemName = "name";
      ID templateID = ID.NewID;
      ID branchId = ID.Null;
      ItemDefinition definition = new ItemDefinition(itemID, itemName, templateID, branchId);

      Language language = Language.Invariant;
      Version version = Version.Parse(1);
      itemData = new ItemData(definition, language, version, fieldList);
      return itemID;
    }
  }
}
