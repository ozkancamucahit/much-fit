namespace Central.Common.Lib.Types;

public class APIResponse<T>
  where T : class
{
  public ResultStatus Status { get; protected set; }

  public T? Data { get; set; }

  public bool IsSuccess
  {
    get
    {
      ResultStatus status = Status;
      if ((uint)status <= 1u || status == ResultStatus.NoContent)
      {
        return true;
      }

      return false;
    }
  }

  public IEnumerable<string>? Errors { get; protected set; }

  public IEnumerable<ValidationError>? ValidationErrors { get; protected set; }

  public string? MessageTR { get; set; }
  public string? MessageEN { get; set; }

  public string CorrelationId { get; protected set; } = string.Empty;

}

public enum ResultStatus
{
  Ok,
  Created,
  Error,
  Forbidden,
  Unauthorized,
  Invalid,
  NotFound,
  NoContent,
  Conflict,
  CriticalError,
  Unavailable
}

public enum ValidationSeverity
{
  Error,
  Warning,
  Info
}



