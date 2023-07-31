namespace CSharpProd.HTTP.SimpleBookstore.Contracts
{
    public class JwtResponse
    {
        public string Data { get; set; }
    }

    public class HttpResponse<T>
    {
        public T Data { get; set; }
    }
}
