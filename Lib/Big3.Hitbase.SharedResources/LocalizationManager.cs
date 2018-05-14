using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

namespace Big3.Hitbase.SharedResources
{
    /// <summary>
    /// Represents a sealed class that manages the localication features.
    /// </summary>
    public static class LocalizationManager
    {
        /// <summary>
        /// Gets or sets the current UI culture.
        /// </summary>
        /// <remarks>
        /// This property changes the UI culture of the current thread to the specified value
        /// and updates all localized property to reflect values of the new culture.
        /// </remarks>
        public static CultureInfo UICulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                Thread.CurrentThread.CurrentUICulture = value;

                UpdateLocalizations();
            }
        }

        /// <summary>
        /// Holds the list of localization instances.
        /// </summary>
        /// <remarks>
        /// <see cref="WeakReference"/> cannot be used as a localization instance
        /// will be garbage collected on the next garbage targetCollection
        /// as the localizaed object does not hold reference to it.
        /// </remarks>
        private static List<Loc> localizations = new List<Loc>();

        /// <summary>
        /// Holds the number of localizations added since the last purge of localizations.
        /// </summary>
        private static int localizationPurgeCount;

        /// <summary>
        /// Adds the specified localization instance to the list of manages localization instances.
        /// </summary>
        /// <param name="localization">The localization instance.</param>
        internal static void AddLocalization(Loc localization)
        {
            if (localization == null)
            {
                throw new ArgumentNullException("localization");
            }

            if (localizationPurgeCount > 50)
            {
                var localizatons = new List<Loc>(localizations.Count);

                foreach (var item in localizations)
                {
                    if (item.IsAlive)
                    {
                        localizatons.Add(item);
                    }
                }

                localizations = localizatons;

                localizationPurgeCount = 0;
            }

            localizations.Add(localization);

            localizationPurgeCount++;
        }

        /// <summary>
        /// Returns the localized string of a given key
        /// </summary>
        /// <param name="key">Key of the localized ressource</param>
        /// <returns>Localized string if the key is found, or an emptry string.</returns>
        public static void SetLocalization(string key, object depProperty, object targetObject)
        {
            new Loc(key, depProperty, targetObject);
        }

        public static string GetLocalization(string key)
        {
            try
            {
                return StringTable.ResourceManager.GetString(key);
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// Updates the localizations.
        /// </summary>
        private static void UpdateLocalizations()
        {
            foreach (var item in localizations)
            {
                item.UpdateTargetValue();
            }
        }
    }
}
