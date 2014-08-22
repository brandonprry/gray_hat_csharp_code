using System;

namespace ch1_the_basics_advanced
{
	public class MainClass {
		public static void Main(string[] args) {
			Firefighter firefighter = new Firefighter ("Joe Carrington", 35);
			firefighter.PensionAmount = 5000;

			PoliceOfficer officer = new PoliceOfficer ("Jane Hope", 32);
			officer.PensionAmount = 5500;
			officer.HasEmergency = true;

			PrintNameAndAge(firefighter);
			PrintNameAndAge(officer);

			PrintPensionAmount(firefighter);
			PrintPensionAmount(officer);

			firefighter.DriveToPlaceOfInterest();
			officer.DriveToPlaceOfInterest();
		} 

		static void PrintNameAndAge (IPerson person) {
			Console.WriteLine("Name: " + person.Name);
			Console.WriteLine("Age: " + person.Age);
		}

		static void PrintPensionAmount(PublicServant servant) {
			if (servant is Firefighter)
				Console.WriteLine("Pension of firefighter: " + servant.PensionAmount);
			else if (servant is PoliceOfficer)
				Console.WriteLine("Pension of officer: " + servant.PensionAmount);
		}
	}
}
