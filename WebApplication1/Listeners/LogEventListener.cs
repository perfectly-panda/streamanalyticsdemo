using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using WebApplication.Hubs;
using System.Threading;
using Newtonsoft.Json;
using System.Text;

namespace WebApplication
{
    public class LogEventListener : IHostedService
    {
        private static SubscriptionClient _logClient;
        private static SubscriptionClient _orderClient;
        private static SubscriptionClient _machineClient;
        private static SubscriptionClient _analyticsClient;
        private IConfiguration _configuration;
        private IHubContext<LogHub> _hubContext;

        public LogEventListener(IConfiguration configuration, IHubContext<LogHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var serviceBusConnectionString = _configuration["Data:ServiceBusConnection"];

            _logClient = new SubscriptionClient(serviceBusConnectionString, "logs", _configuration["Data:ServiceBusSubscription"]);
            _orderClient = new SubscriptionClient(serviceBusConnectionString, "orders", _configuration["Data:ServiceBusSubscription"]);
            _machineClient = new SubscriptionClient(serviceBusConnectionString, "machines", _configuration["Data:ServiceBusSubscription"]);
            _analyticsClient = new SubscriptionClient(serviceBusConnectionString, "analytics", _configuration["Data:ServiceBusSubscription"]);

            RegisterOnMessageHandlerAndReceiveMessages();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _logClient.CloseAsync();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,
                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            _logClient.RegisterMessageHandler(ProcessLogMessagesAsync, messageHandlerOptions);
            _orderClient.RegisterMessageHandler(ProcessOrderMessagesAsync, messageHandlerOptions);
            _machineClient.RegisterMessageHandler(ProcessMachineMessagesAsync, messageHandlerOptions);
            _analyticsClient.RegisterMessageHandler(ProcessAnalyticsMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessLogMessagesAsync(Message message, CancellationToken token)
        {

            await _logClient.CompleteAsync(message.SystemProperties.LockToken);

            await _hubContext.Clients.All.SendAsync("newLog", Encoding.UTF8.GetString(message.Body));
        }

        private async Task ProcessOrderMessagesAsync(Message message, CancellationToken token)
        {

            await _orderClient.CompleteAsync(message.SystemProperties.LockToken);

            await _hubContext.Clients.All.SendAsync("orderUpdate", Encoding.UTF8.GetString(message.Body));
        }

        private async Task ProcessMachineMessagesAsync(Message message, CancellationToken token)
        {

            await _machineClient.CompleteAsync(message.SystemProperties.LockToken);

            await _hubContext.Clients.All.SendAsync("machineUpdate", Encoding.UTF8.GetString(message.Body));
        }

        private async Task ProcessAnalyticsMessagesAsync(Message message, CancellationToken token)
        {

            await _analyticsClient.CompleteAsync(message.SystemProperties.LockToken);

            await _hubContext.Clients.All.SendAsync("analyticsUpdate", Encoding.UTF8.GetString(message.Body));
        }

        // Use this handler to examine the exceptions received on the message pump.
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}