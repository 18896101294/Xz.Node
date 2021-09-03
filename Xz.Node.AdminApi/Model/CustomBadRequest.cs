using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.AdminApi.Model
{
    /// <summary>
    /// 自定义模态验证异常返回信息
    /// </summary>
    public class CustomBadRequest : ValidationProblemDetails
    {
        /// <summary>
        /// 自定义模态验证异常返回信息
        /// </summary>
        /// <param name="context"></param>
        public CustomBadRequest(ActionContext context)
        {
            Title = "WebApi客户端传入的参数无效";
            Detail = "vue客户端（或其他方式）调用WebApi时传入的参数类型与接口需要的类型不匹配";
            Status = 500;
            ConstructErrorMessages(context);
        }

        /// <summary>
        /// 这里我为了返回结果一致性，就直接当异常处理了。
        /// </summary>
        /// <param name="context"></param>
        private void ConstructErrorMessages(ActionContext context)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("客户端传入的参数无效，");
            foreach (var keyModelStatePair in context.ModelState)
            {
                var key = keyModelStatePair.Key;
                var errors = keyModelStatePair.Value.Errors;


                if (errors.Count == 0) continue;

                if (errors.Count == 1)
                {
                    var errorMessage = GetErrorMessage(errors[0]);
                    Errors.Add(key, new[] { errorMessage });

                    msg.AppendLine($"参数名：{key}；错误消息：{errorMessage}");
                }
                else
                {
                    var errorMessages = new string[errors.Count];
                    for (var i = 0; i < errors.Count; i++)
                    {
                        errorMessages[i] = GetErrorMessage(errors[i]);
                    }
                    Errors.Add(key, errorMessages);

                    msg.AppendLine($"参数名：{key}；错误消息：{errorMessages[0]}");
                }
            }
            throw new InfoException(msg.ToString(), 500);
        }

        string GetErrorMessage(ModelError error)
        {
            return string.IsNullOrEmpty(error.ErrorMessage) ? "输入无效。" : error.ErrorMessage;
        }
    }
}
