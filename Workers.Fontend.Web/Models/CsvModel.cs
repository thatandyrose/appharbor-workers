using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Workers.Fontend.Web.Models
{
    public class CsvModel
    {
        public CsvModel()
        {
            Rows = new List<CsvRow>();
        }
        public List<CsvRow> Rows { get; set; }
        public string Filename { get; set; }
    }
    public class CsvRow
    {
        public CsvRow()
        {
            Columns = new List<CsvColumn>();
        }
        public IEnumerable<CsvColumn> Columns { get; set; }
    }
    public class CsvColumn
    {
        public CsvColumn(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}