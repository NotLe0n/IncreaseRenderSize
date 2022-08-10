using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace IncreaseRenderSize.Edits;
internal class RenderTargetEdit : IEdit
{
	public void Load()
	{
		IL.Terraria.Main.InitTargets_int_int += IncreaseRTSize;
	}

	private void IncreaseRTSize(ILContext il)
	{
		var c = new ILCursor(il);

		/*
			C#:
				before:
					height += offScreenRange * 2;
					try {
				after:
					height += offScreenRange * 2;
					width *= 4;
					height *= 4;
					try {
			IL:
				IL_003f: ldarg.2
				IL_0040: ldsfld int32 Terraria.Main::offScreenRange
				IL_0045: ldc.i4.2
				IL_0046: mul
				IL_0047: add
				IL_0048: starg.s 2
			[+]	IL_004a: ldarg.1
			[+]	IL_004b: ldc.i4.4
			[+]	IL_004c: mul
			[+]	IL_004d: starg.s 1
				.try
		*/

		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdarg(2),
			i => i.MatchLdsfld<Main>("offScreenRange"),
			i => i.MatchLdcI4(2),
			i => i.MatchMul(),
			i => i.MatchAdd(),
			i => i.MatchStarg(2)
		)) {
			throw new ILEditException(GetType().FullName, nameof(IncreaseRTSize));
		}

		// width *= 4;
		c.Emit(OpCodes.Ldarg_1);
		c.Emit(OpCodes.Ldc_I4, 4);
		c.Emit(OpCodes.Mul);
		c.Emit(OpCodes.Starg, 1);

		// height *= 4;
		c.Emit(OpCodes.Ldarg_2);
		c.Emit(OpCodes.Ldc_I4, 4);
		c.Emit(OpCodes.Mul);
		c.Emit(OpCodes.Starg, 2);
	}
}
