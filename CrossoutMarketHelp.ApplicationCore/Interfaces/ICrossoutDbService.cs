using CrossoutMarketHelp.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrossoutMarketHelp.ApplicationCore.Interfaces
{
	public interface ICrossoutDbService
	{
		Task<IEnumerable<CrossoutItem>> GetAllItems(CancellationToken cancellationToken);

		Task<IEnumerable<double>> GetItemBuyPriceHistory(int itemId);
	}
}
