using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.InfoSetting
{
    public class Patient
    {
        string _pIdent;//病历号
        string _pname;//病人姓名
        string _birth;//生日
        char _sex;//性别 男为M 女为F 其他为O
        string _blood;//血型
        string _race;//种族
        string _address;//病人地址
        string _post;//邮编
        string _phoneNum;//电话号码
        string _workphone;//电话号码，公司
        string _marriage;//婚姻状况
        string _patientType;//病人类型
        string _ybnum;//医保账号
        string _national;//名族
        string _origo;//籍贯
        bool _birthIndic;//多胞胎指示符
        int _birthOrder;//出生次序
        string _demo;//备注
        string _country;//国家
        string _bmilitaryStatus;//军人退伍状态
        string _deathtime;//死亡时间
        bool _bdeath;//死亡标志
        string _age;//年龄
        string _ageunit;//年龄单位    2018-4-28日添加

      
        string _patientid;//住院号/门诊号
        int _bedid;//床位
        string _sickArea;//病区
        string _sickRoom;//病房

        string _feeType;//收费类型
        string _region;//宗教

        public Patient()
        {
            _pIdent = "";
            _pname = "";
            _birth = "";
            _sex='O';
            _blood = "";
            _race = "";
            _address = "";
            _post = "";
            _phoneNum = "";
            _workphone = "";
            _marriage = "";
            _patientType = "";
            _ybnum = "";
            _origo = "";
            _birthIndic = false;
            _birthOrder = 0;
            _demo = "";
            _country = "";
            _bmilitaryStatus = "";
            _deathtime = "";
            _bdeath = false;
            _age = "";
            _ageunit = "";
            _national = "";

            _patientid = "";
            _bedid = 0;
            _sickArea = "";
            _sickRoom = "";
            _feeType = "";
            _region = "";
        }
        /// <summary>
        /// 宗教
        /// </summary>
        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }
        
        /// <summary>
        /// 病历号
        /// </summary>
        public string PIdent
        {
            get { return _pIdent; }set { _pIdent = value; }
        }
        /// <summary>
        /// 病人姓名
        /// </summary>
        public string Pname
        {
            get { return _pname; }set { _pname = value; }
        }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birth
        {
            get { return _birth; }set { _birth = value; }
        }
        /// <summary>
        /// 性别 男为M 女为F 其他为O
        /// </summary>
        public char Sex
        {
            get { return _sex; } set { _sex = value; }
        }
        /// <summary>
        /// 种族
        /// </summary>
        public string Race
        {
            get { return _race; } set { _race = value; }
        }
        /// <summary>
        /// 病人地址
        /// </summary>
        public string Address
        {
            get { return _address; } set { _address = value; }
        }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNum
        {
            get { return _phoneNum; } set { _phoneNum = value; }
        }
        /// <summary>
        /// 电话号码，公司
        /// </summary>
        public string Workphone
        {
            get { return _workphone; }set { _workphone = value; }
        }
        /// <summary>
        /// 婚姻状况
        /// </summary>
        public string Marriage
        {
            get { return _marriage; }set { _marriage = value; }
        }
        /// <summary>
        /// 病人类型
        /// </summary>
        public string PatientType
        {
            get { return _patientType; } set { _patientType = value; }
        }
        /// <summary>
        /// 医保账号
        /// </summary>
        public string Ybnum
        {
            get { return _ybnum; } set { _ybnum = value; }
        }
        /// <summary>
        /// 籍贯
        /// </summary>
        public string Origo
        {
            get { return _origo; }set { _origo = value; }
        }
        /// <summary>
        /// 多胞胎指示符
        /// </summary>
        public bool BirthIndic
        {
            get { return _birthIndic; } set { _birthIndic = value; }
        }
        /// <summary>
        /// 出生次序
        /// </summary>
        public int BirthOrder
        {
            get { return _birthOrder; } set { _birthOrder = value; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Demo
        {
            get { return _demo; }set { _demo = value; }
        }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country
        {
            get { return _country; } set { _country = value; }
        }
        /// <summary>
        /// 军人退伍状态
        /// </summary>
        public string BmilitaryStatus
        {
            get { return _bmilitaryStatus; } set { _bmilitaryStatus = value; }
        }
        /// <summary>
        /// 死亡时间
        /// </summary>
        public string Deathtime
        {
            get { return _deathtime; } set { _deathtime = value; }
        }
        /// <summary>
        /// 死亡标志
        /// </summary>
        public bool Bdeath
        {
            get { return _bdeath; }  set { _bdeath = value; }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public string Age
        {
            get { return _age; } set { _age = value; }
        }
        /// <summary>
        /// 血型
        /// </summary>
        public string Blood
        {
            get { return _blood; } set { _blood = value; }
        }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string Post
        {
            get { return _post; } set { _post = value; }
        }
        /// <summary>
        /// 住院号/门诊号
        /// </summary>
        public string Patientid
        {
            get { return _patientid; }
            set { _patientid = value; }
        }
        /// <summary>
        /// 床位
        /// </summary>
        public int Bedid
        {
            get { return _bedid; }
            set { _bedid = value; }
        }
        /// <summary>
        /// 病房
        /// </summary>
        public string SickRoom
        {
            get { return _sickRoom; }
            set { _sickRoom = value; }
        }

        /// <summary>
        /// 病区
        /// </summary>
        public string SickArea
        {
            get { return _sickArea; }
            set { _sickArea = value; }
        }
        /// <summary>
        /// 收费类型
        /// </summary>
        public string FeeType
        {
            get { return _feeType; }
            set { _feeType = value; }
        }
        /// <summary>
        /// 名族
        /// </summary>
        public string National
        {
            get { return _national; }set { _national = value; }
        }
        /// <summary>
        /// 年龄单位
        /// </summary>
        public string Ageunit
        {
            get { return _ageunit; }
            set { _ageunit = value; }
        }

        /// <summary>
        /// 根据病人病历号获取病人信息
        /// </summary>
        /// <param name="pident">病人病历号</param>

        public void LoadData(string[] info)
        {
            PIdent = info[3];
            Pname = info[5];
            Birth = info[7];
            if (info[8] == "M")
                Sex = 'M';
            if (info[8] == "F")
                Sex = 'F';
            if (info[8] == "O")
                Sex = 'O';
            Blood = info[9];
            Race = info[10];
            Address = info[11];
            Post = info[12];
            PhoneNum = info[13];
            Workphone = info[14];
            Marriage = info[16];
            PatientType = info[18];
            Ybnum = info[19];
            Origo = info[23];
            if (info[24] != "")
            {
                BirthIndic = Convert.ToBoolean(info[24]);
            }
            if (info[25] != "")
            {
                BirthOrder = Convert.ToInt32(info[25]);
            }
            Demo = info[26];
            Country = info[28];
            BmilitaryStatus = info[27];
            Deathtime = info[29];
            if (info[30] != "")
            {
                Bdeath = Convert.ToBoolean(info[30]);
            }
            string[] sp = info[31].Split('^');
            if (sp != null)
            {
                Age = sp[0];
                Ageunit = sp[1];
            }
            National = info[22];

            PIdent = info[3];
            Patientid = info[2];
            if (info[4] != "")
            {
                Bedid = Convert.ToInt32(info[4]);
            }
            if (info[6] != "")
            {
                string[] s = info[6].Split('^');
                SickArea = s[0];
                SickRoom = s[1];
            }
            SickRoom = info[6];
            FeeType = info[20];
        }
        /// <summary>
        /// 保存病人信息
        /// </summary>
        /// <returns></returns>
        public bool SaveInfo()
        {
            //年龄 性别 年龄 门诊号 病区 病房 床号 病历号
            string str = @"INSERT INTO tbSampleInfo(PatientName, Sex, Age, ClinicNo, InpatientArea, Ward, BedNo, MedicaRecordNo) " +
                "VALUES(" + Pname + "," + Sex + "," + Age + "," + Patientid + ","+SickArea +","+SickRoom +","+Bedid +","+PIdent+")";
            //样本ID,样本类型，样本来源,样本位号,样本杯类型，重复测量次数，试剂批号，测试项目，急诊,
             str = @"INSERT INTO tbSampleInfo( SampleNo,SampleType, Source, Position,SampleContainer, RepeatCount, RegentBatch, ProjectName, Emergency, Diagnosis, Department, SendDoctor, SendDateTime, InspectDoctor, Status) " +
              "VALUES()";
            return false;
        }
    }
}
