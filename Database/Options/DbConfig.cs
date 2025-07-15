using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.Database.Options
{
    public class DbConfig
    {
        public const string SectionName = "DbConfig";
        public string ConnectionString { get; set; }
    }
}
