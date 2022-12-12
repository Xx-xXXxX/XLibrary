using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XLibrary;

namespace XLibrary
{
	/// <summary>
	/// 用于管理检查id
	/// O(log n) 添加，移除，检查
	/// O(n) 遍历
	/// </summary>
	public class IDManager:IEnumerable<int>
	{
		private readonly SetEx<int> IDSection=new();
		public IDManager() {
		}
		/// <summary>
		/// 改变该id的使用状态
		/// O(log n)
		/// </summary>
		public void ChangeUsing(int id) {
			if (!IDSection.Remove(id)) IDSection.Add(id);
			if (!IDSection.Remove(id+1)) IDSection.Add(id+1);
		}
		/// <summary>
		/// 添加id使用
		/// 实为ChangeUsing
		/// 不检查
		/// O(log n)
		/// </summary>
		public void Add(int id) => ChangeUsing(id);
		/// <summary>
		/// 移除id使用
		/// 实为ChangeUsing
		/// 不检查
		/// O(log n)
		/// </summary>
		public void Remove(int id) => ChangeUsing(id);
		/// <summary>
		/// 获取下一个可用的ID
		/// O(1)
		/// </summary>
		public int NextID() {
			return IDSection.First.Item;
		}
		/// <summary>
		/// 获取比所有已用ID都大的第一个可用的ID
		/// O(1)
		/// </summary>
		public int LastID()
		{
			return IDSection.Last.Item;
		}
		/// <summary>
		/// 枚举所有正在使用的id
		/// O(n)
		/// </summary>
		public IEnumerator<int> GetEnumerator()
		{
			int now=0;
			bool Having=false;
			foreach (var i in IDSection) {
				if (Having)
				{
					while (now < i)
					{
						yield return now;
						now += 1;
					}
					Having = false;
				}
				else {
					now = i; Having = true;
				}
			}
		}
		/// <summary>
		/// 检查该ID是否在使用
		/// O(log n)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool Check(int id) {
			if (id < IDSection.First.Item) return false;
			var i = IDSection.FindNotGreater(id);
			return IDSection.GetIndex(i.Item) % 2 == 0;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		/// <summary>
		/// 重置
		/// </summary>
		public void Clear() {
			IDSection.Clear();
		}

		/// <summary>
		/// 返回对该id的下一个可用ID(如果id在原范围内)
		/// O(log n)
		/// </summary>
		public int NextID(int id)
		{
			if (id >= IDSection.Last.Item) return id+1;
			var i = IDSection.FindGreater(id+1);
			if (IDSection.GetIndex(i.Item) % 2 == 1) return i.Item;
			else return id + 1;
		}
		/// <summary>
		/// 所有使用的ID
		/// O(n)
		/// </summary>\
		public List<int> ToArray() {
			List<int> vs = new();
			foreach (var i in this) vs.Add(i);
			return vs;
		}
	}
}
