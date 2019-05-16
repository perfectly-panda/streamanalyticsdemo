using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using WebApplication;

namespace WebApplication.Hubs
{
    public class LogHub : Hub
    {
    }
}
