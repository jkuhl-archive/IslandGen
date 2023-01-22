namespace IslandGen.Services;

/// Based on: MonoGame GameServiceContainer
/// Source: https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/GameServiceContainer.cs
/// 
/// MIT License - Copyright (C) The Mono.Xna Team
/// This file is subject to the terms and conditions defined in
/// file 'LICENSE.txt', which is part of this source code package.
public static class ServiceManager
{
    private static readonly Dictionary<Type, object?> Services = new();

    /// <summary>
    ///     Add a service provider to this container.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    /// <param name="service">The provider of the service.</param>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="service" /> is <code>null</code>.
    /// </exception>
    public static void AddService<T>(T? service)
    {
        var serviceType = typeof(T);
        if (serviceType == null)
            throw new ArgumentNullException(nameof(serviceType));
        if (service == null)
            throw new ArgumentNullException(nameof(service));

        Services.Add(serviceType, service);
    }

    /// <summary>
    ///     Get a service provider of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service provider.</typeparam>
    /// <returns>
    ///     A service provider of the specified type or <code>null</code> if
    ///     no suitable service provider is registered in this container.
    /// </returns>
    public static T GetService<T>() where T : class
    {
        var requestedType = typeof(T);
        if (requestedType == null)
            throw new ArgumentNullException(nameof(requestedType));

        if (Services.TryGetValue(requestedType, out var service))
            if (service == null)
                throw new NullReferenceException(nameof(requestedType));

        return (T)service!;
    }

    /// <summary>
    ///     Remove the service with the specified type. Does nothing no service of the specified type is registered.
    /// </summary>
    /// <param name="type">The type of the service to remove.</param>
    /// <exception cref="ArgumentNullException">If the specified type is <code>null</code>.</exception>
    public static void RemoveService(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        Services.Remove(type);
    }

    /// <summary>
    ///     Replaces an existing service
    /// </summary>
    /// <param name="service"> The provider of the service. </param>
    /// <typeparam name="T"> The type of the service provider. </typeparam>
    public static void ReplaceService<T>(T? service)
    {
        RemoveService(typeof(T));
        AddService(service);
    }
}