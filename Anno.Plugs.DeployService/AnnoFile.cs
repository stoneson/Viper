using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anno.Plugs.DeployService
{
    public class DeployAgent: Rpc.Storage.ServiceInformation
    {
        
    }
    public class UpLoadFormData
    {
        /// <summary>
        /// 
        /// </summary>
        public string workingDirectory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string nodeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string cmd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string autoStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string deploySecret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string annoProcessDescription { get; set; }
    }

    public class AnnoFile
    {
        //
        // 摘要:
        //     Gets the raw Content-Type header of the uploaded file.
        public string ContentType
        {
            get; set;
        }

        //
        // 摘要:
        //     Gets the raw Content-Disposition header of the uploaded file.
        public string ContentDisposition
        {
            get; set;
        }

        //
        // 摘要:
        //     Gets the file length in bytes.
        public int Length
        {
            get; set;
        }

        //
        // 摘要:
        //     Gets the form field name from the Content-Disposition header.
        public string Name
        {
            get; set;
        }

        //
        // 摘要:
        //     Gets the file name from the Content-Disposition header.
        public string FileName
        {
            get; set;
        }
        /// <summary>
        /// 内容
        /// </summary>
        public byte[] Content { get; set; }
    }
}
