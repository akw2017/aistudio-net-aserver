﻿using Coldairarrow.Entity;
using Coldairarrow.Entity.Base_Manage;
using Coldairarrow.IBusiness;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.Base_Manage
{
    public interface IBase_RoleBusiness : IBaseBusiness<Base_Role>
    {
        Task<PageResult<Base_RoleInfoDTO>> GetDataListAsync(PageInput<RolesInputDTO> input);
        Task<Base_RoleInfoDTO> GetTheDataAsync(string id);
        Task AddDataAsync(Base_RoleInfoDTO input);
        Task UpdateDataAsync(Base_RoleInfoDTO input);
        Task DeleteDataAsync(List<string> ids);
    }

    public class RolesInputDTO
    {
        public string roleId { get; set; }
        public string roleName { get; set; }
    }

    [Map(typeof(Base_Role))]
    public class Base_RoleInfoDTO : Base_Role
    {
        public RoleTypes? RoleType { get { try { return RoleName?.ToEnum<RoleTypes>(); } catch { return null; } } }
        public List<string> Actions { get; set; } = new List<string>();
    }
}