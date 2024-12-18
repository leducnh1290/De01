using De01.DAL;
using De01.Model;
using De01.DAL;
using System.Collections.Generic;

namespace De01.BLL
{
    public class SinhvienBLL
    {
        private SinhvienDAL dal = new SinhvienDAL();

        public List<Sinhvien> GetAllSinhviens()
        {
            return dal.GetAll();
        }
        public List<Lop> GetLop()
        {
            return dal.GetAllLop();
        }
        public void AddSinhvien(Sinhvien sv)
        {
            dal.Add(sv);
        }

        public void UpdateSinhvien(Sinhvien sv)
        {
            dal.Update(sv);
        }

        public void DeleteSinhvien(string maSV)
        {
            dal.Delete(maSV);
        }
        public List<Sinhvien> SearchSinhvien(string maSV, string tenSV, string tenLop)
        {
            return dal.SearchSinhvien(maSV, tenSV, tenLop);
        }

        public List<Sinhvien> SearchSinhvien(string keyword)
        {
            return dal.Search(keyword);
        }
    }
}
