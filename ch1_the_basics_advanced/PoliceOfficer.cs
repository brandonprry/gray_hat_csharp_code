using System;

namespace ch1_the_basics_advanced
{
	public class PoliceOfficer : PublicServant, IPerson
	{
		private bool _hasEmergency = false;

		public PoliceOfficer (string name, int age) {
			this.Name = name;
			this.Age = age;

			this.DriveToPlaceOfInterest += delegate {
				Console.WriteLine("Driving the police car");
				GetInPoliceCar();

				if (this.HasEmergency)
					TurnOnSiren();

				FollowDirections();
			};
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