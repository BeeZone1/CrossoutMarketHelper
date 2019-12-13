using System.Data.Entity;

namespace CrossoutMarketHelp.Wpf.Data
{
	class ItemDescriptionContext : DbContext
	{
		public ItemDescriptionContext() : base("DbConnection") { }

		public DbSet<ItemDescription> ItemDescriptions { get; set; }
	}
}