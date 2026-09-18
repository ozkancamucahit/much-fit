using Microsoft.Extensions.Configuration;

namespace Central.Common.Lib;

public static class CommonExtensions
{
  public static string Underscore(this string value)
      => string.Concat(value.Select(
          (x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));

  public static TModel GetOptions<TModel>(
    this IConfiguration configuration,
    string section) where TModel : new()
  {

    if(String.IsNullOrWhiteSpace(section))
    {
      throw new ArgumentNullException("sectionName", "Section name parameter is required");
    }

    var model = new TModel();
    configuration.GetSection(section.Trim()).Bind(model);

    return model;
  }
}
