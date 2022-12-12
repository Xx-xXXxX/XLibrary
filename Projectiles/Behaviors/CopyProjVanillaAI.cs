using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace XLibrary.Projectiles.Behaviors
{
	/// <summary>
	/// 复制VanillaAI
	/// 用 CopyProjVanillaAI.ai 和 CopyProjVanillaAI.localAI（Update时替换与还原Projectile.ai ...）
	/// </summary>
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
	public class CopyProjVanillaAI : ModProjBehavior<ModProjectile>
	{
		//	int type = npc.type;
		//	bool num = npc.modNPC != null && npc.modNPC.aiType > 0;
		//			if (num)
		//			{
		//				npc.type = npc.modNPC.aiType;
		//			}
		//	npc.VanillaAI();
		//			if (num)
		//			{
		//				npc.type = type;
		//			}
		public const int aiSize = 2;
		public const int localAISize = 2;
		public bool netUpdate;
		public override bool NetUpdate => netUpdate;
		public readonly int type;
		public readonly int aiStyle;
		public override string BehaviorName =>$"CopyNPCVanillaAI:{type}";
		public IRefValue<float>[] ai;
		public IRefValue<float>[] localAI;
		public CopyProjVanillaAI(ModProjectile modProjectile, int type,int aiStyle, IRefValue<float> ai0 = null, IRefValue<float> ai1=null,IRefValue<float> lovalAI0 = null, IRefValue<float> lovalAI1 = null) : base(modProjectile) {
			this.type = type;
			this.aiStyle = aiStyle;
			this.ai = new IRefValue<float>[]{ ai0, ai1};
			this.localAI = new IRefValue<float>[] { lovalAI0, lovalAI1};
		}
		public CopyProjVanillaAI(ModProjectile modProjectile, int type,int aiStyle, IRefValue<float>[] ai = null,IRefValue<float>[] localAI=null) : base(modProjectile)
		{
			this.type = type;
			this.aiStyle = aiStyle;
			this.ai = ai;
			this.localAI = localAI;
		}
		public override void AI()
		{
			int rtype = Projectile.type;
			int raiStyle = Projectile.aiStyle;
			bool rnetUpdate = Projectile.netUpdate;
			Projectile.type = type;
			Projectile.aiStyle = aiStyle;
			Projectile.netUpdate = netUpdate;
			float[] npcai = new float[aiSize];
			float[] npclocalAI = new float[localAISize];
			if (ai != null) {
				for (int i = 0; i < aiSize; ++i) {
					if (ai[i] != null) {
						npcai[i] = Projectile.ai[i];
						Projectile.ai[i] = ai[i].Value;
					}
				}
			}
			if (localAI != null)
			{
				for (int i = 0; i < localAISize; ++i)
				{
					if (localAI[i] != null)
					{
						npclocalAI[i] = Projectile.localAI[i];
						Projectile.localAI[i] = localAI[i].Value;
					}
				}
			}
			Projectile.VanillaAI();
			Projectile.type = rtype;
			Projectile.aiStyle = raiStyle;
			netUpdate = Projectile.netUpdate;
			Projectile.netUpdate = rnetUpdate | netUpdate;
			if (ai != null)
			{
				for (int i = 0; i < aiSize; ++i)
				{
					if (ai[i] != null)
					{
						ai[i].Value= Projectile.ai[i];
						Projectile.ai[i]= npcai[i];
					}
				}
			}
			if (localAI != null)
			{
				for (int i = 0; i < localAISize; ++i)
				{
					if (localAI[i] != null)
					{
						localAI[i].Value= Projectile.localAI[i];
						Projectile.localAI[i]= npclocalAI[i];
					}
				}
			}
		}
		public override void NetUpdateSend(BinaryWriter writer)
		{
			for (int i = 0; i < aiSize; ++i) writer.Write(ai[i].Value);
		}
		public override void NetUpdateReceive(BinaryReader reader)
		{
			for (int i = 0; i < aiSize; ++i) ai[i].Value = reader.ReadSingle();
		}
	}
}
