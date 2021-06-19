using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayambang_ExtractData
{
    class Biometric
    {
        public string memberId { get; set; }
        public string base64Legacy { get; set; }
        public string extLegacy { get; set; }
        public string base64Jpg { get; set; }
        public string base64Ansi { get; set; }
        public string base64Wsq { get; set; }
        public bool isOverride { get; set; }
        public short fpPosition { get; set; }
        public string fpCode { get; set; }

        public Biometric()
        {
            base64Legacy = "";
            extLegacy = "";
            base64Jpg = "";
            base64Ansi = "";
            base64Wsq = "";
            isOverride = false;            
        }
    }
}
