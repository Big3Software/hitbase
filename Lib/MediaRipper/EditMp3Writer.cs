//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
//
//  Email:  yetiicb@hotmail.com
//
//  Copyright (C) 2002-2003 Idael Cardoso. 
//  
//  LAME ( LAME Ain't an Mp3 Encoder ) 
//  You must call the fucntion "beVersion" to obtain information  like version 
//  numbers (both of the DLL and encoding engine), release date and URL for 
//  lame_enc's homepage. All this information should be made available to the 
//  user of your product through a dialog box or something similar.
//  You must see all information about LAME project and legal license infos at 
//  http://www.mp3dev.org/  The official LAME site
//
//  About Thomson and/or Fraunhofer patents:
//  Any use of this product does not convey a license under the relevant 
//  intellectual property of Thomson and/or Fraunhofer Gesellschaft nor imply 
//  any right to use this product in any finished end user or ready-to-use final 
//  product. An independent license for such use is required. 
//  For details, please visit http://www.mp3licensing.com.
//

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Big3.Hitbase.MediaRipper;
using Big3.Hitbase.MediaRipper.MMedia;

namespace Big3.Hitbase.MediaRipper.Mp3
{
    /// <summary>
    /// Summary description for EditMp3Writer.
    /// </summary>
    public class EditMp3Writer : System.Windows.Forms.UserControl, IEditAudioWriterConfig
    {
        private System.Windows.Forms.ToolTip toolTip1;
        private System.ComponentModel.IContainer components;

        private Lame.BE_CONFIG m_Config = null;
        private const string Mpeg1BitRates = "32,40,48,56,64,80,96,112,128,160,192,224,256,320";
        private TabPage tabPage1;
        public TabControl tabControl1;
        private EditFormat editFormat1;
        private TabPage tabPage2;
        private Button buttonStandardCBR;
        private CheckBox checkBoxPrivate;
        private CheckBox checkBoxOriginal;
        private CheckBox checkBoxCRC;
        private CheckBox checkBoxCopyRight;
        private CheckBox checkBoxVBR;
        private GroupBox groupBoxVBR;
        private Button buttonStandardVBR;
        private Label label8;
        private Label label7;
        private ComboBox comboBoxVBRMethod;
        private Label label4;
        private TrackBar trackBarVBRQuality;
        private Label label6;
        private ComboBox comboBoxAvgBitrate;
        private Label label5;
        private ComboBox comboBoxMaxBitRate;
        private Label label3;
        private ComboBox comboBoxBitRate;
        private TextBox textBoxMpegVersion;
        private Label label1;
        private const string Mpeg2BitRates = "8,16,24,32,40,48,56,64,80,96,112,128,144,160";

        public EditMp3Writer()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            m_Config = new Big3.Hitbase.MediaRipper.Lame.BE_CONFIG(editFormat1.Format);
            DoSetInitialValues();

