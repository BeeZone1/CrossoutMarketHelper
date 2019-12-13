using Polly;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CrossoutMarketHelp.Infrastructure.Handlers
{
	public class RetryHandler : DelegatingHandler
	{
		public RetryHandler(HttpClientHandler handler) : base(handler) { }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
			CancellationToken cancellationToken)
		{
			var httpResponseMessage = Policy.Handle<HttpRequestException>()
				.Or<TaskCanceledException>()
				.Or<TimeoutException>()
				.Or<TaskCanceledException>()
				.WaitAndRetryAsync(new [] 
				{
					TimeSpan.FromSeconds(5),
					TimeSpan.FromSeconds(10),
					TimeSpan.FromSeconds(20)
				}).ExecuteAsync(() => base.SendAsync(request, cancellationToken));

			return httpResponseMessage;
		}
	}
}
