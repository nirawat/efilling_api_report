using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IRegisterUserRepository
    {
        Task<ModelRegisterActive_InterfaceData> ActiveUserAccountInterfaceAsync(string RegisterId);
        Task<ModelResponseMessageRegisterUser> AddRegisterUserAsync(EntityRegisterUser entity_model);
        Task<ModelResponseMessageRegisterActive> AddRegisterActiveAsync(EntityRegisterUser entity_model);
        Task<EntityRegisterUser> GetRegisterUserActiveAsync(string RegisterId);
        Task<ModelRegisterActive> GetFullRegisterUserByIdAsync(string RegisterId);
        Task<EntityRegisterUser> GetRegisterUserInActiveAsync(string RegisterId);
        Task<ModelPermissionPage> GetPermissionPageAsync(string RegisterId, string PageCode);
        Task<ModelResponseMessageUpdateUserRegister> ResetPasswordAsync(ModelResetPassword model);
    }
}
