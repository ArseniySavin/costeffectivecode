﻿using System.Data.Entity;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using CostEffectiveCode.Common;
using CostEffectiveCode.AutoMapper;
using CostEffectiveCode.Common.Logger;
using CostEffectiveCode.Common.Scope;
using CostEffectiveCode.Domain.Cqrs;
using CostEffectiveCode.Domain.Cqrs.Commands;
using CostEffectiveCode.Domain.Cqrs.Queries;
using CostEffectiveCode.Domain.Ddd.Specifications;
using CostEffectiveCode.Domain.Ddd.UnitOfWork;
using CostEffectiveCode.EntityFramework6;
using CostEffectiveCode.Sample.Data;
using CostEffectiveCode.Sample.Domain.Entities;
using CostEffectiveCode.WebApi2.DI;
using Microsoft.Owin.Logging;

namespace CostEffectiveCode.WebApi2.Tests.Config
{
    public class IocConfig
    {
        //public static string ConnectionString = @"Data Source=.;Initial Catalog=CostEffectiveCode_Sample_Data_Tests;Integrated Security=False;MultipleActiveResultSets=True;User ID=superuser;Password=Devpas123";

        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            DbContext(builder);
            Cqrs(builder);
            DependencyResolving(builder);
            Mapping(builder);
            Logging(builder);

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            return builder.Build();
        }

        private static void Logging(ContainerBuilder builder)
        {
            builder.RegisterType<ConsoleLogger>()
                .As<Common.Logger.ILogger>()
                .InstancePerRequest();
        }

        private static void DbContext(ContainerBuilder builder)
        {
            builder
                .Register(x => new SampleDbContext(Startup.Config["Data:DefaultConnection:ConnectionString"]))
                .As<SampleDbContext>()
                .As<DbContext>()
                .As<IDataContext>()
                .As<IUnitOfWork>()
                .As<ILinqProvider>()
                .InstancePerLifetimeScope(); ;
        }

        private static void Mapping(ContainerBuilder builder)
        {
            builder
                .RegisterType<AutoMapperWrapper>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        private static void DependencyResolving(ContainerBuilder builder)
        {
            builder.RegisterType<DiContainerScope<IUnitOfWork>>()
                .As<IScope<IUnitOfWork>>()
                .SingleInstance();

            builder.RegisterType<DiContainerScope<IDataContext>>()
                .As<IScope<IDataContext>>()
                .SingleInstance();

            builder
                .Register(x => GetDependencyResolverDiContainer())
                .As<IDiContainer>()
                .SingleInstance();
        }

        private static DependencyResolverDiContainer GetDependencyResolverDiContainer()
        {
            return new DependencyResolverDiContainer(
                Startup.HttpConfiguration.DependencyResolver);
        }

        private static void Cqrs(ContainerBuilder builder)
        {
            builder
                .RegisterType<CommandQueryFactory>()
                .AsImplementedInterfaces()
                .InstancePerRequest();

            builder
                .RegisterGeneric(typeof(CreateEntityCommand<>))
                .InstancePerDependency();

            builder
                .RegisterGeneric(typeof(DeleteEntityCommand<>))
                .InstancePerDependency();

            builder
                .RegisterType<CommitCommand>()
                .InstancePerDependency();

            builder
                .RegisterType<ExpressionQuery<Product>>()
                .As<IQuery<Product, IExpressionSpecification<Product>>>()
                .InstancePerDependency();

            builder
                .RegisterGeneric(typeof(ExpressionSpecification<>))
                .As(typeof(IExpressionSpecification<>));
        }
    }
}
