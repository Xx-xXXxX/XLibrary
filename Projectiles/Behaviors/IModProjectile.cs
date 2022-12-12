using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using XLibrary.Behaviors;

namespace XLibrary.Projectiles.Behaviors
{
	public interface IModProj
	{
		void AI();
		bool? CanCutTiles();
		bool? CanDamage();
		bool? CanHitNPC(NPC target);
		bool CanHitPlayer(Player target);
		bool CanHitPvp(Player target);
		bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox);
		void CutTiles();
		void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI);
		Color? GetAlpha(Color lightColor);
		void GrapplePullSpeed(Player player, ref float speed);
		float GrappleRange();
		void GrappleRetreatSpeed(Player player, ref float speed);
		void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY);
		void Kill(int timeLeft);
		bool MinionContactDamage();
		void ModifyDamageHitbox(ref Rectangle hitbox);
		void ModifyDamageScaling(ref float damageScale);
		void ModifyFishingLine(ref Vector2 lineOriginOffset, ref Color lineColor);
		void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection);
		void ModifyHitPlayer(Player target, ref int damage, ref bool crit);
		void ModifyHitPvp(Player target, ref int damage, ref bool crit);
		void OnHitNPC(NPC target, int damage, float knockback, bool crit);
		void OnHitPlayer(Player target, int damage, bool crit);
		void OnHitPvp(Player target, int damage, bool crit);
		void OnSpawn(IEntitySource source);
		bool OnTileCollide(Vector2 oldVelocity);
		void PostAI();
		void PostDraw(Color lightColor);
		bool PreAI();
		bool PreDraw(ref Color lightColor);
		bool PreDrawExtras();
		bool PreKill(int timeLeft);
		void ReceiveExtraAI(BinaryReader reader);
		void SendExtraAI(BinaryWriter writer);
		bool ShouldUpdatePosition();
		bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac);
	}
	public interface IModProjBehavior : IModProj, IBehavior { }

	//
	// 摘要:
	//     This class serves as a place for you to place all your properties and hooks for
	//     each projectile. Create instances of ModProjectile (preferably overriding this
	//     class) to pass as parameters to Mod.AddProjectile.
	//     The Basic Projectile Guide teaches the basics of making a modded projectile.
	public abstract class ModProjectile_ :  IModProj
	{


		//
		// 摘要:
		//     Gets called when your projectiles spawns in world
		public virtual void OnSpawn(IEntitySource source)
		{
		}


		//
		// 摘要:
		//     Allows you to determine how this projectile behaves. Return false to stop the
		//     vanilla AI and the AI hook from being run. Returns true by default.
		//
		// 返回结果:
		//     Whether or not to stop other AI.
		public virtual bool PreAI()
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to determine how this projectile behaves. This will only be called
		//     if PreAI returns true.
		public virtual void AI()
		{
		}

		//
		// 摘要:
		//     Allows you to determine how this projectile behaves. This will be called regardless
		//     of what PreAI returns.
		public virtual void PostAI()
		{
		}

		//
		// 摘要:
		//     If you are storing AI information outside of the Projectile.ai array, use this
		//     to send that AI information between clients and servers, which will be handled
		//     in Terraria.ModLoader.ModProjectile.ReceiveExtraAI(System.IO.BinaryReader).
		//     Called whenever Terraria.ID.MessageID.SyncProjectile is successfully sent, for
		//     example on projectile creation, or whenever Projectile.netUpdate is set to true
		//     in the update loop for that tick.
		//     Can be called on both server and client, depending on who owns the projectile.
		//
		// 参数:
		//   writer:
		//     The writer.
		public virtual void SendExtraAI(BinaryWriter writer)
		{
		}

		//
		// 摘要:
		//     Use this to receive information that was sent in Terraria.ModLoader.ModProjectile.SendExtraAI(System.IO.BinaryWriter).
		//     Called whenever Terraria.ID.MessageID.SyncProjectile is successfully received.
		//     Can be called on both server and client, depending on who owns the projectile.
		//
		// 参数:
		//   reader:
		//     The reader.
		public virtual void ReceiveExtraAI(BinaryReader reader)
		{
		}

		//
		// 摘要:
		//     Whether or not this projectile should update its position based on factors such
		//     as its velocity, whether it is in liquid, etc. Return false to make its velocity
		//     have no effect on its position. Returns true by default.
		public virtual bool ShouldUpdatePosition()
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to determine how this projectile interacts with tiles. Return false
		//     if you completely override or cancel this projectile's tile collision behavior.
		//     Returns true by default.
		//
		// 参数:
		//   width:
		//     The width of the hitbox this projectile will use for tile collision. If vanilla
		//     doesn't modify it, defaults to Projectile.width.
		//
		//   height:
		//     The height of the hitbox this projectile will use for tile collision. If vanilla
		//     doesn't modify it, defaults to Projectile.height.
		//
		//   fallThrough:
		//     Whether or not this projectile falls through platforms and similar tiles.
		//
		//   hitboxCenterFrac:
		//     Determines by how much the tile collision hitbox's position (top left corner)
		//     will be offset from this projectile's real center. If vanilla doesn't modify
		//     it, defaults to half the hitbox size (new Vector2(0.5f, 0.5f)).
		public virtual bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to determine what happens when this projectile collides with a tile.
		//     OldVelocity is the velocity before tile collision. The velocity that takes tile
		//     collision into account can be found with Projectile.velocity. Return true to
		//     allow the vanilla tile collision code to take place (which normally kills the
		//     projectile). Returns true by default.
		//
		// 参数:
		//   oldVelocity:
		//     The velocity of the projectile upon collision.
		public virtual bool OnTileCollide(Vector2 oldVelocity)
		{
			return true;
		}

		//
		// 摘要:
		//     Return true or false to specify if the projectile can cut tiles like vines, pots,
		//     and Queen Bee larva. Return null for vanilla decision.
		public virtual bool? CanCutTiles()
		{
			return null;
		}

		//
		// 摘要:
		//     Code ran when the projectile cuts tiles. Only runs if CanCutTiles() returns true.
		//     Useful when programming lasers and such.
		public virtual void CutTiles()
		{
		}

		//
		// 摘要:
		//     Allows you to determine whether the vanilla code for Kill and the Kill hook will
		//     be called. Return false to stop them from being called. Returns true by default.
		//     Note that this does not stop the projectile from dying.
		public virtual bool PreKill(int timeLeft)
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to control what happens when this projectile is killed (for example,
		//     creating dust or making sounds). Also useful for creating retrievable ammo. Called
		//     on all clients and the server in multiplayer, so be sure to use `if (Projectile.owner
		//     == Main.myPlayer)` if you are spawning retrievable ammo. (As seen in ExampleJavelinProjectile)
		public virtual void Kill(int timeLeft)
		{
		}

		//
		// 摘要:
		//     Whether or not this projectile is capable of killing tiles (such as grass) and
		//     damaging NPCs/players. Return false to prevent it from doing any sort of damage.
		//     Return true if you want the projectile to do damage regardless of the default
		//     blacklist. Return null to let the projectile follow vanilla can-damage-anything
		//     rules. This is what happens by default.
		public virtual bool? CanDamage()
		{
			return null;
		}

		//
		// 摘要:
		//     Whether or not this minion can damage NPCs by touching them. Returns false by
		//     default. Note that this will only be used if this projectile is considered a
		//     pet.
		public virtual bool MinionContactDamage()
		{
			return false;
		}

		//
		// 摘要:
		//     Allows you to change the hitbox used by this projectile for damaging players
		//     and NPCs.
		//
		// 参数:
		//   hitbox:
		public virtual void ModifyDamageHitbox(ref Rectangle hitbox)
		{
		}

		//
		// 摘要:
		//     Allows you to implement dynamic damage scaling for this projectile. For example,
		//     flails do more damage when in flight and Jousting Lance does more damage the
		//     faster the player is moving. This hook runs on the owner only.
		//
		// 参数:
		//   damageScale:
		//     The damage scaling
		public virtual void ModifyDamageScaling(ref float damageScale)
		{
		}

		//
		// 摘要:
		//     Allows you to determine whether this projectile can hit the given NPC. Return
		//     true to allow hitting the target, return false to block this projectile from
		//     hitting the target, and return null to use the vanilla code for whether the target
		//     can be hit. Returns null by default.
		//
		// 参数:
		//   target:
		//     The target.
		public virtual bool? CanHitNPC(NPC target)
		{
			return null;
		}

		//
		// 摘要:
		//     Allows you to modify the damage, knockback, etc., that this projectile does to
		//     an NPC. This method is only called for the owner of the projectile, meaning that
		//     in multi-player, projectiles owned by a player call this method on that client,
		//     and projectiles owned by the server such as enemy projectiles call this method
		//     on the server.
		//
		// 参数:
		//   target:
		//     The target.
		//
		//   damage:
		//     The modifiable damage.
		//
		//   knockback:
		//     The modifiable knockback.
		//
		//   crit:
		//     The modifiable crit.
		//
		//   hitDirection:
		//     The modifiable hit direction.
		public virtual void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
		}

		//
		// 摘要:
		//     Allows you to create special effects when this projectile hits an NPC (for example,
		//     inflicting debuffs). This method is only called for the owner of the projectile,
		//     meaning that in multi-player, projectiles owned by a player call this method
		//     on that client, and projectiles owned by the server such as enemy projectiles
		//     call this method on the server.
		//
		// 参数:
		//   target:
		//     The target.
		//
		//   damage:
		//     The damage.
		//
		//   knockback:
		//     The knockback.
		//
		//   crit:
		//     The critical hit.
		public virtual void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to determine whether this projectile can hit the given opponent player.
		//     Return false to block this projectile from hitting the target. Returns true by
		//     default.
		//
		// 参数:
		//   target:
		//     The target
		public virtual bool CanHitPvp(Player target)
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to modify the damage, etc., that this projectile does to an opponent
		//     player.
		//
		// 参数:
		//   target:
		//     The target.
		//
		//   damage:
		//     The modifiable damage.
		//
		//   crit:
		//     The modifiable crit.
		public virtual void ModifyHitPvp(Player target, ref int damage, ref bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to create special effects when this projectile hits an opponent player.
		//
		// 参数:
		//   target:
		//     The target.
		//
		//   damage:
		//     The damage.
		//
		//   crit:
		//     The critical hit.
		public virtual void OnHitPvp(Player target, int damage, bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to determine whether this hostile projectile can hit the given player.
		//     Return false to block this projectile from hitting the target. Returns true by
		//     default.
		//
		// 参数:
		//   target:
		//     The target.
		public virtual bool CanHitPlayer(Player target)
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to modify the damage, etc., that this hostile projectile does to a
		//     player.
		//
		// 参数:
		//   target:
		//     The target.
		//
		//   damage:
		//     The modifiable damage.
		//
		//   crit:
		//     The modifiable crit.
		public virtual void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to create special effects when this hostile projectile hits a player.
		//
		// 参数:
		//   target:
		//     The target.
		//
		//   damage:
		//     The damage.
		//
		//   crit:
		//     The critical hit.
		public virtual void OnHitPlayer(Player target, int damage, bool crit)
		{
		}

		//
		// 摘要:
		//     Allows you to use custom collision detection between this projectile and a player
		//     or NPC that this projectile can damage. Useful for things like diagonal lasers,
		//     projectiles that leave a trail behind them, etc.
		//
		// 参数:
		//   projHitbox:
		//     The hitbox of the projectile.
		//
		//   targetHitbox:
		//     The hitbox of the target.
		//
		// 返回结果:
		//     Whether they collide or not.
		public virtual bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return null;
		}

		//
		// 摘要:
		//     If this projectile is a bobber, allows you to modify the origin of the fisihing
		//     line that's connecting to the fishing pole, as well as the fishing line's color.
		//
		// 参数:
		//   lineOriginOffset:
		//     The offset of the fishing line's origin from the player's center.
		//
		//   lineColor:
		//     The fishing line's color, before being overridden by string color accessories.
		public virtual void ModifyFishingLine(ref Vector2 lineOriginOffset, ref Color lineColor)
		{
		}

		//
		// 摘要:
		//     Allows you to determine the color and transparency in which this projectile is
		//     drawn. Return null to use the default color (normally light and buff color).
		//     Returns null by default.
		public virtual Color? GetAlpha(Color lightColor)
		{
			return null;
		}

		//
		// 摘要:
		//     Allows you to draw things behind this projectile. Use the Main.EntitySpriteDraw
		//     method for drawing. Returns false to stop the game from drawing extras textures
		//     related to the projectile (for example, the chains for grappling hooks), useful
		//     if you're manually drawing the extras. Returns true by default.
		public virtual bool PreDrawExtras()
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to draw things behind this projectile, or to modify the way it is
		//     drawn. Use the Main.EntitySpriteDraw method for drawing. Return false to stop
		//     the vanilla projectile drawing code (useful if you're manually drawing the projectile).
		//     Returns true by default.
		//
		// 参数:
		//   lightColor:
		//     The color of the light at the projectile's center.
		public virtual bool PreDraw(ref Color lightColor)
		{
			return true;
		}

		//
		// 摘要:
		//     Allows you to draw things in front of this projectile. Use the Main.EntitySpriteDraw
		//     method for drawing. This method is called even if PreDraw returns false.
		//
		// 参数:
		//   lightColor:
		//     The color of the light at the projectile's center, after being modified by vanilla
		//     and other mods.
		public virtual void PostDraw(Color lightColor)
		{
		}

		//
		// 摘要:
		//     This code is called whenever the player uses a grappling hook that shoots this
		//     type of projectile. Use it to change what kind of hook is fired (for example,
		//     the Dual Hook does this), to kill old hook projectiles, etc.
		public virtual void _____________________(Player player, ref int type)
		{
		}

		//
		// 摘要:
		//     How far away this grappling hook can travel away from its player before it retracts.
		public virtual float GrappleRange()
		{
			return 300f;
		}

		//
		// 摘要:
		//     How many of this type of grappling hook the given player can latch onto blocks
		//     before the hooks start disappearing. Change the numHooks parameter to determine
		//     this; by default it will be 3.
		public virtual void __________(Player player, ref int numHooks)
		{
		}

		//
		// 摘要:
		//     The speed at which the grapple retreats back to the player after not hitting
		//     anything. Defaults to 11, but vanilla hooks go up to 24.
		public virtual void GrappleRetreatSpeed(Player player, ref float speed)
		{
		}

		//
		// 摘要:
		//     The speed at which the grapple pulls the player after hitting something. Defaults
		//     to 11, but the Bat Hook uses 16.
		public virtual void GrapplePullSpeed(Player player, ref float speed)
		{
		}

		//
		// 摘要:
		//     The location that the grappling hook pulls the player to. Defaults to the center
		//     of the hook projectile.
		public virtual void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY)
		{
		}

		//
		// 摘要:
		//     When used in conjunction with "Projectile.hide = true", allows you to specify
		//     that this projectile should be drawn behind certain elements. Add the index to
		//     one and only one of the lists. For example, the Nebula Arcanum projectile draws
		//     behind NPCs and tiles.
		public virtual void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
		}
	}
}
