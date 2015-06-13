using System;

namespace ch13_automating_clamav_filesystem
{
	public enum ClamBytecodeSecurityOptions
	{
		CL_BYTECODE_TRUST_ALL = 0,
		CL_BYTECODE_TRUST_SIGNED,
		CL_BYTECODE_TRUST_NOTHING
	}
}

