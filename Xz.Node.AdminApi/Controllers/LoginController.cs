using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Xz.Node.App.Interface;
using Xz.Node.App.SSO;
using Xz.Node.App.SSO.Request;
using Xz.Node.App.SSO.Response;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Encryption;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;

namespace Xz.Node.AdminApi.Controllers
{
    /// <summary>
    /// 登录接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "系统登录")]
    public class LoginController : ControllerBase
    {
        private string _appKey = "xznode";
        private readonly IAuth _authUtil;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpHelper _httpHelper = null;

        /// <summary>
        /// 系统登录
        /// </summary>
        /// <param name="authUtil"></param>
        /// <param name="configuration">配置中心</param>
        /// <param name="httpContextAccessor"></param>
        public LoginController(IAuth authUtil, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _authUtil = authUtil;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpHelper = new HttpHelper("https://ssl.captcha.qq.com");
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginReq"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginReq loginReq)
        {
            var result = new ResultInfo<LoginResultViewModel>()
            {
                Message = "登录成功",
            };
            try
            {
                #region 防水墙验证
                if (!string.IsNullOrEmpty(loginReq.Ticket) && !string.IsNullOrEmpty(loginReq.Randstr))
                {
                    string url = "/ticket/verify?";//请求地址
                    string aid = "2043018754";//appid
                    string AppSecretKey = "0uYBl0YmA6gqfyQ09TW79sA**";//App Secret Key
                    string UserIp = _httpContextAccessor.HttpContext.GetClientUserIp();//提交验证的用户的IP地址
                    string parameters = string.Format("aid={0}&AppSecretKey={1}&Ticket={2}&Randstr={3}&UserIP={4}", aid, AppSecretKey, loginReq.Ticket, loginReq.Randstr, UserIp);
                    url += parameters;

                    var resultDataStr = _httpHelper.Get(null, url);
                    //验证结果
                    var tencentCaptcha = JsonHelper.Instance.Deserialize<TencentCaptchaResultData>(resultDataStr);
                    //1:通过 0：失败
                    if (tencentCaptcha.response != 1)
                    {
                        throw new Exception($"滑动验证失败！{tencentCaptcha.err_msg}");
                    }
                }
                #endregion

                //解密前端的加密密码
                loginReq.PassWord = EncryptionHelper.DecryptByAES(loginReq.PassWord);
                //再次加密为后台通用密码
                var encryptPassWord = EncryptionHelper.Encrypt(loginReq.PassWord);

                var loginResult = _authUtil.Login(_appKey, loginReq.UserName, encryptPassWord);
                if (loginResult.Code == 200)
                {
                    result.Data = new LoginResultViewModel()
                    {
                        Success = true,
                        Token = loginResult.Token,
                        Expires = loginResult.Expires
                    };

                    //MVC才启用cookie，webApi就先注释掉
                    //Response.Cookies.Append(Define.TOKEN_NAME, result.Data.Token);
                }
                else
                {
                    throw new Exception(loginResult.Message);
                }
            }
            catch (Exception e)
            {
                result.Code = 500;
                result.Message = e.Message;
            }
            return Ok(result);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCaptcha()
        {
            var result = new ResultInfo<CaptchaViewModel>()
            {
                Message = "获取成功",
            };
            var data = SkiaSharpHlper.GetCaptcha();
            result.Data = new CaptchaViewModel()
            {
                ImgData = data.Item1,
                Code = data.Item2
            };
            //return File(bytes, "image/png");
            return Ok(result);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Logout()
        {
            var result = new ResultInfo<object>()
            {
                Message = "退出登录成功",
            };
            bool isEnabledId4 = _configuration.GetSection("AppSetting:IdentityServer4:Enabled").ToBool();
            if (isEnabledId4)
            {
                return SignOut("Cookies", "oidc");
            }
            _authUtil.Logout();
            return Ok(result);
        }
    }
}
