using System;
using System.Windows.Markup;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Big3.Hitbase.SharedResources
{
    /// <summary>
    /// Represents a localization makrup extension.
    /// </summary>
    [MarkupExtensionReturnType(typeof(string))]
    [ContentProperty("Key")]
    public sealed class Loc : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the resource key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the formatting string to use.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The <see cref="DependencyProperty"/> localized by the instance.
        /// </summary>
        private DependencyProperty targetProperty;

        /// <summary>
        /// The instance that owns the <see cref="DependencyProperty"/> localized by the instance.
        /// </summary>
        private WeakReference targetObject;

        /// <summary>
        /// The list of instances created by a template that own the <see cref="DependencyProperty"/>
        /// localized by the instance.
        /// </summary>
        private List<WeakReference> targetObjects;

        /// <summary>
        /// Gets value indicating if the instance localized by this instance is alive.
        /// </summary>
        internal bool IsAlive
        {
            get
            {
                // Verify if the extension is used in a template
                if (this.targetObjects != null)
                {
                    foreach (var item in this.targetObjects)
                    {
                        if (item.IsAlive)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                return this.targetObject.IsAlive;
            }
        }

        /// <summary>
        /// Initializes new instance of the sealed class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        public Loc(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// Initializes new instance of the sealed class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        public Loc(object key)
        {
            this.Key = key.ToString();
        }

        internal Loc(object key, object depProperty, object targetObject)
            : this(key)
        {
            Init(depProperty, targetObject);
            UpdateTargetValue();
        }

        public void Init(object depProperty, object targetObject)
        {
            this.targetProperty = depProperty as DependencyProperty;

            if (this.targetProperty != null)
            {
                if (targetObject is DependencyObject)
                {
                    var targetReference = new WeakReference(targetObject);

                    // Verify if the extension is used in a template
                    // and has been already registered
                    if (this.targetObjects != null)
                    {
                        this.targetObjects.Add(targetReference);
                    }
                    else
                    {
                        this.targetObject = targetReference;

                        LocalizationManager.AddLocalization(this);
                    }
                }
                else
                {
                    // The extension is used in a template
                    this.targetObjects = new List<WeakReference>();

                    LocalizationManager.AddLocalization(this);

                }
            }
        }

        public object GetValue()
        {
            /*if (this.targetObject == null)
            {
                return this;
            }
            else
            {*/
                return GetValue(this.Key, this.Format);
            //}
        }

        /// <summary>
        /// Returns the object that corresponds to the specified resource key.
        /// </summary>
        /// <param name="serviceProvider">An object that can provide services for the markup extension.</param>
        /// <returns>The object that corresponds to the specified resource key.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            this.Init(service.TargetProperty as DependencyProperty, service.TargetObject);
            return GetValue();
        }

        /// <summary>
        /// Updates the value of the localized object.
        /// </summary>
        internal void UpdateTargetValue()
        {
            DependencyProperty targetProperty = this.targetProperty;

            if (targetProperty != null)
            {
                if (this.targetObject != null)
                {
                    DependencyObject targetObject = this.targetObject.Target as DependencyObject;

                    if (targetObject != null)
                    {
                        targetObject.SetValue(targetProperty, GetValue(this.Key, this.Format));
                    }
                }
                else if (this.targetObjects != null)
                {
                    foreach (var item in this.targetObjects)
                    {
                        DependencyObject targetObject = item.Target as DependencyObject;

                        if (targetObject != null)
                        {
                            targetObject.SetValue(targetProperty, GetValue(this.Key, this.Format));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the object that corresponds to the specified resource key.
        /// </summary>
        /// <param name="key">the resource key.</param>
        /// <returns>The object that corresponds to the specified resource key.</returns>
        private static object GetValue(string key, string format)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            string value = LocalizationManager.GetLocalization(key);

            if (string.IsNullOrEmpty(format))
            {
                return value;
            }
            else
            {
                return string.Format(format, value);
            }
        }
    }
}
