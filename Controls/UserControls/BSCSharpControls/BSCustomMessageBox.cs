using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Biosystems.Ax00.Controls.UserControls
{
    public class BSCustomMessageBox
    {
        public enum BSMessageBoxDefaultButton
        {
            LeftButton,
            MiddleButton,
            RightButton
        }
        /// <summary>
        /// This user control will manage the FlexibleMessageBox properties so that it displays the 
        /// correct information in each button.
        /// </summary>
        /// <param name="owner">This is the messagebox's parent form. It is used to control the location of the Box.</param>
        /// <param name="text">This is the message that you want to display to the user.</param>
        /// <param name="caption">This is the Text on the form's header (Title bar).</param>
        /// <param name="buttons">This is set of buttons to be displayed.</param>
        /// <param name="icon">The desired MessageBoxIcon. </param>
        /// <param name="defaultButton">This parameter represents the button that will be focused when the messagebox appears. </param>
        /// <param name="leftButtonText">The text to display in the left side button.</param>
        /// <param name="middleButtonText">The text to display in the middle button.</param>
        /// <param name="rightButtonText">The text to display in the right button.</param>
        /// <returns>DialogResult with the selected button. </returns>
        /// <remarks>BSCustomMessageBox created by CF - 18/10/2013 - Ax00 v3.0.0
        /// Original FlexibleMessageBox by Jörg Reichert, code published at http://www.codeproject.com/Articles/601900/FlexibleMessageBox
        /// Modified to the needs and requirements of BioSystems. 
        /// </remarks>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, string leftButtonText = "", string middleButtonText = "", string rightButtonText = "")
        {
           
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                case MessageBoxButtons.YesNoCancel:
                    return FlexibleMessageBox.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button3, leftButtonText, middleButtonText, rightButtonText);
                case MessageBoxButtons.OK:
                    return FlexibleMessageBox.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button3, string.Empty, string.Empty, leftButtonText);
                case MessageBoxButtons.OKCancel:
                case MessageBoxButtons.RetryCancel:
                case MessageBoxButtons.YesNo:
                    return FlexibleMessageBox.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button3, string.Empty, leftButtonText, middleButtonText);
                default:
                    return FlexibleMessageBox.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button3);

            }
        }

        //JV 29/10/2013 Option to define the default button
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, BSMessageBoxDefaultButton defaultButton, string leftButtonText = "", string middleButtonText = "", string rightButtonText = "")
        {
            MessageBoxDefaultButton button;
            switch (defaultButton)
            {
                case BSMessageBoxDefaultButton.LeftButton:
                    button = MessageBoxDefaultButton.Button1;
                    break;
                case BSMessageBoxDefaultButton.MiddleButton:
                    button = MessageBoxDefaultButton.Button2;
                    break;
                case BSMessageBoxDefaultButton.RightButton:
                default:
                    button = MessageBoxDefaultButton.Button3;
                    break;
            }
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                case MessageBoxButtons.YesNoCancel:
                    return FlexibleMessageBox.Show(owner, text, caption, buttons, icon, button, leftButtonText, middleButtonText, rightButtonText);
                case MessageBoxButtons.OK:
                    return FlexibleMessageBox.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button3, leftButtonText, middleButtonText, rightButtonText);
                case MessageBoxButtons.OKCancel:
                case MessageBoxButtons.RetryCancel:
                case MessageBoxButtons.YesNo:
                    if (button == MessageBoxDefaultButton.Button1) button = MessageBoxDefaultButton.Button2;
                    else if (button == MessageBoxDefaultButton.Button3) button = MessageBoxDefaultButton.Button1;
                    return FlexibleMessageBox.Show(owner, text, caption, buttons, icon, button, leftButtonText, middleButtonText, rightButtonText);
                default:
                    return FlexibleMessageBox.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button3);
            }
        }
        //JV 29/10/2013
    }
}
