using System.IO;
using System.Collections.Generic;

namespace Cur.IO{
	public static class Read{
		public static object ReadFile(string path, bool join=true){
			List<string> contents = new();

      		using (StreamReader reader = new StreamReader(path)){
				string line = reader.ReadLine();
				while(line!=null){
					contents.Add(line);
					line = reader.ReadLine();
				}
			}

			return join ? string.Join(' ', contents) : contents;
		}
	}
}