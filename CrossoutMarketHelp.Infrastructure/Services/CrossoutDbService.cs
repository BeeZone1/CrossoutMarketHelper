using CrossoutMarketHelp.ApplicationCore.Entities;
using CrossoutMarketHelp.ApplicationCore.Interfaces;
using CrossoutMarketHelp.Infrastructure.Handlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CrossoutMarketHelp.Infrastructure.Services
{
	public class CrossoutDbService : ICrossoutDbService
	{
		private HttpClient _httpClient;

		private readonly string _crossoutDbBaseUrl = "https://crossoutdb.com";
		private readonly string _allItemsURL = "/api/v1/items";
		private readonly string _buypriceURL = "/api/v1/market/buyprice/";
		private readonly string _sellpriceURL = "/api/v1/market/sellprice/";

		public CrossoutDbService()
		{
			_httpClient = new HttpClient(new RetryHandler(new HttpClientHandler()));
			_httpClient.BaseAddress = new Uri(_crossoutDbBaseUrl);
			_httpClient.DefaultRequestHeaders.ExpectContinue = false;
			_httpClient.Timeout = new TimeSpan(0, 2, 0);
		}

		/// <summary>
		/// Get all items info
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<CrossoutItem>> GetAllItems(CancellationToken cancellationToken)
		{
			var crossoutItems = new List<CrossoutItem>();

			string jsonStringAllItems = await _httpClient.GetStringAsync(_allItemsURL);
			JArray allInfoFromCrossoutDB = JsonConvert.DeserializeObject<JArray>(jsonStringAllItems);

			foreach (var item in allInfoFromCrossoutDB)
			{
				cancellationToken.ThrowIfCancellationRequested();

				try
				{
					if (item["removed"].ToString() == "1")
						continue;

					var itemId = (int)item["id"];

					crossoutItems.Add(new CrossoutItem
					{
						id = itemId,
						urlCrossoutDB = new Uri($"{_crossoutDbBaseUrl}/item/{itemId}"),
						image = new Uri($"/Img/{itemId}.png", UriKind.Relative),
						name = item["name"].ToString(),
						buyPrice = (double)item["formatBuyPrice"],
						sellPrice = (double)item["formatSellPrice"],
						craftingSum = (double)item["formatCraftingMargin"]
						//buyPriceAverage = buyPriceAverage,
						//buyPriceCompare = Math.Round((100 - formatBuyPrice * 100 / buyPriceAverage), 2),
					});
				}
				catch (Exception ex)
				{
					//TODO: log
				}
			}

			return crossoutItems;
		}

		/// <summary>
		/// Get price history on exact item
		/// </summary>
		/// <param name="itemId"></param>
		/// <returns></returns>
		public async Task<IEnumerable<double>> GetItemBuyPriceHistory(int itemId)
		{
			var httpResponse = await _httpClient.GetAsync(string.Concat(_buypriceURL, itemId), CancellationToken.None);
			var jsonStringSellPrice = httpResponse.Content.ReadAsStringAsync();
			var jsonArr = JsonConvert.DeserializeObject<JArray>(jsonStringSellPrice.Result).Children();

			var itemBuyPriceHistory = jsonArr.Select(x => x[1].ToObject<double>()).ToList();

			return itemBuyPriceHistory;
		}
	}
}
