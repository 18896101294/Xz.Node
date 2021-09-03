using System;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.Auth.Revelance;
using Xz.Node.App.Auth.Revelance.Request;
using Xz.Node.App.Auth.Role.Request;
using Xz.Node.App.Auth.Role.Response;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.Auth.Role
{
    /// <summary>
    /// 角色管理
    /// </summary>
    public class RoleApp : BaseStringApp<Auth_RoleInfo, XzDbContext>
    {
        private readonly RevelanceApp _revelanceApp;
        /// <summary>
        /// 角色管理入参
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="app"></param>
        /// <param name="auth"></param>
        public RoleApp(IUnitWork<XzDbContext> unitWork, IRepository<Auth_RoleInfo, XzDbContext> repository,
            RevelanceApp app, IAuth auth) : base(unitWork, repository, auth)
        {
            _revelanceApp = app;
        }

        /// <summary>
        /// 加载当前登录用户可访问的全部角色
        /// </summary>
        public PageInfo<LoadRolesPageView> LoadRolesPage(LoadRolesPageReq req)
        {
            var page = new PageInfo<LoadRolesPageView>()
            {
                PageIndex = req.page,
                PageSize = req.limit,
                Datas = new List<LoadRolesPageView>()
            };
            var loginUser = _auth.GetCurrentUser();
            //这里因为角色不会太多，做了假分页，不会消耗多少性能
            var roles = loginUser.Roles;
            if (!string.IsNullOrEmpty(req.Name))
            {
                roles = roles.Where(u => u.Name == req.Name).ToList();
            }
            if (!string.IsNullOrEmpty(req.Code))
            {
                roles = roles.Where(u => u.Code == req.Code).ToList();
            }
            if (req.Status.HasValue)
            {
                roles = roles.Where(u => u.Status == req.Status).ToList();
            }
            var resultData = roles.Skip((req.page - 1) * req.limit).Take(req.limit).Select(o => new LoadRolesPageView()
            {
                Id = o.Id,
                Name = o.Name,
                Code = o.Code,
                Status = o.Status,
                UserNames = new List<string>()
            }).ToList();
            if (resultData == null || resultData.Count() == 0)
            {
                return page;
            }
            var roleIds = resultData.Select(o => o.Id);
            var relevanceDatas = UnitWork.Find<Auth_RelevanceInfo>(o => o.Key == Define.USERROLE && roleIds.Contains(o.SecondId));
            if (relevanceDatas != null && relevanceDatas.Count() > 0)
            {
                resultData.ForEach(role =>
                {
                    var userRoleDatas = relevanceDatas.Where(o => o.SecondId == role.Id);
                    if (userRoleDatas != null && userRoleDatas.Count() > 0)
                    {
                        var userIds = userRoleDatas.Select(o => o.FirstId).Distinct().ToList();
                        var userNames = UnitWork.Find<Auth_UserInfo>(o => userIds.Contains(o.Id)).Select(o => o.Name);
                        role.UserNames = userNames.ToList();
                    }
                });
            }
            page.Datas = resultData;
            page.Total = roles.Count();
            return page;
        }

        /// <summary>
        /// 添加角色，如果当前登录用户不是System，则直接把新角色分配给当前登录用户
        /// </summary>
        public void AddRole(Auth_RoleInfo req)
        {
            UnitWork.ExecuteWithTransaction(() =>
            {
                this.CheckRoleInfo(req);
                req.CreateId = _auth.GetCurrentUser().User.Id;
                UnitWork.Add(req);
                UnitWork.Save();
                //如果当前账号不是SYSTEM，则直接分配
                var loginUser = _auth.GetCurrentUser();
                if (loginUser.User.Account != Define.SYSTEM_USERNAME)
                {
                    _revelanceApp.Assign(new AssignReq
                    {
                        type = Define.USERROLE,
                        firstId = loginUser.User.Id,
                        secIds = new[] { req.Id }
                    });
                }
            });
        }

        /// <summary>
        /// 更新角色属性
        /// </summary>
        public void UpdateRole(Auth_RoleInfo req)
        {
            this.CheckRoleInfo(req);
            UnitWork.Update<Auth_RoleInfo>(u => u.Id == req.Id, u => new Auth_RoleInfo
            {
                Name = req.Name,
                Code = req.Code,
                Status = req.Status
            });
        }

        /// <summary>
        /// 删除角色,包含用户角色关系、角色资源关系、角色菜单关系、角色模块关系、角色数据字段关系
        /// </summary>
        /// <param name="ids"></param>
        public override void Delete(string[] ids)
        {
            UnitWork.ExecuteWithTransaction(() =>
            {
                //删除用户角色关系
                UnitWork.Delete<Auth_RelevanceInfo>(u => (u.Key == Define.USERROLE)
                                              && ids.Contains(u.SecondId));
                //删除角色资源关系、角色菜单关系、角色模块关系、角色数据字段关系
                UnitWork.Delete<Auth_RelevanceInfo>(u => (u.Key == Define.ROLERESOURCE || u.Key == Define.ROLEELEMENT || u.Key == Define.ROLEMODULE || u.Key == Define.ROLEDATAPROPERTY)
                                              && ids.Contains(u.FirstId));
                UnitWork.Delete<Auth_RoleInfo>(u => ids.Contains(u.Id));
                UnitWork.Save();
            });
        }

        /// <summary>
        /// 为角色分配用户
        /// </summary>
        /// <param name="req"></param>
        public void RoleAllocationUsers(AssignRoleUsersReq req)
        {
            if(string.IsNullOrEmpty(req.RoleId))
            {
                throw new InfoException("角色Id不能为空");
            }
            //为角色分配用户
            _revelanceApp.AssignRoleUsers(req);
        }

        /// <summary>
        /// 为角色分配模块
        /// </summary>
        /// <param name="req"></param>
        public void RoleAllocationModules(AssignRoleModulesReq req)
        {
            if (string.IsNullOrEmpty(req.RoleId))
            {
                throw new InfoException("角色Id不能为空");
            }
            //为角色分配模块
            _revelanceApp.AssignRoleModules(req);
        }

        /// <summary>
        /// 为角色分配菜单
        /// </summary>
        /// <param name="reqs"></param>
        public void RoleAllocationMenus(List<AssignRoleMenusReq> reqs)
        {
            //为角色分配菜单
            _revelanceApp.AssignRoleMenus(reqs);
        }

        /// <summary>
        /// 为角色分配字段
        /// </summary>
        /// <param name="reqs"></param>
        public void RoleAllocationDatas(List<AssignDataReq> reqs)
        {
            //为角色分配字段
            _revelanceApp.AssignData(reqs);
        }


        /// <summary>
        /// 验证角色名和代码是否已经存在
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private void CheckRoleInfo(Auth_RoleInfo req)
        {
            if (string.IsNullOrEmpty(req.Code))
            {
                throw new InfoException("角色代码不能为空");
            }
            if (string.IsNullOrEmpty(req.Name))
            {
                throw new InfoException("角色名不能为空");
            }

            if (string.IsNullOrEmpty(req.Id))
            {
                var roleCount = UnitWork.Find<Auth_RoleInfo>(o => o.Name == req.Name || o.Code == req.Code).Count();
                if (roleCount > 0)
                {
                    throw new InfoException($"角色名{req.Name}或角色代码{req.Code}已存在");
                }
            }
            else
            {
                var roleCount = UnitWork.Find<Auth_RoleInfo>(o => o.Id != req.Id && (o.Name == req.Name || o.Code == req.Code)).Count();
                if (roleCount > 0)
                {
                    throw new InfoException($"角色名{req.Name}或角色代码{req.Code}已存在");
                }
            }
        }
    }
}
