using System;

namespace IncreaseRenderSize;
public class ILEditException : Exception
{
	public ILEditException(string containingMember, string method)
		: base($"IL edit at \"{containingMember}::{method}()\" failed! Please contact NotLe0n!")
	{ }
}