            tabControl1.TabPages.RemoveAt(0);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private bool m_FireConfigChangeEvent = true;

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Big3.Hitbase.MediaRipper.WaveFormat waveFormat1 = new Big3.Hitbase.MediaRipper.WaveFormat();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.editFormat1 = new Big3.Hitbase.MediaRipper.MMedia.EditFormat();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMpegVersion = new System.Windows.Forms.TextBox();
            this.comboBoxBitRate = new System.Windows.Forms.ComboBox();
            this.groupBoxVBR = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxMaxBitRate = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxAvgBitrate = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.trackBarVBRQuality = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxVBRMethod = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonStandardVBR = new System.Windows.Forms.Button();
            this.checkBoxVBR = new System.Windows.Forms.CheckBox();
            this.checkBoxCopyRight = new System.Windows.Forms.CheckBox();
            this.checkBoxCRC = new System.Windows.Forms.CheckBox();
            this.checkBoxOriginal = new System.Windows.Forms.CheckBox();
            this.checkBoxPrivate = new System.Windows.Forms.CheckBox();
            this.buttonStandardCBR = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.groupBoxVBR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVBRQuality)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.editFormat1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(328, 254);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Input format";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(336, 280);
            this.tabControl1.TabIndex = 0;
            // 
            // editFormat1
            // 
            this.editFormat1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editFormat1.Format = waveFormat1;
            this.editFormat1.Location = new System.Drawing.Point(0, 0);
            this.editFormat1.Name = "editFormat1";
            this.editFormat1.ReadOnly = true;
            this.editFormat1.Size = new System.Drawing.Size(328, 254);
            this.editFormat1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(195, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bitrate bzw. minimale Bitrate bei VBR:";
            // 
            // textBoxMpegVersion
            // 
            this.textBoxMpegVersion.Location = new System.Drawing.Point(267, 0);
            this.textBoxMpegVersion.Name = "textBoxMpegVersion";
            this.textBoxMpegVersion.ReadOnly = true;
            this.textBoxMpegVersion.Size = new System.Drawing.Size(45, 20);
            this.textBoxMpegVersion.TabIndex = 2;
            this.textBoxMpegVersion.Visible = false;
            // 
            // comboBoxBitRate
            // 
            this.comboBoxBitRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBitRate.Location = new System.Drawing.Point(8, 26);
            this.comboBoxBitRate.Name = "comboBoxBitRate";
            this.comboBoxBitRate.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBitRate.TabIndex = 3;
            this.toolTip1.SetToolTip(this.comboBoxBitRate, "Minimum bit rate if VBR is specified ");
            this.comboBoxBitRate.SelectedIndexChanged += new System.EventHandler(this.BitRateChange);
            // 
            // groupBoxVBR
            // 
            this.groupBoxVBR.Controls.Add(this.buttonStandardVBR);
            this.groupBoxVBR.Controls.Add(this.label8);
            this.groupBoxVBR.Controls.Add(this.label7);
            this.groupBoxVBR.Controls.Add(this.comboBoxVBRMethod);
            this.groupBoxVBR.Controls.Add(this.label4);
            this.groupBoxVBR.Controls.Add(this.trackBarVBRQuality);
            this.groupBoxVBR.Controls.Add(this.label6);
            this.groupBoxVBR.Controls.Add(this.comboBoxAvgBitrate);
            this.groupBoxVBR.Controls.Add(this.label5);
            this.groupBoxVBR.Controls.Add(this.comboBoxMaxBitRate);
            this.groupBoxVBR.Controls.Add(this.label3);
            this.groupBoxVBR.Location = new System.Drawing.Point(8, 96);
            this.groupBoxVBR.Name = "groupBoxVBR";
            this.groupBoxVBR.Size = new System.Drawing.Size(304, 144);
            this.groupBoxVBR.TabIndex = 4;
            this.groupBoxVBR.TabStop = false;
            this.groupBoxVBR.Text = "VBR Optionen";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Maximale Bitrate:";
            // 
            // comboBoxMaxBitRate
            // 
            this.comboBoxMaxBitRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMaxBitRate.Location = new System.Drawing.Point(8, 72);
            this.comboBoxMaxBitRate.Name = "comboBoxMaxBitRate";
            this.comboBoxMaxBitRate.Size = new System.Drawing.Size(121, 21);
            this.comboBoxMaxBitRate.TabIndex = 5;
            this.comboBoxMaxBitRate.SelectedIndexChanged += new System.EventHandler(this.BitRateChange);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Durchschnittliche Bitrate:";
            // 
            // comboBoxAvgBitrate
            // 
            this.comboBoxAvgBitrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAvgBitrate.Location = new System.Drawing.Point(8, 112);
            this.comboBoxAvgBitrate.Name = "comboBoxAvgBitrate";
            this.comboBoxAvgBitrate.Size = new System.Drawing.Size(121, 21);
            this.comboBoxAvgBitrate.TabIndex = 9;
            this.comboBoxAvgBitrate.SelectedIndexChanged += new System.EventHandler(this.Control_Changed);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(152, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 16);
            this.label6.TabIndex = 10;
            this.label6.Text = "VBR Qualität:";
            // 
            // trackBarVBRQuality
            // 
            this.trackBarVBRQuality.LargeChange = 0;
            this.trackBarVBRQuality.Location = new System.Drawing.Point(144, 32);
            this.trackBarVBRQuality.Maximum = 9;
            this.trackBarVBRQuality.Name = "trackBarVBRQuality";
            this.trackBarVBRQuality.Size = new System.Drawing.Size(144, 45);
            this.trackBarVBRQuality.TabIndex = 11;
            this.trackBarVBRQuality.Scroll += new System.EventHandler(this.Control_Changed);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 24);
            this.label4.TabIndex = 6;
            this.label4.Text = "VBR Methode:";
            // 
            // comboBoxVBRMethod
            // 
            this.comboBoxVBRMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVBRMethod.Items.AddRange(new object[] {
            "NONE",
            "DEFAULT",
            "OLD",
            "NEW",
            "MTRH",
            "ABR"});
            this.comboBoxVBRMethod.Location = new System.Drawing.Point(8, 32);
            this.comboBoxVBRMethod.Name = "comboBoxVBRMethod";
            this.comboBoxVBRMethod.Size = new System.Drawing.Size(121, 21);
            this.comboBoxVBRMethod.TabIndex = 7;
            this.comboBoxVBRMethod.SelectedIndexChanged += new System.EventHandler(this.comboBoxVBRMethod_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(152, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 16);
            this.label7.TabIndex = 12;
            this.label7.Text = "Max";
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(256, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 16);
            this.label8.TabIndex = 13;
            this.label8.Text = "Min";
            // 
            // buttonStandardVBR
            // 
            this.buttonStandardVBR.Location = new System.Drawing.Point(155, 110);
            this.buttonStandardVBR.Name = "buttonStandardVBR";
            this.buttonStandardVBR.Size = new System.Drawing.Size(133, 23);
            this.buttonStandardVBR.TabIndex = 14;
            this.buttonStandardVBR.Text = "Standard variable Bitrate";
            this.buttonStandardVBR.UseVisualStyleBackColor = true;
            this.buttonStandardVBR.Click += new System.EventHandler(this.buttonStandard_Click);
            // 
            // checkBoxVBR
            // 
            this.checkBoxVBR.Location = new System.Drawing.Point(8, 72);
            this.checkBoxVBR.Name = "checkBoxVBR";
            this.checkBoxVBR.Size = new System.Drawing.Size(192, 24);
            this.checkBoxVBR.TabIndex = 5;
            this.checkBoxVBR.Text = "Variable Bitrate aktivieren (VBR)";
            this.checkBoxVBR.CheckedChanged += new System.EventHandler(this.checkBoxVBR_CheckedChanged);
            // 
            // checkBoxCopyRight
            // 
            this.checkBoxCopyRight.Location = new System.Drawing.Point(8, 48);
            this.checkBoxCopyRight.Name = "checkBoxCopyRight";
            this.checkBoxCopyRight.Size = new System.Drawing.Size(72, 24);
            this.checkBoxCopyRight.TabIndex = 6;
            this.checkBoxCopyRight.Text = "Copyright";
            this.toolTip1.SetToolTip(this.checkBoxCopyRight, "Controls the copyrightb bit of MP3 stream");
            this.checkBoxCopyRight.CheckedChanged += new System.EventHandler(this.Control_Changed);
            // 
            // checkBoxCRC
            // 
            this.checkBoxCRC.Location = new System.Drawing.Point(88, 48);
            this.checkBoxCRC.Name = "checkBoxCRC";
            this.checkBoxCRC.Size = new System.Drawing.Size(72, 24);
            this.checkBoxCRC.TabIndex = 7;
            this.checkBoxCRC.Text = "CRC";
            this.toolTip1.SetToolTip(this.checkBoxCRC, "If set enables CRC-checksum in the bitstream");
            this.checkBoxCRC.CheckedChanged += new System.EventHandler(this.Control_Changed);
            // 
            // checkBoxOriginal
            // 
            this.checkBoxOriginal.Location = new System.Drawing.Point(168, 48);
            this.checkBoxOriginal.Name = "checkBoxOriginal";
            this.checkBoxOriginal.Size = new System.Drawing.Size(72, 24);
            this.checkBoxOriginal.TabIndex = 8;
            this.checkBoxOriginal.Text = "Original";
            this.toolTip1.SetToolTip(this.checkBoxOriginal, "Controls the original bit of MP3 stream");
            this.checkBoxOriginal.CheckedChanged += new System.EventHandler(this.Control_Changed);
            // 
            // checkBoxPrivate
            // 
            this.checkBoxPrivate.Location = new System.Drawing.Point(248, 48);
            this.checkBoxPrivate.Name = "checkBoxPrivate";
            this.checkBoxPrivate.Size = new System.Drawing.Size(72, 24);
            this.checkBoxPrivate.TabIndex = 9;
            this.checkBoxPrivate.Text = "Private";
            this.toolTip1.SetToolTip(this.checkBoxPrivate, "Controls the private bit of MP3 stream");
            this.checkBoxPrivate.CheckedChanged += new System.EventHandler(this.Control_Changed);
            // 
            // buttonStandardCBR
            // 
            this.buttonStandardCBR.Location = new System.Drawing.Point(179, 24);
            this.buttonStandardCBR.Name = "buttonStandardCBR";
            this.buttonStandardCBR.Size = new System.Drawing.Size(133, 23);
            this.buttonStandardCBR.TabIndex = 15;
            this.buttonStandardCBR.Text = "Standard feste Bitrate";
            this.buttonStandardCBR.UseVisualStyleBackColor = true;
            this.buttonStandardCBR.Click += new System.EventHandler(this.buttonStandardCBR_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonStandardCBR);
            this.tabPage2.Controls.Add(this.checkBoxPrivate);
            this.tabPage2.Controls.Add(this.checkBoxOriginal);
            this.tabPage2.Controls.Add(this.checkBoxCRC);
            this.tabPage2.Controls.Add(this.checkBoxCopyRight);
            this.tabPage2.Controls.Add(this.checkBoxVBR);
            this.tabPage2.Controls.Add(this.groupBoxVBR);
            this.tabPage2.Controls.Add(this.comboBoxBitRate);
            this.tabPage2.Controls.Add(this.textBoxMpegVersion);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(328, 254);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "MP3 config";
            // 
            // EditMp3Writer
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "EditMp3Writer";
            this.Size = new System.Drawing.Size(336, 280);
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.groupBoxVBR.ResumeLayout(false);
            this.groupBoxVBR.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVBRQuality)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        #region IEditAudioWriterConfig Members

