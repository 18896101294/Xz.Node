using System;
using Xz.Node.Framework.Cache;
using Xz.Node.Framework.Common;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;
using Xz.Node.App.AppManagers;
using Xz.Node.Framework.Jwt;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.App.SSO
{
    /// <summary>
    /// 登录解析 处理登录逻辑，验证客户段提交的账号密码，保存登录信息
    /// </summary>
    public class LoginParse
    {
        //这个地方使用IRepository<Auth_UserInfo> 而不使用UserManagerApp是防止循环依赖
        public readonly IRepository<Auth_UserInfo, XzDbContext> _app;
        private readonly ICacheContext _cacheContext;
        private readonly AppManager _appInfoService;
        private readonly IJwtTokenHelper _jwtTokenHelper;
        private readonly IConfigurationRoot _configuration;
        private readonly bool _isEnabledId4;
        private readonly bool _isEnabledJwt;
        private readonly bool _isEnabledOAuth2;

        public LoginParse(AppManager infoService,
            ICacheContext cacheContext,
            IRepository<Auth_UserInfo, XzDbContext> userApp,
            IJwtTokenHelper jwtTokenHelper)
        {
            _appInfoService = infoService;
            _cacheContext = cacheContext;
            _app = userApp;
            _jwtTokenHelper = jwtTokenHelper;
            _configuration = ConfigHelper.GetConfigRoot();
            _isEnabledId4 = _configuration[$"AppSetting:IdentityServer4:Enabled"].ToBool();
            _isEnabledJwt = _configuration[$"AppSetting:Jwt:Enabled"].ToBool();
            _isEnabledOAuth2 = _configuration[$"AppSetting:OAuth2:Enabled"].ToBool();
        }

        public LoginResult Do(PassportLoginRequest model)
        {
            var result = new LoginResult();
            try
            {
                model.Trim();
                //todo:如果需要判定应用，可以取消该注释
                var appInfo = _appInfoService.GetByAppKey(model.AppKey);
                if (appInfo == null)
                {
                    throw new Exception("应用不存在");
                }
                //获取用户信息
                Auth_UserInfo userInfo = null;
                if (model.Account == Define.SYSTEM_USERNAME)
                {
                    userInfo = new Auth_UserInfo
                    {
                        Id = Guid.Empty.ToString(),
                        Account = Define.SYSTEM_USERNAME,
                        Name = "超级管理员",
                        Password = Define.SYSTEM_USERPWD
                    };
                }
                else
                {
                    userInfo = _app.FirstOrDefault(u => u.Account == model.Account);
                }

                if (userInfo == null)
                {
                    throw new Exception("用户不存在");
                }
                if (userInfo.Password != model.Password)
                {
                    throw new Exception("密码错误");
                }

                if (userInfo.Status != 0)
                {
                    throw new Exception("账号状态异常，可能已停用");
                }

                var currentSession = new UserAuthSession()
                {
                    Account = model.Account,
                    Name = userInfo.Name,
                    AppKey = model.AppKey,
                    CreateTime = DateTime.Now,
                    //IpAddress = HttpContext.Current.Request.UserHostAddress
                };

                var timeOut = DateTime.Now.AddDays(10);
                if (_isEnabledJwt)
                {
                    //jwt的授权方式
                    var dic = new Dictionary<string, string>();
                    dic.Add("userId", userInfo.Id);
                    dic.Add("account", userInfo.Account);
                    var tokenResult = _jwtTokenHelper.CreateToken(dic);//键值对创建
                    //var tokenResult1 = _jwtTokenHelper.CreateToken(userInfo);//根据userinfo创建
                    currentSession.Token = tokenResult.TokenStr;
                    timeOut = tokenResult.Expires;
                }

                if (_isEnabledOAuth2)
                {
                    currentSession.Token = userInfo.Id + "_" + userInfo.Account + "_" + Guid.NewGuid().ToString().GetHashCode().ToString("x");
                }

                //存入缓存
                var cacheKey = $"{userInfo.Id}_{userInfo.Account}_token";
                if (_cacheContext.Get<UserAuthSession>(cacheKey) != null)
                {
                    _cacheContext.Remove(cacheKey);
                }
                _cacheContext.Set(cacheKey, currentSession, timeOut);

                result.Code = 200;
                result.Token = currentSession.Token;
                result.Expires = timeOut;
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}