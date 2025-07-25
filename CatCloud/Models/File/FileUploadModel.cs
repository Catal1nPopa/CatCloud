﻿using System;

namespace CatCloud.Models.File
{
    public class FileUploadModel
    {
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
        public long FileSize { get; set; }
        public Guid UploadedByUserId { get; set; }
        public string ContentType { get; set; }
        public bool ShouldEncrypt { get; set; }

        public FileUploadModel(string name, DateTime uploaded, long fileSize,string contentType, bool shouldEncrypt)
        {
            FileName = name;
            UploadedAt = uploaded;
            FileSize = fileSize;
            ContentType = contentType;
            ShouldEncrypt = shouldEncrypt;
        }
    }
}