        public AudioWriterConfig Config
        {
            get
            {
                Big3.Hitbase.MediaRipper.Lame.BE_CONFIG cfg = new Big3.Hitbase.MediaRipper.Lame.BE_CONFIG(editFormat1.Format, uint.Parse(comboBoxBitRate.SelectedItem.ToString()));
                cfg.format.lhv1.bCopyright = checkBoxCopyRight.Checked ? 1 : 0;
                cfg.format.lhv1.bCRC = checkBoxCRC.Checked ? 1 : 0;
                cfg.format.lhv1.bOriginal = checkBoxOriginal.Checked ? 1 : 0;
                cfg.format.lhv1.bPrivate = checkBoxPrivate.Checked ? 1 : 0;
                /*
                dwStructVersion - version of the structure, should be set to 1.
                dwStructSize - size of the overall BE_CONFIG structure, in this case 331 bytes.
                dwSampleRate - sample rate in Hz for the MP3 file. This can be set to one of: 
                32000, 44100 or 48000 for MPEG-I
                16000, 22050 or 24000 for MPEG-I
                8000, 11025 or 12000 for MPEG-II.5
                dwReSampleRate - specifes the resampling rate to use against the input stream. Setting this to 0 causes the encoder to decide the rate. 
                nMode - the stereo mode of the Mp3 file. This can be one of: 
                BE_MP3_MODE_STEREO
                BE_MP3_MODE_JSTEREO
                BE_MP3_MODE_DUALCHANNEL
                BE_MP3_MODE_MONO
                dwBitRate - specifies the bit rate to use. For Constant Bit-Rate (CBR) encoding, this is the actual bit rate, whereas for Variable Bit-Rate (VBR) encoding it specifies the minimum. Allowable values are: 
                8 - MPEG-II only
                16 - MPEG-II only
                24 - MPEG-II only
                32
                40
                48
                56
                64
                80
                96
                112
                128
                144 - MPEG-II only
                160
                192 - MPEG-I only.
                224 - MPEG-I only.
                256 - MPEG-I only.
                320 - MPEG-I only.
                dwMaxBitrate - When VBR mode is enabled, this specifies the maximum allowed bitrate. The same values as dwBitRate are used.For CBR mode this setting is ignored.
                nPreset - one of the preset encoding options to use. Note that presets can overwrite the other settings, since the call to apply the preset's setting is made just before the encoder is initialised. 
                LQP_NOPRESET - don't use any presets
                LQP_NORMAL_QUALITY - quality is set to 5.
                LQP_LOW_QUALITY - quality is set to 9.
                LQP_HIGH_QUALITY - quality is set to 2.
                LQP_VOICE_QUALITY - use for voice encoding.
                LQP_R3MIX - r3mix preset option
                LQP_VERYHIGH_QUALITY - quality is set to 0
                LQP_STANDARD - lame command line alt-preset standard
                LQP_FAST_STANDARD - lame command line alt-preset fast standard
                LQP_EXTREME - lame command line alt-preset extreme
                LQP_FAST_EXTREME - lame command line alt-preset fast extreme.
                LQP_INSANE - lame command line alt-preset insane.
                LQP_ABR - lame command line alt-preset abr.
                LQP_CBR - lame command line alt-preset cbr.
                Note that some presets configure all of the settings, whereas others only affect parts of the configuration. For example, LQP_HIGH_QUALITY does not affect the bit-rate or sampling frequency, whereas LQP_STANDARD will set VBR mode and the bit rate to 200-240kbps. 
                dwMpegVersion - the MPEG version to use, one of 
                MPEG1 = 1
                MPEG2 = 0
                dwPsyModel - this parameter does not appear to be used currently. Assumption is that it would be for adjusting a parameter to the Psycho-Acoustic model.
                dwEmphasis - - this parameter does not appear to be used currently. Assumption is to configure the emphasis amount.
                bPrivate - If this is set to TRUE (1) the Private bit in the MP3 stream will be set.
                bCRC - Set this to TRUE in order to enable CRC-checksum in the bitstream.
                bCopyright - If this is set to TRUE the Copyright bit in the MP3 stream will be set.
                bOriginal - If this is set to TRUE the Original bit in the MP3 stream will be set.
                bWriteVBRHeader - Specifes if the XING VBR header should be written or not. When this option is enabled, you have to call the beWriteVBRHeader function when encoding has been completed. Keep in mind that the VBR info tag can also be written for CBR encoded files, the TAG info can be useful for additional info like encoder delay and the like.
                bEnableVBR - Whether to enable VBR encoding.
                nVBRQuality - VBR quality option, one of 
                VBR_QUALITY_0_HIGHEST = 0
                VBR_QUALITY_1_HIGH = 1
                VBR_QUALITY_2_HIGH = 2
                VBR_QUALITY_3_MID = 3
                VBR_QUALITY_4_MID = 4
                VBR_QUALITY_5_MID = 5
                VBR_QUALITY_6_MID = 6
                VBR_QUALITY_7_LOW = 7
                VBR_QUALITY_8_LOW = 8
                VBR_QUALITY_9_LOWEST = 9
                dwVbrAbr_bps - specifies an Average Bit Rate (ABR) encoding. If this is specified, the lame encoder ignores the nVBRQuality settings. To use this, bEnableVBR must be set to TRUE and the nVbrMethod parameter should be set to VBR_METHOD_ABR). The allowed range for the average bit rate is an integer value between: 
                MPEG-I: 32000 .. 320000 bps
                MPEG-II: 8000 .. 160000 bps
                nVBRMethod - the VBR method to use. One of 
                VBR_METHOD_NONE = -1
                VBR_METHOD_DEFAULT = 0
                VBR_METHOD_OLD = 1
                VBR_METHOD_NEW = 2
                VBR_METHOD_ABR = 4
                bNoBitRes - Disables the bit-resorvoir and disables the insertion of padded frames.
                bStrictIso - Set strict ISO compatibility for the encoding.
                nQuality - Set the quality, from 0 (highest) to 9 (lowest). For backward compatibility reasons, the quality value must have the high-byte set to Not the low-byte. So the values will be: &HFF00 = 0, &HFE01 = 1 ... &HF609 = 9.
                bPadding - Not used, provided for future expansion.
                 */
                
                if (checkBoxVBR.Checked)
                {
                    cfg.format.lhv1.bEnableVBR = 1;
                    cfg.format.lhv1.nVbrMethod = (Lame.VBRMETHOD)(comboBoxVBRMethod.SelectedIndex - 1);
                    /*          if (comboBoxVBRMethod.SelectedIndex > 0)
                              {
                                cfg.format.lhv1.nVbrMethod = (Lame.VBRMETHOD)(comboBoxVBRMethod.SelectedIndex-1);
                              }
                              else
                              {
                                cfg.format.lhv1.nVbrMethod = Lame.VBRMETHOD.VBR_METHOD_DEFAULT;
                              }*/
                    cfg.format.lhv1.dwMaxBitrate = uint.Parse(comboBoxMaxBitRate.SelectedItem.ToString());
                    if (cfg.format.lhv1.dwMaxBitrate < cfg.format.lhv1.dwBitrate)
                    {
                        cfg.format.lhv1.dwMaxBitrate = cfg.format.lhv1.dwBitrate;
                    }
                    cfg.format.lhv1.dwVbrAbr_bps = uint.Parse(comboBoxAvgBitrate.SelectedItem.ToString());
                    if (cfg.format.lhv1.dwVbrAbr_bps < cfg.format.lhv1.dwBitrate)
                    {
                        cfg.format.lhv1.dwVbrAbr_bps = cfg.format.lhv1.dwBitrate;
                    }
                    cfg.format.lhv1.nVBRQuality = trackBarVBRQuality.Value;
                }
                else
                {
                    cfg.format.lhv1.bEnableVBR = 0;
                }
                return new Mp3WriterConfig(editFormat1.Format, cfg);
            }
            set
            {
                editFormat1.Format = value.Format;
                m_Config = ((Mp3WriterConfig)value).Mp3Config;
                DoSetInitialValues();
            }
        }

