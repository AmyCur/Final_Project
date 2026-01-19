using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace FileManagement.IO;


public static class Read{
	public static object ReadFile(object path, bool join=true){
		if(path is string){
			List<string> contents = new();

			using (StreamReader reader = new StreamReader((string)path)){
				string line = reader.ReadLine();
				while(line!=null){
					contents.Add(line);
					line = reader.ReadLine();
				}
			}

			return join ? string.Join(' ', contents) : contents;
		}

		return (path as TextAsset).text;
		
	}
}
