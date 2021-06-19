using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayambang_ExtractData
{

    [Serializable, System.Xml.Serialization.XmlRoot("MemberData")]
    public class MemberData
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Gender { get; set; }
        public string CivilStatus { get; set; }
        public string DateOfBirth { get; set; }
        public string EmploymentStatus { get; set; }
        public string RegisteredVoter { get; set; }
        public string TIN { get; set; }
        public string Country_BirthAddress { get; set; }
        public string Province_BirthAddress { get; set; }
        public string City_BirthAddress { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string DistinguishingMark { get; set; }
        public string FirstName_Father { get; set; }
        public string MiddleName_Father { get; set; }
        public string LastName_Father { get; set; }
        public string Suffix_Father { get; set; }
        public string FirstName_Mother { get; set; }
        public string MiddleName_Mother { get; set; }
        public string LastName_Mother { get; set; }
        public string Suffix_Mother { get; set; }
        public string FirstName_Spouse { get; set; }
        public string MiddleName_Spouse { get; set; }
        public string LastName_Spouse { get; set; }
        public string Suffix_Spouse { get; set; }
        public string NoOfChildren { get; set; }
        public string RoomFloorUnitBldg { get; set; }
        public string HouseLotBlock { get; set; }
        public string Streetname { get; set; }
        public string Subdivision { get; set; }
        public string Barangay { get; set; }
        public string District { get; set; }
        public string Country_Address { get; set; }
        public string Province_Address { get; set; }
        public string City_Address { get; set; }
        public string Postal { get; set; }
        public string MobileNos { get; set; }
        public string TelephoneNos { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName_Contact { get; set; }
        public string MiddleName_Contact { get; set; }
        public string LastName_Contact { get; set; }
        public string Suffix_Contact { get; set; }
        public string Relation { get; set; }
        public string RoomFloorUnitBldg_Contact { get; set; }
        public string HouseLotBlock_Contact { get; set; }
        public string Streetname_Contact { get; set; }
        public string Subdivision_Contact { get; set; }
        public string Barangay_Contact { get; set; }
        public string District_Contact { get; set; }
        public string Country_Address_Contact { get; set; }
        public string Province_Address_Contact { get; set; }
        public string City_Address_Contact { get; set; }
        public string Postal_Contact { get; set; }
        public string MobileNos_Contact { get; set; }
        public string TelephoneNos_Contact { get; set; }
        public string EmailAddress_Contact { get; set; }
        public string RecardReason { get; set; }
        public string LeftPrimaryFingerCode { get; set; }
        public string LeftThumbFingerCode { get; set; }
        public string RightPrimaryFingerCode { get; set; }
        public string RightThumbFingerCode { get; set; }
        public string PhotoOverride { get; set; }
        public string SignatureOverride { get; set; }
        public string PhotoICAO { get; set; }
        public string PhotoScore { get; set; }
        public string SessionReference { get; set; }
        public string Timestamp { get; set; }
        public string OperatorID { get; set; }
        public string TerminalName { get; set; }
    }
}
