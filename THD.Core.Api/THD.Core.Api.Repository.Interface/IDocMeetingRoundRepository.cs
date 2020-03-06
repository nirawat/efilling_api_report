using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using THD.Core.Api.Models;
using THD.Core.Api.Entities.Tables;

namespace THD.Core.Api.Repository.Interface
{
    public interface IDocMeetingRoundRepository
    {
        Task<ModelCountOfYear> GetMeetingRoundOfProjectAsync(int year);
    }
}
