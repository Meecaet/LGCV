﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCV_Fiches.Models.CVViewModels
{
    public class PerfectionnementViewModel : ViewModel
    {
        public string GraphIdGenre { get; set; }
        public string Description { get; set; }
        public int Annee { get; set; }
    }
}
