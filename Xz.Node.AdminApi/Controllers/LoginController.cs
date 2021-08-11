using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Xz.Node.App.Interface;
using Xz.Node.App.SSO.Request;
using Xz.Node.App.SSO.Response;
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
        /// <summary>
        /// 系统登录
        /// </summary>
        /// <param name="authUtil"></param>
        /// <param name="configuration">配置中心</param>
        public LoginController(IAuth authUtil, IConfiguration configuration)
        {
            _authUtil = authUtil;
            _configuration = configuration;
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
                var encryptPassWord = EncryptionHelper.Encrypt(loginReq.PassWord);

                var loginResult = _authUtil.Login(_appKey, loginReq.UserName, encryptPassWord);
                if (loginResult.Code == 200)
                {
                    result.Data = new LoginResultViewModel()
                    {
                        Success = true,
                        Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI0OWRmMTYwMi1mNWYzLTRkNTItYWZiNy0zODAyZGE2MTk1NTgiLCJhY2NvdW50IjoiYWRtaW4iLCJuYmYiOjE2Mjg1MDEzNTEsImV4cCI6MTYyOTM2NTM1MSwiaXNzIjoiWHouTm9kZSIsImF1ZCI6Inh6In0.GGZMFui28W3ieHBJUiI-Yy7KbdL_6JvfrzjXdGxa2h4",//loginResult.Token,
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
