using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace Xz.Node.IdentityServer
{
    /// <summary>
    /// IdentityServer4配置类
    /// </summary>
    public class Config
    {
        /// <summary>
        /// scopes define the resources in your system
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        /// <summary>
        /// API信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new[]
            {
                //api信息不能重复
                new ApiResource("xznodeapi", "XzNode.AdminApi")
                {
                    UserClaims =  { ClaimTypes.Name, JwtClaimTypes.Name }
                },
                //new ApiResource("xznodeweb", "XzNode.AdminWeb")
                //{
                //    UserClaims =  { ClaimTypes.Name, JwtClaimTypes.Name }
                //}
            };
        }

        /// <summary>
        /// 客户端信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients(bool isProduction)
        {
            //获取token的方式，启动id4项目后根据config配置获取
            //http://localhost:12796/connect/authorize?client_id=XzNode.AdminApi&redirect_uri=http://localhost:52787/swagger/oauth2-redirect.html&response_type=token&scope=xznodeapi

            var host = "http://xznode.club";// "http://xznode.club" "http://localhost"
            if (isProduction)
            {
                host = "http://xznode.club";//生产环境时，切换为正式服务器地址,这里发布的时候一定要记得改呀
            }
            return new[]
            {
                new Client
                {
                    ClientId = "XzNode.AdminApi",//客户端名称
                    ClientName = "xz.node.adminapi认证",//客户端描述
                    AllowedGrantTypes = GrantTypes.Implicit,//Implicit 隐式方式
                    AllowAccessTokensViaBrowser = true,//是否通过浏览器为此客户端传输访问令牌
                    // 设置 Token 过期时间 10天，默认是一个小时，如果勾选记住我，会保存为30天
                    //AccessTokenLifetime = 10*60*60*24,
                    RedirectUris =
                    {
                        //登录成功后返回的客户端地址,可以允许多个,如果有多个集群的服务就像这样子配置多个集群的返回地址即可
                        $"{host}:52789/swagger/oauth2-redirect.html",
                        $"{host}:52788/swagger/oauth2-redirect.html",
                        $"{host}:52787/swagger/oauth2-redirect.html",
                    },
                    AllowedScopes = { "xznodeapi" }//指定允许客户端请求的api范围,这个就是登录之后的拥有者，不同的客户端token共用就是通过这个控制
                },
                new Client
                {
                    ClientId = "XzNode.AdminWeb",
                    ClientName = "xz.node.adminweb认证",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    //RequireConsent = false, //禁用 consent 页面确认
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = 
                    {
                        "http://192.168.1.109/#/IdentityServerCallBack?",
                        //"http://localhost:8081/#/IdentityServerCallBack?"
                    },
                    PostLogoutRedirectUris = 
                    {
                        "http://xznode.club",
                        //"http://localhost:8081"
                    },
                    AllowedCorsOrigins = {
                        "http://xznode.club",
                        //"http://localhost:8081"
                    },
                    AllowedScopes = { "xznodeapi" },
                },
                new Client
                {
                    ClientId = "XzNode.Mvc",
                    ClientName = "xz.node.mvc认证",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    // 登录成功回调处理地址，处理回调返回的数据
                    RedirectUris = { $"{host}:1802/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { $"{host}:1802/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "xznodeapi"
                    }
                },
            };
        }
    }
}
