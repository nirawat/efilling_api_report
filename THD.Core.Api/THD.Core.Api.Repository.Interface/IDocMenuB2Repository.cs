using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMenuB2Repository
    {
        Task<ModelMenuB2_InterfaceData> MenuB2InterfaceDataAsync(string RegisterId);
        Task<IList<ModelSelectOption>> GetAllLabNumberAsync();
        Task<ModelMenuB2Data> GetLabNumberWithDataB2Async(string lab_number);
        Task<ModelResponseMessageAddDocB2> AddDocMenuB2Async(ModelMenuB2 model);
    }
}
