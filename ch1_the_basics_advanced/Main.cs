using System;

namespace ch1_the_basics_advanced
{
	public class MainClass {
		public static void Main(string[] args) {
			Firefighter firefighter = new Firefighter ("Joe Carrington", 35);
			firefighter.PensionAmount = 5000;

			PrintNameAndAge(firefighter);
			PrintPensionAmount(firefighter);

			firefighter.DriveToPlaceOfInterest();

			PoliceOfficer officer = new PoliceOfficer ("Jane Hope", 32);
			officer.PensionAmount = 5500;

			PrintNameAndAge(officer);
			PrintPensionAmount(officer);

			officer.DriveToPlaceOfInterest();

			officer = new PoliceOfficer ("John Valor", 32, true);
			PrintNameAndAge(officer);
			officer.DriveToPlaceOfInterest ();
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
