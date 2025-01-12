﻿using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Coldairarrow.Entity.OA_Manage
{
    /// <summary>
    /// OA表单流程
    /// </summary>
    [PushMessageType]
    [Table("OA_UserForm")]
    public class OA_UserForm : MessageBaseEntity
    {        
        [MaxLength(50)]
        public string DefFormId { get; set; }
        [MaxLength(255)]
        public string DefFormName { get; set; }
        [MaxLength(50)]
        public string DefFormJsonId { get; set; }
        public int DefFormJsonVersion { get; set; }
        public int Grade { get; set; }
        public double Flag { get; set; }
        public string Remarks { get; set; }
        public string Appendix { get; set; }
        public string ExtendJSON { get; set; }
        [MaxLength(255)]
        public string ApplicantUser { get; set; }
        [MaxLength(50)]
        public string ApplicantUserId { get; set; }
        [MaxLength(255)]
        public string ApplicantDepartment { get; set; }
        [MaxLength(50)]
        public string ApplicantDepartmentId { get; set; }
        [MaxLength(255)]
        public string ApplicantRole { get; set; }
        [MaxLength(50)]
        public string ApplicantRoleId { get; set; }     
        
        public string UserRoleNames { get; set; }
        public string UserRoleIds { get; set; }
        public string AlreadyUserNames { get; set; }
        public string AlreadyUserIds { get; set; }
        public int Status { get; set; }
        [MaxLength(50)]
        public string Type { get; set; }
        [MaxLength(50)]
        public string SubType { get; set; }
        [MaxLength(50)]
        public string Unit { get; set; }
        public DateTime? ExpectedDate { get; set; }
        [MaxLength(500)]
        public string CurrentNode { get; set; }

    }
}
