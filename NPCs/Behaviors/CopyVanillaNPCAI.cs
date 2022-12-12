using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria.ModLoader;

namespace XLibrary.NPCs.Behaviors
{
	public class CopyNPCVanillaAI : ModNPCBehavior<ModNPC>
	{
		public bool netUpdate;
		public override bool NetUpdate => netUpdate;
		public const int aiSize = 4;
		public const int localAISize = 4;
		public readonly int type;
		public readonly int aiStyle;
		public override string BehaviorName => $"CopyNPCVanillaAI:{type} {aiStyle}";
		public IRefValue<float>[] ai;
		public IRefValue<float>[] localAI;
		public CopyNPCVanillaAI(ModNPC modNPC, int type, int aiStyle, IRefValue<float> ai0 = null, IRefValue<float> ai1 = null, IRefValue<float> ai2 = null, IRefValue<float> ai3 = null, IRefValue<float> lovalAI0 = null, IRefValue<float> lovalAI1 = null, IRefValue<float> lovalAI2 = null, IRefValue<float> lovalAI3 = null) : base(modNPC)
		{
			this.type = type;
			this.aiStyle = aiStyle;
			this.ai = new IRefValue<float>[] { ai0, ai1, ai2, ai3 };
			this.localAI = new IRefValue<float>[] { lovalAI0, lovalAI1, lovalAI2, lovalAI3 };
		}
		public CopyNPCVanillaAI(ModNPC modNPC, int type, int aiStyle, IRefValue<float>[] ai = null, IRefValue<float>[] localAI = null) : base(modNPC)
		{
			this.type = type;
			this.aiStyle = aiStyle;
			this.ai = ai;
			this.localAI = localAI;
		}
		public override void AI()
		{

			int rtype = NPC.type;
			int raiStyle = NPC.aiStyle;
			bool rUpdate = NPC.netUpdate;
			NPC.netUpdate = netUpdate;
			NPC.type = type;
			NPC.aiStyle = aiStyle;
			float[] NPCai = new float[aiSize];
			float[] NPClocalAI = new float[localAISize];
			if (ai != null)
			{
				for (int i = 0; i < aiSize; ++i)
				{
					if (ai[i] != null)
					{
						NPCai[i] = NPC.ai[i];
						NPC.ai[i] = ai[i].Value;
					}
				}
			}
			if (localAI != null)
			{
				for (int i = 0; i < localAISize; ++i)
				{
					if (localAI[i] != null)
					{
						NPClocalAI[i] = NPC.localAI[i];
						NPC.localAI[i] = localAI[i].Value;
					}
				}
			}
			NPC.VanillaAI();
			netUpdate = NPC.netUpdate;
			NPC.type = rtype;
			NPC.aiStyle = raiStyle;
			NPC.netUpdate = rUpdate | netUpdate;
			if (ai != null)
			{
				for (int i = 0; i < aiSize; ++i)
				{
					if (ai[i] != null)
					{
						ai[i].Value = NPC.ai[i];
						NPC.ai[i] = NPCai[i];
					}
				}
			}
			if (localAI != null)
			{
				for (int i = 0; i < localAISize; ++i)
				{
					if (localAI[i] != null)
					{
						localAI[i].Value = NPC.localAI[i];
						NPC.localAI[i] = NPClocalAI[i];
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
