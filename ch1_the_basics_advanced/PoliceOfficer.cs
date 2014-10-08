using System;

namespace ch1_the_basics_advanced
{
	public class PoliceOfficer : PublicServant, IPerson
	{
		private bool _hasEmergency = false;

		public PoliceOfficer (string name, int age, bool hasEmergency = false) {
			this.Name = name;
			this.Age = age;
			this.HasEmergency = hasEmergency;

			if (this.HasEmergency) {
				this.DriveToPlaceOfInterest += delegate {
					Console.WriteLine ("Driving the police car with siren");
					GetInPoliceCar ();
					TurnOnSiren ();
					FollowDirections ();
				};
			} else {
				this.DriveToPlaceOfInterest += delegate {
					Console.WriteLine ("Driving the police car");
					GetInPoliceCar ();
					FollowDirections ();
				};
			}
		}

		//implement the IPerson interface
		public string Name { get; set; }
		public int Age { get; set; }

		public bool HasEmergency {
			get { return _hasEmergency; }
			set { _hasEmergency = value; }
		}

		private void GetInPoliceCar() {}
		private void TurnOnSiren() {}
		private void FollowDirections() {}
	}
}