using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Central.ServiceDefaults.Extensions;

public static class JSONOptionsExtensions
{

  public static IMvcBuilder AddCustomJSONOptions(this IMvcBuilder mvcBuilder)
  {
    mvcBuilder
    .AddJsonOptions(opt =>
    {
      opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
      opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      opt.JsonSerializerOptions.WriteIndented = true;
      opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

    });

    return mvcBuilder;
  }

}
