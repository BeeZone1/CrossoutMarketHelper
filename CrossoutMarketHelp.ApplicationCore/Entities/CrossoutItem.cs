using System;

namespace CrossoutMarketHelp.ApplicationCore.Entities
{
	public class CrossoutItem
	{
		public int id { get; set; }

		public int rarityId { get; set; }

		public int categoryId { get; set; }

		public int typeId { get; set; }

		public int factionNumber { get; set; }

		public Uri urlCrossoutDB { get; set; }

		public Uri image { get; set; }

		public string name { get; set; }

		public double craftingSum { get; set; }

		public double buyPrice { get; set; }

		public double buyPriceAverage { get; set; }

		public double buyPriceCompare { get; set; }

		public double userBuyPrice { get; set; }

		public double sellPrice { get; set; }

		public double sellPriceAverage { get; set; }

		public double userSellPrice { get; set; }
	}
}
