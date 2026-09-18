namespace Central.AppHost.Extensions;

public static class AppHostExtensions
{
  public static IResourceBuilder<PostgresServerResource> RegisterPostgres(
    this IDistributedApplicationBuilder builder
    ,int machinePort = 5445
    )
  {
    var username = builder.AddParameter("feedRDBuser", secret: true);
    var password = builder.AddParameter("feedRDBpass", secret: true);

    var postgres = builder
    .AddPostgres("postgres", username, password)
    .WithEndpoint(name: "postgresendpoint", scheme: "tcp", port: machinePort, targetPort: 5432, isProxied: false)
    .WithContainerName("feedR-postgres-db")
    .WithImageTag("17.7")
    .WithDataVolume("feedRdb-data")
    .WithLifetime(ContainerLifetime.Persistent);

    return postgres;
  }

  public static IResourceBuilder<ContainerResource> RegisterConsul(
    this IDistributedApplicationBuilder builder
    ,string containerName
    ,int machinePort = 8500
    )
  {
    var consul = builder
      .AddContainer(containerName, image: "hashicorp/consul", tag: "1.21.0")
      .WithContainerName(containerName)
      .WithEndpoint(name: "consulendpoint", scheme: "tcp", port: machinePort, targetPort: 8500, isProxied: false);

    return consul;
  }

  public static (
    IResourceBuilder<ContainerResource> Prometheus,
    IResourceBuilder<ContainerResource> Grafana
  ) RegisterMonitoring(
    this IDistributedApplicationBuilder builder
    ,int prometheusPort = 9090
    ,int grafanaPort = 3000
    )
  {
    var monitoringDir = Path.Combine(AppContext.BaseDirectory, "monitoring");

    var prometheus = builder
      .AddContainer("prometheus", image: "prom/prometheus", tag: "v2.53.0")
      .WithContainerName("feedr-prometheus")
      .WithHttpEndpoint(name: "http", port: prometheusPort, targetPort: 9090, isProxied: false)
      .WithArgs(
        "--config.file=/etc/prometheus/prometheus.yml",
        "--storage.tsdb.retention.time=7d")
      .WithContainerRuntimeArgs(
        "--add-host", "host.docker.internal:host-gateway",
        "--user", "0:0")
      .WithBindMount(
        Path.Combine(monitoringDir, "prometheus", "prometheus.yml"),
        "/etc/prometheus/prometheus.yml",
        isReadOnly: true);

    var grafana = builder
      .AddContainer("grafana", image: "grafana/grafana", tag: "11.2.0")
      .WithContainerName("feedr-grafana")
      .WithHttpEndpoint(name: "http", port: grafanaPort, targetPort: 3000, isProxied: false)
      .WithEnvironment("GF_SECURITY_ADMIN_USER", "admin")
      .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "admin")
      .WithEnvironment("GF_USERS_ALLOW_SIGN_UP", "false")
      .WithContainerRuntimeArgs(
        "--add-host", "host.docker.internal:host-gateway",
        "--user", "0:0")
      .WithBindMount(
        Path.Combine(monitoringDir, "grafana", "provisioning", "datasources"),
        "/etc/grafana/provisioning/datasources",
        isReadOnly: true)
      .WithBindMount(
        Path.Combine(monitoringDir, "grafana", "provisioning", "dashboards"),
        "/etc/grafana/provisioning/dashboards",
        isReadOnly: true)
      .WithBindMount(
        Path.Combine(monitoringDir, "grafana", "dashboards"),
        "/var/lib/grafana/dashboards",
        isReadOnly: true)
      .WaitFor(prometheus);

    return (prometheus, grafana);
  }





}
