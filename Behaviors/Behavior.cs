using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLibrary.Behaviors
{
	/// <summary>
	/// 行为，用于自动机
	/// </summary>
	public interface IBehavior
	{
		/// <summary>
		/// 是否正在进行，初始值应为false暂停
		/// 如果CanPause(),Active应在Pause()后set为false，Activate同理
		/// </summary>
		bool Active { get; set; }
		/// <summary>
		/// 它是否能暂停,true会
		/// 考虑Active
		/// </summary>
		bool CanPause();
		/// <summary>
		/// 进行暂停时
		/// 设置Active=false
		/// </summary>
		void OnPause();
		/// <summary>
		/// 它能否激活,true会
		/// 考虑!Active
		/// </summary>
		bool CanActivate();
		/// <summary>
		/// 进行激活时
		/// 设置Active=true
		/// </summary>
		void OnActivate();
		/// <summary>
		/// 开始(注册)时
		/// </summary>
		void Initialize();
		/// <summary>
		/// 终止时
		/// 因为某些原因，不保证执行
		/// </summary>
		void Dispose();
		/// <summary>
		/// 用于联机同步发送，如果需要完整同步(Server存在需要同步世界的客户端)会全部使用，否则在NetUpdate时使用
		/// </summary>
		/// <param name="writer"></param>
		void NetUpdateSend(BinaryWriter writer);
		/// <summary>
		/// 用于联机同步接收，如果需要完整同步(Server存在需要同步世界的客户端)会全部使用，否则在NetUpdate时使用
		/// </summary>
		/// <param name="reader"></param>
		void NetUpdateReceive(BinaryReader reader);
		/// <summary>
		/// 用于通用的数据传输
		/// </summary>
		object Call(params object[] vs);
		/// <summary>
		/// 行为的名称
		/// </summary>
		string BehaviorName { get; }
		/// <summary>
		/// 是否进行同步
		/// </summary>
		bool NetUpdate { get; }
	}
	public abstract class Behavior : IBehavior
	{
		protected bool active = false;
		public bool Active { get => active; set => active = value; }

		public abstract string BehaviorName { get; }

		public abstract bool NetUpdate { get; }

		public virtual object Call(params object[] vs)
		{
			return null;
		}

		public virtual bool CanActivate()
		{
			return !Active;
		}

		public virtual bool CanPause()
		{
			return Active;
		}

		public virtual void Dispose()
		{

		}

		public virtual void Initialize()
		{

		}

		public virtual void NetUpdateReceive(BinaryReader reader)
		{

		}

		public virtual void NetUpdateSend(BinaryWriter writer)
		{

		}

		public virtual void OnActivate()
		{
			Active = true;
		}

		public virtual void OnPause()
		{
			Active = false;
		}

	}
	public static class BehaviorUtils {

		public static bool TrySetActive(this IBehavior behavior,bool active)
		{
			if (behavior.Active == active) return false;
			else if (behavior.Active && !active)
			{
				if (!behavior.CanPause()) return false;
				behavior.OnPause();
				behavior.Active = false;
				return true;
			}
			else {
				if (!behavior.CanActivate()) return false;
				behavior.OnActivate();
				behavior.Active = true;
				return true;
			}
		}
	}
}
