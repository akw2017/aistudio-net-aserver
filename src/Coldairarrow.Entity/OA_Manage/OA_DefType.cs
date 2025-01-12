﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coldairarrow.Entity.OA_Manage
{
    /// <summary>
    /// OA表单字段定义
    /// </summary>
    [Table("OA_DefType")]
    public class OA_DefType : BaseEntity
    {
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Type { get; set; }
        [MaxLength(50)]
        public string Unit { get; set; }
    }
}
