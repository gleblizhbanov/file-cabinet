﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Sex Sex { get; set; }

        public short KidsCount { get; set; }

        public decimal Budget { get; set; }

        public char Currency { get; set; }
    }
}
