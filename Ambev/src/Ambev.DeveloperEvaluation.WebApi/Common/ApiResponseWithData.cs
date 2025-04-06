namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class ApiResponseWithData<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponseWithData<T> SuccessResponse(T data, string message = "Operação realizada com sucesso.")
    {
        return new ApiResponseWithData<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

}
