using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuD2Repository
    {
        Task<ModelMenuD2_InterfaceData> MenuD2InterfaceDataAsync(string RegisterId);
        Task<ModelMenuD2ProjectNumberData> GetProjectNumberWithDataD2Async(string project_number);
        Task<ModelResponseD2Message> AddDocMenuD2Async(ModelMenuD2 model);
    }
}
