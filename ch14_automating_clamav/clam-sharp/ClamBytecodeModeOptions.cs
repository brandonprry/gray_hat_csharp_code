using System;

namespace ch13_automating_clamav_filesystem
{
	public enum ClamBytecodeModeOptions
	{
		CL_BYTECODE_MODE_AUTO = 0,
		CL_BYTECODE_MODE_JIT,
		CL_BYTECODE_MODE_INTERPRETER,
		CL_BYTECODE_MODE_TEST,
		CL_BYTECODE_MODE_OFF
	}
}

