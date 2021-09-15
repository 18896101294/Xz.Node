using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.App.Auth.Module.Response;
using Xz.Node.App.Auth.Revelance.Request;
using Xz.Node.App.Base;
using Xz.Node.App.Interface;
using Xz.Node.Framework.Common;
using Xz.Node.Repository;
using Xz.Node.Repository.Domain.Auth;
using Xz.Node.Repository.Interface;

namespace Xz.Node.App.Auth.Revelance
{
    /// <summary>
    /// 资源分配
    /// </summary>
    public class RevelanceApp : BaseStringApp<Auth_RelevanceInfo, XzDbContext>
    {
        private readonly ILogger<RevelanceApp> _logger;
        /// <summary>
        /// 资源分配
        /// </summary>
        /// <param name="unitWork"></param>
        /// <param name="repository"></param>
        /// <param name="auth"></param>
        /// <param name="logger"></param>
        public RevelanceApp(IUnitWork<XzDbContext> unitWork, IRepository<Auth_RelevanceInfo, XzDbContext> repository, IAuth auth, ILogger<RevelanceApp> logger) : base(unitWork,
            repository, auth)
        {
            _logger = logger;
        }

        /// <summary>
        /// 添加关联
        /// 比如给用户分配资源，那么firstId就是用户ID，secIds就是资源ID列表
        /// </summary>
        /// <param name="req"></param>
        public void Assign(AssignReq req)
        {
            Assign(req.type, req.secIds.ToLookup(u => req.firstId));
        }

        /// <summary>
        /// 添加关联，需要人工删除以前的关联
        /// </summary>
        /// <param name="key"></param>
        /// <param name="idMaps"></param>
        public void Assign(string key, ILookup<string, string> idMaps)
        {
            UnitWork.BatchAdd((from sameVals in idMaps
                               from value in sameVals
                               select new Auth_RelevanceInfo
                               {
                                   Key = key,
                                   FirstId = sameVals.Key,
                                   SecondId = value,
                                   OperatorId = _auth.GetCurrentUser().User.Id,
                                   OperateTime = DateTime.Now
                               }).ToArray());
            UnitWork.Save();
        }

        /// <summary>
        /// 添加关联，需要人工删除以前的关联
        /// </summary>
        /// <param name="key"></param>
        /// <param name="firstId"></param>
        /// <param name="secondId"></param>
        /// <param name="threeIds"></param>
        public void Assign(string key, string firstId, string secondId, string[] threeIds)
        {
            var addData = new List<Auth_RelevanceInfo>();
            foreach (var threeId in threeIds)
            {
                addData.Add(new Auth_RelevanceInfo()
                {
                    Key = key,
                    FirstId = firstId,
                    SecondId = secondId,
                    ThirdId = threeId,
                    OperatorId = _auth.GetCurrentUser().User.Id,
                    OperateTime = DateTime.Now
                });
            }
            if (addData.Count > 0)
            {
                UnitWork.BatchAdd(addData.ToArray());
                UnitWork.Save();
            }
        }

        /// <summary>
        /// 取消关联
        /// </summary>
        public void UnAssign(AssignReq req)
        {
            if (req.secIds == null || req.secIds.Length == 0)
            {
                DeleteBy(req.type, req.firstId);
            }
            else
            {
                DeleteBy(req.type, req.secIds.ToLookup(u => req.firstId));
            }
        }

        /// <summary>
        /// 删除关联
        /// </summary>
        /// <param name="key">关联标识</param>
        /// <param name="idMaps">关联的&lt;firstId, secondId&gt;数组</param>
        private void DeleteBy(string key, ILookup<string, string> idMaps)
        {
            foreach (var sameVals in idMaps)
            {
                foreach (var value in sameVals)
                {
                    _logger.LogInformation($"start=> delete {key} {sameVals.Key} {value}");
                    try
                    {
                        UnitWork.Delete<Auth_RelevanceInfo>(u => u.Key == key && u.FirstId == sameVals.Key && u.SecondId == value);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, e.Message);
                    }
                    _logger.LogInformation($"end=> {key} {sameVals.Key} {value}");
                }
            }
        }

        /// <summary>
        /// 删除关联
        /// </summary>
        /// <param name="key"></param>
        /// <param name="firstIds"></param>
        public void DeleteBy(string key, params string[] firstIds)
        {
            UnitWork.Delete<Auth_RelevanceInfo>(u => firstIds.Contains(u.FirstId) && u.Key == key);
        }


