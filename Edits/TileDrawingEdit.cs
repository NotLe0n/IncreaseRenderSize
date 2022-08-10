using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace IncreaseRenderSize.Edits;
internal class TileDrawingEdit : IEdit
{
	public void Load()
	{
		IL.Terraria.Main.DoDraw_Tiles_Solid += MoveRenderTarget;
		IL.Terraria.GameContent.Drawing.TileDrawing.Draw += OffsetDraw;
	}

	private void OffsetDraw(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C# (L-326):
				before:
					Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
				after:
					Vector2 unscaledPosition = Main.Camera.UnscaledPosition - new Vector2(Main.screenWidth, Main.screenHeight);
			IL:
				IL_0033: ldsfld class Terraria.Graphics.Camera Terraria.Main::Camera
				IL_0038: callvirt instance valuetype [FNA]Microsoft.Xna.Framework.Vector2 Terraria.Graphics.Camera::get_UnscaledPosition()
			[+]	IL_003d: ldsfld int32 Terraria.Main::screenWidth
			[+]	IL_0042: conv.r4
			[+]	IL_0043: ldc.r4 0.0
			[+]	IL_0048: newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
			[+]	IL_004d: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Subtraction(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
				IL_0052: stloc.1
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsfld<Main>("Camera"),
			i => i.MatchCallvirt<Terraria.Graphics.Camera>("get_UnscaledPosition")
		)) {
			throw new ILEditException(GetType().FullName, nameof(OffsetDraw));
		}

		c.EmitOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Subtraction);
	}

	private void MoveRenderTarget(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C# (L-49979):
				before:
					spriteBatch.Draw(tileTarget, sceneTilePos - screenPosition, Microsoft.Xna.Framework.Color.White);
				after:
					spriteBatch.Draw(tileTarget, sceneTilePos - screenPosition - new Vector2(screenWidth, screenHeight), Microsoft.Xna.Framework.Color.White);
			IL:
				IL_005e: ldsfld class [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch Terraria.Main::spriteBatch
				IL_0063: ldarg.0
				IL_0064: ldfld class [FNA]Microsoft.Xna.Framework.Graphics.RenderTarget2D Terraria.Main::tileTarget
				IL_0069: ldsfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Terraria.Main::sceneTilePos
				IL_006e: ldsfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Terraria.Main::screenPosition
				IL_0073: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Subtraction(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
			[+]	IL_0078: ldsfld int32 Terraria.Main::screenWidth
			[+]	IL_007d: conv.r4
			[+]	IL_007e: ldc.r4 0.0
			[+]	IL_0083: newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
			[+]	IL_0088: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Subtraction(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
				IL_008d: call valuetype [FNA]Microsoft.Xna.Framework.Color [FNA]Microsoft.Xna.Framework.Color::get_White()
				IL_0092: callvirt instance void [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch::Draw(class [FNA]Microsoft.Xna.Framework.Graphics.Texture2D, valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Color)
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsfld<Main>("spriteBatch"),
			i => i.MatchLdarg(0),
			i => i.MatchLdfld<Main>("tileTarget"),
			i => i.MatchLdsfld<Main>("sceneTilePos"),
			i => i.MatchLdsfld<Main>("screenPosition"),
			i => i.MatchCall<Vector2>("op_Subtraction")
		)) {
			throw new ILEditException(GetType().FullName, nameof(MoveRenderTarget));
		}

		c.EmitOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Subtraction);
	}
}
