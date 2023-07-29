using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace IncreaseRenderSize.Edits;
internal class WallAndBlackEdit : IEdit
{
	public void Load()
	{
		IL_Main.DoDraw_WallsAndBlacks += MoveRenderTarget;
		Terraria.GameContent.Drawing.IL_WallDrawing.DrawWalls += OffsetDrawWalls;
		IL_Main.DrawBlack += OffsetDrawBlack;
	}

	private void OffsetDrawBlack(ILContext il)
	{
		var c = new ILCursor(il);

		/*
				IL_001b: ldsfld int32 Terraria.Main::offScreenRange
				IL_0020: conv.r4
				IL_0021: newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
			[+]	IL_0026: ldsfld int32 Terraria.Main::screenWidth
			[+]	IL_002b: conv.r4
			[+]	IL_002c: ldsfld int32 Terraria.Main::screenHeight
			[+]	IL_0031: conv.r4
			[+]	IL_0032: newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
			[+]	IL_0037: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Addition(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
		*/

		/*if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsfld<Main>("offScreenRange"),
			i => i.MatchConvR4(),
			i => i.MatchNewobj(ILHelper.Vector2_ctor_float_float)
		)) {
			throw new ILEditException(GetType().FullName, nameof(MoveRenderTarget));
		}

		c.EmitOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Addition);*/

		/*
			C# (L-45683):
				before:
					spriteBatch.Draw(TextureAssets.BlackTile.Value, new Vector2(num7 << 4, i << 4) - screenPosition + value, new Microsoft.Xna.Framework.Rectangle(0, 0, j - num7 << 4, 16), Microsoft.Xna.Framework.Color.Black);
				after:
					spriteBatch.Draw(TextureAssets.BlackTile.Value, new Vector2(num7 << 4, i << 4) - screenPosition + value + new Vector2(Main.screenWidth, Main.screenHeight), new Microsoft.Xna.Framework.Rectangle(0, 0, j - num7 << 4, 16), Microsoft.Xna.Framework.Color.Black);
																															^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
			IL:
				IL_0412: shl
				IL_0413: conv.r4
				IL_0414: newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
				IL_0419: ldsfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Terraria.Main::screenPosition
				IL_041e: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Subtraction(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
				IL_0423: ldloc.1
				IL_0424: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Addition(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
			[+]	IL_0429: ldsfld int32 Terraria.Main::screenWidth
			[+]	IL_042e: conv.r4
			[+]	IL_042f: ldc.r4 0.0
			[+]	IL_0434: newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
			[+]	IL_0439: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Addition(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
				IL_043e: ldc.i4.0
				IL_043f: ldc.i4.0
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchShl(),
			i => i.MatchConvR4(),
			i => i.MatchNewobj<Vector2>(),
			i => i.MatchLdsfld<Main>("screenPosition"),
			i => i.MatchCall<Vector2>("op_Subtraction"),
			i => i.MatchLdloc(1),
			i => i.MatchCall<Vector2>("op_Addition")
		)) {
			throw new ILEditException(GetType().FullName, nameof(MoveRenderTarget));
		}

		c.EmitOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Addition);
	}

	private void OffsetDrawWalls(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C# (L-24):
				before:
					Vector2 screenPosition = Main.screenPosition;
				after:
					Vector2 screenPosition = Main.screenPosition - new Vector2(Main.screenWidth, Main.screenHeight);
			IL:
				IL_0013: ldsfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Terraria.Main::screenPosition
			[+]	IL_0018: ldsfld int32 Terraria.Main::screenWidth
			[+]	IL_001d: conv.r4
			[+]	IL_001e: ldsfld int32 Terraria.Main::screenHeight
			[+]	IL_0023: conv.r4
			[+]	IL_0024: newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
			[+]	IL_0029: call valuetype [FNA]Microsoft.Xna.Framework.Vector2 [FNA]Microsoft.Xna.Framework.Vector2::op_Subtraction(valuetype [FNA]Microsoft.Xna.Framework.Vector2, valuetype [FNA]Microsoft.Xna.Framework.Vector2)
				IL_002e: stloc.3
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsfld<Main>("screenPosition")
		)) {
			throw new ILEditException(GetType().FullName, nameof(MoveRenderTarget));
		}

		c.EmitOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Subtraction);

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
			throw new ILEditException(GetType().FullName, nameof(OffsetDrawBlack));
		}
		
		c.EmitScaledOffsetVectorX();
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
			throw new ILEditException(GetType().FullName, nameof(OffsetDrawBlack));
		}
		
		c.EmitScaledOffsetVectorY();
		c.Emit(OpCodes.Conv_I4);
		c.Emit(OpCodes.Ldc_I4, 16);
		c.Emit(OpCodes.Div);
		c.Emit(OpCodes.Add);
		
		if (!c.TryGotoNext(MoveType.After,
			    i => i.MatchCall<Main>("GetScreenOverdrawOffset"),
			    i => i.MatchStloc(24))) {
			throw new ILEditException(GetType().FullName, nameof(OffsetDrawBlack));
		}
		
		c.Emit(OpCodes.Ldloc, 15);
		c.Emit(OpCodes.Ldloc, 16);
		c.Emit(OpCodes.Ldloc, 17);
		c.Emit(OpCodes.Ldloc, 18);
		c.Emit(OpCodes.Ldloc, 19);
		c.Emit(OpCodes.Ldloc, 20);
		c.Emit(OpCodes.Ldloc, 24);
		c.EmitDelegate((int n6, int n7, int n8, int n9, int n10, int n11, Point so) =>
		{
			Main.NewText("num6 = " + n6);
			Main.NewText("num7 = " + n7);
			Main.NewText("num8 = " + n8);
			Main.NewText("num9 = " + n9);
			Main.NewText("num10 = " + n10);
			Main.NewText("num11 = " + n11);
			Main.NewText("so = " + so);
			Main.NewText("i1 = [" + (n8 - n11 + so.Y) + "; " + (n9 + n11 - so.Y) + ")");
			Main.NewText("i2 = [" + (n6 - n10 + so.X) + "; " + (n7 + n10 - so.X) + ")");
		});
	}

	private void MoveRenderTarget(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C# (L-50078):
				before:
					spriteBatch.Draw(blackTarget, sceneTilePos - screenPosition, Microsoft.Xna.Framework.Color.White);
				after:
					spriteBatch.Draw(blackTarget, sceneTilePos - screenPosition - new Vector2(screenWidth, screenHeight), Microsoft.Xna.Framework.Color.White);
			IL:
				IL_005e: ldsfld class [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch Terraria.Main::spriteBatch
				IL_0063: ldarg.0
				IL_0064: ldfld class [FNA]Microsoft.Xna.Framework.Graphics.RenderTarget2D Terraria.Main::blackTarget
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
			i => i.MatchLdfld<Main>("blackTarget"),
			i => i.MatchLdsfld<Main>("sceneTilePos"),
			i => i.MatchLdsfld<Main>("screenPosition"),
			i => i.MatchCall<Vector2>("op_Subtraction")
		)) {
			throw new ILEditException(GetType().FullName, nameof(MoveRenderTarget));
		}

		c.EmitOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Subtraction);

		/*
			C# (L-50080):
				before:
					spriteBatch.Draw(wallTarget, sceneWallPos - screenPosition, Microsoft.Xna.Framework.Color.White);
				after:
					spriteBatch.Draw(wallTarget, sceneWallPos - screenPosition - new Vector2(screenWidth, screenHeight), Microsoft.Xna.Framework.Color.White);
			IL:
				IL_005e: ldsfld class [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch Terraria.Main::spriteBatch
				IL_0063: ldarg.0
				IL_0064: ldfld class [FNA]Microsoft.Xna.Framework.Graphics.RenderTarget2D Terraria.Main::wallTarget
				IL_0069: ldsfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Terraria.Main::sceneWallPos
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
			i => i.MatchLdfld<Main>("wallTarget"),
			i => i.MatchLdsfld<Main>("sceneWallPos"),
			i => i.MatchLdsfld<Main>("screenPosition"),
			i => i.MatchCall<Vector2>("op_Subtraction")
		)) {
			throw new ILEditException(GetType().FullName, nameof(MoveRenderTarget));
		}

		c.EmitOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Subtraction);
	}
}
