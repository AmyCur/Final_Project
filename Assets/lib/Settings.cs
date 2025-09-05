using UnityEngine;
using Globals;
using TMPro;

namespace AmyCurr.Settings
{
    [System.Serializable]
    public enum languages
    {
        english,
        spanish,
        french,
        german
    }

    public static class Settings
    {
        public static class Development
        {
            public static bool devMode = true;
        }

        /*public static class Language
        {

            public static languages language = languages.english;

            public static void ChangeLanguage(languages l, bool updateLanguage = true)
            {
                language = l;
                if (updateLanguage) UpdateLanguageText();
            }

            static void UpdateLanguageText()
            {
                GameObject[] texts = GameObject.FindGameObjectsWithTag(glob.languageTextTag);
                foreach (GameObject t in texts)
                {
                    TranslateText tt = t.GetComponent<TranslateText>();
                    TMP_Text text = t.GetComponent<TMP_Text>();
                    string str = text.text;

                    switch (language)
                    {
                        case (languages.english):
                            text.text = tt.english;
                            break;
                        case (languages.spanish):
                            text.text = tt.spanish;
                            break;
                        case (languages.french):
                            text.text = tt.french;
                            break;
                        case (languages.german):
                            text.text = tt.german;
                            break;
                    }

                    if (text.text.Length == 0)
                    {
                        text.text = str;
                    }
                }
            }
        }*/
    }
}
