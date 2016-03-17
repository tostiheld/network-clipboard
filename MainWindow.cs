﻿using System;
using System.Text;
using Eto.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NetworkClipboard
{
	public partial class MainWindow : Form
	{
        private Controller controller;

        public MainWindow(Controller c)
		{
            if (c == null)
            {
                throw new ArgumentNullException();
            }
            controller = c;
            controller.NewPaste += Controller_NewPaste;

			Layout();
		}

        private void Controller_NewPaste (string channel, DateTime timestamp, string text)
        {
            TextArea target = null;
            TabPage targetPage = null;
            foreach (TabPage page in tabs.Pages)
            {
                if (page.Text == channel)
                {
                    target = page.Content as TextArea;
                    targetPage = page;
                }
            }

            if (target == null)
            {
                targetPage = AddNewChannel(channel);
                target = targetPage.Content as TextArea;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(timestamp.ToString());
            sb.AppendLine("--------------------");
            sb.AppendLine(text);
            sb.AppendLine("--------------------");
            sb.AppendLine();

            target.Append(sb.ToString(), true);
            return;
        }

        private TabPage AddNewChannel(string name)
        {
            TabPage newPage = new TabPage()
            {
                Text = name,
                Content = ReadOnlyTextArea()
            };
            tabs.Pages.Insert(tabs.Pages.Count - 1, newPage);
            return newPage;
        }

        private void PlusPage_Click (object sender, EventArgs e)
        {
            TextInputDialog input = new TextInputDialog();
            string name = input.AskInput();

            if (!String.IsNullOrWhiteSpace(name))
            {
                AddNewChannel(name);
            }

            tabs.SelectedIndex = tabs.Pages.Count - 2;
        }

		private void QuitCommand_Executed (object sender, EventArgs e)
		{
			Close();
		}

		private void PasteCommand_Executed(object sender, EventArgs e)
		{
			Clipboard c = new Clipboard();
			if (c.Text == null)
			{
				return;
			}
			string text = c.Text;

            if (text.Length > UdpMessenger.MaxBufferSize)
            {
                MessageBox.Show("Paste is too big. Try something smaller");
                return;
            }

            controller.Paste(tabs.SelectedPage.Text, text);
		}
	}
}


