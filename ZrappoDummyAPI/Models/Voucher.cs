using System;
using System.Collections;
using System.Collections.Generic;

namespace ContactList.Models
{
    public class Voucher
    {
        public Guid Id { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherText { get; set; }
        public ICollection<VoucherRow> Rows { get; set; }
    }
}