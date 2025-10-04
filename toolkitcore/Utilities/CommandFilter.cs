/*
 * COMMUNITY PRESERVATION NOTICE
 * 
 * Based on: ToolkitCore (https://github.com/harleyknd1/ToolkitCore)
 * License: MIT - Added by SirRandoo on October 4, 2025
 * Original Source: https://github.com/hodlhodl1132/ToolkitCore (abandoned)
 * 
 * MAJOR MODIFICATIONS © 2025 Captolamia:
 * - Complete rewrite of event handlers for TwitchLib 3.1.4 → 3.4.0
 * - Obsoleted deprecated interfaces and methods  
 * - Updated to modern C# patterns and practices
 * 
 * This file contains substantial original work representing a major
 * derivative work. Modifications offered under GNU GPL v3.
 * 
 * Community maintainers have approved continued development.
 *
 * This file is unchanged.
 */

using System.Collections.Generic;

namespace ToolkitCore.Utilities
{
    public class CommandFilter
    {
        public static IEnumerable<string> Parse(string input)
        {
            List<string> stringList = new List<string>();
            string str = "";
            bool flag1 = false;
            bool flag2 = false;
            foreach (char ch in input)
            {
                if (flag2 && !ch.Equals('"'))
                {
                    flag2 = false;
                    str += "\\";
                }
                switch (ch)
                {
                    case ' ':
                        if (!flag1)
                        {
                            stringList.Add(str);
                            str = "";
                            break;
                        }
                        goto default;
                    case '"':
                        if (!flag2)
                        {
                            flag1 = !flag1;
                            break;
                        }
                        str += ch.ToString();
                        flag2 = false;
                        break;
                    case '\\':
                        flag2 = true;
                        break;
                    default:
                        str += ch.ToString();
                        break;
                }
            }
            if (str.Length > 0)
                stringList.Add(str);
            return (IEnumerable<string>)stringList.ToArray();
        }
    }
}
