 using Microsoft.WindowsAzure.Storage;
using System.IO;

namespace Telemedicine.Service.Models
{
    public class HttpPostedFile
    {
        public Stream InputStream { get; internal set; }
        public AccessCondition ContentLength { get; internal set; }
    }
}