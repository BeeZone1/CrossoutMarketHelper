using System.Globalization;

namespace CrossoutMarketHelp.Wpf.Formatters
{
	public class PriceFormatter
	{
		public static string FormatPrice(int price)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", price / 100m);
		}

		public static string FormatPrice(decimal price)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", price / 100m);
		}
	}
}
