using DigitalFilingSystem.Converter;
using DigitalFilingSystem.Enums;
using DigitalFilingSystem.Models;
using DigitalFilingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DigitalFilingSystem.BLL
{
    public class ImageIndexBLL
    {
        private readonly ApplicationDbContext context;
        public ImageIndexBLL()
        {
            context = new ApplicationDbContext();
        }

        public Guid InsertImageIndex(ImageUpload upload, string username)
        {
            ImageIndex imageIndex = null;
            if (!upload.IsMeetingMinute && !string.IsNullOrEmpty(upload.MeetingMinute) && !string.IsNullOrEmpty(upload.MinuteYear))
            {
                imageIndex = context.ImageIndexes.FirstOrDefault(a => a.FileOpenYear == upload.MinuteYear && a.LetterNo == upload.MeetingMinute && a.IsMeetingMinute == true);
                if (imageIndex == null)
                {
                    throw new Exception($"Meeting minute not found");
                }
            }
            ImageIndex image = new ImageIndex();
            image.Id = Guid.NewGuid();
            image.IsMeetingMinute = upload.IsMeetingMinute;
            image.InformattedLetterNo = upload.InformattedLetterNo;
            image.Subject = upload.Subject;
            image.UploadedDate = upload.UploadedDate;
            image.Keyword = upload.Keyword;
            image.LetterNo = upload.LetterNo;
            image.DivisonCode = upload.DivisonCode;
            image.DepartmentCode = upload.DepartmentCode;
            image.GOCode = upload.GOCode;
            image.DeskCode = upload.DeskCode;
            image.SubjectCode = upload.SubjectCode;
            image.FileCode = upload.FileCode;
            image.FileOpenYear = upload.FileOpenYear;
            image.MinuteYear = upload.MinuteYear;
            image.MeetingMinute = upload.MeetingMinute;
            if (imageIndex != null)
            {
                image.ParentId = imageIndex.Id;
            }

            image.Status = (int)StatusEnum.Active;
            image.CreatedBy = username;
            image.CreatedDate = DateConverter.GetLocalZoneDate(DateTime.UtcNow); //update when host in remote.
            context.ImageIndexes.Add(image);
            context.SaveChanges();
            return image.Id;
        }

        internal object GetDashboard(string id = "")
        {
            var today = DateConverter.GetLocalZoneDate(DateTime.UtcNow);
            var lastWeek = today.AddDays(-7);
            var lastMonth = today.AddMonths(-1);
            int todayUpload = 0;
            int lastWeekUpload = 0;
            int lastMonthUpload = 0;
            int totalUpload = 0;
            if (string.IsNullOrEmpty(id))
            {
               todayUpload = context.ImageIndexes.Where(a => a.CreatedDate.Value.Day == today.Day && a.CreatedDate.Value.Month == today.Month && a.CreatedDate.Value.Year == today.Year).Count();
                lastWeekUpload = context.ImageIndexes.Count(a => a.CreatedDate <= today && a.CreatedDate >= lastWeek);
                lastMonthUpload = context.ImageIndexes.Count(a => a.CreatedDate <= today && a.CreatedDate >= lastMonth);
                totalUpload = context.ImageIndexes.Count();
            }
            else
            {
                var user = context.AspNetUsers.FirstOrDefault(a => a.Id == id);
                if(user != null)
                {
                    string username = user.UserName;
                    todayUpload = context.ImageIndexes.Where(a => a.CreatedBy == username && a.CreatedDate.Value.Day == today.Day && a.CreatedDate.Value.Month == today.Month && a.CreatedDate.Value.Year == today.Year).Count();
                    lastWeekUpload = context.ImageIndexes.Count(a => a.CreatedBy == username && a.CreatedDate <= today && a.CreatedDate >= lastWeek);
                    lastMonthUpload = context.ImageIndexes.Count(a => a.CreatedBy == username && a.CreatedDate <= today && a.CreatedDate >= lastMonth);
                    totalUpload = context.ImageIndexes.Count(a => a.CreatedBy == username);
                }
                else
                {
                    throw new Exception("User not found");
                }
                
            }
            
           

            return new { Today = todayUpload, LastWeek = lastWeekUpload, LastMonth = lastMonthUpload, Total = totalUpload };
        }

        public Guid UpdateImageIndex(ImageUpload upload, string username)
        {
            ImageIndex image = context.ImageIndexes.FirstOrDefault(a => a.DivisonCode == upload.DivisonCode &&
            a.DepartmentCode == upload.DepartmentCode
            && a.GOCode == upload.GOCode
            && a.DeskCode == upload.DeskCode
            && a.SubjectCode == upload.SubjectCode
            && a.FileCode == upload.FileCode
            && a.FileOpenYear == upload.FileOpenYear
            && a.LetterNo == upload.LetterNo);

            if (image == null)
            {
                throw new Exception("Document not found to update");
            }

            ImageIndex imageIndex = null;
            if (!upload.IsMeetingMinute && !string.IsNullOrEmpty(upload.MeetingMinute) && !string.IsNullOrEmpty(upload.MinuteYear))
            {
                imageIndex = context.ImageIndexes.FirstOrDefault(a => a.FileOpenYear == upload.MinuteYear && a.LetterNo == upload.MeetingMinute && a.IsMeetingMinute == true);
                if (imageIndex == null)
                {
                    throw new Exception($"Meeting not found by {upload.ParentMinute}");
                }
            }

            image.IsMeetingMinute = upload.IsMeetingMinute;
            image.InformattedLetterNo = upload.InformattedLetterNo;
            image.Subject = upload.Subject;
            image.UploadedDate = upload.UploadedDate;
            image.Keyword = upload.Keyword;
            image.MinuteYear = upload.MinuteYear;
            image.MeetingMinute = upload.MeetingMinute;
            if (imageIndex != null)
            {
                image.ParentId = imageIndex.Id;
            }
            image.Status = (int)StatusEnum.Active;
            image.UpdatedBy = username;
            image.UpdatedDate = DateConverter.GetLocalZoneDate(DateTime.UtcNow); //update when host in remote.
            context.Entry(image).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            //InsertImage(upload, image.Id);
            context.SaveChanges();
            return image.Id;
        }

        public List<ImageUpload> GetTodayImage()
        {
            List<ImageUpload> lstImageIndex = new List<ImageUpload>();
            DateTime date = DateConverter.GetLocalZoneDate(DateTime.UtcNow);
            List<ImageIndex> result = context.ImageIndexes.Where(a => a.CreatedDate.Value.Day == date.Day && a.CreatedDate.Value.Month == date.Month && a.CreatedDate.Value.Year == date.Year).ToList();
            if (result != null && result.Count > 0)
            {
                List<Guid> lstIndexId = result.Select(s => s.Id).ToList();
                List<Image> lstImage = context.Images.Where(a => lstIndexId.Contains(a.IndexId) && a.Status == (int)StatusEnum.Active).ToList();
                if (lstImage == null || (lstImage != null && lstImage.Count <= 0))
                {
                    throw new Exception("No result found.");
                }
                else
                {
                    foreach (ImageIndex index in result)
                    {
                        ImageUpload imageUpload = GetImageUpload(lstImage, index);
                        lstImageIndex.Add(imageUpload);
                    }
                }
            }
            return lstImageIndex;
        }

        public void InsertImage(string file, string fileName, Guid indexId)
        {
            Image image = new Image();
            image.Id = Guid.NewGuid();
            image.IndexId = indexId;
            image.Photo = file;
            image.FileName = fileName;
            image.Status = (int)StatusEnum.Active;
            image.CreatedBy = "";
            image.CreatedDate = DateConverter.GetLocalZoneDate(DateTime.UtcNow);

            context.Images.Add(image);
            context.SaveChanges();
        }

        public List<ImageUpload> SearchImage(ImageUpload search)
        {
            List<ImageUpload> lstImageIndex = new List<ImageUpload>();
            List<ImageIndex> result = null;
            if (search.IsInformattedLetterNo && !string.IsNullOrEmpty(search.InformattedLetterNo))
            {
                // search by informatted letter No
                SqlParameter letterNo = new SqlParameter("@informattedLetterNo", search.InformattedLetterNo);
                result = context.Database.SqlQuery<ImageIndex>("spSearchWithInformattedLetterNo @informattedLetterNo", letterNo).ToList();

            }
            else
            {
                // Search by letter No
                if (!string.IsNullOrEmpty(search.DivisonCode) &&
                    !string.IsNullOrEmpty(search.DepartmentCode) &&
                    !string.IsNullOrEmpty(search.GOCode) &&
                    !string.IsNullOrEmpty(search.SubjectCode) &&
                    !string.IsNullOrEmpty(search.FileCode) &&
                    !string.IsNullOrEmpty(search.DeskCode) &&
                    !string.IsNullOrEmpty(search.LetterNo))
                {
                    SqlParameter divisionCode = new SqlParameter("@divisionCode", search.DivisonCode);
                    SqlParameter departmentCode = new SqlParameter("@departmentCode", search.DepartmentCode);
                    SqlParameter gOCode = new SqlParameter("@gOCode", search.GOCode);
                    SqlParameter deskCode = new SqlParameter("@deskCode", search.DeskCode);
                    SqlParameter subjectCode = new SqlParameter("@subjectCode", search.SubjectCode);
                    SqlParameter fileCode = new SqlParameter("@fileCode", search.FileCode);
                    SqlParameter fileOpenYearCode = new SqlParameter("@fileOpenYearCode", search.FileOpenYear);
                    SqlParameter letterNo = new SqlParameter("@letterNo", search.LetterNo);
                    result = context.Database.SqlQuery<ImageIndex>("spSearchWithLetterNo @divisionCode, @departmentCode, @gOCode, @deskCode, @subjectCode, @fileCode, @fileOpenYearCode, @letterNo", divisionCode, departmentCode, gOCode, deskCode, subjectCode, fileCode, fileOpenYearCode, letterNo).ToList();

                }
            }

            if (!string.IsNullOrEmpty(search.Keyword))
            {
                // Search by keyword
                SqlParameter keyword = new SqlParameter("@keyword", search.Keyword);
                var resultByKeyword = context.Database.SqlQuery<ImageIndex>("exec spSearchWithKeyword @keyword", keyword).ToList();
                if (resultByKeyword != null && resultByKeyword.Count > 0)
                {
                    if (result == null)
                    {
                        result = new List<ImageIndex>();
                    }

                    result.AddRange(resultByKeyword);
                }
            }

            if (result != null && result.Count > 0)
            {
                // get image from table
                List<Guid> lstIndexId = result.Select(s => s.Id).ToList();
                List<Image> lstImage = context.Images.Where(a => lstIndexId.Contains(a.IndexId) && a.Status == (int)StatusEnum.Active).ToList();
                if (lstImage == null || (lstImage != null && lstImage.Count <= 0))
                {
                    throw new Exception("No result found.");
                }
                else
                {
                    foreach (ImageIndex index in result)
                    {
                        ImageUpload imageUpload = GetImageUpload(lstImage, index);
                        lstImageIndex.Add(imageUpload);
                    }
                }
            }
            else
            {
                throw new Exception("No result found.");
            }
            return lstImageIndex;
        }

        public List<ImageUpload> SearchInAdnvace(ImageUpload search)
        {
            List<ImageUpload> lstImageIndex = new List<ImageUpload>();
            List<ImageIndex> result = null;
            string query = "SELECT * FROM ImageIndex WHERE ";
            string condition = string.Empty;
            if (!string.IsNullOrEmpty(search.Keyword))
            {
                condition += " Keyword LIKE CONCAT('% " + search.Keyword + "%')";
            }

            if (!string.IsNullOrEmpty(search.Subject))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " OR [Subject] LIKE CONCAT('% " + search.Keyword + "%'";
                }
                else
                {
                    condition += " [Subject] LIKE CONCAT('% " + search.Keyword + "%'";
                }
            }

            if (!string.IsNullOrEmpty(search.DivisonCode))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND DivisonCode = '" + search.DivisonCode + "'";
                }
                else
                {
                    condition += " DivisonCode = '" + search.DivisonCode + "'";
                }
            }

            if (!string.IsNullOrEmpty(search.DepartmentCode))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND DepartmentCode = '" + search.DepartmentCode + "'";
                }
                else
                {
                    condition += " DepartmentCode = '" + search.DepartmentCode + "'";
                }
            }

            if (!string.IsNullOrEmpty(search.DeskCode))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND DeskCode = '" + search.DeskCode + "'";
                }
                else
                {
                    condition += " DeskCode = '" + search.DeskCode + "'";
                }
            }

            if (!string.IsNullOrEmpty(search.SubjectCode))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND SubjectCode = '" + search.SubjectCode + "'";
                }
                else
                {
                    condition += " SubjectCode = '" + search.SubjectCode + "'";
                }
            }

            if (!string.IsNullOrEmpty(search.FileCode))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND FileCode = '" + search.FileCode + "'";
                }
                else
                {
                    condition += " FileCode = '" + search.FileCode + "'";
                }
            }

            if (!string.IsNullOrEmpty(search.FileOpenYear))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND FileOpenYear = '" + search.FileOpenYear + "'";
                }
                else
                {
                    condition += " FileOpenYear = '" + search.FileOpenYear + "'";
                }
            }

            if (!string.IsNullOrEmpty(search.LetterNo))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND ( LetterNo = '" + search.LetterNo + "'" + " OR MeetingMinute = '" + search.LetterNo + "' )";
                }
                else
                {
                    condition += " ( LetterNo = '" + search.LetterNo + "'" + " OR MeetingMinute = '" + search.LetterNo + "' )";
                }
            }

            if (!string.IsNullOrEmpty(search.MeetingMinute))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND ( LetterNo = '" + search.LetterNo + "'" + " OR MeetingMinute = '" + search.LetterNo + "' )";
                }
                else
                {
                    condition += " ( LetterNo = '" + search.LetterNo + "'" + " OR MeetingMinute = '" + search.LetterNo + "' )";
                }
            }

            if (!string.IsNullOrEmpty(search.MinuteYear))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND MinuteYear = '" + search.MinuteYear + "'";
                }
                else
                {
                    condition += " MinuteYear = '" + search.MinuteYear + "'";
                }
            }

            if (!string.IsNullOrEmpty(search.GOCode))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND GOCode = '" + search.GOCode + "'";
                }
                else
                {
                    condition += " GOCode = '" + search.GOCode + "'";
                }
            }

            if (search.UploadedDate != null && search.UploadedDate.Value != DateTime.MinValue)
            {
                var date = search.UploadedDate.Value.Date;
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND UploadedDate = '" + date.Year + "-" + date.Month.ToString().PadLeft(2, '0') + "-" + date.Day.ToString().PadLeft(2, '0') + "'";
                }
                else
                {
                    condition += " UploadedDate = '" + date.Year + "-" + date.Month.ToString().PadLeft(2, '0') + "-" + date.Day.ToString().PadLeft(2, '0') + "'";
                }
            }

            if (string.IsNullOrEmpty(condition))
            {
                throw new Exception("No condition found for search");
            }
            query += condition;

            //result = context.Database.SqlQuery<ImageIndex>("spAdvanceSearch @keyword, @divisionCode, @departmentCode, @deskCode, @subjectCode, @fileCode, @fileOpenYearCode, @letterNo", keyWord, divisionCode, departmentCode, deskCode, subjectCode, fileCode, fileOpenYearCode, letterNo).ToList();
            result = context.Database.SqlQuery<ImageIndex>(query).ToList();


            if (result != null && result.Count > 0)
            {
                // get image from table
                List<Guid> lstIndexId = result.Select(s => s.Id).ToList();
                List<Image> lstImage = context.Images.Where(a => lstIndexId.Contains(a.IndexId) && a.Status == (int)StatusEnum.Active).ToList();
                if (lstImage == null || (lstImage != null && lstImage.Count <= 0))
                {
                    throw new Exception("No result found.");
                }
                else
                {
                    foreach (ImageIndex index in result)
                    {
                        ImageUpload imageUpload = GetImageUpload(lstImage, index);
                        lstImageIndex.Add(imageUpload);
                    }
                }
            }
            else
            {
                throw new Exception("No result found.");
            }
            return lstImageIndex;
        }

        public ImageUpload GetImageIndexById(string id)
        {
            Guid indexId = Guid.Empty;
            try
            {
                indexId = new Guid(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Index Id not found");
            }

            ImageIndex imageIndex = context.ImageIndexes.Find(indexId);
            if (imageIndex == null)
            {
                throw new Exception("Document not found");
            }

            var images = context.Images.Where(a => a.IndexId == indexId).Select(s => new Files { Id = s.Id.ToString(), Name = s.FileName }).ToList();
            if (images == null || images.Count <= 0)
            {
                throw new Exception("Document image not found");
            }

            ImageUpload imageUpload = GetImageUpload(images, imageIndex);

            return imageUpload;
        }

        public object GetLastImageIndex()
        {
            var imageIndex = context.ImageIndexes.OrderBy(a => a.CreatedDate).AsEnumerable().Select(s => new
            {
                s.DivisonCode,
                s.DepartmentCode,
                s.GOCode,
                s.SubjectCode,
                s.DeskCode,
                s.LetterNo,
                s.FileCode,
                s.FileOpenYear
            }).LastOrDefault();
            if (imageIndex == null)
            {
                throw new Exception("Document not found");
            }

            return imageIndex;
        }

        public Image GetImageById(string id)
        {
            Guid imageId = Guid.Empty;
            try
            {
                imageId = new Guid(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Index Id not found");
            }

            var images = context.Images.FirstOrDefault(a => a.Id == imageId);
            if (images == null)
            {
                throw new Exception("Document image not found");
            }

            return images;
        }

        public void DeleteFileById(string id)
        {
            Guid imageId = Guid.Empty;
            try
            {
                imageId = new Guid(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Index Id not found");
            }

            var image = context.Images.Find(imageId);
            if (image != null)
            {
                context.Images.Remove(image);
                context.SaveChanges();
            }
        }

        private static ImageUpload GetImageUpload(List<Image> lstImage, ImageIndex index)
        {
            ImageUpload imageUpload = new ImageUpload();
            var img = lstImage.Where(a => a.IndexId == index.Id).Select(s => new Files { Id = s.Id.ToString(), Name = s.FileName, File = s.Photo }).ToList();
            imageUpload.IndexId = index.Id;
            imageUpload.DivisonCode = index.DivisonCode;
            imageUpload.DepartmentCode = index.DepartmentCode;
            imageUpload.SubjectCode = index.SubjectCode;
            imageUpload.GOCode = index.GOCode;
            imageUpload.FileCode = index.FileCode;
            imageUpload.DeskCode = index.DeskCode;
            imageUpload.LetterNo = index.LetterNo;
            imageUpload.FileOpenYear = index.FileOpenYear;
            imageUpload.MeetingMinute = index.MeetingMinute;
            imageUpload.FileType = (index.IsMeetingMinute ?? false) == false ? "Letter" : "Minute";
            imageUpload.MinuteYear = index.MinuteYear;
            imageUpload.IsInformattedLetterNo = index.InformattedLetterNo != null;
            imageUpload.InformattedLetterNo = index.InformattedLetterNo;
            imageUpload.Keyword = index.Keyword;
            imageUpload.Subject = index.Subject;
            imageUpload.UploadedDate = index.UploadedDate;
            if (img != null)
            {
                //imageUpload.Files = img;
            }

            return imageUpload;
        }

        private static ImageUpload GetImageUpload(List<Files> img, ImageIndex index)
        {
            ImageUpload imageUpload = new ImageUpload();
            imageUpload.IndexId = index.Id;
            imageUpload.DivisonCode = index.DivisonCode;
            imageUpload.DepartmentCode = index.DepartmentCode;
            imageUpload.GOCode = index.GOCode;
            imageUpload.SubjectCode = index.SubjectCode;
            imageUpload.FileCode = index.FileCode;
            imageUpload.DeskCode = index.DeskCode;
            imageUpload.LetterNo = index.LetterNo;
            imageUpload.FileOpenYear = index.FileOpenYear;
            imageUpload.MeetingMinute = index.MeetingMinute;
            imageUpload.MinuteYear = index.MinuteYear;
            imageUpload.IsInformattedLetterNo = index.InformattedLetterNo != null;
            imageUpload.InformattedLetterNo = index.InformattedLetterNo;
            imageUpload.Keyword = index.Keyword;
            imageUpload.Subject = index.Subject;
            imageUpload.FileType = (index.IsMeetingMinute ?? false) == false ? "Minute" : "Letter";
            imageUpload.UploadedDate = index.UploadedDate;
            if (img != null)
            {
                imageUpload.Files = img;
            }

            return imageUpload;
        }

        public object GetAllUser()
        {
            var users = context.AspNetUsers.Select(s => new
            {
                s.Id,
                s.Email
            }).ToList();

            return users;
        }

        public object GetAllRoles()
        {
            var roles = context.AspNetRoles.Where(a => true).Select(s => new { s.Id, s.Name }).ToList();
            return roles;
        }

        public List<UserRoleViewModel> GetUserRoles()
        {
            List<UserRoleViewModel> userRoles = new List<UserRoleViewModel>();
            var users = context.AspNetUsers.ToList();
            var roles = context.AspNetRoles.ToList();
            foreach(var user in users)
            {
                UserRoleViewModel model = new UserRoleViewModel();
                model.UserId = user.Id;
                model.UserName = user.UserName;
                var assignedRole = user.AspNetRoles.ToList();
                foreach(var role in roles)
                {
                    UserRole userRole = new UserRole();
                    var r = assignedRole.FirstOrDefault(a => a.Id == role.Id);
                    userRole.Id = role.Id;
                    userRole.Name = role.Name;
                    if(r != null)
                    {
                        userRole.IsSelected = true;
                    }
                    else
                    {
                        userRole.IsSelected = false;
                    }
                    model.Roles.Add(userRole);
                }
                userRoles.Add(model);
            }

            return userRoles;
        }
    }
}