﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldairarrow.Entity.D_Manage
{
    /// <summary>
    /// 通告读取标记
    /// </summary>
    [Table("D_NoticeReadingMarks")]
    public class D_NoticeReadingMarks : BaseEntity
    {
        public string NoticeId { get; set; }
    }
}
