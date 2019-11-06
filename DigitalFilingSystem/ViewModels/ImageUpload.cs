using System;
using System.Collections.Generic;

namespace DigitalFilingSystem.ViewModels
{
    public class ImageUpload
    {
        public Guid IndexId { get; set; }
        public string Subject { get; set; }
        public string Keyword { get; set; }
        public string DivisonCode { get; set; }
        public string DepartmentCode { get; set; }
        public string SubjectCode { get; set; }
        public string FileCode { get; set; }
        public string GOCode { get; set; }
        public string FileOpenYear { get; set; }
        public string DeskCode { get; set; }
        public string LetterNo { get; set; }
        public string MeetingMinute { get; set; }
        public string MinuteYear { get; set; }
        public bool IsMeetingMinute { get; set; }
        public string FileType { get; set; }
        public bool IsInformattedLetterNo { get; set; }
        public string InformattedLetterNo { get; set; }
        public string ParentMinute { get; set; }
        public DateTime? UploadedDate { get; set; }
        public List<Files>  Files{ get; set; }
    }

    public class Files
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
    }
}