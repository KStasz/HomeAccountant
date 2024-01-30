namespace HomeAccountant.Core.Model
{
    public class ServiceResponse<T> : ServiceResponse
    {
        public ServiceResponse() : base(false)
        {
            Errors = new List<string>()
            {
                "Wystąpił błąd"
            };
        }

        public ServiceResponse(T result) : base(true)
        {
            Value = result;
        }

        public ServiceResponse(bool isSucceed) : base(isSucceed)
        {
            Value = default;
        }

        public ServiceResponse(params string[] errors) : base(false, new List<string>(errors))
        {

        }

        public ServiceResponse(bool isSucceed, IEnumerable<string>? errorCollection) : base(isSucceed, errorCollection)
        {
            Value = default;
        }

        public T? Value { get; set; }
    }

    public class ServiceResponse
    {

        public ServiceResponse()
        {
            
        }

        public ServiceResponse(bool isSucceed)
        {
            Result = isSucceed;
        }

        public ServiceResponse(bool isSucceed, IEnumerable<string>? errors)
        {
            Result = isSucceed;
            Errors = errors;
        }

        public bool Result { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
