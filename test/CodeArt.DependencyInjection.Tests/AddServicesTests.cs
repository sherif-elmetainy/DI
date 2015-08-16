// Copyright (c) Sherif Elmetainy (Code Art).
// Licensed under the MIT License. See LICENSE.txt in the solution root for license information.

using System.Reflection;
using Microsoft.Framework.DependencyInjection;
using Xunit;
// ReSharper disable UnusedMember.Global

namespace CodeArt.DependencyInjection.Tests
{
    /// <summary>
    ///     Test methods for adding implemented interfaces and types in an assembly
    /// </summary>
    public class AddServicesTests
    {
        /// <summary>
        ///     Test <see cref="CommonServiceCollectionExtensions.AddImplementedInterfaces{TImpementationType}"/> functionality
        /// </summary>
        [Fact]
        public void TestAddImplementedInterfaces()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddImplementedInterfaces<HelloService>(ServiceLifetime.Scoped);

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IHelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
        }

        /// <summary>
        ///     Test <see cref="CommonServiceCollectionExtensions.TryAddImplementedInterfaces{TImpementationType}"/> functionality
        /// </summary>
        [Fact]
        public void TestTryAddImplementedInterfaces()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.TryAddImplementedInterfaces<HelloService>(ServiceLifetime.Scoped);

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IHelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            serviceCollection.TryAddImplementedInterfaces<HelloService>(ServiceLifetime.Scoped);

            Assert.Equal(1, serviceCollection.Count);
        }

        /// <summary>
        ///     Test <see cref="CommonServiceCollectionExtensions.AddImplementedInterfaces"/> functionality
        /// </summary>
        [Fact]
        public void TestNonGenericAddImplementedInterfaces()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddImplementedInterfaces(typeof(HelloService), ServiceLifetime.Scoped);

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IHelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
        }

        /// <summary>
        ///     Test <see cref="CommonServiceCollectionExtensions.TryAddImplementedInterfaces"/> functionality
        /// </summary>
        [Fact]
        public void TestNonGenericTryAddImplementedInterfaces()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.TryAddImplementedInterfaces(typeof(HelloService), ServiceLifetime.Scoped);

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IHelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            serviceCollection.TryAddImplementedInterfaces(typeof(HelloService), ServiceLifetime.Scoped);

            Assert.Equal(1, serviceCollection.Count);
        }

        /// <summary>
        ///     Test <see cref="CommonServiceCollectionExtensions.AddSelfAndImplementedInterfaces{TImpementationType}"/> functionality
        /// </summary>
        [Fact]
        public void TestAddSelfAndImplementedInterfaces()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSelfAndImplementedInterfaces<HelloService>(ServiceLifetime.Scoped);

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IHelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(HelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
        }

        /// <summary>
        ///     Test <see cref="CommonServiceCollectionExtensions.TryAddSelfAndImplementedInterfaces{TImpementationType}"/> functionality
        /// </summary>
        [Fact]
        public void TestTryAddSelfAndImplementedInterfaces()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.TryAddSelfAndImplementedInterfaces<HelloService>(ServiceLifetime.Scoped);

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IHelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(HelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            serviceCollection.TryAddImplementedInterfaces<HelloService>(ServiceLifetime.Scoped);

            Assert.Equal(2, serviceCollection.Count);
        }

        /// <summary>
        ///     Test <see cref="CommonServiceCollectionExtensions.AddSelfAndImplementedInterfaces"/> functionality
        /// </summary>
        [Fact]
        public void TestNonGenericAddSelfAndImplementedInterfaces()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSelfAndImplementedInterfaces(typeof(HelloService), ServiceLifetime.Scoped);

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IHelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(HelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
        }

        /// <summary>
        ///     Test <see cref="CommonServiceCollectionExtensions.TryAddSelfAndImplementedInterfaces"/> functionality
        /// </summary>
        [Fact]
        public void TestNonGenericTryAddSelfAndImplementedInterfaces()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.TryAddSelfAndImplementedInterfaces(typeof(HelloService), ServiceLifetime.Scoped);

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IHelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(HelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            serviceCollection.TryAddImplementedInterfaces(typeof(HelloService), ServiceLifetime.Scoped);

            Assert.Equal(2, serviceCollection.Count);
        }

        /// <summary>
        ///     Test <see cref="CommonServiceCollectionExtensions.AddAssembly"/> functionality
        /// </summary>
        [Fact]
        public void TestAddAssembly()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddAssembly(typeof(HelloService).GetTypeInfo().Assembly, ServiceLifetime.Scoped, t => t == typeof(HelloService));

            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(IHelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            Assert.Contains(serviceCollection, s => s.ServiceType == typeof(HelloService) && s.ImplementationType == typeof(HelloService) && s.Lifetime == ServiceLifetime.Scoped);
            Assert.Equal(2, serviceCollection.Count);
        }
    }
}
