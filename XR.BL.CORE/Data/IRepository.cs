/*********************************************************************************** 
*        Cteate by :   xierui
*        Date      :   2016/9/20 9:55:05 
*        Desc      :   数据仓库接口
************************************************************************************/
using System.Collections.Generic;
using System.Linq;

namespace XR.BL.Core.Data
{
    public partial interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// 得到相关主键Id产品
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns>Entity</returns>
        T GetById(object id);

        /// <summary>
        /// 插入一个实体
        /// </summary>
        /// <param name="entity">实体</param>
        void Insert(T entity);

        /// <summary>
        /// 插入一个集合
        /// </summary>
        /// <param name="entities">集合</param>
        void Insert(IEnumerable<T> entities);

        /// <summary>
        /// 更新一个实体
        /// </summary>
        /// <param name="entity">更新一个集合</param>
        void Update(T entity);

        /// <summary>
        /// 更新一个实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        /// 删一个实体
        /// </summary>
        /// <param name="entity">实体</param>
        void Delete(T entity);

        /// <summary>
        /// 删除一个实体集合
        /// </summary>
        /// <param name="entities">Entities</param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// 得到一个表的所有数据
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// 只读时使用
        /// </summary>
        IQueryable<T> TableNoTracking { get; }
    }
}
