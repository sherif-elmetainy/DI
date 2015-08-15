// Copyright (c) Sherif Elmetainy (Code Art). All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the solution root for license information.

using CodeArt.DependencyInjection;
using System;
using System.Linq;
using Microsoft.Framework.Internal;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    ///     Extension methods for setting adding support for Decorators
    /// </summary>
    public static class DecoratorServiceCollectionExtensions
    {
        /// <summary>
        ///     Whether the decorator can be used to decorate the service specified by the service descriptor
        /// </summary>
        /// <param name="decoratorServiceLifetime">The <see cref="ServiceLifetime"/> of the decorator service.</param>
        /// <param name="serviceDescriptor">The service descriptor of to test</param>
        /// <param name="serviceType">The service type that the decorator can decorate.</param>
        /// <returns>True if the decorator can be used to decorate the specified service.</returns>
        private static bool CanDecorate(ServiceLifetime decoratorServiceLifetime, ServiceDescriptor serviceDescriptor, Type serviceType)
        {
            if (serviceDescriptor.ServiceType != serviceType)
                return false;
            if (serviceDescriptor.ImplementationInstance == null && serviceDescriptor.ImplementationFactory == null)
            {
                if (serviceDescriptor.ImplementationType == null)
                    return false; // We should never get here because the service will always have one of these.

                // If the implementation type is the same as the service type,
                // we can't create decorate it because the IServiceProvider.GetService(serviceType) will return the decorator type and to get a service to decorate we need it to return the implementation type.
                if (serviceDescriptor.ServiceType == serviceDescriptor.ImplementationType)
                    return false;
            }
            // it's ok for a transient decorator to hold a reference to a scoped or singleton service
            if (decoratorServiceLifetime == ServiceLifetime.Transient)
                return true;
            // A scoped decorator cannot hold a reference to a transient service
            else if (decoratorServiceLifetime == ServiceLifetime.Scoped)
                return serviceDescriptor.Lifetime != ServiceLifetime.Transient;
            // A singleton decorator cannot hold a reference to a scoped or transient service
            else
                return serviceDescriptor.Lifetime == ServiceLifetime.Singleton;
        }

        /// <summary>
        ///     Creates a service factory for the decorator service
        /// </summary>
        /// <param name="services">services collection</param>
        /// <param name="descriptor">service descriptor to decorate</param>
        /// <param name="decoratorInitializer">method to initilize the decorator instance</param>
        /// <returns>Factory method for the decorator service</returns>
        private static Func<IServiceProvider, object> CreateDecoratorServiceFactory(IServiceCollection services, ServiceDescriptor descriptor, Func<IServiceProvider, object, object> decoratorInitializer)
        {
            var singletonInstance = descriptor.ImplementationInstance;
            if (descriptor.ImplementationInstance != null)
            {
                // singleton instance, create a factory that wraps that instance
                return sp => decoratorInitializer(sp, singletonInstance);
            }
            var origFactory = descriptor.ImplementationFactory;
            if (origFactory != null)
            {
                // service having an implementation factory. Creates a factory that calls that factory, then wraps the result.
                return sp =>
                {
                    var instance = origFactory(sp);
                    return instance == null ? null : decoratorInitializer(sp, instance);
                };
            }
            if (services.All(sd => sd.ServiceType != descriptor.ImplementationType))
            {
                // Add the implementation type as a service type to the service collection
                // This will allow our factory to call IServiceProvider.GetService(impelementationType) 
                // and have the service created and all it's dependencies injected.
                services.Add(new ServiceDescriptor(descriptor.ImplementationType, descriptor.ImplementationType, descriptor.Lifetime));
            }
            var type = descriptor.ImplementationType;
            return sp =>
            {
                var instance = sp.GetService(type);
                return instance == null ? null : decoratorInitializer(sp, instance);
            };
        }

        /// <summary>
        ///     Add a service that implements the decorator pattern to the <see cref="IServiceCollection"/>. 
        /// </summary>
        /// <typeparam name="TService">The type of service to decorate.</typeparam>
        /// <typeparam name="TDecorator">The type of service that implements the decorator pattern</typeparam>
        /// <param name="services">The service collection to add the decorator sevice to</param>
        /// <param name="lifetime">The lifetime of the decorator service instance (defaults to transient)</param>
        /// <returns>A reference to the service collection</returns>
        /// <remarks>
        ///     All <see cref="ServiceDescriptor"/> instances in the service collection whose <see cref="ServiceDescriptor.ServiceType"/> is the same as <typeparamref name="TService"/>, 
        ///     will be replaced by another that uses the service type specified by <typeparamref name="TDecorator"/> instead.
        ///     The service specified by <typeparamref name="TDecorator"/> will be also added to the <see cref="IServiceCollection"/> as a transient service. If it is not already added.
        ///     If it is required that the decorator service have different life time, it can be added to the service collection before the call to AddDecorator. 
        ///     But note that a service cannot decorate a service whose lifetime is shorter (For example a singleton service cannot decorate a transient or scoped service).
        /// </remarks>
        public static IServiceCollection AddDecorator<TService, TDecorator>([NotNull] this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient) where TDecorator : class, TService, IDecorator<TService>
        {
            var decoratorServiceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(TDecorator));
            var decoratorLifeTime = decoratorServiceDescriptor?.Lifetime ?? lifetime;
            if (decoratorServiceDescriptor == null)
            {
                // Add the decorator service
                services.AddTransient<TDecorator, TDecorator>();
            }
            for (var index = 0; index < services.Count; index++)
            {
                var descriptor = services[index];
                if (CanDecorate(decoratorLifeTime, descriptor, typeof(TService)))
                {
                    var factory = CreateDecoratorServiceFactory(services, descriptor, (sp, o) =>
                    {
                        var decorator = sp.GetRequiredService<TDecorator>();
                        decorator.DecoratedService = (TService)o;
                        return decorator;
                    });
                    var replacementDescriptor = new ServiceDescriptor(descriptor.ServiceType, factory, decoratorLifeTime);
                    services[index] = replacementDescriptor;
                }
            }

            return services;
        }

        /// <summary>
        ///     Add a service that implements the decorator pattern to the <see cref="IServiceCollection"/>. 
        /// </summary>
        /// <param name="services">The service collection to add the decorator sevice to</param>
        /// <param name="serviceType">The type of service to decorate.</param>
        /// <param name="decoratorType">The type of service that implements the decorator pattern</param>
        /// <param name="lifetime">lifetime of the decorator service (defaults to transient)</param>
        /// <returns>A reference to the service collection</returns>
        /// <remarks>
        ///     All <see cref="ServiceDescriptor"/> instances in the service collection whose <see cref="ServiceDescriptor.ServiceType"/> is the same as <paramref name="serviceType"/>, 
        ///     will be replaced by another that uses the service type specified by <paramref name="decoratorType"/> instead.
        ///     The service specified by decorator type will be also added to the <see cref="IServiceCollection"/> as a transient service. If it is not already added.
        ///     If it is required that the decorator service have different life time, it can be added to the service collection before the call to AddDecorator. 
        ///     But note that a service cannot decorate a service whose lifetime is shorter (For example a singleton service cannot decorate a transient or scoped service).
        /// </remarks>
        public static IServiceCollection AddDecorator([NotNull] this IServiceCollection services, [NotNull] Type serviceType, [NotNull] Type decoratorType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (!decoratorType.GetTypeInfo().IsClass)
            {
                throw new ArgumentException($"The type '{decoratorType.FullName}' specified in {nameof(decoratorType)} is not a class.", nameof(decoratorType));
            }
            if (!serviceType.IsAssignableFrom(decoratorType))
            {
                throw new ArgumentException($"The type '{serviceType.FullName}' specified in {nameof(serviceType)} parameter is not assignable from the type '{decoratorType.FullName}' specified in the {nameof(decoratorType)} parameter.", nameof(decoratorType));
            }
            if (decoratorType.GetTypeInfo().IsAbstract)
            {
                throw new ArgumentException($"The type '{decoratorType.FullName}' specified in {nameof(decoratorType)} is abstract.", nameof(decoratorType));
            }
            if (decoratorType == serviceType)
            {
                throw new ArgumentException($"The type '{serviceType.FullName}' specified in {nameof(serviceType)} parameter cannot be the same as the type specified in the {nameof(decoratorType)} parameter.", nameof(decoratorType));
            }

            var decoratorInterfaceType = typeof(IDecorator<>).MakeGenericType(serviceType);
            if (!decoratorInterfaceType.IsAssignableFrom(decoratorType))
            {
                throw new ArgumentException($"The type '{decoratorType.FullName}' specified in {nameof(decoratorType)} does not implement IDecorator<{serviceType.FullName}>.", nameof(decoratorType));
            }
            var decoratorServiceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == decoratorType);
            var decoratorLifeTime = decoratorServiceDescriptor?.Lifetime ?? lifetime;
            if (decoratorServiceDescriptor == null)
            {
                // Add the decorator service
                services.AddTransient(decoratorType, decoratorType);
            }
            var wrappedServiceProperty = decoratorInterfaceType.GetProperty(nameof(IDecorator<object>.DecoratedService));
            for (var i = 0; i < services.Count; i++)
            {
                var descriptor = services[i];
                if (CanDecorate(decoratorLifeTime, descriptor, serviceType))
                {
                    var factory = CreateDecoratorServiceFactory(services, descriptor, (sp, o) =>
                    {
                        var decorator = sp.GetRequiredService(decoratorType);
                        wrappedServiceProperty.SetValue(decorator, o);
                        return decorator;
                    });
                    var replacementDescriptor = new ServiceDescriptor(descriptor.ServiceType, factory, decoratorLifeTime);
                    services[i] = replacementDescriptor;
                }
            }

            return services;
        }


        /// <summary>
        ///     Add a service that implements the decorator pattern to the <see cref="IServiceCollection"/>. 
        /// </summary>
        /// <typeparam name="TService">The type of service to decorate.</typeparam>
        /// <param name="services">The service collection to add the decorator sevice to</param>
        /// <param name="decoratorFactory">The factory method to create the decorator instance.</param>
        /// <param name="lifetime">lifetime of the decorator service (defaults to transient)</param>
        /// <returns>A reference to the service collection</returns>
        /// <remarks>
        ///     All <see cref="ServiceDescriptor"/> instances in the service collection whose <see cref="ServiceDescriptor.ServiceType"/> is the same as <typeparamref name="TService"/>, 
        ///     will be replaced by another is created by the <paramref name="decoratorFactory"/> method.
        ///     This overload can be use to provide decorators that do not implement the <see cref="IDecorator{TService}" /> interface
        /// </remarks>
        public static IServiceCollection AddDecorator<TService>([NotNull] this IServiceCollection services,
            [NotNull] Func<IServiceProvider, TService, TService> decoratorFactory, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            for (var index = 0; index < services.Count; index++)
            {
                var descriptor = services[index];
                if (CanDecorate(lifetime, descriptor, typeof(TService)))
                {
                    var factory = CreateDecoratorServiceFactory(services, descriptor, (sp, o) => o == null ? default(TService) : decoratorFactory(sp, (TService)o));
                    var replacementDescriptor = new ServiceDescriptor(descriptor.ServiceType, factory, lifetime);
                    services[index] = replacementDescriptor;
                }
            }

            return services;
        }

        /// <summary>
        ///     Add a service that implements the decorator pattern to the <see cref="IServiceCollection"/>. 
        /// </summary>
        /// <param name="services">The service collection to add the decorator sevice to</param>
        /// <param name="serviceType">The service type to decorator</param>
        /// <param name="decoratorFactory">The factory method to create the decorator instance.</param>
        /// <param name="lifetime">lifetime of the decorator service (defaults to transient)</param>
        /// <returns>A reference to the service collection</returns>
        /// <remarks>
        ///     All <see cref="ServiceDescriptor"/> instances in the service collection whose <see cref="ServiceDescriptor.ServiceType"/> is the same as <paramref name="serviceType"/>, 
        ///     will be replaced by another is created by the <param name="decoratorFactory"></param> method.
        ///     This overload can be use to provide decorators that do not implement the <see cref="IDecorator{TService}" /> interface
        /// </remarks>
        public static IServiceCollection AddDecorator([NotNull] this IServiceCollection services, [NotNull] Type serviceType, [NotNull] Func<IServiceProvider, object, object> decoratorFactory, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            for (var index = 0; index < services.Count; index++)
            {
                var descriptor = services[index];
                if (CanDecorate(lifetime, descriptor, serviceType))
                {
                    var factory = CreateDecoratorServiceFactory(services, descriptor, (sp, o) => o == null ? null : decoratorFactory(sp, o));
                    var replacementDescriptor = new ServiceDescriptor(descriptor.ServiceType, factory, lifetime);
                    services[index] = replacementDescriptor;
                }
            }

            return services;
        }
    }
}
