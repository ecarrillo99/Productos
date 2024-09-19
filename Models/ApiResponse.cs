namespace ProductsApiRest.Models
{   
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        // Método estático para respuestas exitosas
        public static ApiResponse<T> Ok(T data, string message = "Success", int code = 1)
        {
            return new ApiResponse<T>
            {
                Code = code,
                Status = true,
                Message = message,
                Data = data
            };
        }

        // Método estático para respuestas de error
        public static ApiResponse<T> Error(string message, int code = 2)
        {
            return new ApiResponse<T>
            {
                Code = code,
                Status = false,
                Message = message,
                Data = default
            };
        }
    }
}