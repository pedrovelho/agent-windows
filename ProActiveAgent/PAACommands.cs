using System;
using System.Collections.Generic;
using System.Text;

/** Enumeration type of ProActive Agent Service custom commands
 * 
 *  There are following commands:
 *      Start - trigger action
 *      Stop  - stops triggered action or do nothing when it has not been triggered
 */

namespace ProActiveAgent
{
    public enum PAACommands
    {
        ScreenSaverStart = 128,
        ScreenSaverStop = 129,
        GlobalStop = 130
        /*AllowRuntime = 131,
        ForbidRuntime = 132*/
    }
}
