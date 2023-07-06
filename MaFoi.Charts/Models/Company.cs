using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace MaFoi.Charts.Models
{
  public class Company : BaseTableEntity
  {
    [StringLength(50), Column(TypeName = "varchar")]
    public string Code { get; set; }
    public DateTime DatePosted { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]

    public string Name { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]
    public string Logo { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]

    public string Email { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]

    public string ContactNumber { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]
    public string CompanyType { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]
    public string BusinessType { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]

    public string PAN { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string pan_fullname { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string pan_surname { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string pan_designation { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]
    public string pan_mobile { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string pan_email  { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string pan_place  { get; set; }
    [StringLength(30), Column(TypeName = "varchar")]
    public string gstn_no { get; set; }

    [StringLength(20), Column(TypeName = "varchar")]
    public string CcEmailAlert { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]

    public string TAN { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]
    public string WebsiteUrl { get; set; }
    [StringLength(500), Column(TypeName = "varchar")]

    public string CompanyAddress { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string City { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string State { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string Country { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]
    public string PostalCode { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]
    public string ContactPersonName { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string ContactPersonEmail { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string ContactPersonDesignation { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]
    public string ContactPersonMobile { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string EstablishmentType { get; set; }

    [StringLength(50), Column(TypeName = "varchar")]
    public string PF_Ac_No { get; set; }

    [StringLength(100), Column(TypeName = "varchar")]
    public string PF_Establishment_Code { get; set; }

    [StringLength(10), Column(TypeName = "varchar")]
    public string PF_Deduction_Percent { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]

    public string PF_Base_Limit { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string PF_Establishment_Id { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string ESIC_Ac_No { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]
    public string ESIC_CutOff_Limit { get; set; }
    [StringLength(10), Column(TypeName = "varchar")]
    public string ESIC_Deduction_Percent { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]
    public string ESIC_Contribution { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]
    public string ESIC_FullName { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string ESIC_Designation { get; set; }
    [StringLength(100), Column(TypeName = "varchar")]
    public string ESIC_Place { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string ParentCompanyId { get; set; }
    public bool IsParent { get; set; }
    public bool IsActive { get; set; }
  }
}
