using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeDbDemo
{
  using FluentAssertions;
  using Sitecore;
  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Exceptions;
  using Sitecore.FakeDb;
  using Sitecore.Shell.Framework.Commands.Carousel;
  using Xunit;

  public class ItemCreation
  {
    [Fact]
    public void CanCreateItem()
    {
      using (var db = new Db())
      {
        db.Add(
          new DbItem("home")
          {
            {"Title", "Hello"}
          });
        var item = db.GetItem("/sitecore/content/home");

        item["Title"].Should().Be("Hello");
        item.ID.Should().NotBe(ID.Null);
        item["__Created By"].Should().Be(@"default\Anonymous");

      }
    }

    [Fact]
    public void CanAddChildItems()
    {
      using (var db = new Db())
      {
        db.Add(new DbItem("parent")); 
        var item = db.GetItem("/sitecore/content/parent");

        item.Add("child", new TemplateID(item.TemplateID));

        db.GetItem("/sitecore/content/parent/child").Should().NotBeNull();

      }
    }

    [Fact]
    public void FakeDBItem_WhenUpdated_UpdatesFieldAndRevision()
    {
      using (var db = new Db())
      {
        db.Add(
          new DbItem("home")
          {
            {"Title", "Hello"}
          });
        var item = db.GetItem("/sitecore/content/home");

        item["Title"].Should().Be("Hello");
        
        string firstRevision = item[FieldIDs.Revision];
 
        Action editItem = () => item["Title"] = "Goodbye";

        editItem.ShouldThrowExactly<Sitecore.Exceptions.EditingNotAllowedException>("because item is not in editing mode");

        item.Editing.BeginEdit();
        editItem();
        item.Editing.EndEdit();

        item["Title"].Should().Be("Goodbye");
        item[FieldIDs.Revision].Should()
          .NotBe(firstRevision, "because FakeDB updates the revision field when an item is edited");


      }
    }

    
  }
}
