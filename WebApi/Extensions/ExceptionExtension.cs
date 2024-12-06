namespace WebApi.Extensions
{
    public static class ExceptionExtension
    {
        public static string MessageAll(this Exception ex)
        {
            var message = ex.Message;
            var inner = ex.InnerException;
            var isInner = (ex.InnerException is not null);

            while (isInner) 
            {
                message += $" --->>> {inner?.Message}";

                inner = ex.InnerException;
                isInner = (inner?.InnerException is not null);
            }

            return message;
        }
    }
}