        #endregion

        #region IConfigControl Members

        public void DoApply()
        {
            // TODO:  Add EditMp3Writer.DoApply implementation
        }

        public string ControlName
        {
            get
            {
                return "MP3 Writer config";
            }
        }

        public event System.EventHandler ConfigChange;

        public Control ConfigControl
        {
            get
            {
                return this;
            }
        }

        public void DoSetInitialValues()
        {
            m_FireConfigChangeEvent = false;
            try
            {
                int i;
                string[] rates;
                Lame.LHV1 hv = m_Config.format.lhv1;
                editFormat1.DoSetInitialValues();
                if (hv.dwMpegVersion == Lame.LHV1.MPEG2)
                {
                    textBoxMpegVersion.Text = "MPEG2";
                    rates = Mpeg2BitRates.Split(',');
                }
                else
                {
                    textBoxMpegVersion.Text = "MPEG1";
                    rates = Mpeg1BitRates.Split(',');
                }
                comboBoxBitRate.Items.Clear();
                comboBoxBitRate.Items.AddRange(rates);
                comboBoxMaxBitRate.Items.Clear();
                comboBoxMaxBitRate.Items.AddRange(rates);
                comboBoxAvgBitrate.Items.Clear();
                comboBoxAvgBitrate.Items.AddRange(rates);

                i = comboBoxBitRate.Items.IndexOf(hv.dwBitrate.ToString());
                comboBoxBitRate.SelectedIndex = i;

                i = comboBoxAvgBitrate.Items.IndexOf(hv.dwVbrAbr_bps.ToString());
                comboBoxAvgBitrate.SelectedIndex = i;

                i = comboBoxMaxBitRate.Items.IndexOf(hv.dwMaxBitrate.ToString());
                comboBoxMaxBitRate.SelectedIndex = i;

                checkBoxCopyRight.Checked = hv.bCopyright != 0;
                checkBoxCRC.Checked = hv.bCRC != 0;
                checkBoxOriginal.Checked = hv.bOriginal != 0;
                checkBoxPrivate.Checked = hv.bPrivate != 0;
                comboBoxVBRMethod.SelectedIndex = (int)hv.nVbrMethod + 1;
                if ((hv.nVBRQuality >= 0) && (hv.nVBRQuality <= 9))
                {
                    trackBarVBRQuality.Value = hv.nVBRQuality;
                }
                else
                {
                    trackBarVBRQuality.Value = 0;
                }
                checkBoxVBR.Checked = groupBoxVBR.Enabled = hv.bEnableVBR != 0;
            }
            finally
            {
                m_FireConfigChangeEvent = true;
            }
        }

