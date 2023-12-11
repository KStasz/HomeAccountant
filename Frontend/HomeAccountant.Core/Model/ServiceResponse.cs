using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAccountant.Core.Model
{
    public class ServiceResponse<T> : ServiceResponse
    {
        public ServiceResponse(T result) : base(true)
        {
            Result = result;
        }

        public ServiceResponse(bool isSucceed) : base(isSucceed)
        {
            Result = default;
        }

        public ServiceResponse(bool isSucceed, IEnumerable<string>? errors) : base(isSucceed, errors)
        {
            Result = default;
        }

        public T? Result { get; set; }
    }

    public class ServiceResponse
    {
        public ServiceResponse(bool isSucceed)
        {
            IsSucceed = isSucceed;
        }

        public ServiceResponse(bool isSucceed, IEnumerable<string>? errors)
        {
            IsSucceed = isSucceed;
            Errors = errors;
        }

        public bool IsSucceed { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
