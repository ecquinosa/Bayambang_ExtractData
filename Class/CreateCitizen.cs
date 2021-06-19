using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayambang_ExtractData.Class
{
    class CreateCitizen
    {
        public string isLazyLoading { get; set; }
        public string applicationId { get; set; }
        public string notificationId { get; set; }
        public objGeoLocation geoLocation { get; set; }
        public objPayload payload { get; set; }

        public CreateCitizen()
        {
            isLazyLoading = "false";
            notificationId = "";
            applicationId = "";
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
            public string memberId { get; set; }
            public string leafId { get; set; }
            public string institutionId { get; set; }
            public string lastName { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string suffix { get; set; }
            public string gender { get; set; }
            public string civilStatusId { get; set; }
            public string civilStatus { get; set; }
            public string citizenshipId { get; set; }
            public string citizenship { get; set; }
            public string birthDate { get; set; }
            public string birthCityId { get; set; }
            public string birthCity { get; set; }
            public string birthProvinceId { get; set; }
            public string birthProvince { get; set; }
            public string birthCountryId { get; set; }
            public string birthCountry { get; set; }
            public string height { get; set; }
            public string weight { get; set; }
            public string tin { get; set; }
            public string distinguishingRemarks { get; set; }
            public int noOfChildren { get; set; }
            public string employmentStatusId { get; set; }
            public string employmentStatus { get; set; }
            public bool isRegisteredVoter { get; set; }
            public bool isPwd { get; set; }
            public bool isDependent { get; set; }
            public string parentId { get; set; }
            public string parentName { get; set; }

            public objAddress address { get; set; }
            public objRelationship[] relationship { get; set; }
            public objContact[] contact { get; set; }
            public objContactPerson contactPerson { get; set; }
            public objPhoto photo { get; set; }
            public objSignature signature { get; set; }
            public objBiometric[] biometric { get; set; }

            public objPayload()
            {
                memberId = "";
                civilStatusId = "00000000-0000-0000-0000-000000000000";
                citizenshipId = "00000000-0000-0000-0000-000000000000";
                birthCityId = "DA7C463E-F36B-1410-8BF4-0003021A1292";
                birthProvinceId = "1CDB2C1D-11A3-EA11-AA80-0A46F5C1402C";
                birthCountryId = "E5706C7A-ED9F-EA11-AA80-0A46F5C1402C";
                employmentStatusId = "00000000-0000-0000-0000-000000000000";
                parentId = "00000000-0000-0000-0000-000000000000";
                parentName = "";
                isPwd = false;
                isDependent = false;
        }

            public class objAddress
            {
                public string permanentRoomFloorUnitBldg { get; set; }
                public string permanentHouseLotBlock { get; set; }
                public string permanentStreetname { get; set; }
                public string permanentSubdivision { get; set; }
                public string permanentBarangayId { get; set; }
                public string permanentBarangay { get; set; }
                public string permanentCityId { get; set; }
                public string permanentCity { get; set; }
                public string permanentProvinceId { get; set; }
                public string permanentProvince { get; set; }
                public string permanentCountryId { get; set; }
                public string permanentCountry { get; set; }
                public string permanentPostal { get; set; }
                public string permanentDistrictId { get; set; }
                public string permanentDistrict { get; set; }
                public string presentRoomFloorUnitBldg { get; set; }
                public string presentHouseLotBlock { get; set; }
                public string presentStreetname { get; set; }
                public string presentSubdivision { get; set; }
                public string presentBarangayId { get; set; }
                public string presentBarangay { get; set; }
                public string presentCityId { get; set; }
                public string presentCity { get; set; }
                public string presentProvinceId { get; set; }
                public string presentProvince { get; set; }
                public string presentCountryId { get; set; }
                public string presentCountry { get; set; }
                public string presentPostal { get; set; }
                public string presentDistrictId { get; set; }
                public string presentDistrict { get; set; }
                public string fullAddress { get; set; }

                public objAddress()
                {
                    permanentBarangayId = "00000000-0000-0000-0000-000000000000";
                    permanentCityId = "DA7C463E-F36B-1410-8BF4-0003021A1292";
                    permanentProvinceId = "1CDB2C1D-11A3-EA11-AA80-0A46F5C1402C";
                    permanentCountryId = "E5706C7A-ED9F-EA11-AA80-0A46F5C1402C";
                    permanentDistrictId = "00000000-0000-0000-0000-000000000000";
                    presentBarangayId = "00000000-0000-0000-0000-000000000000";
                    presentCityId = "DA7C463E-F36B-1410-8BF4-0003021A1292";
                    presentProvinceId = "1CDB2C1D-11A3-EA11-AA80-0A46F5C1402C";
                    presentCountryId = "E5706C7A-ED9F-EA11-AA80-0A46F5C1402C";
                    presentDistrictId = "00000000-0000-0000-0000-000000000000";
                }

            }

            public class objRelationship
            {
                public string lastName { get; set; }
                public string firstName { get; set; }
                public string middleName { get; set; }
                public string suffix { get; set; }
                public string relationshipTypeId { get; set; }
                public string relationshipType { get; set; }

                public enum enum_relationshipType
                { 
                    Father = 1,
                    Mother,
                    Spouse
                }

                public string GetRelationshipTypeUUID(enum_relationshipType relationshipType)
                {
                    string uuid = "";

                    switch (relationshipType)
                    {
                        case enum_relationshipType.Father:
                            uuid = "1FDFDBEE-6EB1-EA11-AA80-0A75E0AFB438";
                            break;
                        case enum_relationshipType.Mother:
                            uuid =  "20DFDBEE-6EB1-EA11-AA80-0A75E0AFB438";
                            break;
                        case enum_relationshipType.Spouse:
                            uuid = "C8F11960-6FB1-EA11-AA80-0A75E0AFB438";
                            break;
                    }

                    return uuid;
                }
            }

            public class objContact
            {
                public string type { get; set; }
                public string value { get; set; }

                public enum enum_contactType
                {
                    TelephoneNos = 1,
                    MobileNos,
                    Email
                }               

                public string GetContactTypeUUID(enum_contactType contactType)
                {
                    string uuid = "";

                    switch (contactType)
                    {
                        case enum_contactType.TelephoneNos:
                            uuid = "F56998D0-2BAB-EA11-AA80-0A46F5C1402C";
                            break;
                        case enum_contactType.MobileNos:
                            uuid = "F66998D0-2BAB-EA11-AA80-0A46F5C1402C";
                            break;
                        case enum_contactType.Email:
                            uuid = "334EE2D8-2BAB-EA11-AA80-0A46F5C1402C";
                            break;
                    }

                    return uuid;
                }
            }

            public class objContactPerson
            {
                public string lastName { get; set; }
                public string firstName { get; set; }
                public string middleName { get; set; }
                public string suffix { get; set; }
                public string telephoneNos { get; set; }
                public string mobileNos { get; set; }
                public string email { get; set; }
                public string presentRoomFloorUnitBldg { get; set; }
                public string presentHouseLotBlock { get; set; }
                public string presentStreetname { get; set; }
                public string presentSubdivision { get; set; }
                public string presentBarangayId { get; set; }
                public string presentBarangay { get; set; }
                public string presentCityId { get; set; }
                public string presentCity { get; set; }
                public string presentProvinceId { get; set; }
                public string presentProvince { get; set; }
                public string presentCountryId { get; set; }
                public string presentCountry { get; set; }
                public string presentPostal { get; set; }
                public string presentDistrictId { get; set; }
                public string presentDistrict { get; set; }

                public objContactPerson()
                {
                    presentBarangayId = "00000000-0000-0000-0000-000000000000";
                    presentCityId = "00000000-0000-0000-0000-000000000000";
                    presentProvinceId = "00000000-0000-0000-0000-000000000000";
                    presentCountryId = "00000000-0000-0000-0000-000000000000";
                    presentDistrictId = "00000000-0000-0000-0000-000000000000";
                }
            }

            public class objPhoto
            {
                public string memberId { get; set; }
                public string ext { get; set; }
                public string base64 { get; set; }

                public objPhoto()
                {
                    memberId = "";                    
                }
            }

            public class objSignature
            {
                public string memberId { get; set; }
                public string ext { get; set; }
                public string base64 { get; set; }

                public objSignature()
                {
                    memberId = "";
                }
            }

            public class objBiometric
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

                public enum FingerPosition : short
                {
                    RightThumb = 1,
                    RightIndex,
                    RightMiddle,
                    RightRing,
                    RightPinkie,
                    LeftThumb,
                    LeftIndex,
                    LeftMiddle,
                    LeftRing,
                    LeftPinkie
                }

                public objBiometric()
                {
                    memberId = "";
                    base64Legacy = "";
                    extLegacy = "";
                    base64Jpg = "";                    
                    isOverride = false;
                }                

                public string GetFingerCode(short fp)
                {
                    string code = "";
                    switch (fp)
                    {
                        case (short)FingerPosition.RightThumb:
                            code = "t";
                            break;
                        case (short)FingerPosition.RightIndex:
                            code = "i";
                            break;
                        case (short)FingerPosition.RightMiddle:
                            code = "m";
                            break;
                        case (short)FingerPosition.RightRing:
                            code = "r";
                            break;
                        case (short)FingerPosition.RightPinkie:
                            code = "p";
                            break;
                        case (short)FingerPosition.LeftThumb:
                            code = "t";
                            break;
                        case (short)FingerPosition.LeftIndex:
                            code = "i";
                            break;
                        case (short)FingerPosition.LeftMiddle:
                            code = "m";
                            break;
                        case (short)FingerPosition.LeftRing:
                            code = "r";
                            break;
                        case (short)FingerPosition.LeftPinkie:
                            code = "p";
                            break;
                    }

                    return code;
                }
            }

        }
    }
}
