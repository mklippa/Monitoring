﻿using System;
using System.Collections.Generic;

namespace MonitoringService.Models
{
    public class AgentInfo
    {
        public int AgentInfoId { get; set; }

        public int AgentId { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ReportId { get; set; }

        public Report Report { get; set; }

        public List<Error> Errors { get; set; }
    }
}