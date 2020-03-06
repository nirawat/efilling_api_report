using System;
using System.Collections.Generic;
using THD.Core.Api.Business.Interface;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using THD.Core.Api.Repository.Interface;
using System.Text;
using THD.Core.Api.Models;
using THD.Core.Api.Helpers;

namespace THD.Core.Api.Business
{
    public class SystemService : ISystemService
    {
        private readonly ISystemRepository _ISystemRepository;

        public SystemService(
            ISystemRepository SystemRepository)
        {
            _ISystemRepository = SystemRepository;
        }

        public async Task<bool> LicenseKeyValidate(string LicenseKey)
        {
            return await _ISystemRepository.LicenseKeyValidate(LicenseKey);
        }

        public async Task<ModelResponseMessageLogin> LogInAsync(ModelUserLogin model)
        {
            ModelResponseMessageLogin resp = new ModelResponseMessageLogin();

            if (string.IsNullOrWhiteSpace(model.userid)) resp.Message = "UserId is require.";
            if (string.IsNullOrWhiteSpace(model.passw)) resp.Message = "Password is require.";

            EntityLogSystem entity_model = new EntityLogSystem();

            entity_model.userid = model.userid;
            entity_model.passw = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.passw));
            entity_model.log_event = "Login to system.";
            entity_model.log_date = DateTime.Now;

            resp = await _ISystemRepository.LogIn(entity_model);

            if (!string.IsNullOrEmpty(resp.Data.RegisterId))
            {
                Guid guid = Guid.NewGuid();
                resp.Data.Guid = guid.ToString();
                resp.Data.Token = String.Empty;
                resp.Data.RegisterId = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(resp.Data.RegisterId));
            }

            return resp;

        }
        public async Task<ModelResponseMessageLogin> LogOutAsync(ModelUserLogin model)
        {
            ModelResponseMessageLogin resp = new ModelResponseMessageLogin();

            if (string.IsNullOrWhiteSpace(model.userid)) resp.Message = "UserId is require.";
            if (string.IsNullOrWhiteSpace(model.passw)) resp.Message = "Password is require.";

            EntityLogSystem entity_model = new EntityLogSystem();

            entity_model.register_id = Encoding.UTF8.GetString(Convert.FromBase64String(model.registerid));
            entity_model.userid = model.userid;
            entity_model.log_event = "Logout to system.";
            entity_model.log_date = DateTime.Now;

            resp = await _ISystemRepository.LogIn(entity_model);

            return resp;

        }

    }
}
