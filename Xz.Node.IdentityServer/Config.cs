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
                new ApiResource("xznodeapi", "XzNode.AdminApi")
                {
                    UserClaims =  { ClaimTypes.Name, JwtClaimTypes.Name }
                }
            };
        }

        /// <summary>
        /// 客户端信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients(bool isProduction)
        {
            //获取token的方式，启动id4项目后根据config配置获取
            //http://localhost:12796/connect/authorize?client_id=XzNode.AdminApi&redirect_uri=http://localhost:9528/dashboard&response_type=token&scope=xznodeapi

            var host = "http://localhost";
            if (isProduction)
            {
                host = "http://localhost";//生产环境时，切换为正式服务器地址,这里发布的时候一定要记得改呀
            }
            return new[]
            {
                new Client
                {
                    ClientId = "XzNode.AdminApi",//客户端名称
                    ClientName = "xz.node.adminapi认证",//客户端描述
                    AllowedGrantTypes = GrantTypes.Implicit,//Implicit 隐式方式
                    AllowAccessTokensViaBrowser = true,//是否通过浏览器为此客户端传输访问令牌
                    RedirectUris =
                    {
                        //登录成功后返回的客户端地址,可以允许多个,如果有多个集群的服务就像这样子配置多个集群的返回地址即可
                        $"{host}:52789/swagger/oauth2-redirect.html", 
                        $"{host}:52788/swagger/oauth2-redirect.html",
                        $"{host}:52787/swagger/oauth2-redirect.html",
                    },
                    AllowedScopes = { "xznodeapi" }
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
