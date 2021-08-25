using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xz.Node.App.Auth.Revelance;
using Xz.Node.App.Auth.Revelance.Request;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Extensions;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;

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
    }
}
