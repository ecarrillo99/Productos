namespace ProductsApiRest.Models
{
    public class ListData<T>
    {
        public T Items { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }   
}