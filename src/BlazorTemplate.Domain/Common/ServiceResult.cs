namespace BlazorTemplate.Domain.Common
{
    public class ServiceResult
    {
        private static readonly ServiceResult _success = new() { IsSuccess = true };
        protected List<string> _errors = new();

        public bool IsSuccess { get; protected set; }

        public IEnumerable<string> Errors => _errors;

        public static ServiceResult Success => _success;

        public static ServiceResult Error(params string[] errors)
        {
            var result = new ServiceResult();

            result.AddErrors(errors);

            return result;
        }

        protected void AddErrors(params string[] errors)
        {
            if (errors != null && errors.Any())
            {
                _errors.AddRange(errors.Where(e => !string.IsNullOrWhiteSpace(e)));
            }
        }

        public override string ToString()
        {
            return IsSuccess ? "Success" : string.Join(Environment.NewLine, Errors);
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        private ServiceResult(bool isSuccess, T? data)
        {
            Data = data;
            IsSuccess = isSuccess;
        }

        public T? Data { get; private set; } = default;

        public static new ServiceResult<T> Success(T data) => new(true, data);

        public static ServiceResult<T> Error(string message, T? data = default)
        {
            var result = new ServiceResult<T>(false, data);

            result.AddErrors(message);

            return result;
        }
    }
}