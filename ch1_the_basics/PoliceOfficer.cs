using System;

namespace ch1_the_basics
{
	public class PoliceOfficer : PublicServant, IPerson
	{
		private bool _hasEmergency = false;

		public PoliceOfficer (string name, int age) {
			this.Name = name;
			this.Age = age;
		}

		//implement the IPerson interface
		public string Name { get; set; }
		public int Age { get; set; }

		public bool HasEmergency {
			get { return _hasEmergency; }
			set { _hasEmergency = value; }
		}

		public override void DriveToPlaceOfInterest() {
			GetInPoliceCar ();

			if (this.HasEmergency)
				TurnOnSiren ();

			FollowDirections ();
		}

		private void GetInPoliceCar() {}
		private void TurnOnSiren() {}
		private void FollowDirections() {}
	}
}

