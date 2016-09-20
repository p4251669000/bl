/*********************************************************************************** 
*        Cteate by :   xierui
*        Date      :   2016/9/20 10:45:53 
*        Desc      :   EF context
************************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using XR.BL.Core;
using XR.BL.Data.Mapping;

namespace XR.BL.Data
{
    public class XRObjectContext : DbContext, IDbContext
    {
        public XRObjectContext()
            : base("Entity")
        {
        }

        #region 公用

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //自由加载所有的类
            //System.Type configType = typeof(LanguageMap);   
            //var typesToRegister = Assembly.GetAssembly(configType).GetTypes()

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !String.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(XREntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            // 手动加载
            //modelBuilder.Configurations.Add(new LanguageMap());



            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 在上下文中附加一个实体或返回一个已经连接的实体
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>附加的实体</returns>
        protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            //在这里，直到实体框架真正支持存储过程 / 小程序
            //否则，直到实体连接到上下文,加载实体的导航属性不会被加载，
            var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
            if (alreadyAttached == null)
            {
                //附加新的实体
                Set<TEntity>().Attach(entity);
                return entity;
            }

            //实体已经被加载
            return alreadyAttached;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 创建一个数据脚本
        /// </summary>
        /// <returns>SQL生成数据库</returns>
        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        /// <summary>
        /// 得到 DbSet
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>DbSet</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// 执行存储过程并加载结束时的实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="commandText">sql文本</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体</returns>
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            //添加参数到 command
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("不支持参数类型");

                    commandText += i == 0 ? " " : ", ";

                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //输出参数
                        commandText += " 输出";
                    }
                }
            }

            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();

            //性能破解应用这里所描述的
            bool acd = this.Configuration.AutoDetectChangesEnabled;
            try
            {
                this.Configuration.AutoDetectChangesEnabled = false;

                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);
            }
            finally
            {
                this.Configuration.AutoDetectChangesEnabled = acd;
            }

            return result;
        }

        /// <summary>
        /// 创建一个原始的SQL查询将返回给定的泛型类型的元素。类型可以是任何类型，该类型可以具有与从查询返回的列的名称相匹配的属性，也可以是一个简单的原始类型。类型不必是实体类型。这个查询的结果永远不会被上下文跟踪，即使返回的对象的类型是实体类型。
        /// </summary>
        /// <typeparam name="TElement">一个实体query.</typeparam>
        /// <param name="sql">sql query 句子.</param>
        /// <param name="parameters">应用到sql的参数.</param>
        /// <returns>结果</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        /// <summary>
        /// 执行给定的DDL DML命令对数据库.
        /// </summary>
        /// <param name="sql">sql 内容</param>
        /// <param name="doNotEnsureTransaction">false -事务没有保证; true - 事务有保证.</param>
        /// <param name="timeout">超时值，以秒为单位</param>
        /// <param name="parameters">sql参数.</param>
        /// <returns>返回的结果.</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //超时前
                previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
            }

            var transactionalBehavior = doNotEnsureTransaction
                ? TransactionalBehavior.DoNotEnsureTransaction
                : TransactionalBehavior.EnsureTransaction;
            var result = this.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);

            if (timeout.HasValue)
            {
                //返回超时状态
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
            }

            //返回结果
            return result;
        }

        /// <summary>
        /// 附加一个实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("实体");

            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        #endregion

        #region 性能

        /// <summary>
        /// 获取或设置一个值，指示是否启用了代理创建设置（用于EF）
        /// </summary>
        public virtual bool ProxyCreationEnabled
        {
            get
            {
                return this.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Configuration.ProxyCreationEnabled = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示是否启用自动检测更改设置（用于EF）
        /// </summary>
        public virtual bool AutoDetectChangesEnabled
        {
            get
            {
                return this.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                this.Configuration.AutoDetectChangesEnabled = value;
            }
        }

        #endregion
    }
}