        /// <summary>
        /// 根据关联表的一个键获取另外键的值
        /// </summary>
        /// <param name="key">映射标识</param>
        /// <param name="returnSecondIds">返回的是否为映射表的第二列，如果不是则返回第一列</param>
        /// <param name="ids">已知的ID列表</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> Get(string key, bool returnSecondIds, params string[] ids)
        {
            if (returnSecondIds)
            {
                return Repository.Find(u => u.Key == key
                                            && ids.Contains(u.FirstId)).Select(u => u.SecondId).ToList();
            }
            else
            {
                return Repository.Find(u => u.Key == key
                                            && ids.Contains(u.SecondId)).Select(u => u.FirstId).ToList();
            }
        }

        /// <summary>
        /// 根据key ,firstId,secondId获取thirdId
        /// </summary>
        /// <param name="key"></param>
        /// <param name="firstId"></param>
        /// <param name="secondId"></param>
        /// <returns></returns>
        public List<string> Get(string key, string firstId, string secondId)
        {
            return Repository.Find(u => u.Key == key && u.FirstId == firstId && u.SecondId == secondId)
                .Select(u => u.ThirdId).ToList();
        }

        /// <summary>
        /// 根据key ,firstId,secondIds获取thirdId
        /// </summary>
        /// <param name="key"></param>
        /// <param name="firstId"></param>
        /// <param name="secondIds"></param>
        /// <returns></returns>
        public List<string> Get(string key, string firstId, List<string> secondIds)
        {
            return Repository.Find(u => u.Key == key && u.FirstId == firstId && secondIds.Contains(u.SecondId))
                .Select(u => u.ThirdId).ToList();
        }

        /// <summary>
        /// 获取角色已分配的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="firstId"></param>
        /// <param name="secondIds"></param>
        /// <returns></returns>
        public List<LoadPropertiesForRoleView> GetRoleProp(string key, string firstId, List<string> secondIds)
        {
            return Repository.Find(u => u.Key == key && u.FirstId == firstId && secondIds.Contains(u.SecondId))
                .Select(u => new LoadPropertiesForRoleView()
                {
                    ModuleId = u.SecondId,
                    KeyId = $"{u.SecondId}_{u.ThirdId}" //防止key重复
                }).ToList();
        }

        /// <summary>
        /// 分配数据字段权限
        /// </summary>
        /// <param name="request"></param>
        public void AssignData(AssignDataReq request)
        {
            if (request == null)
            {
                return;
            }

            var operatorId = _auth.GetCurrentUser().User.Id;

            if (!request.Properties.Any())
            {
                return;
            }
            var addDatas = (from prop in request.Properties
                                select new Auth_RelevanceInfo
                                {
                                    Key = Define.ROLEDATAPROPERTY,
                                    FirstId = request.RoleId,
                                    SecondId = prop.Split("_")[0],
                                    ThirdId = prop.Split("_")[1],
                                    OperatorId = operatorId,
                                    OperateTime = DateTime.Now
                                }).ToArray();

            UnitWork.ExecuteWithTransaction(() =>
            {
                //删除以前的所有字段
                UnitWork.Delete<Auth_RelevanceInfo>(u => request.RoleId == u.FirstId && u.Key == Define.ROLEDATAPROPERTY);

                //批量分配角色字段
                UnitWork.BatchAdd(addDatas.ToArray());

                UnitWork.Save();
            });
        }

        /// <summary>
        /// 取消数据字段分配
        /// </summary>
        /// <param name="request"></param>
        public void UnAssignData(AssignDataReq request)
        {
            if (request.Properties == null || request.Properties.Length == 0)
            {
                if (string.IsNullOrEmpty(request.ModuleId)) //模块为空，直接把角色的所有授权删除
                {
                    DeleteBy(Define.ROLEDATAPROPERTY, request.RoleId);
                }
                else //把角色的某一个模块权限全部删除
                {
                    DeleteBy(Define.ROLEDATAPROPERTY, new[] { request.ModuleId }.ToLookup(u => request.RoleId));
                }
            }
            else //按具体的id删除
            {
                foreach (var property in request.Properties)
                {
                    UnitWork.Delete<Auth_RelevanceInfo>(u => u.Key == Define.ROLEDATAPROPERTY
                                                    && u.FirstId == request.RoleId
                                                    && u.SecondId == request.ModuleId
                                                    && u.ThirdId == property);
                }
            }
        }

