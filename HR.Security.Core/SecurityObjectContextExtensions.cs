using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace HR.Security.Core
{
    public partial class SecurityObjectContext
    {
        public override int SaveChanges()
        {
            OnBeforeSave();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            OnBeforeSave();

            return base.SaveChangesAsync();
        }

        private void OnBeforeSave()
        {
            var addedAuditedEntities = ChangeTracker.Entries<EntityBase>().Where(x => x.State == EntityState.Added).Select(x => x.Entity);
            var modifiedAuditedEntities = ChangeTracker.Entries<EntityBase>().Where(x => x.State == EntityState.Modified).Select(x => x.Entity);

            foreach (var addedEntity in addedAuditedEntities)
            {
                addedEntity.CreatedBy = CurrentThreadIdentityUserId;
                addedEntity.CreatedDate = DateTime.Now;
                addedEntity.LastModifiedBy = CurrentThreadIdentityUserId;
                addedEntity.LastModifiedDate = DateTime.Now;
            }

            foreach (var modifiedEntity in modifiedAuditedEntities)
            {
                //exclude field
                Entry(modifiedEntity).Property(x => x.CreatedBy).IsModified = false;
                Entry(modifiedEntity).Property(x => x.CreatedDate).IsModified = false;

                modifiedEntity.LastModifiedBy = CurrentThreadIdentityUserId;
                modifiedEntity.LastModifiedDate = DateTime.Now;
            }
        }

        private Guid CurrentThreadIdentityUserId
        {
            get
            {
                Guid userId = Guid.Empty;

                if(System.Threading.Thread.CurrentPrincipal.Identity is ClaimsIdentity)
                {
                    var claimsIdentity = System.Threading.Thread.CurrentPrincipal.Identity as ClaimsIdentity;

                    var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                    if(userIdClaim != null)
                    {
                        Guid.TryParse(userIdClaim.Value, out userId);
                    }
                }

                return userId;
            }
        }
    }
}
