// Copyright (c) Sherif Elmetainy (Code Art). All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the solution root for license information.

namespace CodeArt.DependencyInjection.Tests
{
    /// <summary>
    ///     Sample service
    /// </summary>
    public class HelloService : IHelloService
    {
        /// <summary>
        ///     Sample service method
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string SayHello(string name)
        {
            return "Hello " + name + ".";
        }
    }
}
