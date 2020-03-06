using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THD.Core.Api.Entities.Tables;
using System.Text;
using THD.Core.Api.Models;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDropdownListRepository
    {
        Task<IList<ModelSelectOption>> GetAllRegisterUserByCharacterAsync(string character);

        //Task<IList<ModelSelectOption>> GetAllProjectNumberWithNameAsync(string ProjectNumber);
    }
}
