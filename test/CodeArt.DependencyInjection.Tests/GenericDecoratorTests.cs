// Copyright (c) Sherif Elmetainy (Code Art). All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the solution root for license information.

using System.Linq;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace CodeArt.DependencyInjection.Tests
{
    /// <summary>
    ///     Test methods for AddDecorator functionality
    /// </summary>
    public class GenericDecoratorTests
    {
        /// <summary>
        ///     Test that default hello service without any decorator works
        /// </summary>
        [Fact]
        public void TestHelloService()
        {
            var services = new ServiceCollection();
            services.AddScoped<IHelloService, HelloService>();

            var provider = services.BuildServiceProvider();

            var helloService = provider.GetRequiredService<IHelloService>();
            Assert.NotNull(helloService);
            var result = helloService.SayHello("world");
            Assert.Equal("Hello world.", result);
        }

        /// <summary>
        ///     Test the generic overload of the AddService with a class that implements <see cref="IDecorator{TService}"/>
        /// </summary>
        [Fact]
        public void TestHelloServiceDecorator()
        {
            var services = new ServiceCollection();

            services.AddInstance<IHelloService>(new HelloService());

            services.AddSingleton<IHelloService>(sp => new HelloService());
            services.AddScoped<IHelloService>(sp => new HelloService());
            services.AddTransient<IHelloService>(sp => new HelloService());

            services.AddSingleton<IHelloService, HelloService>();
            services.AddScoped<IHelloService, HelloService>();
            services.AddTransient<IHelloService, HelloService>();

            services.AddDecorator<IHelloService, HelloServiceDecorator>();

            var provider = services.BuildServiceProvider();

            var helloServices = provider.GetRequiredServices<IHelloService>();
            Assert.NotNull(helloServices);
            var collection = helloServices as IHelloService[] ?? helloServices.ToArray();
            Assert.Equal(7, collection.Length);
            Assert.NotEmpty(collection);
            foreach (var helloService in collection)
            {
                Assert.NotNull(helloService);
                Assert.Equal("Decorated: Hello world.", helloService.SayHello("world"));
            }
        }

        /// <summary>
        ///     Test the generic overload of the AddService with a class that implements <see cref="IDecorator{TService}"/> but with a decorator that has more lifetime than decorated service.
        ///     This this result in no decorations
        /// </summary>
        [Fact]
        public void TestHelloServiceNoDecorator()
        {
            var services = new ServiceCollection();
            services.AddSingleton<HelloServiceDecorator>();

            services.AddScoped<IHelloService>(sp => new HelloService());
            services.AddTransient<IHelloService>(sp => new HelloService());

            services.AddScoped<IHelloService>(sp => new HelloService());
            services.AddTransient<IHelloService>(sp => new HelloService());

            services.AddScoped<IHelloService, HelloService>();
            services.AddTransient<IHelloService, HelloService>();

            services.AddDecorator<IHelloService, HelloServiceDecorator>();

            var provider = services.BuildServiceProvider();

            var helloServices = provider.GetRequiredServices<IHelloService>();
            Assert.NotNull(helloServices);
            var collection = helloServices as IHelloService[] ?? helloServices.ToArray();
            Assert.NotEmpty(collection);
            foreach (var helloService in collection)
            {
                Assert.NotNull(helloService);
                Assert.Equal("Hello world.", helloService.SayHello("world"));
            }
        }

        /// <summary>
        ///     Test the non generic overload of the AddService with a class that implements <see cref="IDecorator{TService}"/>
        /// </summary>
        [Fact]
        public void TestHelloNonGenericServiceDecorator()
        {
            var services = new ServiceCollection();

            services.AddInstance<IHelloService>(new HelloService());

            services.AddSingleton<IHelloService>(sp => new HelloService());
            services.AddScoped<IHelloService>(sp => new HelloService());
            services.AddTransient<IHelloService>(sp => new HelloService());

            services.AddSingleton<IHelloService, HelloService>();
            services.AddScoped<IHelloService, HelloService>();
            services.AddTransient<IHelloService, HelloService>();

            services.AddDecorator(typeof(IHelloService), typeof(HelloServiceDecorator));

            var provider = services.BuildServiceProvider();

            var helloServices = provider.GetRequiredServices<IHelloService>();
            Assert.NotNull(helloServices);
            var collection = helloServices as IHelloService[] ?? helloServices.ToArray();
            Assert.Equal(7, collection.Length);
            Assert.NotEmpty(collection);
            foreach (var helloService in collection)
            {
                Assert.NotNull(helloService);
                Assert.Equal("Decorated: Hello world.", helloService.SayHello("world"));
            }
        }

        /// <summary>
        ///     Test the non generic overload of the AddService with a class that implements <see cref="IDecorator{TService}"/> but with a decorator that has more lifetime than decorated service.
        ///     This this result in no decorations
        /// </summary>
        [Fact]
        public void TestHelloNonGenericServiceNoDecorator()
        {
            var services = new ServiceCollection();
            services.AddSingleton<HelloServiceDecorator>();

            services.AddScoped<IHelloService>(sp => new HelloService());
            services.AddTransient<IHelloService>(sp => new HelloService());

            services.AddScoped<IHelloService>(sp => new HelloService());
            services.AddTransient<IHelloService>(sp => new HelloService());

            services.AddScoped<IHelloService, HelloService>();
            services.AddTransient<IHelloService, HelloService>();

            services.AddDecorator(typeof(IHelloService), typeof(HelloServiceDecorator));

            var provider = services.BuildServiceProvider();

            var helloServices = provider.GetRequiredServices<IHelloService>();
            Assert.NotNull(helloServices);
            var collection = helloServices as IHelloService[] ?? helloServices.ToArray();
            Assert.NotEmpty(collection);
            foreach (var helloService in collection)
            {
                Assert.NotNull(helloService);
                Assert.Equal("Hello world.", helloService.SayHello("world"));
            }
        }

        /// <summary>
        ///     Test the generic overload of the AddService with a class that DOES NOT implement <see cref="IDecorator{TService}"/>
        /// </summary>
        [Fact]
        public void TestHelloServiceDecoratorNoInterface()
        {
            var services = new ServiceCollection();

            services.AddInstance<IHelloService>(new HelloService());

            services.AddSingleton<IHelloService>(sp => new HelloService());
            services.AddScoped<IHelloService>(sp => new HelloService());
            services.AddTransient<IHelloService>(sp => new HelloService());

            services.AddSingleton<IHelloService, HelloService>();
            services.AddScoped<IHelloService, HelloService>();
            services.AddTransient<IHelloService, HelloService>();

            services.AddDecorator<IHelloService>((sp, s) => new HelloServiceDecoratorNoInterface(s));

            var provider = services.BuildServiceProvider();

            var helloServices = provider.GetRequiredServices<IHelloService>();
            Assert.NotNull(helloServices);
            var collection = helloServices as IHelloService[] ?? helloServices.ToArray();
            Assert.Equal(7, collection.Length);
            Assert.NotEmpty(collection);
            foreach (var helloService in collection)
            {
                Assert.NotNull(helloService);
                Assert.Equal("Decorated without interface: Hello world.", helloService.SayHello("world"));
            }
        }

        /// <summary>
        ///     Test the non generic overload of the AddService with a class that DOES NOT implement <see cref="IDecorator{TService}"/>
        /// </summary>
        [Fact]
        public void TestHelloNonGenericServiceDecoratorNoInterface()
        {
            var services = new ServiceCollection();

            services.AddInstance<IHelloService>(new HelloService());

            services.AddSingleton<IHelloService>(sp => new HelloService());
            services.AddScoped<IHelloService>(sp => new HelloService());
            services.AddTransient<IHelloService>(sp => new HelloService());

            services.AddSingleton<IHelloService, HelloService>();
            services.AddScoped<IHelloService, HelloService>();
            services.AddTransient<IHelloService, HelloService>();

            services.AddDecorator(typeof(IHelloService), (sp, s) => new HelloServiceDecoratorNoInterface((IHelloService)s));

            var provider = services.BuildServiceProvider();

            var helloServices = provider.GetRequiredServices<IHelloService>();
            Assert.NotNull(helloServices);
            var collection = helloServices as IHelloService[] ?? helloServices.ToArray();
            Assert.Equal(7, collection.Length);
            Assert.NotEmpty(collection);
            foreach (var helloService in collection)
            {
                Assert.NotNull(helloService);
                Assert.Equal("Decorated without interface: Hello world.", helloService.SayHello("world"));
            }
        }

        /// <summary>
        ///     Decorate a service using two different decorators (i.e. the first decorator is decorated by a second one)
        /// </summary>
        [Fact]
        public void TestHelloServiceDoubleDecoration()
        {
            var services = new ServiceCollection();
            services.AddInstance<IHelloService>(new HelloService());

            services.AddDecorator<IHelloService, HelloServiceDecorator>();
            services.AddDecorator<IHelloService>((sp, s) => new HelloServiceDecoratorNoInterface(s));
            var provider = services.BuildServiceProvider();


            var helloServices = provider.GetRequiredServices<IHelloService>();
            Assert.NotNull(helloServices);
            var collection = helloServices as IHelloService[] ?? helloServices.ToArray();
            Assert.Equal(1, collection.Length);
            foreach (var helloService in collection)
            {
                Assert.NotNull(helloService);
                Assert.Equal("Decorated without interface: Decorated: Hello world.", helloService.SayHello("world"));
            }
        }
    }
}
