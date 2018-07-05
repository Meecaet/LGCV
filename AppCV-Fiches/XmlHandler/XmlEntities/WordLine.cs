using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace XmlHandler.XmlEntities
{
    public class WordLine
    {
        private List<XmlDocRow> rows;

        public WordLine()
        {
            rows = new List<XmlDocRow>();
        }

        public void AddRow(XmlDocRow row)
        {
            rows.Add(row);
        }

        public string GetText()
        {
            StringBuilder sb = new StringBuilder();
            foreach (XmlDocRow row in rows)
            {
                sb.Append(row.GetText());
            }

            return sb.ToString();
        }

        public bool IsBold()
        {
            return rows.Any(x => x.IsBold());
        }

        public bool IsItalic()
        {
            return rows.Any(x => x.IsItalic());
        }
    }
}
