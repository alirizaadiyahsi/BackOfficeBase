using System.Collections.Generic;

namespace BackOfficeBase.Application.Shared.Dto
{
    public class AppServiceResult<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }

        public AppServiceResult()
        {

        }

        public static AppServiceResult<T> Succeed(T result)
        {
            return new AppServiceResult<T>(result);
        }

        public static AppServiceResult<T> Failed(List<string> errors)
        {
            return new AppServiceResult<T>(errors);
        }

        private AppServiceResult(T result)
        {
            Data = result;
            Success = true;
        }

        private AppServiceResult(List<string> errors)
        {
            Errors = errors;
            Success = false;
        }
    }
}
