﻿// Copyright (c) Sherif Elmetainy (Code Art).
// Licensed under the MIT License. See LICENSE.txt in the solution root for license information.

namespace CodeArt.DependencyInjection.Tests
{
    /// <summary>
    ///     Sample service interface
    /// </summary>
    public interface IHelloService
    {
        /// <summary>
        ///     Sample service method
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string SayHello(string name);
    }
}
