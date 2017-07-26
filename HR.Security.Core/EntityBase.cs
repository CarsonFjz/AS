using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Security.Core
{
    public abstract partial class EntityBase
    {
        public EntityBase()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntityBase);
        }

        private static bool IsTransient(EntityBase obj)
        {
            return obj != null && Equals(obj.ID, default(Guid));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public virtual bool Equals(EntityBase other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(ID, other.ID))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Equals(ID, default(int)))
                return base.GetHashCode();
            return ID.GetHashCode();
        }

        public static bool operator ==(EntityBase x, EntityBase y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(EntityBase x, EntityBase y)
        {
            return !(x == y);
        }
    }
}
