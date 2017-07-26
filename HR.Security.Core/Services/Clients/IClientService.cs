using HR.Security.Core.Domain;
using System;
using System.Threading.Tasks;

namespace HR.Security.Core.Services.Clients
{
    public partial interface IClientService
    {
        Task<Client> GetByIdAsync(object clientId);
    }
}
