using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Telemedicine.Service.Models;  
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace Telemedicine.Service.Managers
{
    public class CommonFunctions
    {
        CloudBlobClient cloudBlobClient;
        CloudBlobContainer cloudBlobContainer;
        BlobContainerPermissions permissions;
        CloudBlockBlob cloudBlockBlob;
        CloudStorageAccount storageAccount;
        readonly string storageConnectionString = Constants.GetAzureStorageConnectionString();
        readonly string azureVirtualPath = Constants.GetAzureVirtualPath();

        /*
        Developer Name             : Padmanaban(Joshua) M
        Date of Modified / Created : July-10-2019
        Name of the Class/         : AzureImageUploads
        Functionality              : It contains code for azure blob uploads.
        */
        async public Task<string[]> AzureUploads(AzureUploads azureUploads)
        {
            _ = azureUploads.HttpPostedFile;
            string[] result = new string[2];
            if (azureUploads.ESignatureData == null) azureUploads.ESignatureData = new byte[0];
            try
            {
                if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
                {
                    cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("userfiles");
                    await cloudBlobContainer.CreateIfNotExistsAsync();
                    permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);
                    string blobReferencePath = azureUploads.FolderPath + azureUploads.FileName;

                    cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobReferencePath);
                    if (azureUploads.IsImageFile) { cloudBlockBlob.Properties.ContentType = "image/jpeg"; } 

                    if (azureUploads.ESignatureData.Length > 0)
                    {
                        await cloudBlockBlob.UploadFromByteArrayAsync(azureUploads.ESignatureData, 0, azureUploads.ESignatureData.Length);
                    }
                    else
                    {
                        foreach (var objFile in azureUploads.IformFile)
                        {
                            using (var fileStream = objFile.OpenReadStream())
                            {
                                await cloudBlockBlob.UploadFromStreamAsync(fileStream);
                            }
                        }                  
                    }
                    result[0] = cloudBlockBlob.Name;
                    result[1] = cloudBlockBlob.Uri.ToString();

                    //result = azureVirtualPath + blobReferencePath;
                    //return cloudBlockBlob;
                   var output = IsBlobExist(result[1]);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
                //result = "Error while creating object in azure blob"; 
            }
            return result;
        }

        /*
        Developer Name             : Padmanaban(Joshua) M
        Date of Modified / Created : July-17-2019
        Name of the Class/         : GetBlobsList
        Functionality              : It contains code for getting blob from azure storage.
        */
        //public ArrayList GetBlobsList(AzureUploads azureUploads)
        //{
        //    ArrayList objBlobsList = new ArrayList();
        //    string result = string.Empty, blobReferencePath = string.Empty;
        //    try
        //    {
        //        if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
        //        {
        //            cloudBlobClient = storageAccount.CreateCloudBlobClient();
        //            cloudBlobContainer = cloudBlobClient.GetContainerReference("userfiles");
        //            var blobList = cloudBlobContainer.ListBlobs(prefix: azureUploads.FolderPath, useFlatBlobListing: true);
        //            foreach (var blob in blobList)
        //            {
        //                objBlobsList.Add(blob.StorageUri.PrimaryUri.AbsoluteUri);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        result = "Error while getting blobs from azure storage";
        //    }
        //    return objBlobsList;
        //}

        /*
        Developer Name             : Padmanaban(Joshua) M
        Date of Modified / Created : July-17-2019
        Name of the Class/         : IsBlobExist
        Functionality              : It checks whether a blob is exist or not in azure cloud storage.
        */
        async public Task<bool> IsBlobExist(string blobFileName)
        {
            bool isBlobExist = false;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                cloudBlobClient = storageAccount.CreateCloudBlobClient();
                var blob = cloudBlobClient.GetContainerReference("userfiles").GetBlockBlobReference(blobFileName);
                isBlobExist = await blob.ExistsAsync();
                if (isBlobExist)
                {
                    isBlobExist = true;
                }
                else
                {
                    isBlobExist = false;
                }
            }
            return isBlobExist;
        }
    }
}
