using CrossoutMarketHelp.ApplicationCore.Entities;
using CrossoutMarketHelp.ApplicationCore.Interfaces;
using CrossoutMarketHelp.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CrossoutMarketHelp.Wpf
{
	public partial class MainWindow : Window
	{
		private CancellationTokenSource cnclToken = new CancellationTokenSource();
		private readonly ICrossoutDbService _crossoutDbService;

		public MainWindow(ICrossoutDbService crossoutDbService)
		{
			_crossoutDbService = crossoutDbService;

			InitializeComponent();

			ServicePointManager.DefaultConnectionLimit = 1000;
		}

		public async void UpdateAllItems()
		{
			var crossoutItems = new List<CrossoutItem>();

			// Day count in radiobutton
			var radioButtons = mainGrid.Children.OfType<RadioButton>().ToList();

			// All items info
			var allItemsFromCrossoutDb = await _crossoutDbService.GetAllItems(cnclToken.Token);

			progressBar.Maximum = allItemsFromCrossoutDb.Count();
			foreach (var item in allItemsFromCrossoutDb)
			{
				cnclToken.Token.ThrowIfCancellationRequested();

				progressBar.Value++;
				try
				{
					// Price range in textbox (default is 30 to 500)
					if (item.buyPrice > int.Parse(txtPriceTo.Text) || item.buyPrice < int.Parse(txtPriceFrom.Text))
						continue;

					// 1-item price history
					var itemBuyPriceHistory = await _crossoutDbService.GetItemBuyPriceHistory(item.id);

					// Picks price in the last n days depending on radiobutton
					int.TryParse(radioButtons.Where(x => x.IsChecked ?? false).Single().Content.ToString(), out int priceCount);
					double buyPriceAverage = itemBuyPriceHistory
						.Take(priceCount * 288) //288 5-mins interval in a day
						.Average() / 100;

					// Add info about every item into model
					item.buyPriceAverage = buyPriceAverage;
					item.buyPriceCompare = Math.Round((100 - item.buyPrice * 100 / buyPriceAverage), 2);

					crossoutItems.Add(item);
				}
				catch (Exception ex)
				{
					List1.Items.Add(ex.Message);
				}

				//TODO: Temporary log
				commonDataGrid.ItemsSource = crossoutItems;
				List1.Items.Add("id=" + item.id);
				CollectionViewSource.GetDefaultView(commonDataGrid.ItemsSource).Refresh();
			}
		}

		private void BtnRefresh_Click(object sender, RoutedEventArgs e)
		{
			progressBar.Value = 0;
			cnclToken.Cancel();
			List1.Items.Clear();
			commonDataGrid.ItemsSource = null;
			commonDataGrid.Items.Clear();
			cnclToken = new CancellationTokenSource();

			//Read all from Crossoutdb
			UpdateAllItems();
		}

		/// <summary>
		/// Open CrossoutDB on image click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LnkImg_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
		}

		private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		//private void BtnAddToFavorite_Click(object sender, RoutedEventArgs e)
		//{
		//	try
		//	{
		//		var favoriteItems = favoriteGrid.ItemsSource as List<CrossoutItem> ?? new List<CrossoutItem>();
		//		favoriteItems.Add((CrossoutItem)((Button)e.Source).DataContext);
		//		favoriteGrid.ItemsSource = favoriteItems;
		//		CollectionViewSource.GetDefaultView(favoriteGrid.ItemsSource).Refresh();
		//	}
		//	catch (Exception ex)
		//	{
		//		List1.Items.Add(ex.Message);
		//	}
		//}

		//private void BtnRemoveFavorite_Click(object sender, RoutedEventArgs e)
		//{
		//	try
		//	{
		//		var favoriteItems = favoriteGrid.ItemsSource as List<CrossoutItem> ?? new List<CrossoutItem>();
		//		favoriteItems.Remove((CrossoutItem)((Button)e.Source).DataContext);
		//		favoriteGrid.ItemsSource = favoriteItems;

		//		CollectionViewSource.GetDefaultView(favoriteGrid.ItemsSource).Refresh();
		//	}
		//	catch (Exception ex)
		//	{
		//		List1.Items.Add(ex.Message);
		//	}
		//}

		/*
		DispatcherTimer timerUpdateFavorite = new DispatcherTimer();
		timerUpdateFavorite.Tick += TimerUpdateFavorite_Tick;
		timerUpdateFavorite.Interval = TimeSpan.FromSeconds(10);
		timerUpdateFavorite.Start();

		private async void TimerUpdateFavorite_Tick(object sender, EventArgs e)
        {
            using (HttpClient clientFavorite = new HttpClient(new RetryHandler(new HttpClientHandler())))
            {
                string jsonStringAllItems = await clientFavorite.GetStringAsync(allItemsURL);
                JArray allInfoFromCrossoutDB = JsonConvert.DeserializeObject<JArray>(jsonStringAllItems);
                foreach (var item in favoriteItems)
                {
                    //JObject jobjItem = allInfoFromCrossoutDB.Children<JObject>().FirstOrDefault(x => (int)x["id"] == item.id);

                    string jsonStringBuyPrice = await clientFavorite.GetStringAsync(String.Concat("https://crossoutdb.com/api/v1/market/buyprice/", item.id));
                    var jsonArrBuy = JsonConvert.DeserializeObject<JArray>(jsonStringBuyPrice).Children();
                    double buyPriceAverage = jsonArrBuy
                     .Take(10000)
                     .Average((x => x[1].ToObject<int>())) / 100;

                    string jsonStringSellPrice = await clientFavorite.GetStringAsync(String.Concat("https://crossoutdb.com/api/v1/market/sellprice/", item.id));
                    var jsonArrSell = JsonConvert.DeserializeObject<JArray>(jsonStringSellPrice).Children();
                    double sellPriceAverage = jsonArrSell
                        .Take(10000)
                        .Average((x => x[1].ToObject<int>())) / 100;

                    item.buyPriceAverage = buyPriceAverage;
                    item.sellPriceAverage = sellPriceAverage;
                    CollectionViewSource.GetDefaultView(favoriteGrid.ItemsSource).Refresh();
                }

            };
        }
		*/
	}
}
