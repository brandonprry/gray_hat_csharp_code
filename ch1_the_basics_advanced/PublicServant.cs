using System;

namespace ch1_the_basics_advanced
{
	public abstract class PublicServant
	{
		public int PensionAmount { get; set; }

		public DriveToPlaceOfInterestDelegate DriveToPlaceOfInterest { get; set; }

		public delegate void DriveToPlaceOfInterestDelegate();
	}
}

