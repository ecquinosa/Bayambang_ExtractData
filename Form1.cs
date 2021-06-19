using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics.Tracing;
using System.IO;
using System.Web;
using System.Runtime.InteropServices;
using System.Diagnostics.Tracing;
using System.Xml;
using System.Xml.Serialization;

namespace Bayambang_ExtractData
{
    public partial class Form1 : Form
    {

        public enum FingerPosition
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


        public Form1()
        {
            InitializeComponent();
        }        

        private void Form1_Load(object sender, EventArgs e)
        {                  
            CheckForIllegalCrossThreadCalls = false;
            //txtDirectory.Text = @"D:\WORK\FILCONNECT\acc employee data - Copy\consolidatedACC2.txt";
            //txtDirectory.Text = @"D:\WORK\BAYAMBANG\OUTPUT2 - Copy";
            //txtRootUrl.Text = "http://localhost:8080/kyc";
            //txtRootUrl.Text = "http://120.28.145.26:8084/member/kyc"; 
        }

        private string CheckFieldLimit(string obj, int size)
        {
            if (obj == "")
                return obj;
            else if (size > obj.Length)
                return obj;
            else
                return obj.Substring(0, size);
        }

        private void PopulateHeader(string xmlPath)
        {
            XmlTextReader xmlReader = new XmlTextReader(xmlPath);

            StringBuilder sb = new StringBuilder();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == System.Xml.XmlNodeType.Element & xmlReader.Name != "CARD_DATA" & xmlReader.Name != "MemberData")
                {
                    string strField = xmlReader.Name;

                    if (sb.ToString() == "") sb.Append(strField);
                    else sb.Append("|" + strField);
                }
            }

            xmlReader.Close();

