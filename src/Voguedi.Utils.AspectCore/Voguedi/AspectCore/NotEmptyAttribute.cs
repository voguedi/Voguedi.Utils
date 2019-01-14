using System;
using System.Threading.Tasks;
using AspectCore.DynamicProxy.Parameters;

namespace Voguedi.AspectCore
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NotEmptyAttribute : ParameterInterceptorAttribute
    {
        #region ParameterInterceptorAttribute

        public override Task Invoke(ParameterAspectContext context, ParameterAspectDelegate next)
        {
            if (string.IsNullOrWhiteSpace(context.Parameter.Value?.ToString().Trim() ?? string.Empty))
                throw new ArgumentNullException(context.Parameter.Name);

            return next(context);
        }

        #endregion
    }
}
