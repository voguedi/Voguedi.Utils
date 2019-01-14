using System;
using System.Threading.Tasks;
using AspectCore.DynamicProxy.Parameters;

namespace Voguedi.AspectCore
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NotNullAttribute : ParameterInterceptorAttribute
    {
        #region ParameterInterceptorAttribute

        public override Task Invoke(ParameterAspectContext context, ParameterAspectDelegate next)
        {
            if (context.Parameter.Value == null)
                throw new ArgumentNullException(context.Parameter.Name);

            return next(context);
        }

        #endregion
    }
}
