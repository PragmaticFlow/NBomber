namespace BookstoreSimulator.Contracts
{
    public class ResponseBS<T>
    {
        public T Data { get; private set; }
        public ResponseBS(T data)
        {
            Data = data;
        }
    }
}
