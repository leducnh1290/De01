using De01.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace De01.DAL
{
    public class SinhvienDAL
    {
        private Model1 db = new Model1();

        // Lấy danh sách sinh viên
        public List<Sinhvien> GetAll()
        {
            return db.Sinhviens.Include("Lop").ToList();
        }
        public List<Lop> GetAllLop()
        {
            return db.Lops.ToList();
        }
        public List<Sinhvien> SearchSinhvien(string maSV, string tenSV, string tenLop)
        {
            return db.Sinhviens
                .Where(s =>
                    (string.IsNullOrEmpty(maSV) || s.MaSV.Contains(maSV)) &&      // Kiểm tra Mã SV
                    (string.IsNullOrEmpty(tenSV) || s.HoTenSV.Contains(tenSV)) && // Kiểm tra Tên SV
                    (string.IsNullOrEmpty(tenLop) || s.Lop.TenLop.Contains(tenLop)) // Kiểm tra Tên Lớp
                )
                .ToList();
        }


        // Thêm sinh viên
        public void Add(Sinhvien sv)
        {
            db.Sinhviens.Add(sv);
            db.SaveChanges();
        }

        // Cập nhật sinh viên
        public void Update(Sinhvien sv)
        {
            var entity = db.Sinhviens.Find(sv.MaSV);
            if (entity != null)
            {
                entity.HoTenSV = sv.HoTenSV;
                entity.NgaySinh = sv.NgaySinh;
                entity.MaLop = sv.MaLop;
                db.SaveChanges();
            }
        }

        // Xóa sinh viên
        public void Delete(string maSV)
        {
            var sv = db.Sinhviens.Find(maSV);
            if (sv != null)
            {
                db.Sinhviens.Remove(sv);
                db.SaveChanges();
            }
        }

        // Tìm kiếm sinh viên
        public List<Sinhvien> Search(string keyword)
        {
            return db.Sinhviens
                .Where(s => s.MaSV.Contains(keyword) ||
                            s.HoTenSV.Contains(keyword) ||
                            s.Lop.TenLop.Contains(keyword))
                .ToList();
        }
    }
}
