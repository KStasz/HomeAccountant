namespace Domain.Dtos
{
    public class ServiceResponse<T> : ServiceResponse
    {
        public ServiceResponse(T value)
            : base(true)
        {
            Value = value;
        }

        public ServiceResponse(bool result)
            : base(result)
        {
        }

        public ServiceResponse(IEnumerable<string> errors)
            : base(errors)
        {
        }

        public T? Value { get; set; }
    }

    public class ServiceResponse
    {
        public ServiceResponse(bool result)
        {
            Result = result;
            Errors = null;
        }

        public ServiceResponse(IEnumerable<string> errorCollection)
        {
            Result = false;
            Errors = errorCollection;
        }

        public ServiceResponse(params string[] errors)
        {
            Result = false;
            Errors = errors;
        }

        public bool Result { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
