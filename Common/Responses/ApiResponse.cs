namespace Common.Responses;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public string[] Messages { get; set; } = [];
}
