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
