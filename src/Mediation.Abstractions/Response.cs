namespace Burnout.Mediation
{
    public class Response<T>
    {
        public T Value { get; }

        public Response(
            T value)
        {
            Value = value;
        }
    }
}
