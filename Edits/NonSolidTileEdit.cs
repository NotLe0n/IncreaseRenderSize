using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace IncreaseRenderSize.Edits;
internal class NonSolidTileEdit : IEdit
{
	public void Load()
	{
		IL_Main.DoDraw_Tiles_NonSolid += MoveRenderTarget;
	}

	private void MoveRenderTarget(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C# (L-50008):
				before:
					spriteBatch.Draw(tile2Target, sceneTile2Pos - screenPosition, Microsoft.Xna.Framework.Color.White);
				after:
					spriteBatch.Draw(tile2Target, sceneTile2Pos - screenPosition - new Vector2(Main.screenWidth, Main.screenHeight), Microsoft.Xna.Framework.Color.White);
			IL:
				IL_005e: ldsfld class [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch Terraria.Main::spriteBatch
				IL_0063: ldarg.0
				IL_0064: ldfld class [FNA]Microsoft.Xna.Framework.Graphics.RenderTarget2D Terraria.Main::tile2Target
				IL_0069: ldsfld valuetype [FNA]Microsoft.Xna.Framework.Vector2 Terraria.Main::sceneTile2Pos
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
			i => i.MatchLdfld<Main>("tile2Target"),
			i => i.MatchLdsfld<Main>("sceneTile2Pos"),
			i => i.MatchLdsfld<Main>("screenPosition"),
			i => i.MatchCall<Vector2>("op_Subtraction")
		)) {
			throw new ILEditException(GetType().FullName, nameof(MoveRenderTarget));
		}

		c.EmitOffsetVector();
		c.Emit(OpCodes.Call, ILHelper.Vector2_op_Subtraction);
	}
}
