// Copyright (c) Sherif Elmetainy (Code Art). All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the solution root for license information.

namespace CodeArt.DependencyInjection.Tests
{
    /// <summary>
    /// Sample service that implements decorator pattern without implemening <see cref="IDecorator{TService}"/>
    /// </summary>
    public class HelloServiceDecoratorNoInterface : IHelloService
    {
        /// <summary>
        /// Wrapped service
        /// </summary>
        private readonly IHelloService _helloService;

        /// <summary>
        /// Constructor. Initializes a new instance of <see cref="HelloServiceDecoratorNoInterface"/>
        /// </summary>
        /// <param name="helloService">service to wrap and decorate</param>
        public HelloServiceDecoratorNoInterface(IHelloService helloService)
        {
            _helloService = helloService;
        }


        /// <summary>
        /// Say hello implementation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string SayHello(string name)
        {
            return "Decorated without interface: " + _helloService.SayHello(name);
        }
    }
}
