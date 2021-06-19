using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics.Tracing;
using System.Xml;
using System.Xml.Serialization;
using System.Web;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
//using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Bayambang_ExtractData
{
    public partial class BayambangUploadingToFilConnect2 : Form
    {
        public BayambangUploadingToFilConnect2()
        {
            InitializeComponent();
        }

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

        private void BayambangUploadingToFilConnect_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            //try
            //{ 
            //if (!checkCubaoVpnConnection())
            //{
            //    MessageBox.Show("Please check your cubao vpn connection...", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    Environment.Exit(0);
            //}
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Please check your cubao vpn connection." + Environment.NewLine + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    Environment.Exit(0);
            //}

            txtRootUrl.Text = Properties.Settings.Default.Url;
            txtApplicationId.Text = Properties.Settings.Default.AppId;
            txtLeafId.Text = Properties.Settings.Default.LeafId;
            txtAuthURL.Text = Properties.Settings.Default.AuthUrl;
            txtAuthUser.Text = Properties.Settings.Default.AuthUser;
            txtAuthPass.Text = Properties.Settings.Default.AuthPass;
            txtAuthGrantType.Text = Properties.Settings.Default.AuthGrant;
            txtAuthClientId.Text = Properties.Settings.Default.AuthClient;
            txtNotifId.Text = Properties.Settings.Default.NotifId;
            txtTokenResetCntr.Text = Properties.Settings.Default.TokenResetCntr.ToString();
            txtInterval.Text = Properties.Settings.Default.SleepInterval.ToString();

            GetDataFromPlant2();

            if (!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");
            if (!Directory.Exists("Payload")) Directory.CreateDirectory("Payload");
            if (!Directory.Exists("Exceptions")) Directory.CreateDirectory("Exceptions");
            if (!Directory.Exists("Processed")) Directory.CreateDirectory("Processed");
        }

        private int recordCntr = 0;
        private short tokenResetterCnt = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to continue?" + Environment.NewLine + Environment.NewLine + "Upload photo: " + chkPhoto.Checked.ToString() + Environment.NewLine + "Upload signature: " + chkSignature.Checked.ToString() + Environment.NewLine + "Upload bio: " + chkBio.Checked.ToString(), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                if (!IsNumeric(txtInterval.Text))
                {
                    MessageBox.Show("Please check interval value.");
                    return;
                }

                if (!IsNumeric(txtTokenResetCntr.Text))
                {
                    MessageBox.Show("Please check token reset cntr value.");
                    return;
                }

                if (Directory.GetDirectories(txtDirectory.Text).Length > 0)
                {
                    Thread t = new Thread(new ThreadStart(GetDemographic));
                    t.Start();
                }
                else
                {
                    MessageBox.Show("Folder '" + txtDirectory.Text + "' is empty...", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private List<Demographic> demographics = null;

        private void GetDemographic()
        {        
            Cursor = Cursors.WaitCursor;
            this.Enabled = false;

            WriteToLog(string.Format("[START] Start of process"));

            LabelStatus("Populating demographics");

            List<string> notIncludedFields = new List<string>();
            notIncludedFields.Add("photoExt");
            notIncludedFields.Add("photoBase64");
            notIncludedFields.Add("signatureExt");
            notIncludedFields.Add("signatureBase64");
            notIncludedFields.Add("leftBackupWsqBase64");
            notIncludedFields.Add("leftPrimaryWsqBase64");
            notIncludedFields.Add("leftMiddleWsqBase64");
            notIncludedFields.Add("leftRingWsqBase64");
            notIncludedFields.Add("leftPinkyWsqBase64");
            notIncludedFields.Add("rightBackupWsqBase64");
            notIncludedFields.Add("rightPrimaryWsqBase64");
            notIncludedFields.Add("rightMiddleWsqBase64");
            notIncludedFields.Add("rightRingWsqBase64");
            notIncludedFields.Add("rightPinkyWsqBase64");
            notIncludedFields.Add("leftBackupAnsiBase64");
            notIncludedFields.Add("leftPrimaryAnsiBase64");
            notIncludedFields.Add("leftMiddleAnsiBase64");
            notIncludedFields.Add("leftRingAnsiBase64");
            notIncludedFields.Add("leftPinkyAnsiBase64");
            notIncludedFields.Add("rightBackupAnsiBase64");
            notIncludedFields.Add("rightPrimaryAnsiBase64");
            notIncludedFields.Add("rightMiddleAnsiBase64");
            notIncludedFields.Add("rightRingAnsiBase64");
            notIncludedFields.Add("rightPinkyAnsiBase64");
            notIncludedFields.Add("leftBackupJpgBase64");
            notIncludedFields.Add("leftPrimaryJpgBase64");
            notIncludedFields.Add("leftMiddleJpgBase64");
            notIncludedFields.Add("leftRingJpgBase64");
            notIncludedFields.Add("leftPinkyJpgBase64");
            notIncludedFields.Add("rightBackupJpgBase64");
            notIncludedFields.Add("rightPrimaryJpgBase64");
            notIncludedFields.Add("rightMiddleJpgBase64");
            notIncludedFields.Add("rightRingJpgBase64");
            notIncludedFields.Add("rightPinkyJpgBase64");

            demographics = null;
            demographics = new List<Demographic>();

            int cntr = 0;
            int good = 0;
            int bad = 0;

            StringBuilder sb = new StringBuilder();
            StringBuilder sbConso = new StringBuilder();
            StringBuilder sbLine = new StringBuilder();

            StringBuilder sbHeader = new StringBuilder();
            bool isHeaderDone = false;           

            string refFolder = txtDirectory.Text.Substring(txtDirectory.Text.LastIndexOf(@"\") + 1);

            foreach (string subDir2 in Directory.GetDirectories(txtDirectory.Text))
            {
                System.Diagnostics.Stopwatch watchPerFolder = new System.Diagnostics.Stopwatch();
                watchPerFolder.Start();

                demographics = null;
                demographics = new List<Demographic>();

                string line = "";
                sbLine.Clear();

                string f = subDir2.Substring(subDir2.LastIndexOf(@"\") + 1);

                //if (f == "BYMBNG001_20161021_013748_0010")
                //{
                //    Console.WriteLine("Test");
                //}

                System.Diagnostics.Stopwatch watchDemographic = new System.Diagnostics.Stopwatch();
                WriteToLog(string.Format("[PROCESS] Generate demographic"));
                watchDemographic.Start();
                Demographic demographic = GetMemberDemographic(subDir2, ref line);
                watchDemographic.Stop();
                TimeSpan timeSpan = watchDemographic.Elapsed;
                if(demographic!=null)WriteToLog(string.Format("[SUCCESS] {0} {1} Success demographic creation {2}s {3}ms", f, demographic.sessionReference, timeSpan.Seconds, timeSpan.Milliseconds));
                
                if (demographic == null)
                {
                    bad += 1;
                    WriteToLog(string.Format("[FAILED] {0} {1} Failed to generate demographic", f, demographic.sessionReference));
                    if (!Directory.Exists(string.Format("Exceptions\\{0}", f))) Directory.Move(subDir2, string.Format("Exceptions\\{0}", f));
                    else Directory.Move(subDir2, string.Format("Exceptions\\{0}_{1}", f, DateTime.Now.ToString("hhmmss")));
                }
                else
                {                                    
                    if (demographic.backOcr == "")
                    {
                        bad += 1;
                        WriteToLog(string.Format("[FAILED] {0} {1} No backOcr", f, demographic.sessionReference));
                        if (!Directory.Exists(string.Format("Exceptions\\{0}", f))) Directory.Move(subDir2, string.Format("Exceptions\\{0}", f));
                        else Directory.Move(subDir2, string.Format("Exceptions\\{0}_{1}", f, DateTime.Now.ToString("hhmmss")));
                    }                   
                    else
                    {
                        good += 1;
                        demographics.Add(demographic);

                        Type t = demographic.GetType();
                        System.Reflection.PropertyInfo[] props = t.GetProperties();

                        foreach (var prop in props)
                        {
                            if (notIncludedFields.Find(item => item.Equals(prop.Name)) == null)
                            {
                                if (!isHeaderDone)
                                {
                                    if (prop.Name != "id")
                                    {
                                        if (sbHeader.ToString() == "") sbHeader.Append(prop.Name);
                                        else sbHeader.Append("|" + prop.Name);
                                    }
                                }

                                if (sbLine.ToString() == "") sbLine.Append(prop.GetValue(demographic));
                                else sbLine.Append("|" + prop.GetValue(demographic));
                            }
                        }

                        sbLine.Append("|" + demographic.path.Substring(demographic.path.LastIndexOf(@"\") + 1));
                        sbHeader.Append("|Folder");

                        if (!isHeaderDone) sb.Append(sbHeader.ToString() + Environment.NewLine);
                        isHeaderDone = true;

                        if (sbLine.ToString().Trim() != "")
                        {
                            sb.Append(sbLine.ToString() + Environment.NewLine);
                            //sbConso.Append(line + Environment.NewLine);
                        }

                        WriteToLog(string.Format("[SUCCESS] {0} {1} Added to list", f, demographic.sessionReference));
                        LabelStatus(string.Format("Good: {0} Bad: {1} Total: {2} ", good, bad, cntr));

                        try
                        {
                            //if (!checkCubaoVpnConnection()) WriteToLog(string.Format("[FAILED] {0} {1} Unable to connect to vpn", f, demographic.sessionReference));                            
                            //else UploadDemographicDataCitizen(demographic, subDir2);
                            UploadDemographicDataCitizen(demographic, subDir2);
                        }
                        catch (Exception ex)
                        {
                            //WriteToLog(string.Format("[FAILED] {0} {1} Unable to connect to vpn", f, demographic.sessionReference));
                            //MessageBox.Show("Please check your cubao vpn connection." + Environment.NewLine + Environment.NewLine + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }                        
                    }

                    //}
                }
                cntr += 1;
                LabelStatus(string.Format("Populating demographics, Good: {0} Bad: {1} Total: {2} ", good, bad, cntr));

                watchPerFolder.Stop();
                TimeSpan timeSpanPerFolder = watchPerFolder.Elapsed;
                WriteToLog(string.Format("[DONE] {0} {1}s {2}ms", demographic.sessionReference, timeSpanPerFolder.Seconds, timeSpanPerFolder.Milliseconds));
            }

            if (sb.ToString() != "") SaveLog(string.Format(@"{0}\{1}_{2}.txt", Application.StartupPath + @"\Logs", refFolder, DateTime.Now.ToString("yyyyMMdd_hhmmss")), sb.ToString());

            Cursor = Cursors.Default;
            this.Enabled = true;

            WriteToLog(string.Format("[END] End of process"));

            MessageBox.Show("Done!",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Information);
        }       

        private delegate void SafeCallDelegate(string desc);
        private delegate void delStartProcess();

        private void LabelStatus(string desc)
        {
            if (label2.InvokeRequired)
            {
                var d = new SafeCallDelegate(LabelStatus);
                label2.Invoke(d, new object[] { desc });
            }
            else label2.Text = desc;
            Application.DoEvents();
        }

        private void WriteToRTB(string desc)
        {
            if (rtb.InvokeRequired)
            {
                var d = new SafeCallDelegate(WriteToRTB);
                rtb.Invoke(d, new object[] { desc });
            }
            else rtb.AppendText(desc);
            rtb.ScrollToCaret();
            Application.DoEvents();
        }

        private string HandleNode(XmlNodeList xnList, string field)
        {
            try
            {
                if (xnList[0][field] != null)
                {
                    string value = xnList[0][field].InnerText.Replace("\r", "").Replace("\n", "").Trim();

                    if (value.ToUpper().Contains("-SELECT")) value = "NULL";

                    if (field == "Weight")
                    {
                        if (value != "NULL") if (value.Length > 3) value = value.Substring(0, 3);
                    }
                    else if (field == "Height")
                    {
                        if (value != "NULL") if (value.Length > 5) value = value.Substring(0, 5);
                    }
                    else if (field == "NoOfChildren")
                    {
                        if (value != "NULL")
                        {
                            if (value == "") value = "0";
                            else if (!IsNumeric(value))
                            {
                                var noOfChild = value.Substring(0, 1);
                                if (!IsNumeric(noOfChild)) value = "0";
                                else value = noOfChild;
                            }
                            else if (Convert.ToInt32(value) > 20) value = "0";
                        }
                    }

                    System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.None;
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[ ]{2,}", options);
                    value = regex.Replace(value, " ");

                    return value;
                }
                else return "";
            }
            catch
            {
                return "";
            }
        }

        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        private Demographic GetMemberDemographic(string dir, ref string line)
        {
            string xmlFile = "";
            foreach (string file in Directory.GetFiles(dir))
            {
                if (Path.GetExtension(file).ToUpper() == ".XML")
                {
                    xmlFile = file;
                    break;
                }
            }

            if (!File.Exists(xmlFile))
            {
                WriteToLog(xmlFile + ". XML file not found");
                return null;
            }
            else
            {
                try
                {
                    Demographic demographic = new Demographic();

                    XmlDocument doc = new XmlDocument();
                    doc.Load(xmlFile);
                    XmlNodeList xnList = doc.SelectNodes("MemberData");                  

                    demographic.firstName = HandleNode(xnList, "FirstName");
                    demographic.middleName = HandleNode(xnList, "MiddleName");
                    demographic.lastName = HandleNode(xnList, "LastName");
                    demographic.suffix = HandleNode(xnList, "Suffix");
                    demographic.gender = HandleNode(xnList, "Gender");
                    demographic.civilStatus = HandleNode(xnList, "CivilStatus");
                    demographic.dateOfBirth = HandleNode(xnList, "DateOfBirth");
                    demographic.employmentStatus = HandleNode(xnList, "EmploymentStatus");
                    demographic.registeredVoter = HandleNode(xnList, "RegisteredVoter");
                    demographic.tIN = HandleNode(xnList, "TIN");
                    demographic.country_BirthAddress = HandleNode(xnList, "Country_BirthAddress");
                    demographic.province_BirthAddress = HandleNode(xnList, "Province_BirthAddress");
                    demographic.city_BirthAddress = HandleNode(xnList, "City_BirthAddress");
                    demographic.height = HandleNode(xnList, "Height");
                    demographic.weight = HandleNode(xnList, "Weight");
                    demographic.distinguishingMark = HandleNode(xnList, "DistinguishingMark");
                    demographic.firstName_Father = HandleNode(xnList, "FirstName_Father");
                    demographic.middleName_Father = HandleNode(xnList, "MiddleName_Father");
                    demographic.lastName_Father = HandleNode(xnList, "LastName_Father");
                    demographic.suffix_Father = HandleNode(xnList, "Suffix_Father");
                    demographic.firstName_Mother = HandleNode(xnList, "FirstName_Mother");
                    demographic.middleName_Mother = HandleNode(xnList, "MiddleName_Mother");
                    demographic.lastName_Mother = HandleNode(xnList, "LastName_Mother");
                    demographic.suffix_Mother = HandleNode(xnList, "Suffix_Mother");
                    demographic.firstName_Spouse = HandleNode(xnList, "FirstName_Spouse");
                    demographic.middleName_Spouse = HandleNode(xnList, "MiddleName_Spouse");
                    demographic.lastName_Spouse = HandleNode(xnList, "LastName_Spouse");
                    demographic.suffix_Spouse = HandleNode(xnList, "Suffix_Spouse");
                    demographic.noOfChildren = HandleNode(xnList, "NoOfChildren");
                    demographic.roomFloorUnitBldg = HandleNode(xnList, "RoomFloorUnitBldg");
                    demographic.houseLotBlock = HandleNode(xnList, "HouseLotBlock");
                    demographic.streetname = HandleNode(xnList, "Streetname");
                    demographic.subdivision = HandleNode(xnList, "Subdivision");
                    demographic.barangay = HandleNode(xnList, "Barangay");
                    demographic.district = HandleNode(xnList, "District");
                    demographic.country_Address = HandleNode(xnList, "Country_Address");
                    demographic.province_Address = HandleNode(xnList, "Province_Address");
                    demographic.city_Address = HandleNode(xnList, "City_Address");
                    demographic.postal = HandleNode(xnList, "Postal");
                    demographic.mobileNos = HandleNode(xnList, "MobileNos");
                    demographic.telephoneNos = HandleNode(xnList, "TelephoneNos");
                    demographic.emailAddress = HandleNode(xnList, "EmailAddress");
                    demographic.firstName_Contact = HandleNode(xnList, "FirstName_Contact");
                    demographic.middleName_Contact = HandleNode(xnList, "MiddleName_Contact");
                    demographic.lastName_Contact = HandleNode(xnList, "LastName_Contact");
                    demographic.suffix_Contact = HandleNode(xnList, "Suffix_Contact");
                    demographic.relation = HandleNode(xnList, "Relation");
                    demographic.roomFloorUnitBldg_Contact = HandleNode(xnList, "RoomFloorUnitBldg_Contact");
                    demographic.houseLotBlock_Contact = HandleNode(xnList, "HouseLotBlock_Contact");
                    demographic.streetname_Contact = HandleNode(xnList, "Streetname_Contact");
                    demographic.subdivision_Contact = HandleNode(xnList, "Subdivision_Contact");
                    demographic.barangay_Contact = HandleNode(xnList, "Barangay_Contact");
                    demographic.district_Contact = HandleNode(xnList, "District_Contact");
                    demographic.country_Address_Contact = HandleNode(xnList, "Country_Address_Contact");
                    demographic.province_Address_Contact = HandleNode(xnList, "Province_Address_Contact");
                    demographic.city_Address_Contact = HandleNode(xnList, "City_Address_Contact");
                    demographic.postal_Contact = HandleNode(xnList, "Postal_Contact");
                    demographic.mobileNos_Contact = HandleNode(xnList, "MobileNos_Contact");
                    demographic.telephoneNos_Contact = HandleNode(xnList, "TelephoneNos_Contact");
                    demographic.emailAddress_Contact = HandleNode(xnList, "EmailAddress_Contact");
                    demographic.recardReason = HandleNode(xnList, "RecardReason");
                    demographic.leftPrimaryFingerCode = HandleNode(xnList, "LeftPrimaryFingerCode");
                    demographic.leftThumbFingerCode = HandleNode(xnList, "LeftThumbFingerCode");
                    demographic.rightPrimaryFingerCode = HandleNode(xnList, "RightPrimaryFingerCode");
                    demographic.rightThumbFingerCode = HandleNode(xnList, "RightThumbFingerCode");
                    demographic.photoOverride = HandleNode(xnList, "PhotoOverride");
                    demographic.signatureOverride = HandleNode(xnList, "SignatureOverride");
                    demographic.photoICAO = HandleNode(xnList, "PhotoICAO");
                    demographic.photoScore = HandleNode(xnList, "PhotoScore");
                    demographic.sessionReference = HandleNode(xnList, "SessionReference");
                    demographic.timestamp = HandleNode(xnList, "Timestamp");
                    demographic.operatorID = HandleNode(xnList, "OperatorID");
                    demographic.terminalName = HandleNode(xnList, "TerminalName");

                    demographic.path = Path.GetDirectoryName(xmlFile);

                    if (IsExistInPlantData(demographic.sessionReference, demographic.firstName, demographic.lastName))
                    {
                        DataRow rw = dtPlantData.Select(string.Format("F16='{0}' AND F2='{1}' AND F4='{2}'", demographic.sessionReference, demographic.firstName, demographic.lastName))[0];
                        demographic.urn = rw["F1"].ToString().Trim();
                        demographic.backOcr = rw["F17"].ToString().Trim();
                        demographic.refSource = rw["F18"].ToString().Trim();
                    }
                    else
                    {
                        demographic.urn = "";
                        demographic.backOcr = "";
                        demographic.refSource = "";
                    }

                    //line = string.Format("{0}|{1}|{2}|{3}", sbNodeLine.ToString(), demographic.urn, demographic.backOcr, demographic.path.Substring(demographic.path.LastIndexOf(@"\") + 1));

                    if (chkPhoto.Checked)
                    {
                        string photo1 = xmlFile.Replace(".xml", "_Photo.jpg");
                        string photo2 = xmlFile.Replace(".xml", "_Photo.png");
                        if (File.Exists(photo1))
                        {
                            demographic.photoExt = "jpg";
                            if (new FileInfo(photo1).Length > 0) demographic.photoBase64 = Convert.ToBase64String(File.ReadAllBytes(photo1));
                        }
                        else if (File.Exists(photo2))
                        {
                            demographic.photoExt = "png";
                            if (new FileInfo(photo2).Length > 0) demographic.photoBase64 = Convert.ToBase64String(File.ReadAllBytes(photo2));
                        }
                    }

                    if (chkSignature.Checked)
                    {
                        //string signature1 = xmlFile.Replace(".xml", "_Signature.tiff");
                        string signature2 = xmlFile.Replace(".xml", "_Signature.jpg");
                        //if (File.Exists(signature1))
                        //{
                        //    demographic.signatureExt = "tiff";
                        //    if (new FileInfo(signature1).Length > 0) demographic.signatureBase64 = Convert.ToBase64String(File.ReadAllBytes(signature1));
                        //}
                        if (File.Exists(signature2))
                        {
                            demographic.signatureExt = "jpg";
                            if (new FileInfo(signature2).Length > 0) demographic.signatureBase64 = Convert.ToBase64String(File.ReadAllBytes(signature2));
                        }
                    }

                    if (chkBio.Checked)
                    {
                        //wsq
                        string lBackupWsq = xmlFile.Replace(".xml", "_Lbackup.wsq");
                        if (File.Exists(lBackupWsq)) if (new FileInfo(lBackupWsq).Length > 0) demographic.leftBackupWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(lBackupWsq));
                        string lPrimaryWsq = xmlFile.Replace(".xml", "_Lprimary.wsq");
                        if (File.Exists(lPrimaryWsq)) if (new FileInfo(lPrimaryWsq).Length > 0) demographic.leftPrimaryWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(lPrimaryWsq));
                        string lMiddleWsq = xmlFile.Replace(".xml", "_Lmiddle.wsq");
                        if (File.Exists(lMiddleWsq)) if (new FileInfo(lMiddleWsq).Length > 0) demographic.leftMiddleWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(lMiddleWsq));
                        string lRingWsq = xmlFile.Replace(".xml", "_Lring.wsq");
                        if (File.Exists(lRingWsq)) if (new FileInfo(lRingWsq).Length > 0) demographic.leftRingWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(lRingWsq));
                        string lPinkyWsq = xmlFile.Replace(".xml", "_Lpinky.wsq");
                        if (File.Exists(lPinkyWsq)) if (new FileInfo(lPinkyWsq).Length > 0) demographic.leftPinkyWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(lPinkyWsq));

                        string rBackupWsq = xmlFile.Replace(".xml", "_Rbackup.wsq");
                        if (File.Exists(rBackupWsq)) if (new FileInfo(rBackupWsq).Length > 0) demographic.rightBackupWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(rBackupWsq));
                        string rPrimaryWsq = xmlFile.Replace(".xml", "_Rprimary.wsq");
                        if (File.Exists(rPrimaryWsq)) if (new FileInfo(rPrimaryWsq).Length > 0) demographic.rightPrimaryWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(rPrimaryWsq));
                        string rMiddleWsq = xmlFile.Replace(".xml", "_Rmiddle.wsq");
                        if (File.Exists(rMiddleWsq)) if (new FileInfo(rMiddleWsq).Length > 0) demographic.rightMiddleWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(rMiddleWsq));
                        string rRingWsq = xmlFile.Replace(".xml", "_Rring.wsq");
                        if (File.Exists(rRingWsq)) if (new FileInfo(rRingWsq).Length > 0) demographic.rightRingWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(rRingWsq));
                        string rPinkyWsq = xmlFile.Replace(".xml", "_Rpinky.wsq");
                        if (File.Exists(rPinkyWsq)) if (new FileInfo(rPinkyWsq).Length > 0) demographic.rightPinkyWsqBase64 = Convert.ToBase64String(File.ReadAllBytes(rPinkyWsq));

                        //ansi
                        string lBackupAnsi = xmlFile.Replace(".xml", "_Lbackup.ansi-fmr");
                        if (File.Exists(lBackupAnsi)) if (new FileInfo(lBackupAnsi).Length > 0) demographic.leftBackupAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(lBackupAnsi));
                        string lPrimaryAnsi = xmlFile.Replace(".xml", "_Lprimary.ansi-fmr");
                        if (File.Exists(lPrimaryAnsi)) if (new FileInfo(lPrimaryAnsi).Length > 0) demographic.leftPrimaryAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(lPrimaryAnsi));
                        string lMiddleAnsi = xmlFile.Replace(".xml", "_Lmiddle.ansi-fmr");
                        if (File.Exists(lMiddleAnsi)) if (new FileInfo(lMiddleAnsi).Length > 0) demographic.leftMiddleAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(lMiddleAnsi));
                        string lRingAnsi = xmlFile.Replace(".xml", "_Lring.ansi-fmr");
                        if (File.Exists(lRingAnsi)) if (new FileInfo(lRingAnsi).Length > 0) demographic.leftRingAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(lRingAnsi));
                        string lPinkyAnsi = xmlFile.Replace(".xml", "_Lpinky.ansi-fmr");
                        if (File.Exists(lPinkyAnsi)) if (new FileInfo(lPinkyAnsi).Length > 0) demographic.leftPinkyAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(lPinkyAnsi));

                        string rBackupAnsi = xmlFile.Replace(".xml", "_Rbackup.ansi-fmr");
                        if (File.Exists(rBackupAnsi)) if (new FileInfo(rBackupAnsi).Length > 0) demographic.rightBackupAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(rBackupAnsi));
                        string rPrimaryAnsi = xmlFile.Replace(".xml", "_Rprimary.ansi-fmr");
                        if (File.Exists(rPrimaryAnsi)) if (new FileInfo(rPrimaryAnsi).Length > 0) demographic.rightPrimaryAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(rPrimaryAnsi));
                        string rMiddleAnsi = xmlFile.Replace(".xml", "_Rmiddle.ansi-fmr");
                        if (File.Exists(rMiddleAnsi)) if (new FileInfo(rMiddleAnsi).Length > 0) demographic.rightMiddleAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(rMiddleAnsi));
                        string rRingAnsi = xmlFile.Replace(".xml", "_Rring.ansi-fmr");
                        if (File.Exists(rRingAnsi)) if (new FileInfo(rRingAnsi).Length > 0) demographic.rightRingAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(rRingAnsi));
                        string rPinkyAnsi = xmlFile.Replace(".xml", "_Rpinky.ansi-fmr");
                        if (File.Exists(rPinkyAnsi)) if (new FileInfo(rPinkyAnsi).Length > 0) demographic.rightPinkyAnsiBase64 = Convert.ToBase64String(File.ReadAllBytes(rPinkyAnsi));

                        //jpg
                        string lBackupJpg = xmlFile.Replace(".xml", "_Lbackup.jpg");
                        if (File.Exists(lBackupJpg)) if (new FileInfo(lBackupJpg).Length > 0) demographic.leftBackupJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(lBackupJpg));
                        string lPrimaryJpg = xmlFile.Replace(".xml", "_Lprimary.jpg");
                        if (File.Exists(lPrimaryJpg)) if (new FileInfo(lPrimaryJpg).Length > 0) demographic.leftPrimaryJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(lPrimaryJpg));
                        string lMiddleJpg = xmlFile.Replace(".xml", "_Lmiddle.jpg");
                        if (File.Exists(lMiddleJpg)) if (new FileInfo(lMiddleJpg).Length > 0) demographic.leftMiddleJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(lMiddleJpg));
                        string lRingJpg = xmlFile.Replace(".xml", "_Lring.jpg");
                        if (File.Exists(lRingJpg)) if (new FileInfo(lRingJpg).Length > 0) demographic.leftRingJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(lRingJpg));
                        string lPinkyJpg = xmlFile.Replace(".xml", "_Lpinky.jpg");
                        if (File.Exists(lPinkyJpg)) if (new FileInfo(lPinkyJpg).Length > 0) demographic.leftPinkyJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(lPinkyJpg));

                        string rBackupJpg = xmlFile.Replace(".xml", "_Rbackup.jpg");
                        if (File.Exists(rBackupJpg)) if (new FileInfo(rBackupJpg).Length > 0) demographic.rightBackupJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(rBackupJpg));
                        string rPrimaryJpg = xmlFile.Replace(".xml", "_Rprimary.jpg");
                        if (File.Exists(rPrimaryJpg)) if (new FileInfo(rPrimaryJpg).Length > 0) demographic.rightPrimaryJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(rPrimaryJpg));
                        string rMiddleJpg = xmlFile.Replace(".xml", "_Rmiddle.jpg");
                        if (File.Exists(rMiddleJpg)) if (new FileInfo(rMiddleJpg).Length > 0) demographic.rightMiddleJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(rMiddleJpg));
                        string rRingJpg = xmlFile.Replace(".xml", "_Rring.jpg");
                        if (File.Exists(rRingJpg)) if (new FileInfo(rRingJpg).Length > 0) demographic.rightRingJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(rRingJpg));
                        string rPinkyJpg = xmlFile.Replace(".xml", "_Rpinky.jpg");
                        if (File.Exists(rPinkyJpg)) if (new FileInfo(rPinkyJpg).Length > 0) demographic.rightPinkyJpgBase64 = Convert.ToBase64String(File.ReadAllBytes(rPinkyJpg));

                    }

                    return demographic;
                }
                catch (Exception ex)
                {
                    WriteToLog(xmlFile + ". Runtime error " + ex.Message);
                    return null;
                }
            }
        }

        private void SaveLog(string fileName, string desc)
        {
            try
            {
                StreamWriter sw = new StreamWriter(fileName, true);
                sw.Write(desc + Environment.NewLine);
                sw.Dispose();
                sw.Close();
            }
            catch (Exception ex)
            {
                SaveLog(fileName, desc);
            }
        }

        private void WriteToLog(string desc)
        {
            string line = string.Format("{0} {1}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss "),  desc);
            SaveLog(Application.StartupPath + @"\Logs\processLog.txt", line);
            WriteToRTB(line + Environment.NewLine);
        }

        private void UploadDemographicDataCitizen(Demographic demographic, string subDir2)
        {
            if (demographics == null)
            {
                MessageBox.Show("Demographic is empty");
                return;
            }

            //if (MessageBox.Show("Are you sure you want to continue?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            //Cursor = Cursors.WaitCursor;
            //this.Enabled = false;

            LabelStatus("Uploading demographics");

            StringBuilder sbSuccess = new StringBuilder();
            StringBuilder sbFailed = new StringBuilder();

            recordCntr = 1;
            
            string exceptions = "";
            string id = demographic.id;
            string firstName = demographic.firstName;
            string middleName = demographic.middleName;
            string lastName = demographic.lastName;
            string dob = Convert.ToDateTime(demographic.dateOfBirth).ToString("yyyy-MM-dd");
            string refData = string.Format("{0} {1} {2}|{3}|{4}", firstName, middleName, lastName, demographic.sessionReference, demographic.path);

            if (IsValidate(demographic, ref exceptions))
            {
                string soapResponse = "";
                string soapRequest = "";
                string errResponse = "";

                LabelStatus("Generating payload");

                try
                {
                    Class.CreateCitizen createCitizen = new Class.CreateCitizen();
                    createCitizen.applicationId = txtApplicationId.Text;
                    if (txtNotifId.Text != "") createCitizen.notificationId = txtNotifId.Text;
                    Class.CreateCitizen.objGeoLocation geoLocation = new Class.CreateCitizen.objGeoLocation();

                    Random rn = new Random();
                    int iRN = rn.Next(1000, 9999);

                    Class.CreateCitizen.objPayload payload = new Class.CreateCitizen.objPayload();
                    payload.institutionId = "D0808731-D59D-EA11-9C14-98541B2295E9";//bayambang
                    payload.leafId = txtLeafId.Text;
                    
                    //payload.institutionId = "D0808731-D59D-EA11-9C14-98541B2295E8";
                    //payload.lastName = demographic.lastName.ToUpper() + iRN.ToString();
                    //payload.firstName = demographic.firstName.ToUpper() + iRN.ToString();
                    //payload.middleName = demographic.middleName.ToUpper() + iRN.ToString();

                    payload.lastName = demographic.lastName.ToUpper();
                    payload.firstName = demographic.firstName.ToUpper();
                    payload.middleName = demographic.middleName.ToUpper();
                    payload.suffix = demographic.suffix.ToUpper();
                    payload.gender = demographic.gender.Substring(0, 1).ToUpper();
                    payload.civilStatus = demographic.civilStatus.ToUpper();
                    payload.citizenship = "FILIPINO";
                    payload.birthDate = string.Format("{0}T16:00:00.000Z", dob);
                    payload.birthCity = demographic.city_BirthAddress.ToUpper();
                    payload.birthProvince = demographic.province_BirthAddress.ToUpper();
                    payload.birthCountry = demographic.country_BirthAddress.ToUpper();
                    payload.height = demographic.height;
                    payload.weight = demographic.weight;
                    payload.tin = demographic.tIN;
                    payload.distinguishingRemarks = demographic.distinguishingMark.ToUpper();
                    if (demographic.noOfChildren != "") payload.noOfChildren = Convert.ToInt32(demographic.noOfChildren);
                    payload.employmentStatus = demographic.employmentStatus.ToUpper();
                    if (demographic.registeredVoter == "YES") payload.isRegisteredVoter = true;
                    else payload.isRegisteredVoter = false;

                    Class.CreateCitizen.objPayload.objAddress address = new Class.CreateCitizen.objPayload.objAddress();
                    address.permanentRoomFloorUnitBldg = demographic.roomFloorUnitBldg.ToUpper();
                    address.permanentHouseLotBlock = demographic.houseLotBlock.ToUpper();
                    address.permanentStreetname = demographic.streetname.ToUpper();
                    address.permanentSubdivision = demographic.subdivision.ToUpper();
                    address.permanentBarangay = demographic.barangay.ToUpper();
                    address.permanentCity = demographic.city_Address.ToUpper();
                    address.permanentProvince = demographic.province_Address.ToUpper();
                    address.permanentCountry = demographic.country_Address.ToUpper();
                    address.permanentPostal = demographic.postal.ToUpper();
                    address.permanentDistrict = demographic.district.ToUpper();
                    address.presentRoomFloorUnitBldg = demographic.roomFloorUnitBldg.ToUpper();
                    address.presentHouseLotBlock = demographic.houseLotBlock.ToUpper();
                    address.presentStreetname = demographic.streetname.ToUpper();
                    address.presentSubdivision = demographic.subdivision.ToUpper();
                    address.presentBarangay = demographic.barangay.ToUpper();
                    address.presentCity = demographic.city_Address.ToUpper();
                    address.presentProvince = demographic.province_Address.ToUpper();
                    address.presentCountry = demographic.country_Address.ToUpper();
                    address.presentPostal = demographic.postal.ToUpper();
                    address.presentDistrict = demographic.district.ToUpper();

                    string fullAddress = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", address.presentPostal, address.presentRoomFloorUnitBldg, address.presentHouseLotBlock, address.presentStreetname, address.presentSubdivision, address.presentBarangay, address.presentCity, address.presentProvince, address.presentCountry);

                    System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.None;
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[ ]{2,}", options);
                    fullAddress = regex.Replace(fullAddress, " ");

                    address.fullAddress = fullAddress;

                    List<Class.CreateCitizen.objPayload.objRelationship> relationships = new List<Class.CreateCitizen.objPayload.objRelationship>();
                    Class.CreateCitizen.objPayload.objRelationship relationshipFather = new Class.CreateCitizen.objPayload.objRelationship();
                    relationshipFather.relationshipTypeId = relationshipFather.GetRelationshipTypeUUID(Class.CreateCitizen.objPayload.objRelationship.enum_relationshipType.Father);
                    relationshipFather.relationshipType = "FATHER";
                    relationshipFather.lastName = demographic.lastName_Father.ToUpper();
                    relationshipFather.firstName = demographic.firstName_Father.ToUpper();
                    relationshipFather.middleName = demographic.middleName_Father.ToUpper();
                    relationshipFather.suffix = demographic.suffix_Father;

                    Class.CreateCitizen.objPayload.objRelationship relationshipMother = new Class.CreateCitizen.objPayload.objRelationship();
                    relationshipMother.relationshipTypeId = relationshipFather.GetRelationshipTypeUUID(Class.CreateCitizen.objPayload.objRelationship.enum_relationshipType.Mother);
                    relationshipMother.relationshipType = "MOTHER";
                    relationshipMother.lastName = demographic.lastName_Mother.ToUpper();
                    relationshipMother.firstName = demographic.firstName_Mother.ToUpper();
                    relationshipMother.middleName = demographic.middleName_Mother.ToUpper();
                    relationshipMother.suffix = demographic.suffix_Mother.ToUpper();

                    Class.CreateCitizen.objPayload.objRelationship relationshipSpouse = new Class.CreateCitizen.objPayload.objRelationship();
                    relationshipSpouse.relationshipTypeId = relationshipSpouse.GetRelationshipTypeUUID(Class.CreateCitizen.objPayload.objRelationship.enum_relationshipType.Spouse);
                    relationshipSpouse.relationshipType = "SPOUSE";
                    relationshipSpouse.lastName = demographic.lastName_Spouse.ToUpper();
                    relationshipSpouse.firstName = demographic.firstName_Spouse.ToUpper();
                    relationshipSpouse.middleName = demographic.middleName_Spouse.ToUpper();
                    relationshipSpouse.suffix = demographic.suffix_Spouse.ToUpper();

                    relationships.Add(relationshipFather);
                    relationships.Add(relationshipMother);
                    if (!string.IsNullOrEmpty(relationshipSpouse.middleName)) relationships.Add(relationshipSpouse);

                    List<Class.CreateCitizen.objPayload.objContact> contacts = null;
                    //if (!string.IsNullOrEmpty(demographic.telephoneNos))
                    //{
                    Class.CreateCitizen.objPayload.objContact contactTelephoneNos = new Class.CreateCitizen.objPayload.objContact();
                    contactTelephoneNos.type = contactTelephoneNos.GetContactTypeUUID(Class.CreateCitizen.objPayload.objContact.enum_contactType.TelephoneNos);
                    contactTelephoneNos.value = demographic.telephoneNos;
                    if (contacts == null) contacts = new List<Class.CreateCitizen.objPayload.objContact>();
                    contacts.Add(contactTelephoneNos);
                    //}

                    //if (!string.IsNullOrEmpty(demographic.mobileNos))
                    //{
                    Class.CreateCitizen.objPayload.objContact contactMobileNos = new Class.CreateCitizen.objPayload.objContact();
                    contactMobileNos.type = contactMobileNos.GetContactTypeUUID(Class.CreateCitizen.objPayload.objContact.enum_contactType.MobileNos);
                    contactMobileNos.value = demographic.mobileNos;
                    if (contacts == null) contacts = new List<Class.CreateCitizen.objPayload.objContact>();
                    contacts.Add(contactMobileNos);
                    //}

                    //if (!string.IsNullOrEmpty(demographic.emailAddress))
                    //{
                    Class.CreateCitizen.objPayload.objContact contactEmail = new Class.CreateCitizen.objPayload.objContact();
                    contactEmail.type = contactEmail.GetContactTypeUUID(Class.CreateCitizen.objPayload.objContact.enum_contactType.Email);
                    contactEmail.value = demographic.emailAddress;
                    if (contacts == null) contacts = new List<Class.CreateCitizen.objPayload.objContact>();
                    contacts.Add(contactEmail);
                    //}

                    Class.CreateCitizen.objPayload.objContactPerson contactPerson = new Class.CreateCitizen.objPayload.objContactPerson();
                    contactPerson.lastName = demographic.lastName_Contact.ToUpper();
                    contactPerson.firstName = demographic.firstName_Contact.ToUpper();
                    contactPerson.middleName = demographic.middleName_Contact.ToUpper();
                    contactPerson.suffix = demographic.suffix_Contact.ToUpper();
                    contactPerson.telephoneNos = demographic.telephoneNos_Contact.ToUpper();
                    contactPerson.mobileNos = demographic.mobileNos_Contact.ToUpper();
                    contactPerson.email = demographic.emailAddress_Contact.ToUpper();
                    contactPerson.presentRoomFloorUnitBldg = demographic.roomFloorUnitBldg_Contact.ToUpper();
                    contactPerson.presentHouseLotBlock = demographic.houseLotBlock_Contact.ToUpper();
                    contactPerson.presentStreetname = demographic.streetname_Contact.ToUpper();
                    contactPerson.presentSubdivision = demographic.subdivision_Contact.ToUpper();
                    contactPerson.presentBarangay = demographic.barangay_Contact.ToUpper();
                    contactPerson.presentCity = demographic.city_Address_Contact.ToUpper();
                    contactPerson.presentProvince = demographic.province_Address_Contact.ToUpper();
                    contactPerson.presentCountry = demographic.country_Address_Contact.ToUpper();
                    contactPerson.presentPostal = demographic.postal_Contact.ToUpper();
                    contactPerson.presentDistrict = demographic.district_Contact.ToUpper();

                    Class.CreateCitizen.objPayload.objPhoto photo = null;
                    if (chkPhoto.Checked)
                    {                        
                        if (!string.IsNullOrEmpty(demographic.photoBase64))
                        {
                            photo = new Class.CreateCitizen.objPayload.objPhoto();
                            photo.ext = demographic.photoExt;
                            photo.base64 = demographic.photoBase64;
                        }
                    }

                    Class.CreateCitizen.objPayload.objSignature signature = null;
                    if (chkSignature.Checked)
                    {                        
                        if (!string.IsNullOrEmpty(demographic.signatureBase64))
                        {
                            signature = new Class.CreateCitizen.objPayload.objSignature();
                            signature.ext = demographic.signatureExt;
                            signature.base64 = demographic.signatureBase64;
                        }
                    }

                    List<Class.CreateCitizen.objPayload.objBiometric> biometrics = null;
                    if (chkBio.Checked)
                    {
                        Class.CreateCitizen.objPayload.objBiometric bioLeftBackup = new Class.CreateCitizen.objPayload.objBiometric();
                        Class.CreateCitizen.objPayload.objBiometric bioLeftPrimary = new Class.CreateCitizen.objPayload.objBiometric();
                        Class.CreateCitizen.objPayload.objBiometric bioRightBackup = new Class.CreateCitizen.objPayload.objBiometric();
                        Class.CreateCitizen.objPayload.objBiometric bioRightPrimary = new Class.CreateCitizen.objPayload.objBiometric();

                        if (string.IsNullOrEmpty(demographic.leftBackupAnsiBase64) & string.IsNullOrEmpty(demographic.leftBackupWsqBase64) & string.IsNullOrEmpty(demographic.leftBackupJpgBase64)) { }
                        else
                        {
                            if (!string.IsNullOrEmpty(demographic.leftBackupAnsiBase64)) bioLeftBackup.base64Ansi = demographic.leftBackupAnsiBase64;
                            if (!string.IsNullOrEmpty(demographic.leftBackupWsqBase64)) bioLeftBackup.base64Wsq = demographic.leftBackupWsqBase64;
                            if (!string.IsNullOrEmpty(demographic.leftBackupJpgBase64)) bioLeftBackup.base64Jpg = demographic.leftBackupJpgBase64;
                            bioLeftBackup.fpPosition = (short)FingerPosition.LeftThumb;
                            bioLeftBackup.fpCode = bioLeftBackup.GetFingerCode((short)FingerPosition.LeftThumb);
                            if (biometrics == null) biometrics = new List<Class.CreateCitizen.objPayload.objBiometric>();
                            biometrics.Add(bioLeftBackup);
                        }

                        if (string.IsNullOrEmpty(demographic.leftPrimaryAnsiBase64) & string.IsNullOrEmpty(demographic.leftPrimaryWsqBase64) & string.IsNullOrEmpty(demographic.leftPrimaryJpgBase64)) { }
                        else
                        {
                            if (!string.IsNullOrEmpty(demographic.leftPrimaryAnsiBase64)) bioLeftPrimary.base64Ansi = demographic.leftPrimaryAnsiBase64;
                            if (!string.IsNullOrEmpty(demographic.leftPrimaryWsqBase64)) bioLeftPrimary.base64Wsq = demographic.leftPrimaryWsqBase64;
                            if (!string.IsNullOrEmpty(demographic.leftPrimaryJpgBase64)) bioLeftPrimary.base64Jpg = demographic.leftPrimaryJpgBase64;
                            bioLeftPrimary.fpPosition = (short)FingerPosition.LeftIndex;
                            bioLeftPrimary.fpCode = bioLeftPrimary.GetFingerCode((short)FingerPosition.LeftIndex);
                            if (biometrics == null) biometrics = new List<Class.CreateCitizen.objPayload.objBiometric>();
                            biometrics.Add(bioLeftPrimary);
                        }

                        if (string.IsNullOrEmpty(demographic.rightBackupAnsiBase64) & string.IsNullOrEmpty(demographic.rightBackupWsqBase64) & string.IsNullOrEmpty(demographic.rightBackupJpgBase64)) { }
                        else
                        {
                            if (!string.IsNullOrEmpty(demographic.rightBackupAnsiBase64)) bioRightBackup.base64Ansi = demographic.rightBackupAnsiBase64;
                            if (!string.IsNullOrEmpty(demographic.rightBackupWsqBase64)) bioRightBackup.base64Wsq = demographic.rightBackupWsqBase64;
                            if (!string.IsNullOrEmpty(demographic.rightBackupJpgBase64)) bioRightBackup.base64Jpg = demographic.rightBackupJpgBase64;
                            bioRightBackup.fpPosition = (short)FingerPosition.RightThumb;
                            bioRightBackup.fpCode = bioRightBackup.GetFingerCode((short)FingerPosition.RightThumb);
                            if (biometrics == null) biometrics = new List<Class.CreateCitizen.objPayload.objBiometric>();
                            biometrics.Add(bioRightBackup);
                        }

                        if (string.IsNullOrEmpty(demographic.rightPrimaryAnsiBase64) & string.IsNullOrEmpty(demographic.rightPrimaryWsqBase64) & string.IsNullOrEmpty(demographic.rightPrimaryJpgBase64)) { }
                        else
                        {
                            if (!string.IsNullOrEmpty(demographic.rightPrimaryAnsiBase64)) bioRightPrimary.base64Ansi = demographic.rightPrimaryAnsiBase64;
                            if (!string.IsNullOrEmpty(demographic.rightPrimaryWsqBase64)) bioRightPrimary.base64Wsq = demographic.rightPrimaryWsqBase64;
                            if (!string.IsNullOrEmpty(demographic.rightPrimaryJpgBase64)) bioRightPrimary.base64Jpg = demographic.rightPrimaryJpgBase64;
                            bioRightPrimary.fpPosition = (short)FingerPosition.RightIndex;
                            bioRightPrimary.fpCode = bioRightPrimary.GetFingerCode((short)FingerPosition.RightIndex);
                            if (biometrics == null) biometrics = new List<Class.CreateCitizen.objPayload.objBiometric>();
                            biometrics.Add(bioRightPrimary);
                        }
                    }

                    payload.address = address;
                    payload.relationship = relationships.ToArray();
                    payload.contact = contacts.ToArray();
                    payload.contactPerson = contactPerson;
                    if (photo != null) payload.photo = photo;
                    if (signature != null) payload.signature = signature;
                    if (biometrics != null) payload.biometric = biometrics.ToArray();

                    createCitizen.geoLocation = geoLocation;
                    createCitizen.payload = payload;

                    soapRequest = Newtonsoft.Json.JsonConvert.SerializeObject(createCitizen);

                    Console.WriteLine(soapRequest);

                    string payloadFolder = string.Format(@"{0}\Payload", Application.StartupPath);
                    string payloadFile = string.Format(@"{0}\{1}_{2}_{3}.txt", payloadFolder, payload.firstName, payload.middleName, payload.lastName);
                    //if (!Directory.Exists(payloadFolder)) Directory.CreateDirectory(payloadFolder);
                    System.IO.File.WriteAllText(payloadFile, soapRequest);

                    LabelStatus("Sending payload to api");
                    WriteToLog(string.Format("[PROCESS] Sending payload to api"));
                    string f = subDir2.Substring(subDir2.LastIndexOf(@"\") + 1);
                    ExecuteApiRequest(txtRootUrl.Text + "/create", soapRequest, ref soapResponse, ref errResponse);
                    if(errResponse!="") WriteToLog(string.Format("[FAILED] {0} {1} Error on execuate api request - " + errResponse, f, demographic.sessionReference));
                    //WriteToLog(string.Format("[DONE] {0} {1} SEND PAYLOAD", f, demographic.sessionReference));

                    if (!Directory.Exists(string.Format("Processed\\{0}", f))) Directory.Move(subDir2, string.Format("Processed\\{0}", f));
                    else Directory.Move(subDir2, string.Format("Processed\\{0}_{1}", f, DateTime.Now.ToString("hhmmss")));
                  
                    recordCntr += 1;
                    if (tokenResetterCnt == Convert.ToInt32(txtTokenResetCntr.Text))
                    {
                        tokenResetterCnt = 1;
                        txtToken.Text = "";
                    }
                    else tokenResetterCnt += 1;
                    System.Threading.Thread.Sleep(Convert.ToInt32(txtInterval.Text));
                }
                catch (Exception ex)
                {
                    ////WriteToLog(refData + "|1|Failed to create payload. " + errResponse + "|0");
                    //SaveLog(Application.StartupPath + @"\Logs\failedLog.txt", refData);
                    //sbFailed.Append(refData + Environment.NewLine);
                    WriteToLog(string.Format("[FAILED] {0} {1} Not valid demographic", demographic.sessionReference, demographic.sessionReference));
                }
            }
            else
            {
                WriteToLog(string.Format("[FAILED] {0} {1} Not valid demographic", demographic.sessionReference, demographic.sessionReference));
                ////WriteToLog(refData + "|1|Failed to include demographic. " + exceptions + "|0");
                //SaveLog(Application.StartupPath + @"\Logs\failedLog.txt", refData);
                //sbFailed.Append(refData + Environment.NewLine);
            }
            //}

            //string endTime = DateTime.Now.ToString();
            //TimeSpan ts = Convert.ToDateTime(endTime) - Convert.ToDateTime(startTime);
            //WriteToLog("End of process|2|" + endTime + "|2");
            //WriteToLog("Process time|2|" + ts.Minutes.ToString("N0") + " minutes|2");

            string sessionRef = DateTime.Now.ToString("hhmmss");

            System.IO.File.WriteAllText(Application.StartupPath + @"\Logs\successLog" + sessionRef + ".txt", sbSuccess.ToString());
            System.IO.File.WriteAllText(Application.StartupPath + @"\Logs\failedLog" + sessionRef + ".txt", sbFailed.ToString());
            rtb.SaveFile(Application.StartupPath + @"\Logs\uploadLog" + sessionRef + ".txt", RichTextBoxStreamType.PlainText);          
        }

        private string lastKeyFile = Application.StartupPath + "\\lastKeyFile";

        private bool IsValidate(Demographic demographic, ref string exception)
        {
            StringBuilder sb = new StringBuilder();

            switch (demographic.gender.Substring(0, 1))
            {
                case "F":
                case "M":
                    break;
                default:
                    sb.Append("Invalid gender. ");
                    break;
            }

            if(chkPhoto.Checked) if (String.IsNullOrEmpty(demographic.photoBase64)) sb.Append("No photo. ");

            exception = sb.ToString();

            if (exception == "") return true;
            else return false;
        }

        public class RefreshTokenResultJSON
        {
            public string access_token { get; set; }
        }

        public bool ExecuteApiRequest(string url, string soapStr, ref string soapResponse, ref string err)
        {
            //byte[] SoapByte;

            ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                   SecurityProtocolType.Tls11 |
                                   SecurityProtocolType.Tls12;

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            System.Net.WebRequest request = System.Net.HttpWebRequest.Create(txtAuthURL.Text);

            try
            {
                // -- Refresh the access token
                

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string accessToken = "OaOXXXXTaSucp8XXcgXXH";

                try
                {
                    if (txtToken.Text == "")
                    {
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";

                        System.Collections.Specialized.NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
                       
                        outgoingQueryString.Add("username", txtAuthUser.Text);
                        outgoingQueryString.Add("password", txtAuthPass.Text);
                        outgoingQueryString.Add("grant_type", txtAuthGrantType.Text);
                        outgoingQueryString.Add("client_id", txtAuthClientId.Text);
                        byte[] postBytes = new ASCIIEncoding().GetBytes(outgoingQueryString.ToString());

                        WriteToLog(string.Format("[PROCESS] Requesting token"));

                        Stream postStream = request.GetRequestStream();
                        postStream.Write(postBytes, 0, postBytes.Length);
                        postStream.Flush();
                        postStream.Close();

                        using (System.Net.WebResponse response = request.GetResponse())
                        {
                            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                            {
                                dynamic jsonResponseText = streamReader.ReadToEnd();
                                RefreshTokenResultJSON jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponseText, typeof(RefreshTokenResultJSON));
                                accessToken = jsonResult.access_token;

                                txtToken.Text = accessToken;
                                Application.DoEvents();

                                jsonResult = null;
                                jsonResponseText = null;
                            }
                        }

                        WriteToLog(string.Format("[DONE] Token generated"));
                    }
                }
                catch (Exception ex2)
                {
                    err = "Failed to generate token " + ex2.Message;
                    return false;
                }

                //return false;

                Send(url, soapStr);

                return true;
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
            finally
            {
                //SoapByte = null;
                myHttpWebRequest = null;
                request = null;

            }
        }

        public async Task<bool> ExecuteApiRequest2(string url, string soapStr)
        {
            byte[] SoapByte;

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            System.Net.WebRequest request = System.Net.HttpWebRequest.Create(txtAuthURL.Text);

            try
            {

                ServicePointManager.Expect100Continue = true;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                       SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;

                string accessToken = "OaOXXXXTaSucp8XXcgXXH";

                try
                {
                    if (txtToken.Text == "")
                    {
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";

                        System.Collections.Specialized.NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
                        //outgoingQueryString.Add("username", "bayambang@filkonnect.com");
                        //outgoingQueryString.Add("password", "4n0T6jeL9oPoxMy3");
                        //outgoingQueryString.Add("grant_type", "password");
                        //outgoingQueryString.Add("client_id", "aris");

                        outgoingQueryString.Add("username", "dev@aris.com");
                        outgoingQueryString.Add("password", "P@ssw0rd");
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
                                RefreshTokenResultJSON jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponseText, typeof(RefreshTokenResultJSON));
                                accessToken = jsonResult.access_token;

                                txtToken.Text = accessToken;
                                Application.DoEvents();

                                jsonResult = null;
                                jsonResponseText = null;
                            }
                        }
                    }
                }
                catch (Exception ex2)
                {
                    //err = "Failed to generate token " + ex2.Message;
                    return false;
                }

                String response2 = await MakeRequestAsync(url, soapStr);

                return true;
            }
            catch (WebException e)
            {
                //err = string.Format("WebException : {0}", e.Status);
                return false;
            }
            catch (Exception e)
            {
                //err = string.Format("Runtime ex : {0}", e.Message);
                return false;
            }
            finally
            {
                SoapByte = null;
                myHttpWebRequest = null;
                request = null;

            }
        }

        private void FinishWebRequest(IAsyncResult result)
        {
            HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
            Console.WriteLine("done");
        }

        private async Task<String> MakeRequestAsync(String url, string soapStr)
        {
            String responseText = await Task.Run(() =>
            {
                try
                {
                    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                    byte[] SoapByte;
                    SoapByte = System.Text.Encoding.UTF8.GetBytes(soapStr);
                    request.ContentType = "application/json";
                    request.ContentLength = SoapByte.Length;

                    request.Headers.Add("Authorization", "bearer " + txtToken.Text);
                    request.Method = "POST";
                    WebResponse response = request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    return new StreamReader(responseStream).ReadToEnd();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                return null;
            });

            return responseText;
        }
       
        private int totalDeleted = 0;
        private bool isDeleteProcessDone = true;
        private int dataThreshold = 1000;
        private int totalDeletedVar = 0;

        private void button2_Click(object sender, EventArgs e)
        {
            totalDeletedVar = 0;
            totalDeleted = 0;

            //Thread t = new Thread(new ThreadStart(UploadDemographicDataCitizen));
            //t.Start();
        }

        private DataTable dtPlantData = null;

        private void GetDataFromPlant2()
        {
            var fileName = string.Format(@"{0}\plant_data.xls", Application.StartupPath);
            var connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;IMEX=1;HDR=NO;TypeGuessRows=0;ImportMixedTypes=Text\"";
            using (var conn = new System.Data.OleDb.OleDbConnection(connectionString))
            {
                conn.Open();

                var sheets = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [" + sheets.Rows[0]["TABLE_NAME"].ToString() + "] WHERE F1<>'URN'";

                    var adapter = new System.Data.OleDb.OleDbDataAdapter(cmd);
                    var ds = new DataSet();
                    adapter.Fill(ds);
                    dtPlantData = ds.Tables[0];
                }
            }
        }

        //private void GetDataFromPlant2()
        //{
        //    var fileName = string.Format(@"{0}\plant_data.xlsx", Application.StartupPath);
        //    //Create a new workbook
        //    Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
        //    //Load a file and imports its data
        //    workbook.LoadFromFile(fileName);

        //    //Initialize worksheet
        //    Spire.Xls.Worksheet sheet = workbook.Worksheets[0];

        //    // get the data source that the grid is displaying data 
        //    dtPlantData = sheet.ExportDataTable();

        //    OfficeOpenXml.ExcelWorkbook w = new OfficeOpenXml.ExcelWorkbook();
        //    w.
        //}

        private bool IsExistInPlantData(string sessionReference, string firstName, string lastName)
        {
            //if (dtPlantData.Select(string.Format("SessionReference='{0}' AND FName='{1}' AND LName='{2}'", sessionReference, firstName, lastName)).Length > 0) return true;
            if (dtPlantData.Select(string.Format("F16='{0}' AND F2='{1}' AND F4='{2}'", sessionReference, firstName, lastName)).Length > 0) return true;
            else return false;
        }

        private void Bak_Code()
        {
            string sourceFolder = @"D:\WORK\BAYAMBANG\uploaded at plant\ReferenceSource20171026_2995";
            string destiFolder = @"D:\WORK\BAYAMBANG\uploaded at plant conso";

            List<string> li = new List<string>();
            li.Add("StationName_20170315_041717_0060");
            li.Add("StationName_20170315_042422_0061");
            li.Add("StationName_20170315_043038_0062");
            li.Add("StationName_20170315_043356_0063");
            li.Add("StationName_20170315_043706_0064");
            li.Add("StationName_20170315_044145_0065");
            li.Add("StationName_20170316_094820_0001");
            li.Add("StationName_20170316_095122_0002");
            li.Add("StationName_20170316_095435_0003");
            li.Add("StationName_20170316_095920_0004");
            li.Add("StationName_20170316_100242_0005");
            li.Add("StationName_20170316_100518_0006");
            li.Add("StationName_20170316_100807_0007");
            li.Add("StationName_20170316_101103_0008");
            li.Add("StationName_20170316_101402_0009");
            li.Add("StationName_20170316_101724_0010");
            li.Add("StationName_20170316_101955_0011");
            li.Add("StationName_20170316_102343_0012");
            li.Add("StationName_20170316_103024_0013");
            li.Add("StationName_20170316_103327_0014");
            li.Add("StationName_20170316_103617_0015");
            li.Add("StationName_20170316_104025_0016");
            li.Add("StationName_20170316_104323_0017");
            li.Add("StationName_20170316_104734_0018");
            li.Add("StationName_20170316_105046_0019");
            li.Add("StationName_20170316_105417_0020");
            li.Add("StationName_20170316_105912_0021");

            foreach (string f in li)
            {
                string refFolder = sourceFolder;
                string urnFolder = string.Format(@"{0}\{1}", refFolder, f);
                string urnDestiFolder = string.Format(@"{0}\{1}", destiFolder, f);

                if (!Directory.Exists(urnDestiFolder))
                {
                    DirectoryCopy(urnFolder, urnDestiFolder, false);
                }
                else
                {
                    WriteToLog(f + " already exist");
                }
            }

            MessageBox.Show("Done!");

            return;
        }

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    Thread t = new Thread(new ThreadStart(GetDemographicConso));
        //    t.Start();
        //}

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sourceFile = @"D:\WORK\BAYAMBANG\missing73.txt";
            string sourceFolder = @"D:\WORK\BAYAMBANG\uploaded at plant conso";
            string destiFolder = @"D:\WORK\BAYAMBANG\test";

            using (StreamReader sr = new StreamReader(sourceFile))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    foreach (string subDir in Directory.GetDirectories(sourceFolder))
                    {
                        string refFolder = string.Format(@"{0}\{1}", subDir, line);
                        if (Directory.Exists(refFolder))
                        {
                            DirectoryCopy(refFolder, string.Format(@"{0}\{1}", destiFolder, line), false);
                            break;
                        }
                    }
                }

                MessageBox.Show("Done!");

                //using (StreamReader sr = new StreamReader(sourceFile))
                //{
                //    while (!sr.EndOfStream)
                //    {
                //        string line = sr.ReadLine();
                //        string refFolder = string.Format(@"{0}\{1}", sourceFolder, line.Split('|')[0]);
                //        if (!Directory.Exists(refFolder)) { WriteToLog(line.Split('|')[0] + " directory not exist (0)"); }
                //        else
                //        {
                //            string urnFolder = string.Format(@"{0}\{1}", refFolder, line.Split('|')[1]);
                //            if (!Directory.Exists(urnFolder)) { WriteToLog(line.Split('|')[1] + " directory not exist (1)"); }
                //            else
                //            {
                //                string urnDestiFolder = string.Format(@"{0}\{1}", destiFolder, line.Split('|')[1]);
                //                if (!Directory.Exists(urnDestiFolder))
                //                {
                //                    //Directory.CreateDirectory(urnDestiFolder);
                //                    DirectoryCopy(urnFolder, urnDestiFolder, false);
                //                }
                //                else
                //                {
                //                    WriteToLog(line.Split('|')[1] + " already exist");
                //                }

                //            }
                //        }
                //    }
            }
        }


        private static ManualResetEvent allDone = new ManualResetEvent(false);
        public static string soapStr10 = "";
        public static string tok = "";

        public void TestAPICall(string url, string soapStr)
        {
            // Create a new HttpWebRequest object.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.contoso.com/example.aspx");

            //request.ContentType = "application/x-www-form-urlencoded";

            // Set the Method property to 'POST' to post data to the URI.
            //byte[] SoapByte;
            //SoapByte = System.Text.Encoding.UTF8.GetBytes(soapStr);
            //request.ContentType = "application/json";
            //request.ContentLength = SoapByte.Length;

            //request.Headers.Add("Authorization", "bearer " + txtToken.Text);
            //request.Method = "POST";

            soapStr10 = soapStr;
            tok = txtToken.Text;

            // start the asynchronous operation
            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);

            // Keep the main thread from continuing while the asynchronous 
            // operation completes. A real world application 
            // could do something useful such as updating its user interface. 
            allDone.WaitOne();
        }

        private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            byte[] SoapByte;
            SoapByte = System.Text.Encoding.UTF8.GetBytes(soapStr10);
            request.ContentType = "application/json";
            request.ContentLength = SoapByte.Length;

            request.Headers.Add("Authorization", "bearer " + tok);
            request.Method = "POST";


            // End the operation
            Stream postStream = request.EndGetRequestStream(asynchronousResult);

            //Console.WriteLine("Please enter the input data to be posted:");
            //string postData = soapStr10; //Console.ReadLine();

            //// Convert the string into a byte array. 
            //byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //// Write to the request stream.
            //postStream.Write(byteArray, 0, postData.Length);
            //postStream.Close();

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }

        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            string responseString = streamRead.ReadToEnd();
            Console.WriteLine(responseString);
            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();
            allDone.Set();
        }

        private bool checkCubaoVpnConnection()
        {
            bool resp = pingCubaoVPN();
            if (!resp) return pingCubaoVPN();
            return resp;

            //using (StreamReader sr = new StreamReader(@"\\172.16.47.171\itdg\checkCon.txt"))
            //{
            //    if (sr.ReadToEnd() == "1") return true; else return false;
            //}
        }

        private bool pingCubaoVPN()
        {
            try
            {
                System.Net.NetworkInformation.Ping myPing = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply reply = myPing.Send("172.16.47.171", 1000);
                if (reply != null)
                {
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success) return true;
                    else return false;

                    //Console.WriteLine("Status :  " + reply.Status + " \n Time : " + reply.RoundtripTime.ToString() + " \n Address : " + reply.Address);
                    ////Console.WriteLine(reply.ToString());

                    //return true;
                }

                return false;
            }
            catch
            {
                Console.WriteLine("ERROR: You have Some TIMEOUT issue");
                return false;
            }
        }


        public void Send(string url, string soapStr)
        {
            System.Diagnostics.Stopwatch watchSend = new System.Diagnostics.Stopwatch();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                   SecurityProtocolType.Tls11 |
                                   SecurityProtocolType.Tls12;

            try
            {
                var uri = new Uri(txtRootUrl.Text);
                string baseUrl = string.Format("http://{0}", uri.Authority);
                if (txtRootUrl.Text.Contains("https://")) baseUrl = string.Format("https://{0}", uri.Authority);
                string otherUrl = uri.LocalPath;

                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(8);
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", txtToken.Text);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //var myContent = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                var buffer = System.Text.Encoding.UTF8.GetBytes(soapStr);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                byteContent.Headers.ContentLength = buffer.Length;

                //startTime = DateTime.Now;
                watchSend.Start();
                WriteToLog(string.Format("[PROCESS] Sending create payload"));

                string resultContent = "";
                HttpResponseMessage response = client.PostAsync(otherUrl, byteContent).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {

                    // Parse the response body.
                    resultContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Success");
                }
                else
                {
                    Console.WriteLine("Failed");
                }

                client.Dispose();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
            finally
            {              
                watchSend.Stop();
                TimeSpan timeSpan = watchSend.Elapsed;
                WriteToLog(string.Format("[DONE] Send complete {0}s {1}ms ", timeSpan.Seconds, timeSpan.Milliseconds));
            }
        }

        private void BayambangUploadingToFilConnect2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Url = txtRootUrl.Text;
            Properties.Settings.Default.AppId = txtApplicationId.Text;
            Properties.Settings.Default.LeafId = txtLeafId.Text;
            Properties.Settings.Default.AuthUrl = txtAuthURL.Text;
            Properties.Settings.Default.AuthUser = txtAuthUser.Text;
            Properties.Settings.Default.AuthPass = txtAuthPass.Text;
            Properties.Settings.Default.AuthGrant = txtAuthGrantType.Text;
            Properties.Settings.Default.AuthClient = txtAuthClientId.Text;
            Properties.Settings.Default.NotifId = txtNotifId.Text;
            Properties.Settings.Default.TokenResetCntr = Convert.ToInt32(txtTokenResetCntr.Text);
            Properties.Settings.Default.SleepInterval = Convert.ToInt32(txtInterval.Text);
            Properties.Settings.Default.Save();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
    }
}
