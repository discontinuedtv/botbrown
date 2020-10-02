namespace BotBrown.DI
{
    using Serilog;
    using Castle.DynamicProxy;
    using System;

    public class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger logger;

        public LoggingInterceptor(ILogger logger)
        {
            this.logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch(Exception e)
            {
                var tempLogger = logger.ForContext(invocation.TargetType);
                tempLogger.Error(e, "Ausnahmefehler aufgetreten");
                throw e;
            }
        }
    }
}
