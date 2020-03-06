using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using THD.Core.Api.Models;
using THD.Core.Api.Entities.Tables;

namespace THD.Core.Api.Repository.Interface
{
    public interface ISystemRepository
    {
        Task<bool> LicenseKeyValidate(string LicenseKey);

        Task<ModelResponseMessageLogin> LogIn(EntityLogSystem entity_model);

        Task<ModelResponseMessageLogin> LogOut(EntityLogSystem entity_model);
    }
}
