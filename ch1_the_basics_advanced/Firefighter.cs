using System;

namespace ch1_the_basics_advanced
{
	public class Firefighter : PublicServant, IPerson
	{
		public Firefighter(string name, int age) {
			this.Name = name;
			this.Age = age;

			this.DriveToPlaceOfInterest += delegate {
				Console.WriteLine("Driving the firetruck");
				GetInFiretruck();
				TurnOnSiren();
				FollowDirections();
			};
		}

		//implement the IPerson interface
		public string Name { get; set; }
		public int Age { get; set; }

		private void GetInFiretruck() {}
		private void TurnOnSiren() {}
		private void FollowDirections() {}
	}
}
