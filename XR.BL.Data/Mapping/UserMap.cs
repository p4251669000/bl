/*********************************************************************************** 
*        Cteate by :   xierui
*        Date      :   2016/9/20 13:47:22 
*        Desc      :   映射User 表
************************************************************************************/
using XR.BL.Core.Doamin;

namespace XR.BL.Data.Mapping
{
    public partial class UserMap : XREntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.ToTable("User");
            this.HasKey(bp => bp.Id);
        }
    }
}

