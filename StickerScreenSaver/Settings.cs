using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace StickerScreenSaver
{
    /// <summary>
    /// Persist raindrop screen saver settings in memory and provide support
    /// for loading from file and persisting to file.
    /// </summary>
    public class Settings
    {
        public const string SettingsFile = "Stickers.xml";

        public string Sets;

        /// <summary>
        /// Instantiate the class, loading settings from a specified file.
        /// If the file doesn't exist, use default values.
        /// </summary>
        /// <param name="sSettingsFilename"></param>
        public Settings()
        {
            SetDefaults();      // Clean object, start w/defaults
        }

        /// <summary>
        /// Set all values to their defaults
        /// </summary>
        public void SetDefaults()
        {
            Sets = string.Empty;
        }

        /// <summary>
        /// Save current settings to external file
        /// </summary>
        /// <param name="sSettingsFilename"></param>
        public void Save(string sSettingsFilename)
        {
            try
            {
                XmlSerializer serial = new XmlSerializer(typeof(Settings));

                FileStream fs = new FileStream(sSettingsFilename, FileMode.Create);
                TextWriter writer = new StreamWriter(fs, new UTF8Encoding());
                serial.Serialize(writer, this);
                writer.Close();
            }
            catch { }
        }

        /// <summary>
        /// Attempt to load settings from external file.  If the file doesn't 
        /// exist, or if there is a problem, no settings are changed.
        /// </summary>
        /// <param name="sSettingsFilename"></param>
        public static Settings Load(string sSettingsFilename)
        {
            Settings settings = null;
            try
            {
                XmlSerializer serial = new XmlSerializer(typeof(Settings));
                FileStream fs = new FileStream(sSettingsFilename, FileMode.OpenOrCreate);
                TextReader reader = new StreamReader(fs);
                settings = (Settings)serial.Deserialize(reader);
            }
            catch
            {
                // If we can't load, just create a new object, which gets default values
                settings = new Settings();
            }

            return settings;
        }
    }
}

