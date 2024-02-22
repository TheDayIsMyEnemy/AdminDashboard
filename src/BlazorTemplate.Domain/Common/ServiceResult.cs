namespace BlazorTemplate.Domain.Common
{
    public class ServiceResult
    {
        private readonly List<string> _messages = new();

        protected ServiceResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }

        public IReadOnlyCollection<string> Messages => _messages.AsReadOnly();

        public static ServiceResult Success => new ServiceResult(true);

        public static ServiceResult Error => new ServiceResult(false);

        public ServiceResult WithMessage(params string[] messages)
        {
            AddMessages(messages);

            return this;
        }

        public ServiceResult WithFormatMessage(string format, params string[] args)
        {
            var message = string.Format(format, args);

            AddMessages(message);

            return this;
        }

        protected void AddMessages(params string[] messages)
        {
            if (messages != null && messages.Any())
            {
                _messages.AddRange(messages.Where(e => !string.IsNullOrWhiteSpace(e)));
            }
        }

        public override string ToString()
            => string.Join(Environment.NewLine, _messages);
    }

    // public class ServiceResult<T> : ServiceResult
    // {
    //     private ServiceResult(bool isSuccess, T? data)
    //     {
    //         Data = data;
    //         IsSuccess = isSuccess;
    //     }

    //     public T? Data { get; private set; } = default;

    //     public static new ServiceResult<T> Success(T data) => new(true, data);

    //     public static ServiceResult<T> Error(string message, T? data = default)
    //     {
    //         var result = new ServiceResult<T>(false, data);

    //         result.AddErrors(message);

    //         return result;
    //     }
    // }
}