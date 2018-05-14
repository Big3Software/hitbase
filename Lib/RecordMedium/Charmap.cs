using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Big3.Hitbase.Configuration;

namespace Big3.Hitbase.RecordMedium
{
    public partial class Charmap : Form
    {
        public Charmap()
        {
            InitializeComponent();

            // Read registry values
            textAnfuehrung.Text = Settings.Current.RecordFileNameCharAnfuehrung;
            textBackslash.Text = Settings.Current.RecordFileNameCharBackslash;
            textDoppelPunkt.Text = Settings.Current.RecordFileNameCharDoppelpunkt;
            textFragezeichen.Text = Settings.Current.RecordFileNameCharFragezeichen;
            textGroesser.Text = Settings.Current.RecordFileNameCharGroesser;
            textKleiner.Text = Settings.Current.RecordFileNameCharKleiner;
            textPipe.Text = Settings.Current.RecordFileNameCharPipe;
            textSlash.Text = Settings.Current.RecordFileNameCharSlash;
            textStern.Text = Settings.Current.RecordFileNameCharStern;
            textUserdefOrg1.Text = Settings.Current.RecordFileNameCharUserOrg1;
            textUserdefOrg2.Text = Settings.Current.RecordFileNameCharUserOrg2;
            textUserdefOrg3.Text = Settings.Current.RecordFileNameCharUserOrg3;
            textUserdefOrg4.Text = Settings.Current.RecordFileNameCharUserOrg4;
            textUserdefOrg5.Text = Settings.Current.RecordFileNameCharUserOrg5;
            textUserdefOrg6.Text = Settings.Current.RecordFileNameCharUserOrg6;
            textUserdefNew1.Text = Settings.Current.RecordFileNameCharUserNew1;
            textUserdefNew2.Text = Settings.Current.RecordFileNameCharUserNew2;
            textUserdefNew3.Text = Settings.Current.RecordFileNameCharUserNew3;
            textUserdefNew4.Text = Settings.Current.RecordFileNameCharUserNew4;
            textUserdefNew5.Text = Settings.Current.RecordFileNameCharUserNew5;
            textUserdefNew6.Text = Settings.Current.RecordFileNameCharUserNew6;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string badchar = "\"\\:?><|/*";
            char[] allbadchar = badchar.ToCharArray();

            if (textAnfuehrung.Text.IndexOfAny(allbadchar) >= 0
                || textBackslash.Text.IndexOfAny(allbadchar) >= 0
                || textDoppelPunkt.Text.IndexOfAny(allbadchar) >= 0
                || textFragezeichen.Text.IndexOfAny(allbadchar) >= 0
                || textGroesser.Text.IndexOfAny(allbadchar) >= 0
                || textKleiner.Text.IndexOfAny(allbadchar) >= 0
                || textPipe.Text.IndexOfAny(allbadchar) >= 0
                || textSlash.Text.IndexOfAny(allbadchar) >= 0
                || textStern.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefOrg1.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefOrg2.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefOrg3.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefOrg4.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefOrg5.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefOrg6.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefNew1.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefNew2.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefNew3.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefNew4.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefNew5.Text.IndexOfAny(allbadchar) >= 0
                || textUserdefNew5.Text.IndexOfAny(allbadchar) >= 0
                )
            {
                MessageBox.Show("Nicht zulässiges Zeichen in einem der Felder ('\"\\:?><|/*')!");
                DialogResult = DialogResult.None;
                return;
            }

            Settings.Current.RecordFileNameCharAnfuehrung = textAnfuehrung.Text;
            Settings.Current.RecordFileNameCharBackslash = textBackslash.Text;
            Settings.Current.RecordFileNameCharDoppelpunkt = textDoppelPunkt.Text;
            Settings.Current.RecordFileNameCharFragezeichen = textFragezeichen.Text;
            Settings.Current.RecordFileNameCharGroesser = textGroesser.Text;
            Settings.Current.RecordFileNameCharKleiner = textKleiner.Text;
            Settings.Current.RecordFileNameCharPipe = textPipe.Text;
            Settings.Current.RecordFileNameCharSlash = textSlash.Text;
            Settings.Current.RecordFileNameCharStern = textStern.Text;
            Settings.Current.RecordFileNameCharUserOrg1 = textUserdefOrg1.Text;
            Settings.Current.RecordFileNameCharUserOrg2 = textUserdefOrg2.Text;
            Settings.Current.RecordFileNameCharUserOrg3 = textUserdefOrg3.Text;
            Settings.Current.RecordFileNameCharUserOrg4 = textUserdefOrg4.Text;
            Settings.Current.RecordFileNameCharUserOrg5 = textUserdefOrg5.Text;
            Settings.Current.RecordFileNameCharUserOrg6 = textUserdefOrg6.Text;
            Settings.Current.RecordFileNameCharUserNew1 = textUserdefNew1.Text;
            Settings.Current.RecordFileNameCharUserNew2 = textUserdefNew2.Text;
            Settings.Current.RecordFileNameCharUserNew3 = textUserdefNew3.Text;
            Settings.Current.RecordFileNameCharUserNew4 = textUserdefNew4.Text;
            Settings.Current.RecordFileNameCharUserNew5 = textUserdefNew5.Text;
            Settings.Current.RecordFileNameCharUserNew6 = textUserdefNew6.Text;
        }
    }
}
