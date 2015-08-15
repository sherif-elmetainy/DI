# CodeArt.DependencyInjection

CodeArt.DependencyInjection is a .NET Library that adds extra functionality to the DI container used in ASP.NET 5 projects (see [ASP.NET 5 DependencyInjection repository](https://github.com/aspnet/DependencyInjection)). The current initial version supports the following features:

* __AddDecorator:__ Adds a service that implements the [Decorator pattern](https://en.wikipedia.org/wiki/Decorator_pattern), causing the DI container to return that service instead of the service you want to decorate.