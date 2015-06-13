using System;

namespace clamsharp
{
	//some of these will *never* be returned. Added only to be thorough.
	public enum ClamReturnCode
	{
		CL_CLEAN = 0,
		CL_SUCCESS = 0,
		CL_VIRUS,
	}
}