            rtb.AppendText(sb.ToString() + Environment.NewLine);
        }

        private void ExtractData(string xmlPath)
        {
            XmlTextReader xmlReader = new XmlTextReader(xmlPath);

            StringBuilder sb = new StringBuilder();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == System.Xml.XmlNodeType.Element & xmlReader.Name != "CARD_DATA" & xmlReader.Name != "MemberData")
                {
                    string strField = xmlReader.Name;
                    string strValue = "";

                    if (strField == "_10")
                    {
                        xmlReader.Read();
                        strValue = xmlReader.Value;
                    }
                    else
                    {
                        xmlReader.Read();
                        strValue = xmlReader.Value.Replace("\r", "").Replace("\n", "").Trim();
                    }

                    if (sb.ToString() == "") sb.Append(strValue);
                    else sb.Append("|" + strValue);
                }
            }

            xmlReader.Close();

            rtb.AppendText(sb.ToString() + Environment.NewLine);
        }

      

        public static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }

        private string Base64ToASCII(byte[] pByteArray)
        {
            return FromHexString(ByteArrayToHexString(pByteArray));
        }

        private string ByteArrayToHexString(byte[] ByteArray)
        {
            string hStr = "";

            for (int i = 0; i <= ByteArray.Length - 1; i++)
                hStr += ByteArray[i].ToString("X2");

            return hStr;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {


            //return;
            //PopulateHeader(txtDirectory.Text + "\\d.xml");
            int cntr = 0;
            foreach (var subDir in System.IO.Directory.GetDirectories(txtDirectory.Text))
            {
                foreach (var file in System.IO.Directory.GetFiles(subDir))
                {
                    if (System.IO.Path.GetExtension(file).ToUpper() == ".XML")
                    {
                        //string folder = subDir.Substring(subDir.LastIndexOf("\\") + 1);
                        try
                        {
                            ExtractData(file);
                            cntr += 1;
                            label2.Text = cntr.ToString();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(System.IO.Path.GetFileName(file) + ". Error " + ex.Message);
                        }
                    }
                }

            }

            MessageBox.Show("Done!");
        }


        public bool Execute_WSDL(string EndPoint, string SoapMessage, ref string SoapResponse, ref string ErrMsg)
        {
            System.Net.WebRequest Request;
            System.Net.WebResponse Response;
            System.IO.Stream DataStream;
            System.IO.StreamReader Reader;
            byte[] SoapByte;
            string SoapStr = SoapMessage;
            bool pSuccess = true;

            try
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                SoapByte = System.Text.Encoding.UTF8.GetBytes(SoapStr);
                Request = System.Net.WebRequest.Create(EndPoint);
                // Request.ContentType = "text/xml; charset=utf-8"
                Request.ContentType = "application/json";
                Request.ContentLength = SoapByte.Length;
                Request.Method = "POST";
                //Request.Timeout = My.Settings.AUB_WS_TIMEOUT * 1000;

                DataStream = Request.GetRequestStream();
                DataStream.Write(SoapByte, 0, SoapByte.Length);
                DataStream.Close();

                Response = Request.GetResponse();

                DataStream = Response.GetResponseStream();
                Reader = new System.IO.StreamReader(DataStream);
                string SD2Request = Reader.ReadToEnd();
                DataStream.Close();
                Reader.Close();
                Response.Close();
                SoapResponse = SD2Request;


                // ServicePointManager.ServerCertificateValidationCallback = Nothing
                return true;
            }
            catch (TimeoutException TimeoutEx)
            {
                ErrMsg = TimeoutEx.Message;
                return false;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                return false;
            }
        }

        public class RefreshTokenResultJSON
        {
            public string access_token { get; set; }
        }

        public bool ExecuteApiRequest(string url, string soapStr, ref string soapResponse, ref string err)
        {
            try
            {
                // -- Refresh the access token
                ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                       SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;

                //System.Net.WebRequest request = System.Net.HttpWebRequest.Create("https://filkonnect.me/auth/realms/filkonnect/protocol/openid-connect/token");
                //System.Net.WebRequest request = System.Net.HttpWebRequest.Create("http://120.28.145.26:9090/auth/realms/D0808731-D59D-EA11-9C14-98541B2295E9/protocol/openid-connect/token");
                System.Net.WebRequest request = System.Net.HttpWebRequest.Create("http://accsandbox.ph:9090/auth/realms/D0808731-D59D-EA11-9C14-98541B2295E9/protocol/openid-connect/token");            
                string accessToken = "OaOXXXXTaSucp8XXcgXXH";

                try
                {
                    if (txtToken.Text == "")
                    {
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";

                        System.Collections.Specialized.NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
                        outgoingQueryString.Add("username", "bayambang@filkonnect.com");
                        outgoingQueryString.Add("password", "4n0T6jeL9oPoxMy3");
                        outgoingQueryString.Add("grant_type", "password");
                        outgoingQueryString.Add("client_id", "aris");
                        byte[] postBytes = new ASCIIEncoding().GetBytes(outgoingQueryString.ToString());

                        Stream postStream = request.GetRequestStream();
                        postStream.Write(postBytes, 0, postBytes.Length);
                        postStream.Flush();
                        postStream.Close();

                        using (System.Net.WebResponse response = request.GetResponse())
                        {
                            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                            {
                                dynamic jsonResponseText = streamReader.ReadToEnd();
                                // Parse the JSON the way you prefer
                                RefreshTokenResultJSON jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponseText, typeof(RefreshTokenResultJSON));
                                accessToken = jsonResult.access_token;

                                txtToken.Text = accessToken;
                                Application.DoEvents();
                                // For more information, refer to the documentation
                            }
                        }
                    }
                }
                catch (Exception ex2)
                {
                    err = "Failed to generate token " + ex2.Message;
                    return false;
                }

                // -- Create some new data   
                //string dataId = "";

                //request = System.Net.HttpWebRequest.Create(url);
                //request.Method = "POST";
                //request.ContentType = "application/json";
                //request.Headers.Add("Authorization", "Bearer " + accessToken);


                //using (System.IO.StreamWriter tStreamWriter = new System.IO.StreamWriter(request.GetRequestStream()))
                //{
                //    tStreamWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(soapStr));
                //}
                //using (System.Net.WebResponse response = request.GetResponse())
                //{
                //    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                //    {
                //        dynamic jsonResponseText = streamReader.ReadToEnd();
                //         The JSON returned in this example is the ID of our newly created data
                //        dataId = int.Parse(jsonResponseText);
                //         For more information, refer to the documentation
                //    }

                //    return true;
                //}

                //return false;


                byte[] SoapByte;

                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                SoapByte = System.Text.Encoding.UTF8.GetBytes(soapStr);
                myHttpWebRequest.ContentType = "application/json";
                myHttpWebRequest.ContentLength = SoapByte.Length;
                //myHttpWebRequest.AllowAutoRedirect = false;
                //myHttpWebRequest.KeepAlive = false;
                //myHttpWebRequest.Timeout = 30000;
                //myHttpWebRequest.ReadWriteTimeout = 30000;
                //myHttpWebRequest.UserAgent = "test.net";
                //myHttpWebRequest.Accept = "application/json";
                //myHttpWebRequest.ProtocolVersion = HttpVersion.Version11;
                //myHttpWebRequest.Headers.Add("Accept-Language", "de_DE");
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                myHttpWebRequest.Headers.Add("Authorization", "bearer " + txtToken.Text);
                myHttpWebRequest.Method = "POST";

                using (System.IO.Stream DataStream = myHttpWebRequest.GetRequestStream())
                {
                    //DataStream = myHttpWebRequest.GetRequestStream();
                    DataStream.Write(SoapByte, 0, SoapByte.Length);

                    // Sends the HttpWebRequest and waits for a response.
                    using (HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse())
                    {
                        if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (System.IO.Stream DataStream2 = myHttpWebResponse.GetResponseStream())
                            {
                                using (System.IO.StreamReader Reader = new System.IO.StreamReader(DataStream2))
                                {
                                    string SD2Request = Reader.ReadToEnd();
                                    soapResponse = SD2Request;
                                    DataStream2.Dispose();
                                    DataStream2.Close();
                                }
                            }

                            myHttpWebResponse.Dispose();
                            myHttpWebResponse.Close();
                            return true;
                        }
                        else
                        {
                            err = "Failed response is " + myHttpWebResponse.StatusDescription;
                            myHttpWebResponse.Dispose();
                            myHttpWebResponse.Close();
                            return false;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                err = string.Format("WebException : {0}", e.Status);
                return false;
            }
            catch (Exception e)
            {
                err = string.Format("Runtime ex : {0}", e.Message);
                return false;
            }
        }

        int recordCntr = 0;

        private void SaveLog(string fileName, string desc)
        {
            StreamWriter sw = new StreamWriter(fileName, true);
            sw.Write(desc + Environment.NewLine);
            sw.Dispose();
            sw.Close();
        }

        private DataTable dtIdandUUID = null;

        private void LoadIdAndUUID()
        {
            if (dtIdandUUID != null) return;
            dtIdandUUID = new DataTable();
            dtIdandUUID.Columns.Add("id", Type.GetType("System.Int32"));
            dtIdandUUID.Columns.Add("uuid", Type.GetType("System.String"));
            dtIdandUUID.Columns.Add("fn", Type.GetType("System.String"));
            dtIdandUUID.Columns.Add("mn", Type.GetType("System.String"));
            dtIdandUUID.Columns.Add("ln", Type.GetType("System.String"));

            string s = @"D:\WORK\acc_id_uuid.txt";
            foreach (string line in File.ReadAllLines(s))
            {
                DataRow rw = dtIdandUUID.NewRow();
                rw[0] = line.Split('|')[0];
                rw[1] = line.Split('|')[1];
                rw[2] = line.Split('|')[2];
                rw[3] = line.Split('|')[3];
                rw[4] = line.Split('|')[4];
                dtIdandUUID.Rows.Add(rw);
            }
        }

        //private string GenerateBio(string memberFolder)
        //{
        //    List<Biometric> bios = new List<Biometric>();

        //    foreach (string file in Directory.GetFiles(memberFolder))
        //    {
        //        Biometric bio = new Biometric();

        //        switch (Path.GetExtension(file).ToUpper())
        //        {
        //            case ".WSQ":
        //            case ".ANSI-FMR":
        //                if (file.Contains("Lbackup"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.LeftThumb;
        //                    bio.fpCode = GetFingerCode(FingerPosition.LeftThumb);
        //                }
        //                else if (file.Contains("Lprimary"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.LeftIndex;
        //                    bio.fpCode = GetFingerCode(FingerPosition.LeftIndex);
        //                }
        //                else if (file.Contains("Lmiddle"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.LeftMiddle;
        //                    bio.fpCode = GetFingerCode(FingerPosition.LeftMiddle);
        //                }
        //                else if (file.Contains("Lring"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.LeftRing;
        //                    bio.fpCode = GetFingerCode(FingerPosition.LeftRing);
        //                }
        //                else if (file.Contains("Lpinkie"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.LeftPinkie;
        //                    bio.fpCode = GetFingerCode(FingerPosition.LeftPinkie);
        //                }
        //                else if (file.Contains("Rbackup"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.RightThumb;
        //                    bio.fpCode = GetFingerCode(FingerPosition.RightThumb);
        //                }
        //                else if (file.Contains("Rprimary"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.RightIndex;
        //                    bio.fpCode = GetFingerCode(FingerPosition.RightIndex);
        //                }
        //                else if (file.Contains("Rmiddle"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.RightMiddle;
        //                    bio.fpCode = GetFingerCode(FingerPosition.RightMiddle);
        //                }
        //                else if (file.Contains("Rring"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.RightRing;
        //                    bio.fpCode = GetFingerCode(FingerPosition.RightRing);
        //                }
        //                else if (file.Contains("Rpinkie"))
        //                {
        //                    bio.fpPosition = (short)FingerPosition.RightPinkie;
        //                    bio.fpCode = GetFingerCode(FingerPosition.RightPinkie);
        //                }

        //                if(Path.GetExtension(file).ToUpper()==".WSQ") bio.base64Wsq = Convert.ToBase64String(File.ReadAllBytes(file));
        //                else if (Path.GetExtension(file).ToUpper() == ".ANSI-FMR") bio.base64Ansi = Convert.ToBase64String(File.ReadAllBytes(file));

        //                break;                  
        //        }
        //    }
        //}

        private string GetPhoto(string id)
        {
            string response = "err";

            foreach (string subDir in Directory.GetDirectories(@"D:\WORK\FILCONNECT\acc employee data - Copy"))
            {
                string idFile = string.Format(@"{0}\FINAL\{1}\{1}_Photo.jpg", subDir, id);
                if (File.Exists(idFile))
                {
                    return idFile;
                }
            }

            return response;
        }

        private string GetSignature(string id)
        {
            string response = "err";

            foreach (string subDir in Directory.GetDirectories(@"D:\WORK\FILCONNECT\acc employee data - Copy"))
            {
                string idFile = string.Format(@"{0}\FINAL\{1}\{1}_Signature.tiff", subDir, id);
                if (File.Exists(idFile))
                {
                    return idFile;
                }
            }

            return response;
        }

        private void UploadACCEmployee()
        {
            recordCntr = 1;
            int tokenResetter = 1;
            string startTime = DateTime.Now.ToString();
            WriteToLog("Start of process|2|" + startTime + "|2");

            string data = System.IO.File.ReadAllText(txtDirectory.Text);
            StringBuilder sbSuccess = new StringBuilder();
            StringBuilder sbFailed = new StringBuilder();

            foreach (string d in data.Split('\r'))
            {
                string line = d.Replace("\n", "");
                if (line.Trim() != "")
                {
                    if (!line.Contains("EmployeeNo"))
                    {
                        string soapResponse = "";
                        string errResponse = "";

                        string[] lineArr = line.Split('|');
                        string id = lineArr[0];
                        string firstName = lineArr[1];
                        string middleName = lineArr[2];
                        string lastName = lineArr[3];
                        string dob = Convert.ToDateTime(lineArr[8]).ToString("yyyy-MM-dd");

                        string photoBase64 = "";
                        string photoLocation = GetPhoto(id);
                        string signatureBase64 = "";
                        string signatureLocation = GetSignature(id);
                        if (photoLocation != "err") photoBase64 = Convert.ToBase64String(File.ReadAllBytes(photoLocation));
                        if (signatureLocation != "err") signatureBase64 = Convert.ToBase64String(File.ReadAllBytes(signatureLocation));

                        string refData = string.Format("{0} {1} {2}", firstName, middleName, lastName);

                        // string mId = "";
                        // string uuid = "";
                        // string base64 = "";
                        // string imageLocation = "";

                        // if (dtIdandUUID.Select(string.Format("fn='{0}' and mn='{1}' and ln='{2}'", firstName, middleName, lastName)).Length > 0)
                        // {
                        //     DataRow rw = dtIdandUUID.Select(string.Format("fn='{0}' and mn='{1}' and ln='{2}'", firstName, middleName, lastName))[0];
                        //     mId = rw["id"].ToString();
                        //     uuid = rw["uuid"].ToString();                            
                        //     imageLocation = GetSignature(id);
                        //     if (imageLocation != "err") base64 = Convert.ToBase64String(File.ReadAllBytes(imageLocation));
                        //     else break;                                
                        // }

                        //string soapRequest = SoapRequest_FileUpload(Path.GetFileName(imageLocation), base64);

                        string soapRequest = "";

                        // Console.Write(soapRequest);

                        //if (ExecuteApiRequest(txtRootUrl.Text + "/createphoto", soapRequest, ref soapResponse, ref errResponse))
                        //if (ExecuteApiRequest(txtRootUrl.Text + "/uploadFile", soapRequest, ref soapResponse, ref errResponse))
                        //{
                        ////int idIndex = soapResponse.IndexOf("\"response\":", 12);
                        //int idIndex = soapResponse.IndexOf("\"response\":");
                        //if (idIndex != -1)
                        //{
                        //    int approxLength = soapResponse.IndexOf("}", idIndex);
                        //    string location = soapResponse.Substring(idIndex, approxLength - idIndex).Replace("\"response\":", "").Replace("\"", "");
                        //    if (location != "")
                        //    {
                        //        soapRequest = SoapRequest_Createv3(lastName, firstName, middleName, lineArr[3], dob, lineArr[17], lineArr[18], lineArr[12], lineArr[19], "0", "true", "", "", lineArr[6].Substring(0, 1),
                        //                                    lineArr[7], lineArr[16], lineArr[15], "EMPLOYED - PRIVATE", "0000000", lineArr[13],
                        //                                    lineArr[28], lineArr[29], lineArr[30], lineArr[31], lineArr[32], lineArr[35], lineArr[34], lineArr[36], "",
                        //                                    lineArr[28], lineArr[29], lineArr[30], lineArr[31], lineArr[32], lineArr[35], lineArr[34], lineArr[36], "",
                        //                                    lineArr[22], lineArr[20], lineArr[21], lineArr[23], lineArr[26], lineArr[24], lineArr[25], lineArr[27],
                        //                                    lastName, firstName, middleName, lineArr[3], "0000000", lineArr[13], "",
                        //                                    lineArr[28], lineArr[29], lineArr[30], lineArr[31], lineArr[32], lineArr[35], lineArr[34], lineArr[36], "", "0", id,
                        //                                    "", "", "", uuid, Path.GetExtension(imageLocation).Replace(".",""),location);

                        //        if (ExecuteApiRequest(txtRootUrl.Text + "/createSignature", soapRequest, ref soapResponse, ref errResponse))
                        //        { 
                        //        }
                        //     }
                        //}

                        soapRequest = SoapRequest_Createv3(lastName, firstName, middleName, lineArr[3], dob, lineArr[17], lineArr[18], lineArr[12], lineArr[19], "0", "true", "", "", lineArr[6].Substring(0, 1),
                                                                lineArr[7], lineArr[16], lineArr[15], "EMPLOYED - PRIVATE", "0000000", lineArr[13],
                                                                lineArr[28], lineArr[29], lineArr[30], lineArr[31], lineArr[32], lineArr[35], lineArr[34], lineArr[36], "",
                                                                lineArr[28], lineArr[29], lineArr[30], lineArr[31], lineArr[32], lineArr[35], lineArr[34], lineArr[36], "",
                                                                lineArr[22], lineArr[20], lineArr[21], lineArr[23], lineArr[26], lineArr[24], lineArr[25], lineArr[27],
                                                                lastName, firstName, middleName, lineArr[3], "0000000", lineArr[13], "",
                                                                lineArr[28], lineArr[29], lineArr[30], lineArr[31], lineArr[32], lineArr[35], lineArr[34], lineArr[36], "", "0", id,
                                                                "", Path.GetExtension(photoLocation).Replace(".", ""), photoBase64, "", Path.GetExtension(signatureLocation).Replace(".", ""), signatureBase64);

                        Console.WriteLine(soapRequest);

                        if (ExecuteApiRequest(txtRootUrl.Text + "/create", soapRequest, ref soapResponse, ref errResponse))
                        {
                            WriteToLog(refData + "|0|Member added|");
                            SaveLog(Application.StartupPath + @"\Logs\successLog.txt", line);
                            sbSuccess.Append(line + Environment.NewLine);
                        }
                        else
                        {
                            WriteToLog(refData + "|1|Failed to add member. " + errResponse + "|0");
                            SaveLog(Application.StartupPath + @"\Logs\failedLog.txt", line);
                            sbFailed.Append(line + Environment.NewLine);
                        }



                        ////address
                        //if (ExecuteApiRequest(txtRootUrl.Text + "/createphoto", soapRequest, ref soapResponse, ref errResponse)) WriteToLog(refData + "|0|Address added|" + memberId);
                        //else WriteToLog(refData + "|1|Failed to add address. " + errResponse + "|" + memberId);
                        //}
                        //else
                        //{
                        //    WriteToLog(refData + "|1|Failed to add member. " + errResponse + "|0");
                        //    SaveLog(Application.StartupPath + @"\Logs\failedLog.txt", line);
                        //    sbFailed.Append(line + Environment.NewLine);
                        //}

                        recordCntr += 1;
                        if (tokenResetter == 31)
                        {
                            tokenResetter = 1;
                            txtToken.Text = "";
                        }
                        else tokenResetter += 1;
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }

            string endTime = DateTime.Now.ToString();
            TimeSpan ts = Convert.ToDateTime(endTime) - Convert.ToDateTime(startTime);
            WriteToLog("End of process|2|" + endTime + "|2");
            WriteToLog("Process time|2|" + ts.Minutes.ToString("N0") + " minutes|2");

            string sessionRef = DateTime.Now.ToString("hhmmss");

            System.IO.File.WriteAllText(Application.StartupPath + @"\Logs\successLog" + sessionRef + ".txt", sbSuccess.ToString());
            System.IO.File.WriteAllText(Application.StartupPath + @"\Logs\failedLog" + sessionRef + ".txt", sbFailed.ToString());
            rtb.SaveFile(Application.StartupPath + @"\Logs\uploadLog" + sessionRef + ".txt", RichTextBoxStreamType.PlainText);

            MessageBox.Show("Done!");
        }    


        private string SoapRequest_Create(string lastName, string firstName, string middleName, string suffix, string birthDate, string height, string weight,
                                          string tin, string distinguishingRemarks, string noOfChildren, string isRegisteredVoter, string recoveryphone, string recoveryemail,
                                          string gender, string civilStatus, string birthCity, string birthProvince, string employmentStatus, string telephoneNos,
                                          string mobileNos,
                                          string permanentRoomFloorUnitBldg, string permanentHouseLotBlock, string permanentStreetname, string permanentSubdivision,
                                          string permanentBarangay, string permanentCity, string permanentProvince, string permanentPostal, string permanentDistrict,
                                          string presentRoomFloorUnitBldg, string presentHouseLotBlock, string presentStreetname, string presentSubdivision,
                                          string presentBarangay, string presentCity, string presentProvince, string presentPostal, string presentDistrict,
                                          string lastNameFather, string firstNameFather, string middleNameFather, string suffixFather,
                                          string lastNameMother, string firstNameMother, string middleNameMother, string suffixMother,
                                          string lastNameContact, string firstNameContact, string middleNameContact, string suffixContact,
                                          string telephoneNosContact, string mobileNosContact, string emailContact,
                                          string presentRoomFloorUnitBldgContact, string presentHouseLotBlockContact, string presentStreetnameContact, string presentSubdivisionContact,
                                          string presentBarangayContact, string presentCityContact, string presentProvinceContact, string presentPostalContact, string presentDistrictContact, string memberId)

        {
            string payload = System.IO.File.ReadAllText(Application.StartupPath + "\\payload_createv2.txt");
            string fullName = firstName;
            if (middleName == "") fullName += " " + lastName;
            else fullName += " " + middleName.Substring(0, 1) + " " + lastName;

            string verifiedRcoveryphone = telephoneNos;
            if (verifiedRcoveryphone == "") verifiedRcoveryphone = "00000000";

            if (recoveryemail == "") recoveryemail = firstName.Substring(0, 1).ToLower() + lastName.Substring(0, 1).ToLower() + DateTime.Now.ToString("hhmmss") + "@gmail.com";
            if (emailContact == "") emailContact = firstNameContact.Substring(0, 1).ToLower() + lastNameContact.Substring(0, 1).ToLower() + DateTime.Now.ToString("hhmmss") + "@gmail.com";

            bool RegisteredVoter = true;
            if (isRegisteredVoter.ToUpper() == "NO") RegisteredVoter = false;

            payload = payload.Replace("@@lastName", lastName)
                             .Replace("@@firstName", firstName)
                             .Replace("@@middleName", middleName)
                             .Replace("@@suffix", suffix)
                             .Replace("@gender", gender)
                             .Replace("@civilStatus", civilStatus)
                             .Replace("@birthDate", birthDate)
                             .Replace("@birthCity", birthCity)
                             .Replace("@birthProvince", birthProvince)
                             .Replace("@height", height)
                             .Replace("@weight", weight)
                             .Replace("@tin", tin)
                             .Replace("@distinguishingRemarks", distinguishingRemarks)
                             .Replace("@noOfChildren", noOfChildren)
                             .Replace("@employmentStatus", employmentStatus)
                             .Replace("@isRegisteredVoter", RegisteredVoter.ToString().ToLower())
                             .Replace("@@telephoneNos", telephoneNos)
                             .Replace("@@mobileNos", mobileNos)
                             .Replace("@permanentRoomFloorUnitBldg", permanentRoomFloorUnitBldg)
                             .Replace("@permanentHouseLotBlock", permanentHouseLotBlock)
                             .Replace("@permanentStreetname", permanentStreetname)
                             .Replace("@permanentSubdivision", permanentSubdivision)
                             .Replace("@permanentBarangay", permanentBarangay)
                             .Replace("@permanentCity", permanentCity)
                             .Replace("@permanentProvince", permanentProvince)
                             .Replace("@permanentPostal", permanentPostal)
                             .Replace("@permanentDistrict", permanentDistrict)
                             .Replace("@@presentRoomFloorUnitBldg", presentRoomFloorUnitBldg)
                             .Replace("@@presentHouseLotBlock", presentHouseLotBlock)
                             .Replace("@@presentStreetname", presentStreetname)
                             .Replace("@@presentSubdivision", presentSubdivision)
                             .Replace("@@presentBarangay", presentBarangay)
                             .Replace("@@presentCity", presentCity)
                             .Replace("@@presentProvince", presentProvince)
                             .Replace("@@presentPostal", presentPostal)
                             .Replace("@@presentDistrict", presentDistrict)
                             .Replace("@lastNameFather", lastNameFather)
                             .Replace("@firstNameFather", firstNameFather)
                             .Replace("@middleNameFather", middleNameFather)
                             .Replace("@suffixFather", suffixFather)
                             .Replace("@lastNameMother", lastNameMother)
                             .Replace("@firstNameMother", firstNameMother)
                             .Replace("@middleNameMother", middleNameMother)
                             .Replace("@suffixMother", suffixMother)
                             .Replace("@lastNameContact", lastNameContact)
                             .Replace("@firstNameContact", firstNameContact)
                             .Replace("@middleNameContact", middleNameContact)
                             .Replace("@suffixContact", suffixContact)
                             .Replace("@telephoneNosContact", telephoneNosContact)
                             .Replace("@mobileNosContact", mobileNosContact)
                             .Replace("@emailContact", emailContact)
                             .Replace("@presentRoomFloorUnitBldgContact", presentRoomFloorUnitBldgContact)
                             .Replace("@presentHouseLotBlockContact", presentHouseLotBlockContact)
                             .Replace("@presentStreetnameContact", presentStreetnameContact)
                             .Replace("@presentSubdivisionContact", presentSubdivisionContact)
                             .Replace("@presentBarangayContact", presentBarangayContact)
                             .Replace("@presentCityContact", presentCityContact)
                             .Replace("@presentProvinceContact", presentProvinceContact)
                             .Replace("@presentPostalContact", presentPostalContact)
                             .Replace("@presentDistrictContact", presentDistrictContact)
                             .Replace("@recoveryphone", verifiedRcoveryphone)
                             .Replace("@recoveryemail", recoveryemail)
                             .Replace("@email", recoveryemail)
                             .Replace("@displayName", fullName)
                             .Replace("@@memberId", memberId);
            return payload;
        }

        private string SoapRequest_Createv2(string lastName, string firstName, string middleName, string suffix, string birthDate, string height, string weight,
                                     string tin, string distinguishingRemarks, string noOfChildren, string isRegisteredVoter, string recoveryphone, string recoveryemail,
                                     string gender, string civilStatus, string birthCity, string birthProvince, string employmentStatus, string telephoneNos,
                                     string mobileNos,
                                     string permanentRoomFloorUnitBldg, string permanentHouseLotBlock, string permanentStreetname, string permanentSubdivision,
                                     string permanentBarangay, string permanentCity, string permanentProvince, string permanentPostal, string permanentDistrict,
                                     string presentRoomFloorUnitBldg, string presentHouseLotBlock, string presentStreetname, string presentSubdivision,
                                     string presentBarangay, string presentCity, string presentProvince, string presentPostal, string presentDistrict,
                                     string lastNameFather, string firstNameFather, string middleNameFather, string suffixFather,
                                     string lastNameMother, string firstNameMother, string middleNameMother, string suffixMother,
                                     string lastNameContact, string firstNameContact, string middleNameContact, string suffixContact,
                                     string telephoneNosContact, string mobileNosContact, string emailContact,
                                     string presentRoomFloorUnitBldgContact, string presentHouseLotBlockContact, string presentStreetnameContact, string presentSubdivisionContact,
                                     string presentBarangayContact, string presentCityContact, string presentProvinceContact, string presentPostalContact, string presentDistrictContact, string memberId, string id)

        {
            string payload = System.IO.File.ReadAllText(Application.StartupPath + "\\payload_createv2.txt");
            string fullName = firstName;
            if (middleName == "") fullName += " " + lastName;
            else fullName += " " + middleName.Substring(0, 1) + " " + lastName;

            string verifiedRcoveryphone = telephoneNos;
            if (verifiedRcoveryphone == "") verifiedRcoveryphone = "00000000";

            if (recoveryemail == "") recoveryemail = firstName.Substring(0, 1).ToLower() + lastName.Substring(0, 1).ToLower() + DateTime.Now.ToString("hhmmss") + "@gmail.com";
            emailContact = recoveryemail;
            //if (emailContact == "") emailContact = firstNameContact.Substring(0, 1).ToLower() + lastNameContact.Substring(0, 1).ToLower() + DateTime.Now.ToString("hhmmss") + "@gmail.com";

            bool RegisteredVoter = true;
            if (isRegisteredVoter.ToUpper() == "NO") RegisteredVoter = false;


            string mId = "";
            string uuid = "";

            if (dtIdandUUID.Select(string.Format("fn='{0}' and mn='{1}' and ln='{2}'", firstName, middleName, lastName)).Length > 0)
            {
                DataRow rw = dtIdandUUID.Select(string.Format("fn='{0}' and mn='{1}' and ln='{2}'", firstName, middleName, lastName))[0];
                mId = rw["id"].ToString();
                uuid = rw["uuid"].ToString();
                string photoBase64 = "";
                string photoLocation = GetPhoto(id);
                if (photoLocation != "err") photoBase64 = Convert.ToBase64String(File.ReadAllBytes(photoLocation));

                payload = payload.Replace("@@lastName", lastName)
                                 .Replace("@@firstName", firstName)
                                 .Replace("@@middleName", middleName)
                                 .Replace("@@suffix", suffix)
                                 .Replace("@gender", gender)
                                 .Replace("@civilStatus", civilStatus)
                                 .Replace("@birthDate", birthDate)
                                 .Replace("@birthCity", birthCity)
                                 .Replace("@birthProvince", birthProvince)
                                 .Replace("@height", height)
                                 .Replace("@weight", weight)
                                 .Replace("@tin", tin)
                                 .Replace("@distinguishingRemarks", distinguishingRemarks)
                                 .Replace("@noOfChildren", noOfChildren)
                                 .Replace("@employmentStatus", employmentStatus)
                                 .Replace("@isRegisteredVoter", RegisteredVoter.ToString().ToLower())
                                 .Replace("@@telephoneNosContact2", telephoneNos)
                                 .Replace("@@mobileNosContact2", mobileNos)
                                 .Replace("@@emailContact2", recoveryemail)
                                 .Replace("@permanentRoomFloorUnitBldg", permanentRoomFloorUnitBldg)
                                 .Replace("@permanentHouseLotBlock", permanentHouseLotBlock)
                                 .Replace("@permanentStreetname", permanentStreetname)
                                 .Replace("@permanentSubdivision", permanentSubdivision)
                                 .Replace("@permanentBarangay", permanentBarangay)
                                 .Replace("@permanentCity", permanentCity)
                                 .Replace("@permanentProvince", permanentProvince)
                                 .Replace("@permanentPostal", permanentPostal)
                                 .Replace("@permanentDistrict", permanentDistrict)
                                 .Replace("@@presentRoomFloorUnitBldg", presentRoomFloorUnitBldg)
                                 .Replace("@@presentHouseLotBlock", presentHouseLotBlock)
                                 .Replace("@@presentStreetname", presentStreetname)
                                 .Replace("@@presentSubdivision", presentSubdivision)
                                 .Replace("@@presentBarangay", presentBarangay)
                                 .Replace("@@presentCity", presentCity)
                                 .Replace("@@presentProvince", presentProvince)
                                 .Replace("@@presentPostal", presentPostal)
                                 .Replace("@@presentDistrict", presentDistrict)
                                 .Replace("@lastNameFather", lastNameFather)
                                 .Replace("@firstNameFather", firstNameFather)
                                 .Replace("@middleNameFather", middleNameFather)
                                 .Replace("@suffixFather", suffixFather)
                                 .Replace("@lastNameMother", lastNameMother)
                                 .Replace("@firstNameMother", firstNameMother)
                                 .Replace("@middleNameMother", middleNameMother)
                                 .Replace("@suffixMother", suffixMother)
                                 .Replace("@lastNameContact", lastNameContact)
                                 .Replace("@firstNameContact", firstNameContact)
                                 .Replace("@middleNameContact", middleNameContact)
                                 .Replace("@suffixContact", suffixContact)
                                 .Replace("@telephoneNosContact", telephoneNosContact)
                                 .Replace("@mobileNosContact", mobileNosContact)
                                 .Replace("@emailContact", emailContact)
                                 .Replace("@presentRoomFloorUnitBldgContact", presentRoomFloorUnitBldgContact)
                                 .Replace("@presentHouseLotBlockContact", presentHouseLotBlockContact)
                                 .Replace("@presentStreetnameContact", presentStreetnameContact)
                                 .Replace("@presentSubdivisionContact", presentSubdivisionContact)
                                 .Replace("@presentBarangayContact", presentBarangayContact)
                                 .Replace("@presentCityContact", presentCityContact)
                                 .Replace("@presentProvinceContact", presentProvinceContact)
                                 .Replace("@presentPostalContact", presentPostalContact)
                                 .Replace("@presentDistrictContact", presentDistrictContact)
                                 .Replace("@recoveryphone", verifiedRcoveryphone)
                                 .Replace("@recoveryemail", recoveryemail)
                                 .Replace("@displayName", fullName)
                                 .Replace("@@memberId", mId)
                                 .Replace("@uuid", uuid)
                                 .Replace("@base64", photoBase64);
                return payload;
            }
            return "";
        }

        private string SoapRequest_Createv3(string lastName, string firstName, string middleName, string suffix, string birthDate, string height, string weight,
                                    string tin, string distinguishingRemarks, string noOfChildren, string isRegisteredVoter, string recoveryphone, string recoveryemail,
                                    string gender, string civilStatus, string birthCity, string birthProvince, string employmentStatus, string telephoneNos,
                                    string mobileNos,
                                    string permanentRoomFloorUnitBldg, string permanentHouseLotBlock, string permanentStreetname, string permanentSubdivision,
                                    string permanentBarangay, string permanentCity, string permanentProvince, string permanentPostal, string permanentDistrict,
                                    string presentRoomFloorUnitBldg, string presentHouseLotBlock, string presentStreetname, string presentSubdivision,
                                    string presentBarangay, string presentCity, string presentProvince, string presentPostal, string presentDistrict,
                                    string lastNameFather, string firstNameFather, string middleNameFather, string suffixFather,
                                    string lastNameMother, string firstNameMother, string middleNameMother, string suffixMother,
                                    string lastNameContact, string firstNameContact, string middleNameContact, string suffixContact,
                                    string telephoneNosContact, string mobileNosContact, string emailContact,
                                    string presentRoomFloorUnitBldgContact, string presentHouseLotBlockContact, string presentStreetnameContact, string presentSubdivisionContact,
                                    string presentBarangayContact, string presentCityContact, string presentProvinceContact, string presentPostalContact, string presentDistrictContact, string memberId, string id,
                                    string photoMemberId, string photoExt, string photoLocation, string signatureMemberId, string signatureExt, string signatureLocation)

        {
            string payload = System.IO.File.ReadAllText(Application.StartupPath + "\\payload_createv2.txt");
            string fullName = firstName;
            if (middleName == "") fullName += " " + lastName;
            else fullName += " " + middleName.Substring(0, 1) + " " + lastName;

            string verifiedRcoveryphone = telephoneNos;
            if (verifiedRcoveryphone == "") verifiedRcoveryphone = "00000000";

            if (recoveryemail == "") recoveryemail = firstName.Substring(0, 1).ToLower() + lastName.Substring(0, 1).ToLower() + DateTime.Now.ToString("hhmmss") + "@gmail.com";
            emailContact = recoveryemail;
            //if (emailContact == "") emailContact = firstNameContact.Substring(0, 1).ToLower() + lastNameContact.Substring(0, 1).ToLower() + DateTime.Now.ToString("hhmmss") + "@gmail.com";

            bool RegisteredVoter = true;
            if (isRegisteredVoter.ToUpper() == "NO") RegisteredVoter = false;

            payload = payload.Replace("@@lastName", lastName)
                             .Replace("@@firstName", firstName)
                             .Replace("@@middleName", middleName)
                             .Replace("@@suffix", suffix)
                             .Replace("@gender", gender)
                             .Replace("@civilStatus", civilStatus)
                             .Replace("@birthDate", birthDate)
                             .Replace("@birthCity", birthCity)
                             .Replace("@birthProvince", birthProvince)
                             .Replace("@height", height)
                             .Replace("@weight", weight)
                             .Replace("@tin", tin)
                             .Replace("@distinguishingRemarks", distinguishingRemarks)
                             .Replace("@noOfChildren", noOfChildren)
                             .Replace("@employmentStatus", employmentStatus)
                             .Replace("@isRegisteredVoter", RegisteredVoter.ToString().ToLower())
                             .Replace("@@telephoneNosContact2", telephoneNos)
                             .Replace("@@mobileNosContact2", mobileNos)
                             .Replace("@@emailContact2", recoveryemail)
                             .Replace("@permanentRoomFloorUnitBldg", permanentRoomFloorUnitBldg)
                             .Replace("@permanentHouseLotBlock", permanentHouseLotBlock)
                             .Replace("@permanentStreetname", permanentStreetname)
                             .Replace("@permanentSubdivision", permanentSubdivision)
                             .Replace("@permanentBarangay", permanentBarangay)
                             .Replace("@permanentCity", permanentCity)
                             .Replace("@permanentProvince", permanentProvince)
                             .Replace("@permanentPostal", permanentPostal)
                             .Replace("@permanentDistrict", permanentDistrict)
                             .Replace("@@presentRoomFloorUnitBldg", presentRoomFloorUnitBldg)
                             .Replace("@@presentHouseLotBlock", presentHouseLotBlock)
                             .Replace("@@presentStreetname", presentStreetname)
                             .Replace("@@presentSubdivision", presentSubdivision)
                             .Replace("@@presentBarangay", presentBarangay)
                             .Replace("@@presentCity", presentCity)
                             .Replace("@@presentProvince", presentProvince)
                             .Replace("@@presentPostal", presentPostal)
                             .Replace("@@presentDistrict", presentDistrict)
                             .Replace("@lastNameFather", lastNameFather)
                             .Replace("@firstNameFather", firstNameFather)
                             .Replace("@middleNameFather", middleNameFather)
                             .Replace("@suffixFather", suffixFather)
                             .Replace("@lastNameMother", lastNameMother)
                             .Replace("@firstNameMother", firstNameMother)
                             .Replace("@middleNameMother", middleNameMother)
                             .Replace("@suffixMother", suffixMother)
                             .Replace("@lastNameContact", lastNameContact)
                             .Replace("@firstNameContact", firstNameContact)
                             .Replace("@middleNameContact", middleNameContact)
                             .Replace("@suffixContact", suffixContact)
                             .Replace("@telephoneNosContact", telephoneNosContact)
                             .Replace("@mobileNosContact", mobileNosContact)
                             .Replace("@emailContact", emailContact)
                             .Replace("@presentRoomFloorUnitBldgContact", presentRoomFloorUnitBldgContact)
                             .Replace("@presentHouseLotBlockContact", presentHouseLotBlockContact)
                             .Replace("@presentStreetnameContact", presentStreetnameContact)
                             .Replace("@presentSubdivisionContact", presentSubdivisionContact)
                             .Replace("@presentBarangayContact", presentBarangayContact)
                             .Replace("@presentCityContact", presentCityContact)
                             .Replace("@presentProvinceContact", presentProvinceContact)
                             .Replace("@presentPostalContact", presentPostalContact)
                             .Replace("@presentDistrictContact", presentDistrictContact)
                             .Replace("@recoveryphone", verifiedRcoveryphone)
                             .Replace("@recoveryemail", recoveryemail)
                             .Replace("@displayName", fullName)
                             .Replace("@photoMemberId", photoMemberId)
                             .Replace("@photoExt", photoExt)
                             .Replace("@photoLocation", photoLocation)
                             .Replace("@signatureMemberId", signatureMemberId)
                             .Replace("@signatureExt", signatureExt)
                             .Replace("@@memberId", "0")
                             .Replace("@signatureLocation", signatureLocation);
            return payload;

        }

        private string SoapRequest_FileUpload(string fileName, string base64)
        {
            string payload = System.IO.File.ReadAllText(Application.StartupPath + "\\payload_fileUpload.txt");

            payload = payload.Replace("@fileName", fileName)
                                .Replace("@base64", base64);
            return payload;
        }

        private string SoapRequest_Verify(string lastName, string firstName, string birthDate)
        {
            string payload = System.IO.File.ReadAllText(Application.StartupPath + "\\payload_verify");

            payload = payload.Replace("@lastName", lastName)
                             .Replace("@firstName", firstName)
                             .Replace("@birthDate", birthDate)
                             ;

            return payload;
        }

        private void WriteToLog(string desc)
        {
            string line = string.Format("{0}[{1}] {2}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss "), recordCntr.ToString("N0"), desc);
            SaveLog(Application.StartupPath + @"\Logs\processLog.txt", line);
            rtb.AppendText(line + Environment.NewLine);
            rtb.ScrollToCaret();
            Application.DoEvents();
        }

        private void txtDirectory_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDirectory_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtDirectory.Text = ofd.FileName;
            }
            ofd.Dispose();
            ofd = null;
        }
    }
}
