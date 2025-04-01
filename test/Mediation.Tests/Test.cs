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
    class InnerPipelineStep<TResponse> : IPipelineStep<Task<TResponse>> 
    {
        readonly RequestEventLog _messages;

        public InnerPipelineStep(RequestEventLog messages) 
        {
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        public async Task<TResponse> Handle(object input, Func<object, CancellationToken, Task<TResponse>> next, CancellationToken cancellationToken)
        {                
            _messages.Log("Inner Pipe Entering");
            var response = await next(input, cancellationToken)
                .ConfigureAwait(false);
            _messages.Log("Inner Pipe Exiting");
            return response;
        }
    }

    interface IPipelineStepProvider
    {
        bool TryGetStep<TReturn>(IServiceProvider services, out IPipelineStep<TReturn> step);
    }

    private class OuterPipelineStep<TResponse> : IPipelineStep<Task<TResponse>> 
    {
        readonly RequestEventLog _messages;

        public OuterPipelineStep(RequestEventLog messages) {
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        public async Task<TResponse> Handle(object input, Func<object, CancellationToken, Task<TResponse>> next, CancellationToken cancellationToken)
        {
            _messages.Log("Outer Pipe Entering");
            var response = await next(input, cancellationToken)
                .ConfigureAwait(false);
            _messages.Log("Outer Pipe Exiting");
            return response;
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
            .AddMediatorConfiguration(
                new BasicMediationRegistrar(),
                builder => {
                    builder
                        .AddTypes(Assembly.GetExecutingAssembly().GetTypes())
                        .AddNotifications(x => {})
                        .AddRequests(x => {});
                })
            .AddSingleton<RequestEventLog>()
            .AddTransient<IRequestHandler<Ping, Pong>, RequestHandlerTest>()
            .AddTransient(typeof(IPipelineStep<>), typeof(OuterPipelineStep<>))
            .AddTransient(typeof(IPipelineStep<>), typeof(InnerPipelineStep<>))
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
