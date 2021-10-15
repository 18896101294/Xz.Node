using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Options;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Cache;
using Xz.Node.App.SysLogs;
using Xz.Node.App.AuthStrategies;
using Xz.Node.Repository.Domain.System;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Jwt;
using Xz.Node.Framework.Extensions;
using Microsoft.Extensions.Configuration;

namespace Xz.Node.App.SSO
{
    /// <summary>
    /// 使用本地登录。这个注入IAuth时，只需要Mvc一个项目即可，无需webapi的支持
    /// </summary>
    public class LocalAuth : IAuth
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly SysLogApp _logApp;
        private readonly AuthContextFactory _app;
        private readonly LoginParse _loginParse;
        private readonly ICacheContext _cacheContext;
        private readonly IJwtTokenHelper _jwtTokenHelper;
        private readonly bool _isEnabledId4;
        private readonly bool _isEnabledJwt;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="configuration"></param>
        /// <param name="app"></param>
        /// <param name="loginParse"></param>
        /// <param name="cacheContext"></param>
        /// <param name="logApp"></param>
        /// <param name="jwtTokenHelper"></param>
        public LocalAuth(IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            AuthContextFactory app,
            LoginParse loginParse,
            ICacheContext cacheContext,
            SysLogApp logApp,
            IJwtTokenHelper jwtTokenHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _app = app;
            _loginParse = loginParse;
            _cacheContext = cacheContext;
            _logApp = logApp;
            _jwtTokenHelper = jwtTokenHelper;
            _isEnabledId4 = _configuration["AppSetting:IdentityServer4:Enabled"].ToBool();
            _isEnabledJwt = _configuration["AppSetting:Jwt:Enabled"].ToBool();
        }

        /// <summary>
        /// 获取请求的token,返回的是token对应保存的缓存key
        /// </summary>
        /// <returns></returns>
        private string GetToken()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return string.Empty;
            }
            //如果是Identity，则返回信息为用户账号
            if (_isEnabledId4)
            {
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            }

            if (_isEnabledJwt)
            {
                var jwtToken = _httpContextAccessor.HttpContext.Request.Query[Define.JWT_TOKEN_NAME];
                if (!String.IsNullOrEmpty(jwtToken)) return jwtToken;
                return _httpContextAccessor.HttpContext.Request.Headers[Define.JWT_TOKEN_NAME].ToString() ?? string.Empty;
            }

            string token = _httpContextAccessor.HttpContext.Request.Query[Define.TOKEN_NAME];
            if (!String.IsNullOrEmpty(token)) return token;

            token = _httpContextAccessor.HttpContext.Request.Headers[Define.TOKEN_NAME];
            if (!String.IsNullOrEmpty(token)) return token;

            //MVC才启用cookie，webApi就先注释掉
            //var cookie = _httpContextAccessor.HttpContext.Request.Cookies[Define.TOKEN_NAME];
            //return cookie ?? String.Empty;
            return string.Empty;
        }

        /// <summary>
        /// token解析
        /// </summary>
        /// <returns></returns>
        private (UserAuthSession, string) ParseToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                if (_isEnabledJwt)
                {
                    var userId = string.Empty;
                    var account = string.Empty;
                    var tokenResponseEnum = _jwtTokenHelper.Validate(token, a => a["iss"] == "Xz.Node" && a["aud"] == "xz", action => { userId = action["userId"]; account = action["account"]; });
                    if (tokenResponseEnum == TokenResponseEnum.Ok)
                    {
                        var cacheKey = $"{userId}_{account}_token";
                        var tokenCache = _cacheContext.Get<UserAuthSession>(cacheKey);
                        return (tokenCache, cacheKey);
                    }
                }
                else
                {
                    var tokenStrLen = token.Split('_');
                    if (tokenStrLen.Length >= 3)
                    {
                        var cacheKey = $"{tokenStrLen[0]}_{tokenStrLen[1]}_token";
                        var tokenCache = _cacheContext.Get<UserAuthSession>(cacheKey);
                        return (tokenCache, cacheKey);
                    }
                }
            }
            return (null, null);
        }

        /// <summary>
        /// 检查登录
        /// </summary>
        /// <param name="token"></param>
        /// <param name="otherInfo"></param>
        /// <returns></returns>
        public bool CheckLogin(string token = "", string otherInfo = "")
        {
            if (_isEnabledId4)
            {
                return (!string.IsNullOrEmpty(_httpContextAccessor.HttpContext.User.Identity.Name));
            }

            if (string.IsNullOrEmpty(token))
            {
                token = GetToken();
            }

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                var (tokenCache, cacheKey) = this.ParseToken(token);
                return tokenCache != null;
            }
            catch (Exception ex)
            {
                return false;
                //throw ex;
            }
        }

        /// <summary>
        /// 获取当前登录的用户信息
        /// <para>通过URL中的Token参数或Cookie中的Token</para>
        /// </summary>
        /// <returns>LoginUserVM.</returns>
        public AuthStrategyContext GetCurrentUser()
        {
            if (_isEnabledId4)
            {
                return _app.GetAuthStrategyContext(GetToken());
            }
            AuthStrategyContext context = null;

            var token = GetToken();
            //解析token
            var (tokenCache, cacheKey) = this.ParseToken(token);

            if (tokenCache != null)
            {
                context = _app.GetAuthStrategyContext(tokenCache.Account);
            }
            return context;
        }

        /// <summary>
        /// 获取当前登录的用户名
        /// <para>通过URL中的Token参数或Cookie中的Token</para>
        /// </summary>
        /// <param name="otherInfo">The account.</param>
        /// <returns>System.String.</returns>
        public string GetUserName(string otherInfo = "")
        {
            if (_isEnabledId4)
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    return _httpContextAccessor.HttpContext.User.Identity.Name;
                }
                return string.Empty;
            }

            var token = GetToken();
            //解析token
            var (tokenCache, cacheKey) = this.ParseToken(token);

            if (tokenCache != null)
            {
                return tokenCache.Account;
            }
            return string.Empty;
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="appKey">应用程序key.</param>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns>System.String.</returns>
        public LoginResult Login(string appKey, string username, string pwd)
        {
            if (_isEnabledId4)
            {
                throw new Exception("接口启动了OAuth认证, 暂时不能使用该方式登录");
            }

            var result = _loginParse.Do(new PassportLoginRequest
            {
                AppKey = appKey,
                Account = username,
                Password = pwd
            });

            var httpContext = _httpContextAccessor.HttpContext;
            var log = new System_SysLogInfo
            {
                Content = $"用户登录,结果：{result.Message}",
                Result = result.Code == 200 ? 0 : 1,
                CreateId = username,
                Href = "Login/Login",
                Ip = httpContext.GetClientUserIp(),
                CreateName = username,
                TypeName = "登录日志"
            };
            _logApp.Add(log);

            return result;
        }

        /// <summary>
        /// 注销，如果是Identity登录，需要在controller处理注销逻辑
        /// </summary>
        public bool Logout()
        {
            var token = GetToken();
            //解析token
            var (tokenCache, cacheKey) = this.ParseToken(token);
            if (tokenCache != null)
            {
                _cacheContext.Remove(cacheKey);
            }
            return true;
        }
    }
}