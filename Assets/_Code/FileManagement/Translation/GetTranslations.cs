using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cur.IO;
using UnityEngine;

namespace FileManagement.Translation;
	
public static class GetTranslations{
	public static string path => Application.persistentDataPath + "/Translation/Translations.json";
}