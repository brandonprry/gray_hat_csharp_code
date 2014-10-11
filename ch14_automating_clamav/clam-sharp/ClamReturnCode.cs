using System;

namespace clamsharp
{
	//some of these will *never* be returned. Added only to be thorough.
	public enum ClamReturnCode
	{
		CL_CLEAN = 0,
		CL_SUCCESS = 0,
		CL_VIRUS,
		CL_ENULLARG,
		CL_EARG,
		CL_EMALFDB,
		CL_ECVD,
		CL_EVERIFY,
		CL_EUNPACK,
		CL_EOPEN,
		CL_ECREAT,
		CL_EUNLINK,
		CL_ESTAT,
		CL_EREAD,
		CL_ESEEK,
		CL_EWRITE,
		CL_EDUP,
		CL_EACCES,
		CL_ETMPFILE,
		CL_ETMPDIR,
		CL_EMAP,
		CL_EMEM,
		CL_ETIMEOUT,
		CL_BREAK,
		CL_EMAXREC,
		CL_EMAXSIZE,
		CL_EMAXFILES,
		CL_EFORMAT,
		CL_EPARSE,
		CL_EBYTECODE,
		CL_EBYTECODE_TESTFAIL,
		CL_ELOCK,
		CL_EBUSY,
		CL_ESTATE,
		CL_LAST_ERROR
	}
}

