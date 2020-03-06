using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuA2Repository
    {
        Task<ModelMenuA2_InterfaceData> MenuA2InterfaceDataAsync(string RegisterId);

        Task<ModelResponseA2Message> AddDocMenuA2Async(ModelMenuA2 model);
    }
}
