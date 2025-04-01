﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Mediation.Tests; 

class AddNode<T>
    where T : IAdditionOperators<T, T, T>
{
    void Test(T a, T b) {
        var c = a + b;
        int d = 0;
    }
}

public class Test {
    struct Ping {
        public string Message { get; }

        public Ping(string message) {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }

    struct Pong {
        public string Message { get; }

        public Pong(string message) {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }

    class NotificationHandlerTest : INotificationHandler<Ping> {
        public void Handle(Ping request) {
            var x =  new Pong(nameof(Pong));
        }

        public Task HandleAsync(Ping request, CancellationToken cancellationToken) {
            var x = Task.FromResult(new Pong(nameof(Pong)));
            return Task.CompletedTask;
        }
    }

    class RequestHandlerTest : IRequestHandler<Ping, Pong> {
        public Pong Handle(Ping request) {
            return new Pong("Pong");
        }

        public Task<Pong> HandleAsync(Ping request, CancellationToken cancellationToken) {
            return Task.FromResult(new Pong("Pong"));
        }
    }

    class InnerPipelineStep<TRequest, TResponse> : IRequestPipelineStep<TRequest, TResponse> {
        RequestEventLog Messages { get; }

        public InnerPipelineStep(RequestEventLog messages) {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        public TResponse Execute(TRequest request, Func<TResponse> next) {
            Messages.Log("Inner Pipe Executing");
            var response = next();
            Messages.Log("Inner Pipe Executed");
            return response;
        }

        public async Task<TResponse> ExecuteAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken) {
            Messages.Log("Inner Pipe Executing Async");
            var response = await next().ConfigureAwait(false);
            Messages.Log("Inner Pipe Executed Async");
            return response;
        }
    }

    class OuterPipelineStep<TRequest, TResponse> : IRequestPipelineStep<TRequest, TResponse> {
        RequestEventLog Messages { get; }

        public OuterPipelineStep(RequestEventLog messages) {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        public TResponse Execute(TRequest request, Func<TResponse> next) {
            Messages.Log("Outer Pipe Executing");
            var response = next();
            Messages.Log("Outer Pipe Executed");
            return response;
        }

        public async Task<TResponse> ExecuteAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken) {
            Messages.Log("Outer Pipe Executing Async");
            var response = await next().ConfigureAwait(false);
            Messages.Log("Outer Pipe Executed Async");
            return response;
        }
    }

    class RequestEventLog {
        List<string> _Events { get; } = new List<string>();
        public IReadOnlyList<string> Events { get => _Events; }

        public void Log(string @event) {
            _Events.Add(@event);
        }
    }

    IServiceProvider GetRequestServiceProvider() {
        return new ServiceCollection()
            .AddSingleton(new RequestEventLog())
            .AddMediator<DefaultMediator>(Assembly.GetExecutingAssembly())
            .AddTransient(typeof(IRequestPipelineStep<,>), typeof(OuterPipelineStep<,>))
            .AddTransient(typeof(IRequestPipelineStep<,>), typeof(InnerPipelineStep<,>))
            .BuildServiceProvider();
    }

    [Fact]
    public async Task Testing() {
        var provider = GetRequestServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var result = await mediator.RequestAsync<Ping, Pong>(new Ping("Ping"))
            .ConfigureAwait(false);
        await mediator.SendAsync(new Ping("Ping"));
        Assert.IsType<Pong>(result);
        Assert.Equal("Pong", result.Message);
        var log = provider.GetRequiredService<RequestEventLog>();
        Assert.Equal(log.Events, new string[] {
            "Outer Pipe Executing Async",
            "Inner Pipe Executing Async",
            "Inner Pipe Executed Async",
            "Outer Pipe Executed Async"
        });
    }
}
