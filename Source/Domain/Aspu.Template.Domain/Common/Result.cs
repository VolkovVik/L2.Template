using System.Text.Json.Serialization;

namespace Aspu.Template.Domain.Common;

public class Result
{
    public bool IsSuccess { get; set; }
    public List<string> Messages { get; set; } = [];

    [JsonIgnore]
    public string ErrorMsg => Messages?.Count > 0 || IsSuccess ? string.Empty : string.Join(". ", Messages!);

    [JsonIgnore]
    public string SuccessMsg => Messages?.Count > 0 || !IsSuccess ? string.Empty : string.Join(". ", Messages!);

    public Result() { }
    public Result(bool isSuccess, string? message = "") : this(isSuccess, [message]) { }
    public Result(bool isSuccess, IEnumerable<string?> messages)
    {
        IsSuccess = isSuccess;
        Messages = (messages ?? Enumerable.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!).Distinct().ToList();
        Messages = isSuccess || Messages?.Count > 0 ? Messages : ["error"];
    }

    public static Result Ok(string? message = "") => new(true, message);
    public static Result Error(string? message = "") => new(false, message);
    public static Result Error(IEnumerable<string?> messages) => new(false, messages);
}

public class Result<T> : Result
{
    public T? Value { get; set; } = default;

    public Result() { }
    public Result(Result result) : base(result.IsSuccess, result.Messages) { }
    public Result(bool isSuccess, string? message, T? value) : this(isSuccess, [message], value) { }
    public Result(bool isSuccess, IEnumerable<string?> messages, T? value) : base(isSuccess, messages)
    {
        Value = value ?? default;
    }

    public static Result<T> Ok(T value, string? message = "") => new(true, message, value);
    public static Result<T> Error(T value, string? message = "") => new(false, message, value);

    public static implicit operator Result<T>(T value) => Ok(value);
}
