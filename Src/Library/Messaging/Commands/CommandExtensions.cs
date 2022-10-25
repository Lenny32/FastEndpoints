﻿using Microsoft.Extensions.DependencyInjection;

namespace FastEndpoints;

public static class CommandExtensions
{
    internal static readonly Dictionary<Type, HandlerDefinition> handlerCache = new();

    internal class HandlerDefinition
    {
        internal Type HandlerType { get; set; }
        internal ObjectFactory? HandlerCreator { get; set; }
        internal object? ExecuteMethod { get; set; }

        internal HandlerDefinition(Type handlerType)
        {
            HandlerType = handlerType;
        }
    }

    /// <summary>
    /// executes the command and returns a result
    /// </summary>
    /// <typeparam name="TResult">the type of the returned result</typeparam>
    /// <param name="command">the command to execute</param>
    /// <param name="serviceProvider">optional service provider for resolving scoped/transient services</param>
    /// <param name="ct">optional cancellation token</param>
    /// <exception cref="InvalidOperationException">thrown when a handler for the command cannot be instantiated</exception>
    public static Task<TResult> ExecuteAsync<TResult>(this ICommand<TResult> command, IServiceProvider? serviceProvider = null, CancellationToken ct = default)
    {
        var tCommand = command.GetType();

        if (handlerCache.TryGetValue(tCommand, out var hndDef))
        {
            hndDef.HandlerCreator ??= ActivatorUtilities.CreateFactory(hndDef.HandlerType, Type.EmptyTypes);
            object? handler;
            if (serviceProvider is null)
            {
                using var scope = IServiceResolver.RootServiceProvider.CreateScope();
                handler = hndDef.HandlerCreator(scope.ServiceProvider, null);
            }
            else
            {
                handler = hndDef.HandlerCreator(serviceProvider, null);
            }
            hndDef.ExecuteMethod ??= hndDef.HandlerType.HandlerExecutor<TResult>(tCommand, handler);
            return ((Func<object, CancellationToken, Task<TResult>>)hndDef.ExecuteMethod)(command, ct);
        }
        throw new InvalidOperationException($"Unable to create an instance of the handler for command [{tCommand.FullName}]");
    }

    /// <summary>
    /// executes the command that does not return a result
    /// </summary>
    /// <param name="command">the command to execute</param>
    /// <param name="serviceProvider">optional service provider for resolving scoped/transient services</param>
    /// <param name="ct">optional cancellation token</param>
    /// <exception cref="InvalidOperationException">thrown when a handler for the command cannot be instantiated</exception>
    public static Task ExecuteAsync(this ICommand command, IServiceProvider? serviceProvider = null, CancellationToken ct = default)
    {
        var tCommand = command.GetType();

        if (handlerCache.TryGetValue(tCommand, out var hndDef))
        {
            hndDef.HandlerCreator ??= ActivatorUtilities.CreateFactory(hndDef.HandlerType, Type.EmptyTypes);
            object? handler;
            if (serviceProvider is null)
            {
                using var scope = IServiceResolver.RootServiceProvider.CreateScope();
                handler = hndDef.HandlerCreator(scope.ServiceProvider, null);
            }
            else
            {
                handler = hndDef.HandlerCreator(serviceProvider, null);
            }
            hndDef.ExecuteMethod ??= hndDef.HandlerType.HandlerExecutor(tCommand, handler);
            return ((Func<object, CancellationToken, Task>)hndDef.ExecuteMethod)(command, ct);
        }
        throw new InvalidOperationException($"Unable to create an instance of the handler for command [{tCommand.FullName}]");
    }
}