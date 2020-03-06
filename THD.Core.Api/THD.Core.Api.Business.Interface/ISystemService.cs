using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Business.Interface
{
    public interface ISystemService
    {
        Task<bool> LicenseKeyValidate(string LicenseKey);

        Task<ModelResponseMessageLogin> LogInAsync(ModelUserLogin model);

        Task<ModelResponseMessageLogin> LogOutAsync(ModelUserLogin model);
    }
}
