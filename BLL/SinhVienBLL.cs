using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DTO;

namespace BLL
{
    public class SinhVienBLL
    {
        public static List<SinhVien_DTO> LoadSinhVien()
        {
            return DAL.SinhVienDAL.LoadSinhVien();
        }

        public static void InsertSinhVien(SinhVien_DTO sv )
        {
            DAL.SinhVienDAL.InsertSinhVien(sv);
        }

        public static void DeleteSinhVien(string Mssv)
        {
            DAL.SinhVienDAL.DeleteSinhVien(Mssv);
        }

        public static void UpdateSinhVien(string Mssv, SinhVien_DTO sv)
        {
            DAL.SinhVienDAL.UpdateSinhVien(Mssv, sv);
        }
    }
}
