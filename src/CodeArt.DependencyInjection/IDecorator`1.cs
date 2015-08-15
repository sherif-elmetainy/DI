// Copyright (c) Sherif Elmetainy (Code Art). All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the solution root for license information.

namespace CodeArt.DependencyInjection
{
    /// <summary>
    ///     An interface that should be implemented by services that implement the decorator pattern and used by <see cref="Microsoft.Framework.DependencyInjection.DecoratorServiceCollectionExtensions.AddDecorator{TService, TDecorator}(Microsoft.Framework.DependencyInjection.IServiceCollection)" />
    /// </summary>
    /// <typeparam name="TService">The service type to decorate</typeparam>
    public interface IDecorator<TService>
    {
        /// <summary>
        /// Gets or sets the service being decorated.
        /// </summary>
        TService DecoratedService { get; set; }
    }
}
