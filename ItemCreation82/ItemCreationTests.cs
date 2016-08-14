using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemCreation82
{
  using FluentAssertions;
  using NSubstitute;
  using NSubstitute.Core;
  using Sitecore;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Globalization;
  using Sitecore.SecurityModel.License;
  using Xunit;

  public class ItemCreationTests
  {
    [Fact]
    public void CanCreateItem()
    {
      ID itemID = ID.NewID;

      ItemDefinition definition = new ItemDefinition(itemID, "somename", ID.NewID, ID.Null);
      ID field1 = ID.NewID;
      ID field2 = ID.NewID;
      FieldList fields = new FieldList {{field1, "value 1"}, {field2, "value 2"}};
      ItemData data = new ItemData(definition, Language.Invariant, Version.First, fields);

      // Requires Lucene DLL to resolve a few obsolete types.
      Database database = Substitute.For<Database>();

      Item item = new Item(itemID, data, database);

      item[field1].Should().Be("value 1");
      item[field2].Should().Be("value 2");

      // use NSubtitute to make the DB return values.
      database.GetItem(itemID).Returns(item);  


      database.GetItem(itemID).Fields[field1].Value.Should().Be("value 1");
      database.GetItem(ID.NewID).Should().BeNull();
    }

    [Fact]
    public void Item_AddChild_ThrowsTypeInitializationException()
    {
      ID itemID = ID.NewID;

      ItemDefinition definition = new ItemDefinition(itemID, "parent", ID.NewID, ID.Null);
       
      ItemData data = new ItemData(definition, Language.Invariant, Version.First, new FieldList());

      // Requires Lucene DLL to resolve a few obsolete types.
      Database database = Substitute.For<Database>();

      Item item = new Item(itemID, data, database);

      Action addChild = () => item.Add("child", new TemplateID(item.TemplateID));

      addChild.ShouldThrowExactly<TypeInitializationException>("because it can't initialize the item create pipeline");

    }

    [Fact]
    public void CanCreateSubstituteItem_ScriptBehavior()
    {
      ID itemId1 = ID.NewID;
      ID itemId2 = ID.NewID;

      ID templateId = ID.NewID;
      ItemDefinition definition1 = new ItemDefinition(itemId1, "name1", templateId, ID.Null);
      ItemDefinition definition2 = new ItemDefinition(itemId2, "child", templateId, ID.Null);

      ID field1 = ID.NewID;
      ItemData data1 = new ItemData(definition1, Language.Invariant, Version.First, new FieldList { {field1, "value 1"} });
      ItemData data2 = new ItemData(definition2, Language.Invariant, Version.First, new FieldList { { field1, "value 2" } });

      // Requires Lucene DLL to resolve a few obsolete types.
      Database database = Substitute.For<Database>();

      Item item1 = Substitute.ForPartsOf<Item>(itemId1, data1, database);
      Item item2 = Substitute.ForPartsOf<Item>(itemId2, data2, database);

      //item1.When(x => x.Add(Arg.Any<string>(), Arg.Any<TemplateID>())).DoNotCallBase();
      
      item1.Add(Arg.Any<string>(), Arg.Any<TemplateID>()).Returns(item2);
 

      var returnedChild = item1.Add("child", new TemplateID(item1.TemplateID));

      returnedChild.Should().NotBeNull();
      returnedChild.ID.Should().Be(itemId2);

      item1[field1].Should().Be("value 1");
      returnedChild[field1].Should().Be("value 2");

      item1.Received().Add("child", Arg.Any<TemplateID>());
      item1.DidNotReceive().Add("other name", Arg.Any<TemplateID>());


    }
  }
}
