// Copyright (c) Sherif Elmetainy (Code Art). All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the solution root for license information.

namespace CodeArt.DependencyInjection.Tests
{
    /// <summary>
    /// Sample service that implements decorator pattern without implemening <see cref="IDecorator{TService}"/>
    /// </summary>
    public class HelloServiceDecorator : IHelloService, IDecorator<IHelloService>
    {
        /// <summary>
        /// Wrapped service
        /// </summary>
        public IHelloService DecoratedService { get; set; }

        /// <summary>
        /// Say hello decorated implementation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string SayHello(string name)
        {
            return "Decorated: " + DecoratedService.SayHello(name);
        }
    }
}
