using System.Collections.Generic;

namespace CrossoutMarketHelp.Wpf.Data
{
	public class RefillDB
	{
		public static void UpdateDB(Dictionary<int, string> itemNames)
		{
			using (ItemDescriptionContext db = new ItemDescriptionContext())
			{
				foreach (var item in itemNames)
				{
					db.ItemDescriptions.Add(new ItemDescription { ItemId = item.Key, Name = item.Value });
				}
				db.SaveChanges();
			}
		}
	}
}
