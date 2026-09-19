using Central.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);


var _ = builder
  .AddDockerComposeEnvironment("env");

var seq = builder
  .AddSeq("seq", port: 5341)
  .WithDataVolume();

var postgres = builder.RegisterPostgres(machinePort: 5444);
var productsDB = postgres.AddDatabase("FeedRDB");

var cache = builder
  .AddRedis("cache", port: 6363)
  .WithDataVolume(isReadOnly: false)
  .WithContainerName("redis")
  .WithPersistence(
    interval: TimeSpan.FromMinutes(5),
    keysChangedThreshold: 100)
  .WithRedisInsight(containerName: "redis-insights")
  .WithLifetime(ContainerLifetime.Persistent);


var (prometheus, grafana) = builder.RegisterMonitoring();

var userService = builder
  .AddDockerfile("userservice", "../../../") // Sets repo root as build context
  .WithDockerfile("src/Services/UserService/UserService.Api/Dockerfile");

// var feedR = builder
//   .AddProject<Projects.UserService_Api>("feedr-api")
//   .WithHttpEndpoint(port: 8080, name: "http", isProxied: false)
//   .WithHttpHealthCheck("/health")
//   .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
//   .WithReference(seq)
//   .WithReference(productsDB)
//   .WithReference(cache)
//   .WaitFor(seq)
//   .WaitFor(productsDB)
//   .WaitFor(cache);

builder.Build().Run();
