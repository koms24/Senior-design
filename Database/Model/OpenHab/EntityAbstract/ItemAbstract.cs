using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.Database.Model.OpenHab.EntityAbstract
{
    public abstract class ItemAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Uid { get; set; } = string.Empty;
    }
}
