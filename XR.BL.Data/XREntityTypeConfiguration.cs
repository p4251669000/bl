/*********************************************************************************** 
*        Cteate by :   xierui
*        Date      :   2016/9/20 10:53:07 
*        Desc      :   初始化映射时的一些操作
************************************************************************************/
using System.Data.Entity.ModelConfiguration;

namespace XR.BL.Data.Mapping
{
    public abstract class XREntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : class
    {
        protected XREntityTypeConfiguration()
        {
            PostInitialize();
        }

        /// <summary>
        /// 开发人员可以重写此方法在自定义的部分类中
        /// 为构造函数添加一些自定义初始化代码
        /// </summary>
        protected virtual void PostInitialize()
        {

        }
    }
}
