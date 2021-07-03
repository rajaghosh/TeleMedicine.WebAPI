using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Telemedicine.Service.Models
{ 
    public class AzureUploads
    {
        public string FolderPath { get; set; }
        public string UserId { get; set; }
        public HttpPostedFile HttpPostedFile { get; set; }
        public string FileName { get; set; }
        public byte[] ESignatureData { get; set; } 
        public Stream Stream { get; set; }  
        public dynamic IformFile { get; set; }

        public bool IsImageFile { get; set; }
    }

    public class ImagData
    {
        public int ImgId { get; set; }
        public string Data { get; set; }
    }

    public class FileToUpload
    {
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public long LastModifiedTime { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string FileAsBase64 { get; set; }
        public byte[] FileAsByteArray { get; set; }
    } 

    public class PharmacyDetails
    {
        public int PharmacySequenceId { get; set; }
        public string PharmacyLIC { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyBranch { get; set; }
        public string PharmacyContactNum { get; set; }
        public string PharmacyMangerFirstName { get; set; }
        public string PharmacyManagerLastName { get; set; }
        public string PharmacyConsultantFirstName { get; set; }
        public string PharamacyConsultantLastName { get; set; }
        public string PharmacyEmailAddress { get; set; }
        public string PharmacyAddress { get; set; }
        public string PharmacyCity { get; set; }
        public string PharmacyState { get; set; }
        public string PharmacyCountry { get; set; }
        public string PharmacyZipCode { get; set; }
        public string Pharmacy { get; set; }
        public string PharmacyFax { get; set; }
        public string ParmacySevice1 { get; set; }
        public string ParmacySevice2 { get; set; }
        public string ParmacySevice3 { get; set; }
        public DateTime PharmacyCreatedDate { get; set; }
        public DateTime PharmacyUpdatedDate { get; set; }
        public string PharmacyCreatedBy { get; set; }
        public string PharmacyUpdatedBy { get; set; }  
    }

    public class MedicineDetails
    {
        public string Medicine { get; set; }
        public int MedicineId { get; set; }
        public Boolean Value { get; set; } 
        public string Description { get; set; }
        public int PharmacyId { get; set; }
    }
}
