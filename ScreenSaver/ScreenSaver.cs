using System;
using System.Windows.Forms;

/**
 * Main screensaver class conforming to screen saver requirements as for command line parameters
 */

namespace ScreenSaver
{
	public class DotNETScreenSaver
	{
		[STAThread]
		static void Main(string[] args) 
		{
			if (args.Length > 0)
			{
                // we don't have configuration dialog
				if (args[0].ToLower().Trim().Substring(0,2) == "/c")
				{
					MessageBox.Show("This Screen Saver has no options you can set.", "ProActive Agent Screensaver", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else if (args[0].ToLower() == "/s")
				{
					System.Windows.Forms.Application.Run(new ScreenSaverForm());
				}
			}
			else
			{
				System.Windows.Forms.Application.Run(new ScreenSaverForm());
			}
		}
	}
}
