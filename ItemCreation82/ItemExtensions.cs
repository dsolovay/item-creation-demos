using NSubstitute;
using Sitecore.Collections;
using Sitecore.Data.Items;

namespace ItemCreation82
{
	public static class ItemExtensions
	{
		public static void SetChildren(this Item item, ItemList items = null)
		{
			if (items == null)
			{
				items = new ItemList();
			}
			item.GetChildren().Returns(new ChildList(item, items));
		}
	}
}