using Autofac;
using Autofac.Core;
using System;
using System.Diagnostics;
using System.IO;
using Test.Autofac.Service;

namespace Test.Autofac.ConsoleApp
{
    class Program
    {
        private const string RootLifetimeTag = "MyIsolatedRoot";

        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<ConsoleHandler>().As<IHandler>();
            containerBuilder.RegisterType(typeof(TraceHandler)).As<IHandler>().PreserveExistingDefaults();
            containerBuilder.RegisterType(typeof(TraceHandler)).As<IHandler>().IfNotRegistered(typeof(IHandler));
            containerBuilder.RegisterType(typeof(TraceHandler)).As<IHandler>().OnlyIf(reg =>
                 reg.IsRegistered(new TypedService(typeof(IHandler)))
            );

            containerBuilder.RegisterType<TraceHandler>().As<IHandler>();
            containerBuilder.RegisterType<TraceHandler>().As<IHandler>().AsSelf();
            containerBuilder.RegisterType<TraceHandler>().UsingConstructor(typeof(string)).As<IHandler>(); ;

            containerBuilder.RegisterInstance(new TraceHandler()).As<IHandler>();
            containerBuilder.RegisterInstance(new TraceHandler()).As<IHandler>();
            containerBuilder.RegisterInstance(new TraceHandler()).As<IHandler>().ExternallyOwned();

            containerBuilder.RegisterType<StringWriter>().As<TextWriter>();
            containerBuilder.Register(c => new TraceHandler(c.Resolve<TextWriter>()));
            containerBuilder.Register(c => new TraceHandler() { TextWriter = c.ResolveOptional<TextWriter>() });

            containerBuilder.Register<IHandler>((c, p) =>
            {
                var name = p.Named<string>("name");

                if (name.StartsWith("trace"))
                {
                    return new TraceHandler();
                }
                else
                {
                    return new ConsoleHandler();
                }
            });

            containerBuilder.RegisterGeneric(typeof(Generic<>)).As(typeof(IGeneric<>)).InstancePerLifetimeScope();

            var container = containerBuilder.Build();

            using (var scope = container.BeginLifetimeScope(RootLifetimeTag, b =>
            {
                b.RegisterType(typeof(TraceHandler)).As<IHandler>();
            }))
            {
                var childHandler = scope.Resolve<IHandler>();
                childHandler.Handle($"Hi!");
            }

            //using (var scope = container.BeginLifetimeScope())
            //{
            //    var handler = scope.Resolve<IHandler>();
            //    handler.Handle($"Hi!");
            //}

            using (var scope = container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<IHandler>(new NamedParameter("name", "trace"));
                handler.Handle($"Hi!");
            }

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.Resolve<IGeneric<string>>();
                instance.DoSomeThing();
            }

            stopWatch.Stop();

            Console.WriteLine($"Toal time : {stopWatch.ElapsedMilliseconds}");
            Console.ReadLine();
        }
    }
}
