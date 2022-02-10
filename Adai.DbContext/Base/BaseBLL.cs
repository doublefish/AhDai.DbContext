using System.Collections.Generic;

namespace Adai.DbContext
{
	/// <summary>
	/// BaseBLL
	/// </summary>
	/// <typeparam name="Model"></typeparam>
	/// <typeparam name="DAL"></typeparam>
	public abstract class BaseBLL<Model, DAL>
		where Model : BaseModel, new()
		where DAL : BaseDAL<Model>
	{
		DAL dal;

		/// <summary>
		/// Dal
		/// </summary>
		protected DAL Dal
		{
			get
			{
				if (dal == null)
				{
					dal = InitDal();
				}
				return dal;
			}
			set
			{
				dal = value;
			}
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseBLL()
		{

		}

		/// <summary>
		/// 初始化Dal
		/// </summary>
		/// <returns></returns>
		protected abstract DAL InitDal();

		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual Model GetById(string id)
		{
			return Dal.GetById(id);
		}

		/// <summary>
		/// 根据Id查询
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public virtual ICollection<Model> ListByIds(params string[] ids)
		{
			return Dal.ListByIds(ids);
		}

		/// <summary>
		/// 查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filter"></param>
		/// <returns></returns>
		public virtual ICollection<Model> List<T>(T filter) where T : IFilter<Model>
		{
			return Dal.List(filter);
		}

		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public virtual int Add(Model data)
		{
			return Dal.Add(data);
		}

		/// <summary>
		/// 修改
		/// </summary>
		/// <param name="data"></param>
		/// <param name="updateColumns"></param>
		/// <param name="whereColumns"></param>
		/// <returns></returns>
		public virtual int Update(Model data, string[] updateColumns, params string[] whereColumns)
		{
			return Dal.Update(data, updateColumns, whereColumns);
		}
	}
}
