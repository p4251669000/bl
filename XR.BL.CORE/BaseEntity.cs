/*********************************************************************************** 
*        Cteate by :   xierui
*        Date      :   2016/9/20 9:56:08 
*        Desc      :   公有Basic
************************************************************************************/
using System;


namespace XR.BL.Core
{
    public class BaseEntity
    {
        /// <summary>
        /// 公共主键  
        /// </summary>
        public int Id { get; set; } 

        public override bool Equals(object obj)  
        {
            return Equals(obj as BaseEntity);
        }

        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && Equals(obj.Id, default(int)); 
        }

        private Type GetUnproxiedType()
        {
            return GetType();   
        }

        public virtual bool Equals(BaseEntity other)     
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))   
                return true;

            if (!IsTransient(this) && 
                !IsTransient(other) &&
                Equals(Id, other.Id))
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
            if (Equals(Id, default(int)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }
    }
}
