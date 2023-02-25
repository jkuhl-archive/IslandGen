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
    ///     Adds a service
    /// </summary>
    /// <typeparam name="T"> Type of the service </typeparam>
    /// <param name="service"> Service object </param>
    /// <exception cref="ArgumentNullException"> If service is null </exception>
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
    ///     Gets a service of the specified type
    /// </summary>
    /// <typeparam name="T"> Type of the service </typeparam>
    /// <returns> Service of the specified type or null </returns>
    /// <exception cref="ArgumentNullException"> If component is null </exception>
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
    ///     Removes a service with the specified type
    /// </summary>
    /// <param name="type"> Type of the service to remove </param>
    /// <exception cref="ArgumentNullException"> If the specified type is null </exception>
    public static void RemoveService(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        Services.Remove(type);
    }

    /// <summary>
    ///     Replaces an existing service
    /// </summary>
    /// <typeparam name="T"> Type of the service </typeparam>
    /// <param name="service"> Service object </param>
    public static void ReplaceService<T>(T? service)
    {
        RemoveService(typeof(T));
        AddService(service);
    }
}