        /// <summary>
        /// 为角色分配用户，需要统一提交，会删除以前该角色的所有用户
        /// </summary>
        /// <param name="request"></param>
        public void AssignRoleUsers(AssignRoleUsersReq request)
        {
            var operatorId = _auth.GetCurrentUser().User.Id;

            UnitWork.ExecuteWithTransaction(() =>
            {
                //删除以前的所有用户
                UnitWork.Delete<Auth_RelevanceInfo>(u => u.SecondId == request.RoleId && u.Key == Define.USERROLE);
                //批量分配用户角色
                if (request.UserIds != null && request.UserIds.Length > 0)
                {
                    UnitWork.BatchAdd((from firstId in request.UserIds
                                       select new Auth_RelevanceInfo
                                       {
                                           Key = Define.USERROLE,
                                           FirstId = firstId,
                                           SecondId = request.RoleId,
                                           OperatorId = operatorId,
                                           OperateTime = DateTime.Now
                                       }).ToArray());
                }
                UnitWork.Save();
            });
        }

        /// <summary>
        /// 为角色分配模块，需要统一提交，会删除以前该角色的所有模块
        /// </summary>
        /// <param name="request"></param>
        public void AssignRoleModules(AssignRoleModulesReq request)
        {
            var operatorId = _auth.GetCurrentUser().User.Id;

            UnitWork.ExecuteWithTransaction(() =>
            {
                //删除以前的所有模块
                UnitWork.Delete<Auth_RelevanceInfo>(u => u.FirstId == request.RoleId && u.Key == Define.ROLEMODULE);
                //批量分配角色模块
                if (request.ModuleIds != null && request.ModuleIds.Length > 0)
                {
                    UnitWork.BatchAdd((from secondId in request.ModuleIds
                                       select new Auth_RelevanceInfo
                                       {
                                           Key = Define.ROLEMODULE,
                                           FirstId = request.RoleId,
                                           SecondId = secondId,
                                           OperatorId = operatorId,
                                           OperateTime = DateTime.Now
                                       }).ToArray());
                }
                UnitWork.Save();
            });
        }

        /// <summary>
        /// 为角色分配菜单，需要统一提交，会删除以前该角色的所有菜单
        /// </summary>
        /// <param name="requests"></param>
        public void AssignRoleMenus(List<AssignRoleMenusReq> requests)
        {
            if (requests == null || requests.Count() == 0)
            {
                return;
            }
            List<Auth_RelevanceInfo> addDatas = new List<Auth_RelevanceInfo>();
            var operatorId = _auth.GetCurrentUser().User.Id;

            foreach (var request in requests)
            {
                if (!request.MenuIds.Any())
                {
                    continue;
                }
                if (request.MenuIds != null && request.MenuIds.Length > 0)
                {
                    //批量分配角色菜单
                    addDatas.AddRange((from thirdId in request.MenuIds
                                       select new Auth_RelevanceInfo
                                       {
                                           Key = Define.ROLEELEMENT,
                                           FirstId = request.RoleId,
                                           SecondId = request.ModuleId,
                                           ThirdId = thirdId,
                                           OperatorId = operatorId,
                                           OperateTime = DateTime.Now
                                       }).ToArray());
                }
            }

            UnitWork.ExecuteWithTransaction(() =>
            {
                var roleIds = requests.Select(o => o.RoleId).Distinct().ToArray();
                //删除以前的所有菜单
                UnitWork.Delete<Auth_RelevanceInfo>(u => roleIds.Contains(u.FirstId) && u.Key == Define.ROLEELEMENT);

                //批量分配角色菜单
                if (addDatas.Count > 0)
                {
                    UnitWork.BatchAdd(addDatas.ToArray());
                }
                UnitWork.Save();
            });
        }

        /// <summary>
        /// 为部门分配用户，需要统一提交，会删除以前该部门的所有用户
        /// </summary>
        /// <param name="request"></param>
        public void AssignOrgUsers(AssignOrgUsersReq request)
        {
            UnitWork.ExecuteWithTransaction(() =>
            {
                //删除以前的所有用户
                UnitWork.Delete<Auth_RelevanceInfo>(u => u.SecondId == request.OrgId && u.Key == Define.USERORG);
                if (request.UserIds != null && request.UserIds.Length > 0)
                {
                    //批量分配用户角色
                    UnitWork.BatchAdd((from firstId in request.UserIds
                                       select new Auth_RelevanceInfo
                                       {
                                           Key = Define.USERORG,
                                           FirstId = firstId,
                                           SecondId = request.OrgId,
                                           OperateTime = DateTime.Now
                                       }).ToArray());
                    UnitWork.Save();
                }
            });
        }
    }
}
