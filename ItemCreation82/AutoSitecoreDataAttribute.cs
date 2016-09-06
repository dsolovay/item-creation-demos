using System;
using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace ItemCreation82
{
  /// <summary>
  /// Creates NSubstitute Sitecore Items and Databases.
  /// Database object is "Frozen" (see http://blog.ploeh.dk/2010/03/17/AutoFixtureFreeze/)
  /// Extends AutoData with NSubstitute customization.
  /// 
  /// Item fields values such as Name, TemplateID, and method results, such as GetChildren, must be set 
  /// on the Substitute Item in test code.
  /// 
  /// Item ID field is not virtual, so this is created in the SubstituteItemFactory method.
  /// </summary>
  public class AutoSitecoreDataAttribute : AutoDataAttribute
  {
    public AutoSitecoreDataAttribute()
    {
      Fixture.Customize(new AutoNSubstituteCustomization());

      Database db = Substitute.For<Database>();
      Fixture.Inject<Database>(db);

      Func<Item> substituteItemFactory = () =>
      {
        ID itemId = ID.NewID;
        ItemDefinition definition = new ItemDefinition(itemId, string.Empty, ID.Null, ID.Null);
        ItemData data = new ItemData(definition, Language.Current, Sitecore.Data.Version.First, new FieldList());
        Item item = Substitute.For<Item>(itemId, data, db);
        ItemPath itemPath = Substitute.For<ItemPath>(item);
        item.Paths.Returns(itemPath);
        return item;
      };

      Fixture.Register<Item>(substituteItemFactory);
    }
  }
}