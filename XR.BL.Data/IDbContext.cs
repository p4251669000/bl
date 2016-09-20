/*********************************************************************************** 
*        Cteate by :   xierui
*        Date      :   2016/9/20 10:30:45 
*        Desc      :   EF 上下文实例接口
************************************************************************************/
using System.Collections.Generic;
using System.Data.Entity;
using XR.BL.Core;

namespace XR.BL.Data
{
    public interface IDbContext
    {
        /// <summary>
        /// 得到Dbset
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>DbSet</returns>
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// 保存操作
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// 执行存储过程并加载结束时的实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="commandText">sql 内容</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体</returns>
        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new();

        /// <summary>
        /// 创建一个原始的SQL查询将返回给定的泛型类型的元素。类型可以是任何类型，该类型可以具有与从查询返回的列的名称相匹配的属性，也可以是一个简单的原始类型。类型不必是实体类型。这个查询的结果永远不会被上下文跟踪，即使返回的对象的类型是实体类型。
        /// </summary>
        /// <typeparam name="TElement">返回的类型的对象的查询</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">参数.</param>
        /// <returns>结果</returns>
        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);

        /// <summary>
        /// 执行给定的DDL DML命令对数据库.
        /// </summary>
        /// <param name="sql">sql 内容</param>
        /// <param name="doNotEnsureTransaction">false -事务没有保证; true - 事务有保证.</param>
        /// <param name="timeout">超时值，以秒为单位</param>
        /// <param name="parameters">sql参数.</param>
        /// <returns>返回的结果.</returns>
        int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters);

        /// <summary>
        /// 分派一个实体
        /// </summary>
        /// <param name="entity">实体</param>
        void Detach(object entity);

        /// <summary>
        /// 设置一个值，指示否用了代理(使用EF)
        /// </summary>
        bool ProxyCreationEnabled { get; set; }

        /// <summary>
        /// 设置一个值 指示是否启用自动检测更改设置 (使用EF)
        /// </summary>
        bool AutoDetectChangesEnabled { get; set; }
    }
}
