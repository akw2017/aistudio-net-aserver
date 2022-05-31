﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.Base_Manage
{ 
    /// <summary>
     /// 系统字典表
     /// </summary>
    [Table("Base_Dictionary")]
    public class Base_Dictionary : BaseEntity
    {

        /// <summary>
        /// 父级Id
        /// </summary>
        public String ParentId { get; set; }

        /// <summary>
        /// 类型,字典项=0,数据集=1
        /// </summary>
        public DictionaryType Type { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public ControlType ControlType { get; set; } 

        /// <summary>
        /// 数据值
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// Value相同，使用Code区分，暂时没启用
        /// </summary>
        public String Code { get; set; }

        /// <summary>
        /// 显示值
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public String Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

    }
}
