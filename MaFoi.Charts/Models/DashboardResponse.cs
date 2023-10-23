using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class DashboardResponse
    {
    }
    public class DashboardStatus
    {
        public int total { get; set; }
        public int pending { get; set; }
        public int due { get; set; }
        public int late { get; set; }
        public int onTime { get; set; }
        public int nonCompliant { get; set; }
    }
    public class DashboardCategory
    {
        public string name { get; set; }
        public List<Value> values { get; set; }
    }

    public class Value
    {
        public string name { get; set; }
        public int value { get; set; }
    }

    
    public class DashBoardAct
    {
        public string name { get; set; }
        public string establishmentType { get; set; }
        public string lawId { get; set; }
        public Law Law { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class DashBoardActivity
    {
        public string name { get; set; }
        public string type { get; set; }
        public string periodicity { get; set; }
        public string calendarType { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class DashBoardAssociateCompany
    {
        public string code { get; set; }
        public DateTime datePosted { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public string companyType { get; set; }
        public string businessType { get; set; }
        public string pan { get; set; }
        public string pan_fullname { get; set; }
        public string pan_surname { get; set; }
        public string pan_designation { get; set; }
        public string pan_mobile { get; set; }
        public string pan_email { get; set; }
        public string pan_place { get; set; }
        public string gstn_no { get; set; }
        public string ccEmailAlert { get; set; }
        public string tan { get; set; }
        public string websiteUrl { get; set; }
        public string companyAddress { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
        public string contactPersonName { get; set; }
        public string contactPersonEmail { get; set; }
        public string contactPersonDesignation { get; set; }
        public string contactPersonMobile { get; set; }
        public string establishmentType { get; set; }
        public string pF_Ac_No { get; set; }
        public string pF_Establishment_Code { get; set; }
        public string pF_Deduction_Percent { get; set; }
        public string pF_Base_Limit { get; set; }
        public string pF_Establishment_Id { get; set; }
        public string esiC_Ac_No { get; set; }
        public string esiC_CutOff_Limit { get; set; }
        public string esiC_Deduction_Percent { get; set; }
        public string esiC_Contribution { get; set; }
        public string esiC_FullName { get; set; }
        public string esiC_Designation { get; set; }
        public string esiC_Place { get; set; }
        public string parentCompanyId { get; set; }
        public bool isParent { get; set; }
        public bool isActive { get; set; }
        public string isCopied { get; set; }
        public string employees { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class DashBoardCities
    {
        public string code { get; set; }
        public string name { get; set; }
        public string stateId { get; set; }
        public State state { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class DashBoardCompany
    {
        public string code { get; set; }
        public DateTime datePosted { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public string companyType { get; set; }
        public string businessType { get; set; }
        public string pan { get; set; }
        public string pan_fullname { get; set; }
        public string pan_surname { get; set; }
        public string pan_designation { get; set; }
        public string pan_mobile { get; set; }
        public string pan_email { get; set; }
        public string pan_place { get; set; }
        public string gstn_no { get; set; }
        public string ccEmailAlert { get; set; }
        public string tan { get; set; }
        public string websiteUrl { get; set; }
        public string companyAddress { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
        public string contactPersonName { get; set; }
        public string contactPersonEmail { get; set; }
        public string contactPersonDesignation { get; set; }
        public string contactPersonMobile { get; set; }
        public string establishmentType { get; set; }
        public string pF_Ac_No { get; set; }
        public string pF_Establishment_Code { get; set; }
        public string pF_Deduction_Percent { get; set; }
        public string pF_Base_Limit { get; set; }
        public string pF_Establishment_Id { get; set; }
        public string esiC_Ac_No { get; set; }
        public string esiC_CutOff_Limit { get; set; }
        public string esiC_Deduction_Percent { get; set; }
        public string esiC_Contribution { get; set; }
        public string esiC_FullName { get; set; }
        public string esiC_Designation { get; set; }
        public string esiC_Place { get; set; }
        public string parentCompanyId { get; set; }
        public bool isParent { get; set; }
        public bool isActive { get; set; }
        public string isCopied { get; set; }
        public string employees { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class DashBoardComplianceManager
    {
        public string userName { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public DateTime dateOfBirth { get; set; }
        public DateTime lastLoginDate { get; set; }
        public DateTime dateOfJoining { get; set; }
        public string status { get; set; }
        public bool isActive { get; set; }
        public string loginOTP { get; set; }
        public object userRoles { get; set; }
        public int incorrectLogins { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class DashBoardComplianceOwner
    {
        public string userName { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public DateTime dateOfBirth { get; set; }
        public DateTime lastLoginDate { get; set; }
        public DateTime dateOfJoining { get; set; }
        public string status { get; set; }
        public bool isActive { get; set; }
        public string loginOTP { get; set; }
        public object userRoles { get; set; }
        public int incorrectLogins { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class DashBoardDepartment
    {
        public string id { get; set; }
        public string shortCode { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string verticalId { get; set; }
        public DashBoardVertical vertical { get; set; }
    }

    public class DashBoardList
    {
        public string actStateMappingId { get; set; }
        public object actStateMapping { get; set; }
        public string auditStatus { get; set; }
        public string auditRemarks { get; set; }
        public int day { get; set; }
        public string month { get; set; }
        public int year { get; set; }
        public DateTime startDate { get; set; }
        public DateTime dueDate { get; set; }
        public DateTime savedDate { get; set; }
        public DateTime submittedDate { get; set; }
        public DateTime auditedDate { get; set; }
        public string actId { get; set; }
        public DashBoardAct act { get; set; }
        public string ruleId { get; set; }
        public DashBoardRule rule { get; set; }
        public string companyId { get; set; }
        public DashBoardCompany company { get; set; }
        public string associateCompanyId { get; set; }
        public DashBoardAssociateCompany associateCompany { get; set; }
        public string locationId { get; set; }
        public DashBoardLocation location { get; set; }
        public string activityId { get; set; }
        public DashBoardActivity activity { get; set; }
        public string status { get; set; }
        public string formsStatusRemarks { get; set; }
        public bool published { get; set; }
        public string auditted { get; set; }
        public string veriticalId { get; set; }
        public DashBoardVeritical veritical { get; set; }
        public string departmentId { get; set; }
        public DashBoardDepartment department { get; set; }
        public string complianceOwnerId { get; set; }
        public DashBoardComplianceOwner complianceOwner { get; set; }
        public string complianceManagerId { get; set; }
        public DashBoardComplianceManager complianceManager { get; set; }
        public DateTime approverDueDate { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class DashBoardLocation
    {
        public string cityId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public DashBoardCities cities { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }
    public class ToDoWithRuleCompliancedata
    {
        public DashBoardList ToDo { get; set; }
        public RuleComplianceDetail RuleComplianceDetails { get; set; }
    }
    public class DashBoardreport
    {
        public List<DashBoardList> list { get; set; }
        public List<ToDoWithRuleCompliancedata> rulelist { get; set; }
        public int count { get; set; }
        public List<StatusCount> statusCount { get; set; }
    }

    public class DashBoardRule
    {
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string sectionNo { get; set; }
        public string ruleNo { get; set; }
        public string uniqueIdentifier { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class DashBoardState
    {
        public string code { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
    }

    public class StatusCount
    {
        public string key { get; set; }
        public int value { get; set; }
    }

    public class DashBoardVeritical
    {
        public string id { get; set; }
        public string shortCode { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string companyId { get; set; }
        public Company company { get; set; }
    }

    public class DashBoardVertical
    {
        public string id { get; set; }
        public string shortCode { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string companyId { get; set; }
        public Company company { get; set; }
    }

    public class DashboardTblResponse
    {
        public string LawCategory { get; set; }
        public string Act { get; set; }
        public string Rule { get; set; }
        public string Activity { get; set; }
        public string RuleNo { get; set; }
        public string Sectionno { get; set; }
        public string ActivityType { get; set; }
        public string status { get; set; }
        public string Remarks { get; set; }
        public string Description { get; set; }

    }

    public class ComplainceDeptVerticalMap
    {
       public string LawCategory { get; set; }
        public string Act { get; set; }
        public string Rule { get; set; }
        public string Activity { get; set; }
        public string RuleNo { get; set; }
        public string SectionNo { get; set; }
        public string  ActivityType { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public string  Status { get; set; }
         public string Remarks { get; set; }
        public string departmentName { get; set; }
        public string VerticalName { get; set; }
    }

}