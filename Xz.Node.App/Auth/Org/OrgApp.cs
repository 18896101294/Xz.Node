using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xz.Node.App.Auth.Org.Request;
using Xz.Node.App.Auth.Org.Response;
using Xz.Node.App.Auth.Revelance;
using Xz.Node.App.Auth.Revelance.Request;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;
using static Xz.Node.Framework.Model.BaseDto;

namespace Xz.Node.App.Auth.Org
{
    /// <summary>
    /// 部门管理
    /// </summary>
    public class OrgApp : BaseTreeApp<Auth_OrgInfo, XzDbContext>
    {
        private RevelanceApp _revelanceApp;
        /// <summary>
        /// 模块管理
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="revelanceApp"></param>
        /// <param name="auth"></param>
        public OrgApp(IUnitWork<XzDbContext> unitWork, IRepository<Auth_OrgInfo, XzDbContext> repository
            , RevelanceApp revelanceApp, IAuth auth) : base(unitWork, repository, auth)
        {
            _revelanceApp = revelanceApp;
        }

        /// <summary>
        /// 添加部门
        /// </summary>
        public void Add(Auth_OrgInfo org)
        {
            var loginContext = _auth.GetCurrentUser();
            if (loginContext == null)
            {
                throw new InfoException("登录已过期", Define.INVALID_TOKEN);
            }
            CaculateCascade(org);

            UnitWork.ExecuteWithTransaction(() =>
            {
                UnitWork.Add(org);
                UnitWork.Save();

                //如果当前账号不是SYSTEM，则直接分配
                if (loginContext.User.Account != Define.SYSTEM_USERNAME)
                {
                    _revelanceApp.Assign(new AssignReq
                    {
                        type = Define.USERORG,
                        firstId = loginContext.User.Id,
                        secIds = new[] { org.Id }
                    });
                }
            });
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public string Update(Auth_OrgInfo org)
        {
            UpdateTreeObj(org);
            return org.Id;
        }

        /// <summary>
        /// 删除指定ID的部门及其所有子部门
        /// </summary>
        public void DelOrgCascade(List<string> ids)
        {
            var delOrgCascadeIds = UnitWork.Find<Auth_OrgInfo>(u => ids.Contains(u.Id)).Select(u => u.CascadeId).ToArray();
            if (delOrgCascadeIds == null || delOrgCascadeIds.Length == 0)
            {
                throw new InfoException("没有找到部门数据");
            }
            var delOrgIds = new List<string>();
            foreach (var cascadeId in delOrgCascadeIds)
            {
                delOrgIds.AddRange(UnitWork.Find<Auth_OrgInfo>(u => u.CascadeId.Contains(cascadeId)).Select(u => u.Id).ToArray());
            }
            UnitWork.ExecuteWithTransaction(() =>
            {
                UnitWork.Delete<Auth_RelevanceInfo>(u => u.Key == Define.USERORG && delOrgIds.Contains(u.SecondId));
                UnitWork.Delete<Auth_OrgInfo>(u => delOrgIds.Contains(u.Id));
                UnitWork.Save();
            });
        }

        /// <summary>
        /// 加载部门中的用户
        /// </summary>
        /// <param name="req">请求入参</param>
        public PageInfo<OrgUsersView> GetOrgUsers(OrgUsersDto req)
        {
            var pageData = new PageInfo<OrgUsersView>()
            {
                PageIndex = req.page,
                PageSize = req.limit
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
            var lambda = Expression.Lambda<Func<Auth_UserInfo, bool>>(expression, p);

            var query = UnitWork.Find<Auth_OrgInfo>(o => o.Id == req.OrgId).Join(UnitWork.Find<Auth_RelevanceInfo>(o => o.Key == Define.USERORG), a => a.Id, b => b.SecondId, (a, b) => new
            {
                OrgId = a.Id,
                OrgName = a.Name,
                OrgCode = a.CustomCode,
                FirstId = b.FirstId
            }).Join(UnitWork.Find<Auth_UserInfo>(lambda), a => a.FirstId, b => b.Id, (a, b) => new OrgUsersView
            {
                UserId = b.Id,
                Account = b.Account,
                Name = b.Name,
                Sex = b.Sex,
                Status = b.Status,
                BizCode = b.BizCode,
                Avatar = b.Avatar,
                CreateTime = b.CreateTime
            }).Skip(((req.page - 1) * req.limit)).Take(req.limit).ToList();
            if (query != null || query.Count() > 0)
            {
                var userIds = query.Select(o => o.UserId).ToArray();
                //获取关联的角色
                //var roleIds = _revelanceApp.Get(Define.USERROLE, true, userIds);
                //var roleDatas = _revelanceApp.Get(Define.USERROLE, true, userIds);
                //获取关联的部门
                var userOrgRevelanceDatas = UnitWork.Find<Auth_RelevanceInfo>(o => o.Key == Define.USERORG && userIds.Contains(o.FirstId)).ToList();
                var orgIds = userOrgRevelanceDatas.Select(o => o.SecondId);
                var orgDatas = UnitWork.Find<Auth_OrgInfo>(o => orgIds.Contains(o.Id)).ToList();
                foreach (var user in query)
                {
                    var userOrgs = userOrgRevelanceDatas.Where(o => o.FirstId == user.UserId);
                    var userOrgDatas = orgDatas.Where(o => userOrgs.Select(o => o.SecondId).Contains(o.Id));
                    user.OrgIds = userOrgDatas.Select(o => o.Id).ToList();
                    user.OrgNames = userOrgDatas.Select(o => o.Name).ToList();
                }
            }
            pageData.Datas = query.ToList();
            pageData.Total = query.Count();
            return pageData;
        }

        /// <summary>
        /// 加载特定用户的部门
        /// </summary>
        /// <param name="userId">用户唯一标识符。</param>
        public List<Auth_OrgInfo> LoadForUser(string userId)
        {
            var result = from userorg in UnitWork.Find<Auth_RelevanceInfo>(null)
                         join org in UnitWork.Find<Auth_OrgInfo>(null) on userorg.SecondId equals org.Id
                         where userorg.FirstId == userId && userorg.Key == Define.USERORG
                         select org;
            return result.ToList();
        }

        /// <summary>
        /// 获取所有部门，用于下拉框等，不可用作列表
        /// </summary>
        public List<OrgLoadAllView> LoadAll()
        {
            var result = UnitWork.Find<Auth_OrgInfo>(null).Select(o => new OrgLoadAllView()
            {
                Id = o.Id,
                Name = o.Name
            }).ToList();
            return result;
        }
    }
}