        #endregion

        protected virtual void DoConfigChange(System.EventArgs e)
        {
            if (m_FireConfigChangeEvent && (ConfigChange != null))
            {
                ConfigChange(this, e);
            }
        }

        private void checkBoxVBR_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBoxVBR.Checked)
            {
                groupBoxVBR.Enabled = true;
                comboBoxBitRate.SelectedIndex = comboBoxBitRate.Items.IndexOf("128");

                comboBoxAvgBitrate.SelectedIndex = comboBoxAvgBitrate.Items.IndexOf("192");

                comboBoxMaxBitRate.SelectedIndex = comboBoxMaxBitRate.Items.IndexOf("320");
                comboBoxVBRMethod.SelectedIndex = comboBoxVBRMethod.Items.IndexOf("NEW");
                checkBoxCRC.Checked = false;
                checkBoxCopyRight.Checked = false;
                checkBoxOriginal.Checked = false;
                checkBoxPrivate.Checked = false;
                trackBarVBRQuality.Value = 0;
            }
            else
            {
                groupBoxVBR.Enabled = false;
            }
            DoConfigChange(e);
        }

        private void Control_Changed(object sender, System.EventArgs e)
        {
            DoConfigChange(e);
        }

        private void BitRateChange(object sender, System.EventArgs e)
        {
            if (comboBoxMaxBitRate.SelectedIndex < comboBoxBitRate.SelectedIndex)
            {
                comboBoxMaxBitRate.SelectedIndex = comboBoxBitRate.SelectedIndex;
            }
            DoConfigChange(e);
        }

        private void comboBoxVBRMethod_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (checkBoxVBR.Checked && (comboBoxVBRMethod.SelectedIndex == 0))
            {
                comboBoxVBRMethod.SelectedIndex = 1;
            }
            DoConfigChange(e);
        }

        private void buttonStandard_Click(object sender, EventArgs e)
        {
            checkBoxVBR.Checked = true;
            comboBoxBitRate.SelectedIndex = comboBoxBitRate.Items.IndexOf("128");

            comboBoxAvgBitrate.SelectedIndex = comboBoxAvgBitrate.Items.IndexOf("192");

            comboBoxMaxBitRate.SelectedIndex = comboBoxMaxBitRate.Items.IndexOf("320");
            comboBoxVBRMethod.SelectedIndex = comboBoxVBRMethod.Items.IndexOf("NEW");
            checkBoxCRC.Checked = false;
            checkBoxCopyRight.Checked = false;
            checkBoxOriginal.Checked = false;
            checkBoxPrivate.Checked = false; 
            trackBarVBRQuality.Value = 0;
        }

        private void buttonStandardCBR_Click(object sender, EventArgs e)
        {
            checkBoxVBR.Checked = false;

            comboBoxBitRate.SelectedIndex = comboBoxBitRate.Items.IndexOf("192");
            checkBoxCRC.Checked = false;
            checkBoxCopyRight.Checked = false;
            checkBoxOriginal.Checked = false;
            checkBoxPrivate.Checked = false;
        }
    }
}
