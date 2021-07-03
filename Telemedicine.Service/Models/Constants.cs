using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace Telemedicine.Service.Models
{
    public class Constants
    { 
        //Live 
        public static string TokenEXURLLive = "https://api.tokenex.com/TransparentGatewayAPI/Detokenize";
        public static string AuthNetURLLive = "https://api.authorize.net/xml/v1/request.api"; 
        //Test
        public static string TokenEXURLTest = "https://test-api.tokenex.com/TransparentGatewayAPI/Detokenize";
        public static string AuthNetURLTest = "https://apitest.authorize.net/xml/v1/request.api";

        /*
        Developer Name             : Padmanaban(Joshua) M
        Date of Modified / Created : Nov-11-2020
        Name of the Function/      : GetAzureVirtualPath
        Functionality              : This function returns AzureCdnPath from registry to show user images in ewr.
        */
        public static string GetAzureVirtualPath()
        {
            RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            string VirtualPathName = localKey.OpenSubKey(@"SOFTWARE\GracesoftConn").GetValue("AzureCdnPath").ToString();
            return VirtualPathName;
        }

        /*
        Developer Name             : Padmanaban(Joshua) M
        Date of Modified / Created : Nov-11-2020
        Name of the Function/      : GetAzureStorageConnectionString
        Functionality              : This function returns AzureStorageConnectionString to enable connection 
                            between cms editor pages and azure storage.
        */
        public static string GetAzureStorageConnectionString()
        {
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
            {
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }
            string value = localKey.OpenSubKey(@"SOFTWARE\GracesoftConn").GetValue("AzureStorageConString").ToString();
            return value;
        }
    } 


}

