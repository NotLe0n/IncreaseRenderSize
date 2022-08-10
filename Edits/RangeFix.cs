using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using Terraria;

namespace IncreaseRenderSize.Edits;
internal class RangeFixEdit : IEdit
{
	public void Load()
	{
		IL.Terraria.GameContent.Drawing.TileDrawing.Draw += TileRangeFix;
		//IL.Terraria.GameContent.Drawing.WallDrawing.DrawWalls += WallDrawing_DrawWalls;
	}

	private void WallDrawing_DrawWalls(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C#:
				before:
					int num9 = offScreenRange / 16;
				after:
					int num9 = offScreenRange / 16 + (int)(Main.screenWidth * Main.GameZoomTarget) / 16;
			IL:
				IL_0143: ldloc.10
				IL_0144: ldc.i4.s 16
				IL_0146: div
			[+]	IL_0147: ldsfld int32 Terraria.Main::screenWidth
			[+]	IL_014c: conv.r4
			[+]	IL_014d: ldsfld float32 Terraria.Main::GameZoomTarget
			[+]	IL_0152: mul
			[+]	IL_0153: conv.i4
			[+]	IL_0154: ldc.i4.s 16
			[+]	IL_0156: div
			[+]	IL_0157: add
				IL_0158: stloc.s 21
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdloc(1),
			i => i.MatchLdcI4(16),
			i => i.MatchDiv()
		)) {
			throw new ILEditException(GetType().FullName, nameof(WallDrawing_DrawWalls));
		}

		c.Emit(OpCodes.Ldsfld, typeof(Main).GetField("screenWidth"));
		c.Emit(OpCodes.Conv_I4);
		c.Emit(OpCodes.Ldsfld, typeof(Main).GetField("GameZoomTarget"));
		c.Emit(OpCodes.Mul);
		c.Emit(OpCodes.Conv_I4);
		c.Emit(OpCodes.Ldc_I4, 16);
		c.Emit(OpCodes.Div);
		c.Emit(OpCodes.Add);

		/*
			C#:
				before:
					int num10 = offScreenRange / 16;
				after:
					int num10 = offScreenRange / 16 + (int)(Main.screenHeight * Main.GameZoomTarget) / 16;
			IL:
				IL_015a: ldloc.1
				IL_015b: ldc.i4.s 16
				IL_015d: div
			[+]	IL_015e: ldsfld int32 Terraria.Main::screenHeight
			[+]	IL_0163: conv.r4
			[+]	IL_0164: ldsfld float32 Terraria.Main::GameZoomTarget
			[+]	IL_0169: mul
			[+]	IL_016a: conv.i4
			[+]	IL_016b: ldc.i4.s 16
			[+]	IL_016d: div
			[+]	IL_016e: add
				IL_016f: stloc.s 22
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdloc(1),
			i => i.MatchLdcI4(16),
			i => i.MatchDiv()
		)) {
			throw new ILEditException(GetType().FullName, nameof(WallDrawing_DrawWalls));
		}

		c.Emit(OpCodes.Ldsfld, typeof(Main).GetField("screenHeight"));
		c.Emit(OpCodes.Conv_I4);
		c.Emit(OpCodes.Ldsfld, typeof(Main).GetField("GameZoomTarget"));
		c.Emit(OpCodes.Mul);
		c.Emit(OpCodes.Conv_I4);
		c.Emit(OpCodes.Ldc_I4, 16);
		c.Emit(OpCodes.Div);
		c.Emit(OpCodes.Add);
	}

	private void TileRangeFix(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C#:
				before:
					GetScreenDrawArea(unscaledPosition, value + (Main.Camera.UnscaledPosition - Main.Camera.ScaledPosition), out var firstTileX, out var lastTileX, out var firstTileY, out var lastTileY);
				after:
					GetScreenDrawArea(unscaledPosition, value + (Main.Camera.UnscaledPosition + new Vector2(Main.screenWidth, Main.screenHeight) - (Main.Camera.ScaledPosition - new Vector2(Main.screenWidth * Main.GameZoomTarget, Main.screenHeight * Main.GameZoomTarget))), out var firstTileX, out var lastTileX, out var firstTileY, out var lastTileY);
																							  ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^									^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
			IL:
				IL_0174: ldarg.0
				IL_0175: ldloc.1
				IL_0176: ldloc.2
				IL_0177: ldsfld class Terraria.Graphics.Camera Terraria.Main::Camera
				IL_017c: callvirt instance valuetype [FNA]Microsoft.Xna.Framework.Vector2 Terraria.Graphics.Camera::get_UnscaledPosition()
			[+]	IL_0181: ldsfld int32 Terraria.Main::screenWidth
			[+]	IL_0186: conv.r4
			[+]	IL_0187: ldc.r4 0.0
			[+]	IL_018c: newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
			[+]	IL_0191: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Addition(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
				IL_0196: ldsfld class Terraria.Graphics.Camera Terraria.Main::Camera
				IL_019b: callvirt instance valuetype [FNA]Microsoft.Xna.Framework.Vector2 Terraria.Graphics.Camera::get_ScaledPosition()
			[+]	IL_01a0: ldsfld int32 Terraria.Main::screenWidth
			[+]	IL_01a5: conv.r4
			[+]	IL_01a6: ldsfld float32 Terraria.Main::GameZoomTarget
			[+]	IL_01ab: mul
			[+]	IL_01ac: ldc.r4 0.0
			[+]	IL_01b1: newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
			[+]	IL_01b6: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Subtraction(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
				IL_01bb: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Subtraction(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
				IL_01c0: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Addition(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
				IL_01c5: ldloca.s 6
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(0),
			i => i.MatchLdloc(1),
			i => i.MatchLdloc(2),
			i => i.MatchLdsfld<Main>("Camera"),
			i => i.MatchCallvirt<Terraria.Graphics.Camera>("get_UnscaledPosition")
		)) {
			throw new ILEditException(GetType().FullName, nameof(TileRangeFix));
		}

		c.EmitOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Addition);

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsfld<Main>("Camera"),
			i => i.MatchCallvirt<Terraria.Graphics.Camera>("get_ScaledPosition")
		)) {
			throw new ILEditException(GetType().FullName, nameof(TileRangeFix));
		}

		c.EmitScaledOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Subtraction);
	}
}
