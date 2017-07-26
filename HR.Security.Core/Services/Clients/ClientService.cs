using HR.Security.Core.Data;
using HR.Security.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.Security.Core.Services.Clients
{
    public partial class ClientService : IClientService
    {
        private readonly SecurityObjectContext _context;

        public ClientService(SecurityObjectContext context)
        {
            this._context = context;
        }

        public async Task<Client> GetByIdAsync(object clientId)
        {
            return await _context.Set<Client>().FindAsync(clientId);            
        }
    }
}
