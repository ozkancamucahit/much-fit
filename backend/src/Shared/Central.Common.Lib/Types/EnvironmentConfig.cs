namespace Central.Common.Lib.Types;

public sealed class EnvironmentConfig
{
  public bool IsDevelopmentEnvironment { get; init; }
  public bool IsProductionEnvironment { get; init; }
  public bool IsStagingEnvironment { get; init; }
}
