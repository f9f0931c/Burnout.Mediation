using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Burnout.Mediation.Requests;
using Burnout.Mediation.Notifications;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Burnout.Mediation.Tests;

public class Test 
{
    struct Ping 
    {
        public string Message { get; }

        public Ping(string message) 
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }

    struct Pong 
    {
        public string Message { get; }

        public Pong(string message) 
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }

    class RequestHandlerTest : IRequestHandler<Ping, Pong> 
    {
        public Pong Handle(Ping request) 
        {
            return new Pong("Pong");
        }

        public Task<Pong> HandleAsync(Ping request, CancellationToken cancellationToken) 
        {
            return Task.FromResult(new Pong("Pong"));
        }
    }

    class RequestEventLog 
    {
        List<string> _Events { get; } = new List<string>();
        public IReadOnlyList<string> Events { get => _Events; }

        public void Log(string @event) 
        {
            _Events.Add(@event);
        }
    }

    IServiceProvider GetRequestServiceProvider() 
    {
        return new ServiceCollection()
            .AddTransient<IMediator, Mediator>()
            .AddSingleton<RequestEventLog>()
            .AddTransient<IRequestHandler<Ping, Pong>, RequestHandlerTest>()
            .BuildServiceProvider();
    }

    [Fact]
    public async Task Testing() 
    {
        IServiceProvider provider;
        provider = GetRequestServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var result = await mediator
//                .AddPipelines()
            .RequestAsync<Ping, Pong>(new Ping("Ping"))
            .ConfigureAwait(false);
        Assert.IsType<Pong>(result);
        Assert.Equal("Pong", result.Message);
        // var log = provider.GetRequiredService<RequestEventLog>();
        // Assert.Equal(log.Events, new string[] {
        //     "Outer Pipe Executing Async",
        //     "Inner Pipe Executing Async",
        //     "Inner Pipe Executed Async",
        //     "Outer Pipe Executed Async"
        // });
    }
}
