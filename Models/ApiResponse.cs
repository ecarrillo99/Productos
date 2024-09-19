namespace ProductsApiRest.Models
{   
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}