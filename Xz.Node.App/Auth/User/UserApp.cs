using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xz.Node.App.Auth.Org;
using Xz.Node.App.Auth.Revelance;
using Xz.Node.App.Auth.User.Request;
using Xz.Node.App.Auth.User.Response;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.Auth.User
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class UserApp : BaseStringApp<Auth_UserInfo, XzDbContext>
    {
        private readonly RevelanceApp _revelanceApp;
        private readonly OrgApp _orgApp;
        /// <summary>
        /// 用户管理构造
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        /// <param name="revelanceApp"></param>
        /// <param name="orgApp"></param>
        public UserApp(IUnitWork<XzDbContext> unitWork, IRepository<Auth_UserInfo, XzDbContext> repository,
            IAuth auth,
            RevelanceApp revelanceApp,
            OrgApp orgApp) : base(unitWork, repository, auth)
        {
            _revelanceApp = revelanceApp;
            _orgApp = orgApp;
        }

        /// <summary>
        /// 分页获取用户列表信息
        /// </summary>
        /// <param name="req">请求入参</param>
        public PageInfo<LoadUsersPageView> LoadUsersPage(LoadUsersPageReq req)
        {
            var pageData = new PageInfo<LoadUsersPageView>()
            {
                PageIndex = req.page,
                PageSize = req.limit,
                Datas = new List<LoadUsersPageView>()
            };
            var (expression, p) = EFSQLHelpr.CreateBinaryExpression<Auth_UserInfo>();
            if (!string.IsNullOrEmpty(req.Account))
            {
                expression = EFSQLHelpr.ExpressionAndAlso(expression, p, "Account", req.Account);
            }
            if (!string.IsNullOrEmpty(req.Name))
            {
                expression = EFSQLHelpr.ExpressionAndAlso(expression, p, "Name", req.Name);
            }
            if (req.Status.HasValue)
            {
                expression = EFSQLHelpr.ExpressionAndAlso(expression, p, "Status", req.Status.ToString());
            }
            if (req.BeginCreatTime.HasValue)
            {
                expression = EFSQLHelpr.ExpressionAndAlso(expression, p, "CreateTime", req.Status.ToString(), ConditionOperEnum.GreaterThanEqual);
            }
            if (req.EndCreatTime.HasValue)
            {
                expression = EFSQLHelpr.ExpressionAndAlso(expression, p, "CreateTime", req.Status.ToString(), ConditionOperEnum.LessThanEqual);
            }
            var lambda = Expression.Lambda<Func<Auth_UserInfo, bool>>(expression, p);

            var queryDatas = UnitWork.Find<Auth_UserInfo>(lambda).Skip(((req.page - 1) * req.limit)).Take(req.limit).Select(o => new LoadUsersPageView()
            {
                Id = o.Id,
                Account = o.Account,
                Name = o.Name,
                Sex = o.Sex,
                Status = o.Status,
                BizCode = o.BizCode,
                Avatar = o.Avatar,
                CreateTime = o.CreateTime
            });
            if (queryDatas != null || queryDatas.Count() > 0)
            {
                var userIds = queryDatas.Select(o => o.Id).ToArray();
                //获取关联的角色
                //var roleIds = _revelanceApp.Get(Define.USERROLE, true, userIds);
                //var roleDatas = _revelanceApp.Get(Define.USERROLE, true, userIds);
                //获取关联的部门
                var userOrgRevelanceDatas = UnitWork.Find<Auth_RelevanceInfo>(o => o.Key == Define.USERORG && userIds.Contains(o.FirstId)).ToList();
                var orgIds = userOrgRevelanceDatas.Select(o => o.SecondId);
                var orgDatas = UnitWork.Find<Auth_OrgInfo>(o => orgIds.Contains(o.Id)).ToList();
                foreach (var user in queryDatas)
                {
                    var userOrgs = userOrgRevelanceDatas.Where(o => o.FirstId == user.Id);
                    var userOrgDatas = orgDatas.Where(o => userOrgs.Select(o => o.SecondId).Contains(o.Id));
                    user.OrgIds = userOrgDatas.Select(o => o.Id).ToList();
                    user.OrgNames = string.Join(',', userOrgDatas.Select(o => "[" + o.Name + "]"));
                }
                pageData.Datas = queryDatas.ToList();
                pageData.Total = queryDatas.Count();
            }
            return pageData;
        }

        /// <summary>
        /// 根据登录账号获取登录信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Auth_UserInfo GetByAccount(string account, string password)
        {
            return Repository.FirstOrDefault(u => u.Account == account && u.Password == password);
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="req"></param>
        public void SaveUser(UpdateUserReq req)
        {
            if (string.IsNullOrEmpty(req.OrgIds))
            {
                throw new InfoException("请为用户分配部门");
            }
            string[] orgIds = req.OrgIds.Split(',').ToArray();

            Auth_UserInfo requser = req;
            UnitWork.ExecuteWithTransaction(() =>
            {
                if (string.IsNullOrEmpty(req.Id))
                {
                    if (UnitWork.Any<Auth_UserInfo>(u => u.Account == req.Account))
                    {
                        throw new InfoException("用户账号已存在");
                    }
                    requser.Password = Define.INITIAL_PWD;//设置默认密码
                    requser.CreateId = _auth.GetCurrentUser().User.Id;
                    requser.CreateTime = DateTime.Now;
                    UnitWork.Add(requser);
                    //部门关联
                    _revelanceApp.Assign(Define.USERORG, orgIds.ToLookup(u => requser.Id));
                    //角色关联
                    if (!string.IsNullOrEmpty(req.RoleIds))
                    {

                    }
                }
                else
                {
                    UnitWork.Update<Auth_UserInfo>(u => u.Id == req.Id, u => new Auth_UserInfo
                    {
                        Account = req.Account,
                        BizCode = req.BizCode,
                        Name = req.Name,
                        Sex = req.Sex,
                        Status = req.Status
                    });
                    //部门关联
                    _revelanceApp.DeleteBy(Define.USERORG, requser.Id);
                    _revelanceApp.Assign(Define.USERORG, orgIds.ToLookup(u => requser.Id));
                    //角色关联
                    if (!string.IsNullOrEmpty(req.RoleIds))
                    {

                    }
                }
                UnitWork.Save();
            });
        }

        /// <summary>
        /// 删除用户,包含用户与组织关系、用户与角色关系
        /// </summary>
        /// <param name="ids"></param>
        public override void Delete(string[] ids)
        {
            UnitWork.ExecuteWithTransaction(() =>
            {
                UnitWork.Delete<Auth_RelevanceInfo>(u => (u.Key == Define.USERROLE || u.Key == Define.USERORG)
                                               && ids.Contains(u.FirstId));
                UnitWork.Delete<Auth_UserInfo>(u => ids.Contains(u.Id));
                UnitWork.Save();
            });
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="req"></param>
        public void ChangePassword(ChangePasswordReq req)
        {
            Repository.Update(o => o.Id == req.Id, user => new Auth_UserInfo
            {
                Password = Define.INITIAL_PWD
            });
        }

        /// <summary>
        /// 修改用户基本资料
        /// </summary>
        /// <param name="req"></param>
        public void ChangeProfile(ChangeProfileReq req)
        {
            Repository.Update(o => o.Id == req.Id, user => new Auth_UserInfo
            {
                Name = req.Name,
                BizCode = req.BizCode,
                Sex = req.Sex,
                Status = req.Status,
                Avatar = req.Avatar
            });
        }
    }
}
