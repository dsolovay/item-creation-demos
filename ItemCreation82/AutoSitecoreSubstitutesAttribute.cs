using System;
using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Version = Sitecore.Data.Version;

namespace ItemCreation82
{
	/// <summary>
	/// Creates NSubstitute Sitecore Items and Databases.
	/// Database object is "Frozen" (see http://blog.ploeh.dk/2010/03/17/AutoFixtureFreeze/)
	/// Extends AutoData with NSubstitute customization.
	/// 
	/// Item fields values, including ID, Name, TemplateID, and children, must be set 
	/// on the item afterwards.
	/// </summary>
	public class AutoSitecoreSubstitutesAttribute : AutoDataAttribute
	{
		public AutoSitecoreSubstitutesAttribute() : base(new Fixture().Customize(new AutoNSubstituteCustomization()))
		{
			Database db = Substitute.For<Database>();
			Fixture.Inject<Database>(db);

			Func<Item> substituteItemFactory = () =>
			{
				ItemDefinition definition = new ItemDefinition(ID.Null, string.Empty, ID.Null, ID.Null);
				ItemData data = new ItemData(definition, Language.Current, Version.First, new FieldList());
				Item item = Substitute.For<Item>(ID.Null, data, db);
				ItemPath itemPath = Substitute.For<ItemPath>(item);
				item.Paths.Returns(itemPath);
				return item;
			};

			Fixture.Register<Item>(substituteItemFactory);
		}
	}
}