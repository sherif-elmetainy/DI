// Copyright (c) Sherif Elmetainy (Code Art).
// Licensed under the MIT License. See LICENSE.txt in the solution root for license information.


using Microsoft.Framework.DependencyInjection;

namespace CodeArt.DependencyInjection
{
    /// <summary>
    ///     An interface that should be implemented by services that implement the decorator pattern and used by <see cref="CommonServiceCollectionExtensions.AddDecorator{TService, TDecorator}(IServiceCollection,ServiceLifetime)" />
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
