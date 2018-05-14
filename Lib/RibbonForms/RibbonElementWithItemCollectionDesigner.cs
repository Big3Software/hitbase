/*
 
 2008 José Manuel Menéndez Poo
 * 
 * Please give me credit if you use this code. It's all I ask.
 * 
 * Contact me for more info: menendezpoo@gmail.com
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace System.Windows.Forms
{
    internal abstract class RibbonElementWithItemCollectionDesigner
        : ComponentDesigner
    {
        #region Props

        public abstract Ribbon Ribbon { get; }

        public abstract RibbonItemCollection Collection { get; }

        public override DesignerVerbCollection Verbs
        {
            get
            {
                return new DesignerVerbCollection(new DesignerVerb[] { 
                    new DesignerVerb("Add Button", new EventHandler(AddButton)),
                    new DesignerVerb("Add ButtonList", new EventHandler(AddButtonList)),
                    new DesignerVerb("Add ItemGroup", new EventHandler(AddItemGroup)),
                    new DesignerVerb("Add Separator", new EventHandler(AddSeparator)),
                    new DesignerVerb("Add TextBox", new EventHandler(AddTextBox)),
                    new DesignerVerb("Add ComboBox", new EventHandler(AddComboBox)),
                    new DesignerVerb("Add ColorChooser", new EventHandler(AddColorChooser))
                });
            }
        }

        #endregion

        #region Methods

        private void CreateItem(Type t)
        {
            CreateItem(Ribbon, Collection, t);
        }

        protected virtual void CreateItem(Ribbon ribbon, RibbonItemCollection collection, Type t)
        {
            IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;

            if (host != null && collection != null && ribbon != null)
            {
                DesignerTransaction transaction = host.CreateTransaction("AddRibbonItem_" + Component.Site.Name);

                MemberDescriptor member = TypeDescriptor.GetProperties(Component)["Items"];
                base.RaiseComponentChanging(member);

                RibbonItem item = host.CreateComponent(t) as RibbonItem;

                if (!(item is RibbonSeparator)) item.Text = item.Site.Name;

                collection.Add(item);
                ribbon.OnRegionsChanged();

                base.RaiseComponentChanged(member, null, null);
                transaction.Commit();
            }
        }

        protected virtual void AddButton(object sender, EventArgs e)
        {
            CreateItem(typeof(RibbonButton));
        }

        protected virtual void AddButtonList(object sender, EventArgs e)
        {
            CreateItem(typeof(RibbonButtonList));
        }

        protected virtual void AddItemGroup(object sender, EventArgs e)
        {
            CreateItem(typeof(RibbonItemGroup));
        }

        protected virtual void AddSeparator(object sender, EventArgs e)
        {
            CreateItem(typeof(RibbonSeparator));
        }

        protected virtual void AddTextBox(object sender, EventArgs e)
        {
            CreateItem(typeof(RibbonTextBox));
        }

        protected virtual void AddComboBox(object sender, EventArgs e)
        {
            CreateItem(typeof(RibbonComboBox));
        }

        protected virtual void AddColorChooser(object sender, EventArgs e)
        {
            CreateItem(typeof(RibbonColorChooser));
        }

        #endregion
    }
}
