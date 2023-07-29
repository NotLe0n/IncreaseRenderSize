using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;

namespace IncreaseRenderSize;
public static class ILHelper
{
	public static readonly ConstructorInfo Vector2_ctor_float_float = typeof(Vector2).GetConstructor(new Type[] { typeof(float), typeof(float) });
	public static readonly MethodInfo Vector2_op_Addition = typeof(Vector2).GetMethod("op_Addition", BindingFlags.Public | BindingFlags.Static);
	public static readonly MethodInfo Vector2_op_Subtraction = typeof(Vector2).GetMethod("op_Subtraction", BindingFlags.Public | BindingFlags.Static);
	public static readonly MethodInfo Vector2_op_Multiply = typeof(Vector2).GetMethod("op_Multiply", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(Vector2), typeof(float) });
	public static readonly FieldInfo Vector2_X = typeof(Vector2).GetField("X");
	public static readonly FieldInfo Vector2_Y = typeof(Vector2).GetField("Y");


	public static Vector2 OffsetVector => new(Main.screenWidth, Main.screenHeight);
	public static Vector2 ScaledOffsetVector => new(Main.screenWidth * Main.GameZoomTarget, Main.screenHeight * Main.GameZoomTarget);
	public const int RTScaler = 4;

	public static void EmitOffsetVectorX(this ILCursor c)
	{
		c.Emit(OpCodes.Call, typeof(ILHelper).GetMethod("get_" + nameof(OffsetVector)));
		c.Emit(OpCodes.Ldfld, Vector2_X);
	}
	
	public static void EmitOffsetVectorY(this ILCursor c)
	{
		c.Emit(OpCodes.Call, typeof(ILHelper).GetMethod("get_" + nameof(OffsetVector)));
		c.Emit(OpCodes.Ldfld, Vector2_Y);
	}
	
	/// <summary>
	/// Emits this C# code: <code>new Vector2(Main.screenWidth, 0)</code>
	/// </summary>
	public static void EmitOffsetVector(this ILCursor c)
	{
		// [0] => (float)Main.screenWidth
		c.EmitOffsetVectorX();

		// [1] => (float)Main.screenHeight
		c.EmitOffsetVectorY();

		// new Vector2([0], [1])
		c.Emit(OpCodes.Newobj, Vector2_ctor_float_float);
	}

	public static void EmitScaledOffsetVectorX(this ILCursor c)
	{
		c.Emit(OpCodes.Call, typeof(ILHelper).GetMethod("get_" + nameof(ScaledOffsetVector)));
		c.Emit(OpCodes.Ldfld, Vector2_X);
	}
	
	public static void EmitScaledOffsetVectorY(this ILCursor c)
	{
		c.Emit(OpCodes.Call, typeof(ILHelper).GetMethod("get_" + nameof(ScaledOffsetVector)));
		c.Emit(OpCodes.Ldfld, Vector2_Y);
	}
	
	/// <summary>
	/// Emits this C# code: <code>new Vector2(Main.screenWidth * Main.GameZoomTarget, 0)</code>
	/// </summary>
	public static void EmitScaledOffsetVector(this ILCursor c)
	{
		// [0] => (float)Main.screenWidth * Main.gameZoomTarget
		c.EmitScaledOffsetVectorX();

		// [1] => (float)Main.screenHeight * Main.gameZoomTarget
		c.EmitScaledOffsetVectorY();

		// new Vector2([0], [1])
		c.Emit(OpCodes.Newobj, Vector2_ctor_float_float);
	}
}
