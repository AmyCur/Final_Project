using System;

namespace FileManagement.UI{
	public static class Jumble{
		//TODO: Add support for custom character inputs
		static char[] characters = {'!', '£', '$', '€', '%', '^', '&', '*', '(', ')', '-', '_', '+', '='};
		static Random rand;
		
		public static char RandomLetter()
        {
			return characters[rand.Next(0, characters.Length)];
		}

		public static string DistortString(string input, int corruptionRange=5){
			char[] working = input.ToCharArray();
			rand??=new Random();
			for(int i = 0; i < input.Length; i++){
				if(rand.Next(0,corruptionRange) == 0 && working[i] != ' '){
					working[i] = RandomLetter();
				}
			}
			return new string(working);
		}
	}
}