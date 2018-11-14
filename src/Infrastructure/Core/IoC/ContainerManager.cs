using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Massena.Infrastructure.Core.IoC
{
    public static class ContainerManager
    {
        private const string APP_PREFIX = "Concepta";

        public static IServiceProvider AddDryIoc<TCompositionRoot>(this IServiceCollection services)
        {
            var container = new Container(
                rules => rules
                        .WithFactorySelector(Rules.SelectLastRegisteredFactory())
                        .With(FactoryMethod.ConstructorWithResolvableArguments)
                        .WithoutThrowOnRegisteringDisposableTransient()
                        .WithoutThrowIfDependencyHasShorterReuseLifespan()
            ).WithDependencyInjectionAdapter(services) as Container;

            var implementingClasses =
                AppDomain.CurrentDomain.GetAssemblies().ToList()
                    .Where(x => AssemblyPredicate(x))
                    .SelectMany(x => x.GetTypes())
                    .Where(type =>
                        (type.Namespace != null &&NamespacesPredicate(type)) &&
                        type.IsPublic &&                    // get public types 
                        !type.IsAbstract &&                 // which are not interfaces nor abstract
                        type.GetInterfaces().Length != 0)   // which implementing some interface(s)
                    .ToList();

            //container.RegisterDelegate<SingleInstanceFactory>(r => r.Resolve);
            //container.RegisterDelegate<MultiInstanceFactory>(r => serviceType => r.ResolveMany(serviceType));

            Parallel.ForEach(implementingClasses, implementingClass =>
            {
                container.RegisterMany(new[] { implementingClass }, serviceTypeCondition: type => type.IsInterface);
            });

            container.Register<IMediator, Mediator>(Reuse.Singleton);

            return container.ConfigureServiceProvider<TCompositionRoot>();
        }

        private static string AppPrefix()
        {
            return GetPrefix(typeof(ContainerManager).Namespace);
        }
        private static string MediatorPrefix()
        {
            return GetPrefix(typeof(IMediator).Namespace);
        }

        private static string GetPrefix(string fullNamespace)
        {
            var indexOfPoint = fullNamespace.IndexOf(".", StringComparison.Ordinal);

            return (indexOfPoint <= 0) ? fullNamespace : fullNamespace.Substring(0, indexOfPoint);
        }


        private static Func<Type, bool> NamespacesPredicate => (type) =>
        {
            return (
                type.FullName.StartsWith(AppPrefix(), StringComparison.InvariantCultureIgnoreCase) ||
                type.FullName.StartsWith(APP_PREFIX, StringComparison.InvariantCultureIgnoreCase) ||
                type.FullName.StartsWith(MediatorPrefix(), StringComparison.InvariantCultureIgnoreCase)
            );
        };

        private static Func<Assembly, bool> AssemblyPredicate => (type) =>
        {
            return (
                type.FullName.StartsWith(AppPrefix(), StringComparison.InvariantCultureIgnoreCase) ||
                type.FullName.StartsWith(APP_PREFIX, StringComparison.InvariantCultureIgnoreCase) ||
                type.FullName.StartsWith(MediatorPrefix(), StringComparison.InvariantCultureIgnoreCase)
            );
        };
    }
    
}
