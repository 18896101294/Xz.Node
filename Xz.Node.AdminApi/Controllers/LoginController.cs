using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using Xz.Node.App.Interface;
using Xz.Node.App.SSO.Request;
using Xz.Node.App.SSO.Response;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Encryption;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Model;

namespace Xz.Node.AdminApi.Controllers
{
    /// <summary>
    /// 登录接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "系统登录_Login")]
    public class LoginController : ControllerBase
    {
        private string _appKey = "xznode";
        private readonly IOptions<AppSetting> _appConfiguration;
        private readonly IAuth _authUtil;
        public LoginController(IAuth authUtil, IOptions<AppSetting> appConfiguration)
        {
            _authUtil = authUtil;
            _appConfiguration = appConfiguration;
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
            if (_appConfiguration.Value.AuthorizationWay == AuthorizationWayEnum.IdentityServer4)
            {
                return SignOut("Cookies", "oidc");
            }
            _authUtil.Logout();
            return Ok(result);
        }
    }
}
