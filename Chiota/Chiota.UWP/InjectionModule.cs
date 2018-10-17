﻿namespace Chiota.UWP
{
  using Autofac;

  using Chiota.Persistence;
  using Chiota.Services;
  using Chiota.UWP.Persistence;
  using Chiota.UWP.Services;

  using Pact.Palantir.Cache;
  using Pact.Palantir.Repository;

  /// <inheritdoc />
  public class InjectionModule : Module
  {
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<ClipboardService>().As<IClipboardService>();

      // This lines will be merged soon
      builder.RegisterType<SqlLiteTransactionCache>().As<ITransactionCache>();
      builder.RegisterType<SqlLiteTransactionCache>().As<AbstractSqlLiteTransactionCache>();

      // This lines will be merged soon
      builder.RegisterType<SqlLiteContactRepository>().As<IContactRepository>().PropertiesAutowired();
      builder.RegisterType<SqlLiteContactRepository>().As<AbstractSqlLiteContactRepository>().PropertiesAutowired();
    }
  }
}
