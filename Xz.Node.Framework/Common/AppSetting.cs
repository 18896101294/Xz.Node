using System.Collections.Generic;
using Xz.Node.Framework.Enums;

namespace Xz.Node.Framework.Common
{
    /// <summary>
    /// 系统配置项
    /// </summary>
    public class AppSetting
    {
        public AppSetting()
        {
            this.HttpHost = "http://*:52789";
            this.Version = string.Empty;
            this.UploadPath = string.Empty;
            this.IdentityServer4 = new IdentityServer4Config();
            this.Jwt = new JwtConfig();
            this.OAuth2 = new OAuth2Config();
        }
        /// <summary>
        /// SSO地址
        /// </summary>
        public string HttpHost { get; set; }

        /// <summary>
        /// 版本信息
        /// 如果为demo,则屏蔽Post请求
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 数据库类型 SqlServer、MySql
        /// </summary>
        public Dictionary<string, string> DbTypes { get; set; }

        /// <summary> 
        /// 附件上传路径
        /// </summary>
        public string UploadPath { get; set; }

        /// <summary>
        /// IdentityServer4配置
        /// </summary>
        public IdentityServer4Config IdentityServer4 { get; set; }

        /// <summary>
        /// jwt配置
        /// </summary>
        public JwtConfig Jwt { get; set; }

        /// <summary>
        /// OAuth2配置
        /// </summary>
        public OAuth2Config OAuth2 { get; set; }

        /// <summary>
        /// Redis服务器配置
        /// </summary>
        public string RedisConf { get; set; }

        /// <summary>
        /// 授权方式,如果都启用则获取启用的第一个授权方式
        /// </summary>
        public AuthorizationWayEnum AuthorizationWay
        {
            get
            {
                if (this.IdentityServer4.Enabled)
                {
                    return AuthorizationWayEnum.IdentityServer4;
                }
                if (this.Jwt.Enabled)
                {
                    return AuthorizationWayEnum.Jwt;
                }
                if (this.OAuth2.Enabled)
                {
                    return AuthorizationWayEnum.OAuth2;
                }
                return AuthorizationWayEnum.OAuth2;
            }
        }
    }

    /// <summary>
    /// IdentityServer4配置
    /// </summary>
    public class IdentityServer4Config
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// IdentityServer服务器地址
        /// </summary>
        public string IdentityServerUrl { get; set; }
    }

    /// <summary>
    /// Jwt配置
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 密钥地址
        /// </summary>
        public string SecretFile { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 接受人
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 过期时间（天）
        /// </summary>
        public int AccessTokenExpiresDay { get; set; }
    }

    /// <summary>
    /// OAuth2认证
    /// </summary>
    public class OAuth2Config
    {
        /// <summary>
        /// 是启用
        /// </summary>
        public bool Enabled { get; set; }
    }
}
