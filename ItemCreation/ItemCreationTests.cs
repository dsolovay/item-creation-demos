﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemCreation
{
  using FluentAssertions;
  using Sitecore.Data;
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
    }
  }
}
