using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco;
using System.Xml;
using System.IO;

namespace UserGroupPermissions.Businesslogic
{
    public class Languagefiles
    {
        /// <summary>
        /// Loop through the language config folder and add language nodes to the language files
        /// If the language is not in our language file install the english variant.
        /// </summary>
        public static void InstallLanguageKey(string key, string value)
        {
            if (KeyMissing(key))
            {
                string directory = HttpContext.Current.Server.MapPath(FormatUrl("/config/lang"));
                string[] languageFiles = Directory.GetFiles(directory);

                foreach (string languagefile in languageFiles)
                {
                    //Strip 2digit langcode from filename

                    string langcode = languagefile.Substring(languagefile.Length - 6, 2).ToLower();
                    if (langcode == "nl")
                    {
                        UpdateActionsForLanguageFile("nl.xml", key, value);
                    }
                    else
                    {
                        UpdateActionsForLanguageFile(string.Format("{0}.xml", langcode), key, value);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the key is missing.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True when key is missing</returns>
        private static bool KeyMissing(string key)
        {
            return ui.GetText("actions", key) == string.Format("[{0}]", key);
        }

        /// <summary>
        /// Update a language file withe the language xml
        /// </summary>
        private static void UpdateActionsForLanguageFile(string languageFile, string key, string value)
        {
            XmlDocument doc = xmlHelper.OpenAsXmlDocument(string.Format("{0}/config/lang/{1}", umbraco.GlobalSettings.Path, languageFile));
            XmlNode actionNode = doc.SelectSingleNode("//area[@alias='actions']");

            XmlNode node = actionNode.AppendChild(doc.CreateElement("key"));
            XmlAttribute att = node.Attributes.Append(doc.CreateAttribute("alias"));

            att.InnerText = key;
            node.InnerText = value;

            doc.Save(HttpContext.Current.Server.MapPath(string.Format("{0}/config/lang/{1}", umbraco.GlobalSettings.Path, languageFile)));
        }

        /// <summary>
        /// Returns a xml string as xml Node
        /// </summary>
        private XmlNode GetXmlAsNode(string xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            return xmlDocument.DocumentElement;
        }

        private  static string FormatUrl(string url)
        {
            return VirtualPathUtility.ToAbsolute(umbraco.GlobalSettings.Path + url);
        }
    }
}