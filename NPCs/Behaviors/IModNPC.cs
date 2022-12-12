
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ReLogic.Utilities;

using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria;
using Terraria.ModLoader;

namespace XLibrary.NPCs.Behaviors
{
	public interface IModNPC
	{
		void AI();
		void BossLoot(ref string name, ref int potionType);
		bool? CanBeCaughtBy(Item item, Player player);
		bool? CanBeHitByItem(Player player, Item item);
		bool? CanBeHitByProjectile(Projectile projectile);
		bool? CanFallThroughPlatforms();
		bool CanGoToStatue(bool toKingStatue);
		bool? CanHitNPC(NPC target);
		bool CanHitPlayer(Player target, ref int cooldownSlot);
		bool CanTownNPCSpawn(int numTownNPCs, int money);
		bool CheckActive();
		bool CheckConditions(int left, int right, int top, int bottom);
		bool CheckDead();
		void DrawBehind(int index);
		void DrawEffects(ref Color drawColor);
		bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position);
		void DrawTownAttackGun(ref float scale, ref int item, ref int closeness);
		void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset);
		void FindFrame(int frameHeight);
		Color? GetAlpha(Color drawColor);
		void HitEffect(int hitDirection, double damage);
		void LoadData(TagCompound tag);
		void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit);
		void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection);
		void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit);
		void ModifyHitPlayer(Player target, ref int damage, ref bool crit);
		void ModifyNPCLoot(NPCLoot npcLoot);
		bool NeedSaving();
		void OnCaughtBy(Player player, Item item, bool failed);
		void OnGoToStatue(bool toKingStatue);
		void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit);
		void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit);
		void OnHitNPC(NPC target, int damage, float knockback, bool crit);
		void OnHitPlayer(Player target, int damage, bool crit);
		void OnKill();
		void OnSpawn(IEntitySource source);
		void PostAI();
		void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
		bool PreAI();
		bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
		bool PreKill();
		void ReceiveExtraAI(BinaryReader reader);
		void ResetEffects();
		void SaveData(TagCompound tag);
		void ScaleExpertStats(int numPlayers, float bossLifeScale);
		void SendExtraAI(BinaryWriter writer);
		bool SpecialOnKill();
		bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit);
		void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown);
		void TownNPCAttackMagic(ref float auraLightMultiplier);
		void TownNPCAttackProj(ref int projType, ref int attackDelay);
		void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset);
		void TownNPCAttackShoot(ref bool inBetweenShots);
		void TownNPCAttackStrength(ref int damage, ref float knockback);
		void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight);
		void UpdateLifeRegen(ref int damage);
		bool UsesPartyHat();
	}
	//
	// 摘要:
	//     This class serves as a place for you to place all your properties and hooks for
	//     each NPC. Create instances of ModNPC (preferably overriding this class) to pass
	//     as parameters to Mod.AddNPC.
	internal abstract class _ModNPC : IModNPC
	{
		public NPC NPC { get; set; }
		public ModNPC ModNPC { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


		//
		// 摘要:
		//     Gets called when your NPC spawns in world
		public virtual void OnSpawn(IEntitySource source)
		{
		}

		//
		// 摘要:
		//     Allows you to customize this NPC's stats in expert mode. This is useful because
		//     expert mode's doubling of damage and life might be too much sometimes (for example,
		//     with bosses). Also useful for scaling life with the number of players in the
		//     world.
		//
		// 参数:
		//   numPlayers:
		//
		//   bossLifeScale:
		public virtual void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
		}

		//
		// 摘要:
		//     This is where you reset any fields you add to your subclass to their default
		//     states. This is necessary in order to reset your fields if they are conditionally
		//     set by a tick update but the condition is no longer satisfied. (Note: This hook
		//     is only really useful for GlobalNPC, but is included in ModNPC for completion.)
		public virtual void ResetEffects()
		{
		}

		//
		// 摘要:
		//     Allows you to determine how this NPC behaves. Return false to stop the vanilla
		//     AI and the AI hook from being run. Returns true by default.
		public virtual bool PreAI()
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to determine how this NPC behaves. This will only be called if PreAI
		//     returns true.
		public virtual void AI()
		{
		}

		public virtual void PostAI()
		{
		}

		//
		// 摘要:
		//     If you are storing AI information outside of the NPC.ai array, use this to send
		//     that AI information between clients and servers, which will be handled in Terraria.ModLoader.ModNPC.ReceiveExtraAI(System.IO.BinaryReader).
		//     Called whenever Terraria.ID.MessageID.SyncNPC is successfully sent, for example
		//     on NPC creation, on player join, or whenever NPC.netUpdate is set to true in
		//     the update loop for that tick.
		//     Only called on the server.
		//
		// 参数:
		//   writer:
		//     The writer.
		public virtual void SendExtraAI(BinaryWriter writer)
		{
		}

		//
		// 摘要:
		//     Use this to receive information that was sent in Terraria.ModLoader.ModNPC.SendExtraAI(System.IO.BinaryWriter).
		//     Called whenever Terraria.ID.MessageID.SyncNPC is successfully received.
		//     Only called on the client.
		//
		// 参数:
		//   reader:
		//     The reader.
		public virtual void ReceiveExtraAI(BinaryReader reader)
		{
		}

		//
		// 摘要:
		//     Allows you to modify the frame from this NPC's texture that is drawn, which is
		//     necessary in order to animate NPCs.
		//
		// 参数:
		//   frameHeight:
		public virtual void FindFrame(int frameHeight)
		{
		}

		//
		// 摘要:
		//     Allows you to make things happen whenever this NPC is hit, such as creating dust
		//     or gores.
		//     This hook is client side. Usually when something happens when an NPC dies such
		//     as item spawning, you use NPCLoot, but you can use HitEffect paired with a check
		//     for `if (NPC.life <= 0)` to do client-side death effects, such as spawning dust,
		//     gore, or death sounds.
		public virtual void HitEffect(int hitDirection, double damage)
		{
		}

		//
		// 摘要:
		//     Allows you to make the NPC either regenerate health or take damage over time
		//     by setting NPC.lifeRegen. Regeneration or damage will occur at a rate of half
		//     of NPC.lifeRegen per second. The damage parameter is the number that appears
		//     above the NPC's head if it takes damage over time.
		//
		// 参数:
		//   damage:
		public virtual void UpdateLifeRegen(ref int damage)
		{
		}

		//
		// 摘要:
		//     Whether or not to run the code for checking whether this NPC will remain active.
		//     Return false to stop this NPC from being despawned and to stop this NPC from
		//     counting towards the limit for how many NPCs can exist near a player. Returns
		//     true by default.
		public virtual bool CheckActive()
		{
			return true;
		}

		//
		// 摘要:
		//     Whether or not this NPC should be killed when it reaches 0 health. You may program
		//     extra effects in this hook (for example, how Golem's head lifts up for the second
		//     phase of its fight). Return false to stop this NPC from being killed. Returns
		//     true by default.
		public virtual bool CheckDead()
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to call OnKill on your own when the NPC dies, rather then letting
		//     vanilla call it on its own. Returns false by default.
		//
		// 返回结果:
		//     Return true to stop vanilla from calling OnKill on its own. Do this if you call
		//     OnKill yourself.
		public virtual bool SpecialOnKill()
		{
			return false;
		}

		//
		// 摘要:
		//     Allows you to determine whether or not this NPC will do anything upon death (besides
		//     dying). Returns true by default.
		public virtual bool PreKill()
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to make things happen when this NPC dies (for example, dropping items
		//     and setting ModSystem fields). This hook runs on the server/single player. For
		//     client-side effects, such as dust, gore, and sounds, see HitEffect
		public virtual void OnKill()
		{
		}

		//
		// 摘要:
		//     Allows you to determine how and when this NPC can fall through platforms and
		//     similar tiles.
		//     Return true to allow this NPC to fall through platforms, false to prevent it.
		//     Returns null by default, applying vanilla behaviors (based on aiStyle and type).
		public virtual bool? CanFallThroughPlatforms()
		{
			return null;
		}

		//
		// 摘要:
		//     Allows you to determine whether the given item can catch this NPC.
		//     Return true or false to say this NPC can or cannot be caught, respectively, regardless
		//     of vanilla rules.
		//     Returns null by default, which allows vanilla's NPC catching rules to decide
		//     the target's fate.
		//     If this returns false, Terraria.ModLoader.CombinedHooks.OnCatchNPC(Terraria.Player,Terraria.NPC,Terraria.Item,System.Boolean)
		//     is never called.
		//     NOTE: this does not classify the given item as an NPC-catching tool, which is
		//     necessary for catching NPCs in the first place.
		//     To do that, you will need to use the "CatchingTool" set in ItemID.Sets.
		//
		// 参数:
		//   item:
		//     The item with which the player is trying to catch this NPC.
		//
		//   player:
		//     The player attempting to catch this NPC.
		public virtual bool? CanBeCaughtBy(Item item, Player player)
		{
			return null;
		}

		//
		// 摘要:
		//     Allows you to make things happen when the given item attempts to catch this NPC.
		//
		// 参数:
		//   player:
		//     The player attempting to catch this NPC.
		//
		//   item:
		//     The item used to catch this NPC.
		//
		//   failed:
		//     Whether or not this NPC has been successfully caught.
		public virtual void OnCaughtBy(Player player, Item item, bool failed)
		{
		}

		//
		// 摘要:
		//     Allows you to add and modify NPC loot tables to drop on death and to appear in
		//     the Bestiary.
		//     The Basic NPC Drops and Loot 1.4 Guide explains how to use this hook to modify
		//     NPC loot.
		//
		// 参数:
		//   npcLoot:
		public virtual void ModifyNPCLoot(NPCLoot npcLoot)
		{
		}

		//
		// 摘要:
		//     Allows you to customize what happens when this boss dies, such as which name
		//     is displayed in the defeat message and what type of potion it drops.
		//
		// 参数:
		//   name:
		//
		//   potionType:
		public virtual void BossLoot(ref string name, ref int potionType)
		{
		}

		//
		// 摘要:
		//     Allows you to determine whether this NPC can hit the given player. Return false
		//     to block this NPC from hitting the target. Returns true by default. CooldownSlot
		//     determines which of the player's cooldown counters to use (-1, 0, or 1), and
		//     defaults to -1.
		//
		// 参数:
		//   target:
		//
		//   cooldownSlot:
		public virtual bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to modify the damage, etc., that this NPC does to a player.
		//
		// 参数:
		//   target:
		//
		//   damage:
		//
		//   crit:
		public virtual void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to create special effects when this NPC hits a player (for example,
		//     inflicting debuffs).
		//
		// 参数:
		//   target:
		//
		//   damage:
		//
		//   crit:
		public virtual void OnHitPlayer(Player target, int damage, bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to determine whether this NPC can hit the given friendly NPC. Return
		//     true to allow hitting the target, return false to block this NPC from hitting
		//     the target, and return null to use the vanilla code for whether the target can
		//     be hit. Returns null by default.
		//
		// 参数:
		//   target:
		public virtual bool? CanHitNPC(NPC target)
		{
			return null;
		}

		//
		// 摘要:
		//     Allows you to modify the damage, knockback, etc., that this NPC does to a friendly
		//     NPC.
		//
		// 参数:
		//   target:
		//
		//   damage:
		//
		//   knockback:
		//
		//   crit:
		public virtual void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to create special effects when this NPC hits a friendly NPC.
		//
		// 参数:
		//   target:
		//
		//   damage:
		//
		//   knockback:
		//
		//   crit:
		public virtual void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to determine whether this NPC can be hit by the given melee weapon
		//     when swung. Return true to allow hitting the NPC, return false to block hitting
		//     the NPC, and return null to use the vanilla code for whether the NPC can be hit.
		//     Returns null by default.
		//
		// 参数:
		//   player:
		//
		//   item:
		public virtual bool? CanBeHitByItem(Player player, Item item)
		{
			return null;
		}

		//
		// 摘要:
		//     Allows you to modify the damage, knockback, etc., that this NPC takes from a
		//     melee weapon.
		//
		// 参数:
		//   player:
		//
		//   item:
		//
		//   damage:
		//
		//   knockback:
		//
		//   crit:
		public virtual void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to create special effects when this NPC is hit by a melee weapon.
		//
		// 参数:
		//   player:
		//
		//   item:
		//
		//   damage:
		//
		//   knockback:
		//
		//   crit:
		public virtual void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to determine whether this NPC can be hit by the given projectile.
		//     Return true to allow hitting the NPC, return false to block hitting the NPC,
		//     and return null to use the vanilla code for whether the NPC can be hit. Returns
		//     null by default.
		//
		// 参数:
		//   projectile:
		public virtual bool? CanBeHitByProjectile(Projectile projectile)
		{
			return null;
		}

		//
		// 摘要:
		//     Allows you to modify the damage, knockback, etc., that this NPC takes from a
		//     projectile. This method is only called for the owner of the projectile, meaning
		//     that in multi-player, projectiles owned by a player call this method on that
		//     client, and projectiles owned by the server such as enemy projectiles call this
		//     method on the server.
		//
		// 参数:
		//   projectile:
		//
		//   damage:
		//
		//   knockback:
		//
		//   crit:
		//
		//   hitDirection:
		public virtual void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
		}

		//
		// 摘要:
		//     Allows you to create special effects when this NPC is hit by a projectile.
		//
		// 参数:
		//   projectile:
		//
		//   damage:
		//
		//   knockback:
		//
		//   crit:
		public virtual void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to use a custom damage formula for when this NPC takes damage from
		//     any source. For example, you can change the way defense works or use a different
		//     crit multiplier. Return false to stop the game from running the vanilla damage
		//     formula; returns true by default.
		//
		// 参数:
		//   damage:
		//
		//   defense:
		//
		//   knockback:
		//
		//   hitDirection:
		//
		//   crit:
		public virtual bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			return true;
		}



		//
		// 摘要:
		//     Allows you to determine the color and transparency in which this NPC is drawn.
		//     Return null to use the default color (normally light and buff color). Returns
		//     null by default.
		//
		// 参数:
		//   drawColor:
		public virtual Color? GetAlpha(Color drawColor)
		{
			return null;
		}

		//
		// 摘要:
		//     Allows you to add special visual effects to this NPC (such as creating dust),
		//     and modify the color in which the NPC is drawn.
		//
		// 参数:
		//   drawColor:
		public virtual void DrawEffects(ref Color drawColor)
		{
		}

		//
		// 摘要:
		//     Allows you to draw things behind this NPC, or to modify the way this NPC is drawn.
		//     Substract screenPos from the draw position before drawing. Return false to stop
		//     the game from drawing the NPC (useful if you're manually drawing the NPC). Returns
		//     true by default.
		//
		// 参数:
		//   spriteBatch:
		//     The spritebatch to draw on
		//
		//   screenPos:
		//     The screen position used to translate world position into screen position
		//
		//   drawColor:
		//     The color the NPC is drawn in
		public virtual bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to draw things in front of this NPC. Substract screenPos from the
		//     draw position before drawing. This method is called even if PreDraw returns false.
		//
		// 参数:
		//   spriteBatch:
		//     The spritebatch to draw on
		//
		//   screenPos:
		//     The screen position used to translate world position into screen position
		//
		//   drawColor:
		//     The color the NPC is drawn in
		public virtual void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
		}

		//
		// 摘要:
		//     When used in conjunction with "NPC.hide = true", allows you to specify that this
		//     NPC should be drawn behind certain elements. Add the index to one of Main.DrawCacheNPCsMoonMoon,
		//     DrawCacheNPCsOverPlayers, DrawCacheNPCProjectiles, or DrawCacheNPCsBehindNonSolidTiles.
		//
		// 参数:
		//   index:
		public virtual void DrawBehind(int index)
		{
		}

		//
		// 摘要:
		//     Allows you to control how the health bar for this NPC is drawn. The hbPosition
		//     parameter is the same as Main.hbPosition; it determines whether the health bar
		//     gets drawn above or below the NPC by default. The scale parameter is the health
		//     bar's size. By default, it will be the normal 1f; most bosses set this to 1.5f.
		//     Return null to let the normal vanilla health-bar-drawing code to run. Return
		//     false to stop the health bar from being drawn. Return true to draw the health
		//     bar in the position specified by the position parameter (note that this is the
		//     world position, not screen position).
		//
		// 参数:
		//   hbPosition:
		//
		//   scale:
		//
		//   position:
		public virtual bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return null;
		}



		//
		// 摘要:
		//     Whether or not the conditions have been met for this town NPC to be able to move
		//     into town. For example, the Demolitionist requires that any player has an explosive.
		//
		// 参数:
		//   numTownNPCs:
		//
		//   money:
		public virtual bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			return false;
		}

		//
		// 摘要:
		//     Allows you to define special conditions required for this town NPC's house. For
		//     example, Truffle requires the house to be in an aboveground mushroom biome.
		//
		// 参数:
		//   left:
		//
		//   right:
		//
		//   top:
		//
		//   bottom:
		public virtual bool CheckConditions(int left, int right, int top, int bottom)
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to determine whether this town NPC wears a party hat during a party.
		//     Returns true by default.
		public virtual bool UsesPartyHat()
		{
			return true;
		}


		//
		// 摘要:
		//     Whether this NPC can be telported to a King or Queen statue. Returns false by
		//     default.
		//
		// 参数:
		//   toKingStatue:
		//     Whether the NPC is being teleported to a King or Queen statue.
		public virtual bool CanGoToStatue(bool toKingStatue)
		{
			return false;
		}

		//
		// 摘要:
		//     Allows you to make things happen when this NPC teleports to a King or Queen statue.
		//     This method is only called server side.
		//
		// 参数:
		//   toKingStatue:
		//     Whether the NPC was teleported to a King or Queen statue.
		public virtual void OnGoToStatue(bool toKingStatue)
		{
		}

		//
		// 摘要:
		//     Allows you to determine the damage and knockback of this town NPC's attack before
		//     the damage is scaled. (More information on scaling in GlobalNPC.BuffTownNPCs.)
		//
		// 参数:
		//   damage:
		//
		//   knockback:
		public virtual void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
		}

		//
		// 摘要:
		//     Allows you to determine the cooldown between each of this town NPC's attack.
		//     The cooldown will be a number greater than or equal to the first parameter, and
		//     less then the sum of the two parameters.
		//
		// 参数:
		//   cooldown:
		//
		//   randExtraCooldown:
		public virtual void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
		}

		//
		// 摘要:
		//     Allows you to determine the projectile type of this town NPC's attack, and how
		//     long it takes for the projectile to actually appear. This hook is only used when
		//     the town NPC has an attack type of 0 (throwing), 1 (shooting), or 2 (magic).
		//
		// 参数:
		//   projType:
		//
		//   attackDelay:
		public virtual void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
		}

		//
		// 摘要:
		//     Allows you to determine the speed at which this town NPC throws a projectile
		//     when it attacks. Multiplier is the speed of the projectile, gravityCorrection
		//     is how much extra the projectile gets thrown upwards, and randomOffset allows
		//     you to randomize the projectile's velocity in a square centered around the original
		//     velocity. This hook is only used when the town NPC has an attack type of 0 (throwing),
		//     1 (shooting), or 2 (magic).
		//
		// 参数:
		//   multiplier:
		//
		//   gravityCorrection:
		//
		//   randomOffset:
		public virtual void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
		}

		//
		// 摘要:
		//     Allows you to tell the game that this town NPC has already created a projectile
		//     and will still create more projectiles as part of a single attack so that the
		//     game can animate the NPC's attack properly. Only used when the town NPC has an
		//     attack type of 1 (shooting).
		//
		// 参数:
		//   inBetweenShots:
		public virtual void TownNPCAttackShoot(ref bool inBetweenShots)
		{
		}

		//
		// 摘要:
		//     Allows you to control the brightness of the light emitted by this town NPC's
		//     aura when it performs a magic attack. Only used when the town NPC has an attack
		//     type of 2 (magic)
		//
		// 参数:
		//   auraLightMultiplier:
		public virtual void TownNPCAttackMagic(ref float auraLightMultiplier)
		{
		}

		//
		// 摘要:
		//     Allows you to determine the width and height of the item this town NPC swings
		//     when it attacks, which controls the range of this NPC's swung weapon. Only used
		//     when the town NPC has an attack type of 3 (swinging).
		//
		// 参数:
		//   itemWidth:
		//
		//   itemHeight:
		public virtual void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
		{
		}

		//
		// 摘要:
		//     Allows you to customize how this town NPC's weapon is drawn when this NPC is
		//     shooting (this NPC must have an attack type of 1). Scale is a multiplier for
		//     the item's drawing size, item is the ID of the item to be drawn, and closeness
		//     is how close the item should be drawn to the NPC.
		//
		// 参数:
		//   scale:
		//
		//   item:
		//
		//   closeness:
		public virtual void DrawTownAttackGun(ref float scale, ref int item, ref int closeness)
		{
		}

		//
		// 摘要:
		//     Allows you to customize how this town NPC's weapon is drawn when this NPC is
		//     swinging it (this NPC must have an attack type of 3). Item is the Texture2D instance
		//     of the item to be drawn (use Main.itemTexture[id of item]), itemSize is the width
		//     and height of the item's hitbox (the same values for TownNPCAttackSwing), scale
		//     is the multiplier for the item's drawing size, and offset is the offset from
		//     which to draw the item from its normal position.
		//
		// 参数:
		//   item:
		//
		//   itemSize:
		//
		//   scale:
		//
		//   offset:
		public virtual void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
		{
		}

		//
		// 摘要:
		//     Makes this ModNPC save along the world even if it's not a townNPC. Defaults to
		//     false.
		//     NOTE: A town NPC will always be saved.
		//     NOTE: A NPC that needs saving will not despawn naturally.
		public virtual bool NeedSaving()
		{
			return false;
		}

		//
		// 摘要:
		//     Allows you to save custom data for the given item. Allows you to save custom
		//     data for the given npc.
		//     NOTE: The provided tag is always empty by default, and is provided as an argument
		//     only for the sake of convenience and optimization.
		//     NOTE: Try to only save data that isn't default values.
		//     NOTE: The npc may be saved even if NeedSaving returns false and this is not a
		//     townNPC, if another mod returns true on NeedSaving.
		//
		// 参数:
		//   tag:
		//     The TagCompound to save data into. Note that this is always empty by default,
		//     and is provided as an argument
		public virtual void SaveData(TagCompound tag)
		{
		}

		//
		// 摘要:
		//     Allows you to load custom data that you have saved for this npc.
		//
		// 参数:
		//   tag:
		//     The tag.
		public virtual void LoadData(TagCompound tag)
		{
		}
	}
}