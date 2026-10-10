using System;
using System.Collections.Generic;

namespace Bidding_Models.Integrations
{
    public class MociApiResponseDto
    {
        public CrNo crNo { get; set; }
        public List<Activity> activities { get; set; }
        public List<Businesslocation> businessLocation { get; set; }
        public List<Person> persons { get; set; }
        public List<Signatory> signatories { get; set; }
        public CapitalData capitalData { get; set; }
    }

    public class CapitalData
    {
        public float cashCapital { get; set; }
        public string code { get; set; }
        public float assetCapital { get; set; }
        public float totalCapital { get; set; }
        public float shareValue { get; set; }
        public float totalShares { get; set; }
        public string capitalVerified { get; set; }
        public CrNo crNo { get; set; }
    }

    public class CrNo
    {
        public string crNumber { get; set; }
        public string companyNameArabic { get; set; }
        public string companyNameWithLegalArabic { get; set; }
        public string companyNameEnglish { get; set; }
        public string companyNameWithLegalEnglish { get; set; }
        public Country country { get; set; }
        public Legalstatus legalStatus { get; set; }
        public Registrationstatus registrationStatus { get; set; }
        public string registrationDate { get; set; }
        public string establishmentDate { get; set; }
        public string subjectToForeignInvestment { get; set; }
        public string expiryDate { get; set; }
        public string isExpired { get; set; }
    }

    public class Country
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }

    public class Legalstatus
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }

    public class Registrationstatus
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }

    public class Activity
    {
        public CrNo crNo { get; set; }
        public string code { get; set; }
        public string isicCode { get; set; }
        public string isicVersion { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public string registrationDate { get; set; }
    }

    public class Businesslocation
    {
        public string code { get; set; }
        public CrNo crNo { get; set; }
        public Town town { get; set; }
        public State state { get; set; }
        public Governorate governorate { get; set; }
    }

    public class Town
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }

    public class State
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }

    public class Governorate
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }

    public class Designation
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }

    public class Person
    {
        public CrNo crNo { get; set; }
        public string code { get; set; }
        public string idNumber { get; set; }
        public string dateOfBirth { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
        public Gender gender { get; set; }
        public Nationality nationality { get; set; }
    }

    public class Gender
    {
        public string code { get; set; }
    }

    public class Nationality
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }

    public class Signatory
    {
        public CrNo crNo { get; set; }
        public string code { get; set; }
        public Designation designation { get; set; }
        public string hasFullAuthority { get; set; }
        public string hasTechnicalAuthority { get; set; }
        public string hasFinancialAuthority { get; set; }
        public string hasAdministrativeAuthority { get; set; }
        public Signaturetype signatureType { get; set; }
        public string signatoryStartDate { get; set; }
        public Person person { get; set; }
    }

    public class Signaturetype
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }
}
