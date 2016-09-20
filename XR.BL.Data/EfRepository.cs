/*********************************************************************************** 
*        Cteate by :   xierui
*        Date      :   2016/9/20 11:12:37 
*        Desc      :   实现数据仓库接口
************************************************************************************/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using XR.BL.Core;
using XR.BL.Core.Data;

namespace XR.BL.Data
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        #region 对象

        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        #endregion

        #region Ctor

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="context">对象 context</param>
        public EfRepository(IDbContext context)
        {
            this._context = context;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// 得到所有的错误
        /// </summary>
        /// <param name="exc">例如</param>
        /// <returns>错误</returns>
        protected string GetFullErrorText(DbEntityValidationException exc)
        {
            var msg = string.Empty;
            foreach (var validationErrors in exc.EntityValidationErrors)
                foreach (var error in validationErrors.ValidationErrors)
                    msg += string.Format("属性: {0} 错误: {1}", error.PropertyName, error.ErrorMessage) + Environment.NewLine;
            return msg;
        }

        #endregion

        #region 方法

        /// <summary>
        ///通过主键Id得到实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public virtual T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        /// <summary>
        /// 插入一个实体
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("实体");

                this.Entities.Add(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        /// 插入实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("实体集合");

                foreach (var entity in entities)
                    this.Entities.Add(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        /// 更新一个实体
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        /// 更新一个实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("实体集合");

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity">实体</param>
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("实体");

                this.Entities.Remove(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        /// 删除一个实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("实体集合");

                foreach (var entity in entities)
                    this.Entities.Remove(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        #endregion

        #region 属性

        /// <summary>
        /// 得到一个表
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        /// <summary>
        ///只读时使用
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return this.Entities.AsNoTracking();
            }
        }

        /// <summary>
        /// 实体
        /// </summary>
        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        #endregion
    }
}
