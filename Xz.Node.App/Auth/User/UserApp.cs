using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xz.Node.App.Auth.Org;
using Xz.Node.App.Auth.Revelance;
using Xz.Node.App.Auth.Role;
using Xz.Node.App.Auth.User.Request;
using Xz.Node.App.Auth.User.Response;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Encryption;
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

        /// <summary>
        /// 用户管理构造
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        /// <param name="revelanceApp"></param>
        public UserApp(IUnitWork<XzDbContext> unitWork, IRepository<Auth_UserInfo, XzDbContext> repository,
            IAuth auth,
            RevelanceApp revelanceApp) : base(unitWork, repository, auth)
        {
            _revelanceApp = revelanceApp;
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

            var loginUser = _auth.GetCurrentUser();

            var query = UnitWork.Find<Auth_UserInfo>(lambda);

            var userOrgs = from user in query
                           join relevance in UnitWork.Find<Auth_RelevanceInfo>(u => u.Key == Define.USERORG)
                               on user.Id equals relevance.FirstId into temp
                           from r in temp.DefaultIfEmpty()
                           join org in UnitWork.Find<Auth_OrgInfo>(null)
                               on r.SecondId equals org.Id into orgtmp
                           from o in orgtmp.DefaultIfEmpty()
                           select new
                           {
                               user.Account,
                               user.Name,
                               user.Id,
                               user.Sex,
                               user.Status,
                               user.BizCode,
                               user.Avatar,
                               user.CreateId,
                               user.CreateTime,
                               user.TypeId,
                               user.TypeName,
                               r.Key,
                               r.SecondId,
                               OrgId = o.Id,
                               OrgName = o.Name
                           };

            //如果请求的orgId不为空
            if (!string.IsNullOrEmpty(req.OrgId) && !req.OrgId.Equals("0"))
            {
                var org = loginUser.Orgs.SingleOrDefault(u => u.Id == req.OrgId);
                var orgIds = loginUser.Orgs.Where(u => u.Id == org.Id).Select(u => u.Id).ToArray();
                //只获取机构里面的用户
                userOrgs = userOrgs.Where(u => u.Key == Define.USERORG && orgIds.Contains(u.OrgId));
            }
            else //如果请求的orgId为空，即为跟节点，这时可以额外获取到机构已经被删除的用户，从而进行机构分配。可以根据自己需求进行调整
            {
                var orgIds = loginUser.Orgs.Select(u => u.Id).ToArray();

                //获取用户可以访问的机构的用户和没有任何机构关联的用户（机构被删除后，没有删除这里面的关联关系）
                userOrgs = userOrgs.Where(u => (u.Key == Define.USERORG && orgIds.Contains(u.OrgId)) || (u.OrgId == null));
            }

            var userViews = userOrgs.ToList().GroupBy(b => b.Account).Select(u => new LoadUsersPageView
            {
                Id = u.First().Id,
                Account = u.Key,
                Name = u.First().Name,
                Sex = u.First().Sex,
                Status = u.First().Status,
                BizCode = u.First().BizCode,
                Avatar = u.First().Avatar,
                CreateTime = u.First().CreateTime,
                OrgIds = u.Select(x => x.OrgId).ToList(),
                OrgNames = u.Select(x => x.OrgName).ToList()
            });

            pageData.Datas = userViews.OrderBy(u => u.CreateTime).Skip((req.page - 1) * req.limit).Take(req.limit).ToList();
            pageData.Total = userViews.Count();
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
        /// 获取所有用户，用于下拉框等，不可用作列表
        /// </summary>
        public List<LoadUserView> LoadAll()
        {
            var resultData = new List<LoadUserView>();

            var query = UnitWork.Find<Auth_OrgInfo>(null).Join(UnitWork.Find<Auth_RelevanceInfo>(o => o.Key == Define.USERORG), a => a.Id, b => b.SecondId, (a, b) => new
            {
                OrgId = a.Id,
                OrgName = a.Name,
                FirstId = b.FirstId
            }).Join(UnitWork.Find<Auth_UserInfo>(null), a => a.FirstId, b => b.Id, (a, b) => new
            {
                Lable = a.OrgName,
                Account = b.Account,
                Name = b.Name,
                Id = b.Id
            }).ToList();
            if (query != null || query.Count() > 0)
            {
                var groups = query.GroupBy(o => o.Lable);

                foreach (var group in groups)
                {
                    resultData.Add(new LoadUserView()
                    {
                        Lable = group.Key,
                        Options = group.ToList().Select(o=> new LoadUserModel()
                        { 
                            Id = o.Id,
                            Name = o.Name,
                        }).ToList()
                    });
                }
            }
            return resultData;
        }

        /// <summary>
        /// 获取所有用户，不可用作列表
        /// </summary>
        public List<Auth_UserInfo> LoadUserAll()
        {
            var query = UnitWork.Find<Auth_UserInfo>(o => o.Status == 0).ToList();
            return query;
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="req"></param>
        public void SaveUser(UpdateUserReq req)
        {
            if (req.OrgIds.Count() == 0)
            {
                throw new InfoException("请为用户分配部门");
            }

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
                    _revelanceApp.Assign(Define.USERORG, req.OrgIds.ToLookup(u => requser.Id));
                    //角色关联
                    if (req.RoleIds.Count() > 0)
                    {
                        _revelanceApp.Assign(Define.USERROLE, req.OrgIds.ToLookup(u => requser.Id));
                    }
                }
                else
                {
                    if (UnitWork.Any<Auth_UserInfo>(u => u.Id != req.Id && u.Account == req.Account))
                    {
                        throw new InfoException("用户账号已存在");
                    }
                    UnitWork.Update<Auth_UserInfo>(u => u.Id == req.Id, u => new Auth_UserInfo
                    {
                        Account = req.Account,
                        BizCode = req.BizCode,
                        Name = req.Name,
                        Sex = req.Sex,
                        Status = req.Status
                    });
                    _revelanceApp.DeleteBy(Define.USERORG, requser.Id);
                    _revelanceApp.Assign(Define.USERORG, req.OrgIds.ToLookup(u => requser.Id));
                    //角色关联
                    if (req.RoleIds.Count() > 0)
                    {
                        _revelanceApp.DeleteBy(Define.USERROLE, requser.Id);
                        _revelanceApp.Assign(Define.USERROLE, req.RoleIds.ToLookup(u => requser.Id));
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
        /// 禁用用户
        /// </summary>
        /// <param name="req"></param>
        public void DisableUser(BaseIdsReq req)
        {
            if (req.Ids == null || req.Ids.Count() == 0)
            {
                throw new InfoException("用户Id不能为空");
            }

            UnitWork.Update<Auth_UserInfo>(u => req.Ids.Contains(u.Id), u => new Auth_UserInfo
            {
                Status = (int)DisableStatusEnum.Disable
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
                Password = EncryptionHelper.Encrypt(Define.INITIAL_PWD)
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
