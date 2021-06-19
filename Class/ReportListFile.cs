using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayambang_ExtractData.Class
{
    class ReportListFile
    {         
        public string isLazyLoading { get; set; }
        public string applicationId { get; set; }
        public string notificationId { get; set; }
        public objGeoLocation geoLocation { get; set; }
        public objPayload payload { get; set; }

        public ReportListFile()
        {
            isLazyLoading = "false";
            notificationId = "";
            applicationId = "7F0A473E-F36B-1410-8BEB-0003021A1292";            
        }

        public class objGeoLocation
        {
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string dateTime { get; set; }

            public objGeoLocation()
            {
                latitude = "23";
                longitude = "232";
                dateTime = "yyyy-MM-dd HH:mm:ss";
            }
        }

        public class objPayload
        {
            public string lastKey { get; set; }
            public int maxKeys { get; set; }

            public objPayload()
            {
                lastKey = "";
                maxKeys = 10;
            }
        }        
    }
}
