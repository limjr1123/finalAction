using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    // 서비스 등록
    public static void Register<T>(T service)
    {
        services[typeof(T)] = service;
    }

    // 서비스 조회
    public static T Get<T>()
    {
        if (services.TryGetValue(typeof(T), out var service))
            return (T)service;
        throw new Exception($"{typeof(T)} is not registered in ServiceLocator.");
    }
}
