using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace XLibrary.NPCs.Behaviors
{
	public abstract class ModNPCWithBehavior<TModNPC>:ModNPC
		where TModNPC:ModNPC
	{
		public IModNPCBehavior behavior;

		public override void AI()
		{
			behavior.AI();
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			behavior.BossLoot(ref name, ref potionType);
		}

		public override bool? CanBeCaughtBy(Item item, Player player)
		{
			return behavior.CanBeCaughtBy(item, player);
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return behavior.CanBeHitByItem(player, item);
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			return behavior.CanBeHitByProjectile(projectile);
		}

		public override bool? CanFallThroughPlatforms()
		{
			return behavior.CanFallThroughPlatforms();
		}

		public override bool CanGoToStatue(bool toKingStatue)
		{
			return behavior.CanGoToStatue(toKingStatue);
		}

		public override bool? CanHitNPC(NPC target)
		{
			return behavior.CanHitNPC(target);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return behavior.CanHitPlayer(target, ref cooldownSlot);
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			return behavior.CanTownNPCSpawn(numTownNPCs, money);
		}

		public override bool CheckActive()
		{
			return behavior.CheckActive();
		}

		public override bool CheckConditions(int left, int right, int top, int bottom)
		{
			return behavior.CheckConditions(left, right, top, bottom);
		}

		public override bool CheckDead()
		{
			return behavior.CheckDead();
		}

		public override void DrawBehind(int index)
		{
			behavior.DrawBehind(index);
		}

		public override void DrawEffects(ref Color drawColor)
		{
			behavior.DrawEffects(ref drawColor);
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return behavior.DrawHealthBar(hbPosition, ref scale, ref position);
		}

		public override void DrawTownAttackGun(ref float scale, ref int item, ref int closeness)
		{
			behavior.DrawTownAttackGun(ref scale, ref item, ref closeness);
		}

		public override void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
		{
			behavior.DrawTownAttackSwing(ref item, ref itemSize, ref scale, ref offset);
		}

		public override void FindFrame(int frameHeight)
		{
			behavior.FindFrame(frameHeight);
		}

		public override Color? GetAlpha(Color drawColor)
		{
			return behavior.GetAlpha(drawColor);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			behavior.HitEffect(hitDirection, damage);
		}

		public override void LoadData(TagCompound tag)
		{
			behavior.LoadData(tag);
		}

		public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
		{
			behavior.ModifyHitByItem(player, item, ref damage, ref knockback, ref crit);
		}

		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			behavior.ModifyHitByProjectile(projectile, ref damage, ref knockback, ref crit, ref hitDirection);
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			behavior.ModifyHitNPC(target, ref damage, ref knockback, ref crit);
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			behavior.ModifyHitPlayer(target, ref damage, ref crit);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			behavior.ModifyNPCLoot(npcLoot);
		}

		public override bool NeedSaving()
		{
			return behavior.NeedSaving();
		}

		public override void OnCaughtBy(Player player, Item item, bool failed)
		{
			behavior.OnCaughtBy(player, item, failed);
		}

		public override void OnGoToStatue(bool toKingStatue)
		{
			behavior.OnGoToStatue(toKingStatue);
		}

		public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
		{
			behavior.OnHitByItem(player, item, damage, knockback, crit);
		}

		public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
		{
			behavior.OnHitByProjectile(projectile, damage, knockback, crit);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			behavior.OnHitNPC(target, damage, knockback, crit);
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			behavior.OnHitPlayer(target, damage, crit);
		}

		public override void OnKill()
		{
			behavior.OnKill();
		}

		public override void OnSpawn(IEntitySource source)
		{
			behavior.OnSpawn(source);
		}

		public override void PostAI()
		{
			behavior.PostAI();
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			behavior.PostDraw(spriteBatch, screenPos, drawColor);
		}

		public override bool PreAI()
		{
			return behavior.PreAI();
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			return behavior.PreDraw(spriteBatch, screenPos, drawColor);
		}

		public override bool PreKill()
		{
			return behavior.PreKill();
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			behavior.ReceiveExtraAI(reader);
		}

		public override void ResetEffects()
		{
			behavior.ResetEffects();
		}

		public override void SaveData(TagCompound tag)
		{
			behavior.SaveData(tag);
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			behavior.ScaleExpertStats(numPlayers, bossLifeScale);
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			behavior.SendExtraAI(writer);
		}

		public override bool SpecialOnKill()
		{
			return behavior.SpecialOnKill();
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			return behavior.StrikeNPC(ref damage, defense, ref knockback, hitDirection, ref crit);
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			behavior.TownNPCAttackCooldown(ref cooldown, ref randExtraCooldown);
		}

		public override void TownNPCAttackMagic(ref float auraLightMultiplier)
		{
			behavior.TownNPCAttackMagic(ref auraLightMultiplier);
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			behavior.TownNPCAttackProj(ref projType, ref attackDelay);
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			behavior.TownNPCAttackProjSpeed(ref multiplier, ref gravityCorrection, ref randomOffset);
		}

		public override void TownNPCAttackShoot(ref bool inBetweenShots)
		{
			behavior.TownNPCAttackShoot(ref inBetweenShots);
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			behavior.TownNPCAttackStrength(ref damage, ref knockback);
		}

		public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
		{
			behavior.TownNPCAttackSwing(ref itemWidth, ref itemHeight);
		}

		public override void UpdateLifeRegen(ref int damage)
		{
			behavior.UpdateLifeRegen(ref damage);
		}

		public override bool UsesPartyHat()
		{
			return behavior.UsesPartyHat();
		}
	}
}
