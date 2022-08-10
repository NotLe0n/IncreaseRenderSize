using IncreaseRenderSize.Edits;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace IncreaseRenderSize;
public class IncreaseRenderSize : Mod
{
	public IEdit[] edits = {
		new RenderTargetEdit(),
		new TileDrawingEdit(),
		new NonSolidTileEdit(),
		new WallAndBlackEdit(),
		new RangeFixEdit()
	};

	public override void Load()
	{
		foreach (var edit in edits) {
			edit.Load();
		}

		// for RenderTargetEdit to take effect
		Main.RunOnMainThread(() =>
		{
			typeof(Main)
				.GetMethod("InitTargets", BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)
				.Invoke(Main.instance, null);
		});

		base.Load();
	}
}