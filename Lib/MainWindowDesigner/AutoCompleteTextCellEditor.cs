/*
 * Copyright © 2005, Mathew Hall
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 *
 *    - Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 * 
 *    - Redistributions in binary form must reproduce the above copyright notice, 
 *      this list of conditions and the following disclaimer in the documentation 
 *      and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
 * OF SUCH DAMAGE.
 */


using System;
using System.Drawing;
using System.Windows.Forms;

using XPTable.Events;
using XPTable.Models;
using XPTable.Win32;
using Big3.Hitbase.DataBaseEngine;


namespace XPTable.Editors
{
	/// <summary>
	/// A class for editing Cells that contain strings
	/// </summary>
	public class AutoCompleteTextCellEditor : CellEditor
	{
		#region Constructor
		
		/// <summary>
		/// Initializes a new instance of the TextCellEditor class with default settings
		/// </summary>
        public AutoCompleteTextCellEditor(Field field, DataBase db)
            : base()
		{
            AutoCompleteTextBox textbox = new AutoCompleteTextBox();
			textbox.AutoSize = false;
			textbox.BorderStyle = BorderStyle.None;
            textbox.DataBase = db;

			this.Control = textbox;
		}

		#endregion


		#region Methods

		/// <summary>
		/// Sets the location and size of the CellEditor
		/// </summary>
		/// <param name="cellRect">A Rectangle that represents the size and location 
		/// of the Cell being edited</param>
		protected override void SetEditLocation(Rectangle cellRect)
		{
			this.TextBox.Location = cellRect.Location;
			this.TextBox.Size = new Size(cellRect.Width-1, cellRect.Height-1);
		}


		/// <summary>
		/// Sets the initial value of the editor based on the contents of 
		/// the Cell being edited
		/// </summary>
		protected override void SetEditValue()
		{
			this.TextBox.Text = this.EditingCell.Text;
		}


		/// <summary>
		/// Sets the contents of the Cell being edited based on the value 
		/// in the editor
		/// </summary>
		protected override void SetCellValue()
		{
			this.EditingCell.Text = this.TextBox.Text;
		}


		/// <summary>
		/// Starts editing the Cell
		/// </summary>
		public override void StartEditing()
		{
			this.TextBox.KeyPress += new KeyPressEventHandler(OnKeyPress);
            this.TextBox.KeyDown += new KeyEventHandler(TextBox_KeyDown);
			this.TextBox.LostFocus += new EventHandler(OnLostFocus);

            Field field = ((AutoCompleteTextColumn)this.EditingTable.ColumnModel.Columns[this.EditingCell.Index]).Field;

            if (field == Field.TrackTitle)
                ((AutoCompleteTextBox)this.TextBox).AutoCompleteTextBoxType = AutoCompleteTextBoxType.TrackTitle;
            if (field == Field.ArtistTrackName)
                ((AutoCompleteTextBox)this.TextBox).AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;
            if (field == Field.ComposerTrackName)
                ((AutoCompleteTextBox)this.TextBox).AutoCompleteTextBoxType = AutoCompleteTextBoxType.PersonGroup;


			base.StartEditing();

			this.TextBox.Focus();
		}

        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (this.EditingTable != null)
                {
                    this.EditingTable.StopEditing();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (this.EditingTable != null)
                {
                    this.EditingTable.CancelEditing();
                }
            }
        }


		/// <summary>
		/// Stops editing the Cell and commits any changes
		/// </summary>
		public override void StopEditing()
		{
			this.TextBox.KeyPress -= new KeyPressEventHandler(OnKeyPress);
			this.TextBox.LostFocus -= new EventHandler(OnLostFocus);
			
			base.StopEditing();
		}


		/// <summary>
		/// Stops editing the Cell and ignores any changes
		/// </summary>
		public override void CancelEditing()
		{
			this.TextBox.KeyPress -= new KeyPressEventHandler(OnKeyPress);
			this.TextBox.LostFocus -= new EventHandler(OnLostFocus);
			
			base.CancelEditing();
		}


		#endregion


		#region Properties

		/// <summary>
		/// Gets the TextBox used to edit the Cells contents
		/// </summary>
		public TextBox TextBox
		{
			get
			{
				return this.Control as TextBox;
			}
		}

		#endregion


		#region Events

		/// <summary>
		/// Handler for the editors TextBox.KeyPress event
		/// </summary>
		/// <param name="sender">The object that raised the event</param>
		/// <param name="e">A KeyPressEventArgs that contains the event data</param>
		protected virtual void OnKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == AsciiChars.CarriageReturn /*Enter*/)
			{
				if (this.EditingTable != null)
				{
					this.EditingTable.StopEditing();
				}
			}
			else if (e.KeyChar == AsciiChars.Escape)
			{
				if (this.EditingTable != null)
				{
					this.EditingTable.CancelEditing();
				}
			}
		}


		/// <summary>
		/// Handler for the editors TextBox.LostFocus event
		/// </summary>
		/// <param name="sender">The object that raised the event</param>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected virtual void OnLostFocus(object sender, EventArgs e)
		{
			if (this.EditingTable != null)
			{
				this.EditingTable.StopEditing();
			}
		}

		#endregion
	}
}
