using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayambang_ExtractData
{
    public class Demographic
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string suffix { get; set; }
        public string gender { get; set; }
        public string civilStatus { get; set; }
        public string dateOfBirth { get; set; }
        public string employmentStatus { get; set; }
        public string registeredVoter { get; set; }
        public string tIN { get; set; }
        public string country_BirthAddress { get; set; }
        public string province_BirthAddress { get; set; }
        public string city_BirthAddress { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public string distinguishingMark { get; set; }
        public string firstName_Father { get; set; }
        public string middleName_Father { get; set; }
        public string lastName_Father { get; set; }
        public string suffix_Father { get; set; }
        public string firstName_Mother { get; set; }
        public string middleName_Mother { get; set; }
        public string lastName_Mother { get; set; }
        public string suffix_Mother { get; set; }
        public string firstName_Spouse { get; set; }
        public string middleName_Spouse { get; set; }
        public string lastName_Spouse { get; set; }
        public string suffix_Spouse { get; set; }
        public string noOfChildren { get; set; }
        public string roomFloorUnitBldg { get; set; }
        public string houseLotBlock { get; set; }
        public string streetname { get; set; }
        public string subdivision { get; set; }
        public string barangay { get; set; }
        public string district { get; set; }
        public string country_Address { get; set; }
        public string province_Address { get; set; }
        public string city_Address { get; set; }
        public string postal { get; set; }
        public string mobileNos { get; set; }
        public string telephoneNos { get; set; }
        public string emailAddress { get; set; }
        public string firstName_Contact { get; set; }
        public string middleName_Contact { get; set; }
        public string lastName_Contact { get; set; }
        public string suffix_Contact { get; set; }
        public string relation { get; set; }
        public string roomFloorUnitBldg_Contact { get; set; }
        public string houseLotBlock_Contact { get; set; }
        public string streetname_Contact { get; set; }
        public string subdivision_Contact { get; set; }
        public string barangay_Contact { get; set; }
        public string district_Contact { get; set; }
        public string country_Address_Contact { get; set; }
        public string province_Address_Contact { get; set; }
        public string city_Address_Contact { get; set; }
        public string postal_Contact { get; set; }
        public string mobileNos_Contact { get; set; }
        public string telephoneNos_Contact { get; set; }
        public string emailAddress_Contact { get; set; }
        public string recardReason { get; set; }
        public string leftPrimaryFingerCode { get; set; }
        public string leftThumbFingerCode { get; set; }
        public string rightPrimaryFingerCode { get; set; }
        public string rightThumbFingerCode { get; set; }
        public string photoOverride { get; set; }
        public string signatureOverride { get; set; }
        public string photoICAO { get; set; }
        public string photoScore { get; set; }
        public string sessionReference { get; set; }
        public string timestamp { get; set; }
        public string operatorID { get; set; }
        public string terminalName { get; set; }

        public string path { get; set; }
        public string urn { get; set; }
        public string backOcr { get; set; }
        public string refSource { get; set; }

        public string photoExt { get; set; }
        public string photoBase64 { get; set; }
        public string signatureExt { get; set; }
        public string signatureBase64 { get; set; }

        //wsq
        public string leftBackupWsqBase64 { get; set; }
        public string leftPrimaryWsqBase64 { get; set; }
        public string leftMiddleWsqBase64 { get; set; }
        public string leftRingWsqBase64 { get; set; }
        public string leftPinkyWsqBase64 { get; set; }

        public string rightBackupWsqBase64 { get; set; }
        public string rightPrimaryWsqBase64 { get; set; }
        public string rightMiddleWsqBase64 { get; set; }
        public string rightRingWsqBase64 { get; set; }
        public string rightPinkyWsqBase64 { get; set; }

        //ansi
        public string leftBackupAnsiBase64 { get; set; }
        public string leftPrimaryAnsiBase64 { get; set; }
        public string leftMiddleAnsiBase64 { get; set; }
        public string leftRingAnsiBase64 { get; set; }
        public string leftPinkyAnsiBase64 { get; set; }

        public string rightBackupAnsiBase64 { get; set; }
        public string rightPrimaryAnsiBase64 { get; set; }
        public string rightMiddleAnsiBase64 { get; set; }
        public string rightRingAnsiBase64 { get; set; }
        public string rightPinkyAnsiBase64 { get; set; }

        //jpg
        public string leftBackupJpgBase64 { get; set; }
        public string leftPrimaryJpgBase64 { get; set; }
        public string leftMiddleJpgBase64 { get; set; }
        public string leftRingJpgBase64 { get; set; }
        public string leftPinkyJpgBase64 { get; set; }

        public string rightBackupJpgBase64 { get; set; }
        public string rightPrimaryJpgBase64 { get; set; }
        public string rightMiddleJpgBase64 { get; set; }
        public string rightRingJpgBase64 { get; set; }
        public string rightPinkyJpgBase64 { get; set; }
        
    }
}
