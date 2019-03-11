using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;

namespace DAL
{
    public class SinhVienDAL
    {
        public static List<SinhVien_DTO> LoadSinhVien()
        {
            using (QuanLiSinhVienEntities db = new QuanLiSinhVienEntities())
            {
                var sv = db.SinhViens.ToList();
                List<SinhVien_DTO> svDTO = new List<SinhVien_DTO>();
                foreach (SinhVien s in sv)
                {
                    svDTO.Add(ReturnSV_DTO(s));
                }
                return svDTO;
            }
        }

        public static void InsertSinhVien(SinhVien_DTO sv)
        {         
            using (QuanLiSinhVienEntities db = new QuanLiSinhVienEntities())
            {
                db.SinhViens.Add(ReturnSV(sv));
                db.SaveChanges();
            }
        }

        public static void DeleteSinhVien(string Mssv)
        {
            using (QuanLiSinhVienEntities db = new QuanLiSinhVienEntities())
            {
                var sv= db.SinhViens.FirstOrDefault(x => x.mssv == Mssv);
                if (sv != null)
                {
                    db.SinhViens.Remove(sv);
                    db.SaveChanges();
                }
                
            }
        }

        public static void UpdateSinhVien(string Mssv, SinhVien_DTO sv)
        {
            using (QuanLiSinhVienEntities db = new QuanLiSinhVienEntities())
            {
                DeleteSinhVien(Mssv);
                InsertSinhVien(sv);               
            }
        }

        public static SinhVien ReturnSV (SinhVien_DTO svDTO)
        {
            SinhVien sv = new SinhVien();

            sv.hoten = svDTO.hoten;
            sv.mssv = svDTO.mssv;
            sv.lop = svDTO.lop;
            sv.ngaysinh =(DateTime) svDTO.ngaysinh;
            sv.gioitinh = svDTO.gioitinh;
            sv.mavung = svDTO.mavung;
            sv.dienthoai = svDTO.dienthoai;
            sv.email = svDTO.email;


            return sv;
        }

        public static SinhVien_DTO ReturnSV_DTO(SinhVien sv)
        {
            SinhVien_DTO svDTO = new SinhVien_DTO();

            svDTO.hoten = sv.hoten;
            svDTO.mssv = sv.mssv;
            svDTO.lop = sv.lop;
            svDTO.ngaysinh = sv.ngaysinh;
            svDTO.gioitinh = sv.gioitinh;
            svDTO.mavung = sv.mavung;
            svDTO.dienthoai = sv.dienthoai;
            svDTO.email = sv.email;


            return svDTO;
        }
    }
}
