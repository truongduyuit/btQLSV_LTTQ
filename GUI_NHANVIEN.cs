using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using DTO;

namespace QuanLiSinhVien
{
    public partial class GUI_NHANVIEN : Form
    {
        public GUI_NHANVIEN()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string sqlConnectionString = @"Data Source=.\MSSQL;Integrated Security=True";
                using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                {
                    string script = File.ReadAllText(Application.StartupPath + "\\dataa.sql");
                    IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$",
                               RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    conn.Open();
                    foreach (string commandString in commandStrings)
                    {
                        if (commandString.Trim() != "")
                        {
                            using (var command = new SqlCommand(commandString, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch { };
            
            

            LockInsert();
            LoadData();

            txbDienthoai.KeyPress += TxbDienthoai_KeyPress;
        }

        // Load data lên datagridview
        private void LoadData()
        {           
            dgvData.DataSource = BLL.SinhVienBLL.LoadSinhVien();
        }


        // Chỉ cho nhập số vào textbox số điện thoại
        private void TxbDienthoai_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LockInsert()
        {
            txbHoten.Enabled = false;
            txbMssv.Enabled = false;
            txbLop.Enabled = false;
            txbDienthoai.Enabled = false;
            txbEmail.Enabled = false;

            rdbNam.Enabled = false;
            rdbNu.Enabled = false;
            rdbChuaXacDinh.Enabled = false;

            //rdbNam.Checked = false;
            //rdbNu.Checked = false;
            //rdbChuaXacDinh.Checked = false;

            dtpNgaySinh.Enabled = false;
            cbbMaVung.Enabled = false;
        }

        private void AllowInsert()
        {
            txbHoten.Enabled = true;
            txbMssv.Enabled = true;
            txbLop.Enabled = true;
            txbDienthoai.Enabled = true;
            txbEmail.Enabled = true;

            rdbNam.Enabled = true;
            rdbNu.Enabled = true;
            rdbChuaXacDinh.Enabled = true;

            dtpNgaySinh.Enabled = true;
            cbbMaVung.Enabled = true;
        }

        private void ClearDataInsert()
        {
            txbHoten.Text = "";
            txbMssv.Text = "";
            txbLop.Text = "";
            txbDienthoai.Text = "";
            txbEmail.Text = "";

            dtpNgaySinh.Value = DateTime.Now;
            cbbMaVung.Text = "";

            rdbChuaXacDinh.Checked = true;
            dtpNgaySinh.Enabled = true;
            cbbMaVung.Enabled = true;
        }

        // Hàm kiểm tra cú pháp Email
        private bool isValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }


        // Xử lí định dạng họ tên
        private string Hoten(string hoten)
        {
            string result = "";
            string[] words = hoten.Split(' ');

            foreach (string word in words)
            {
                // từ nào là các khoảng trắng thừa thì bỏ  
                if (word.Trim() != "")
                {
                    if (word.Length > 1)
                        result += word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower() + " ";
                    else
                        result += word.ToUpper() + " ";
                }

            }

            return result.Trim();
        }
        
        private bool KiemTraDataInsert()
        {
            // Kiểm tra text = null            
            if (txbHoten.Text.Trim() == "")
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu", "Thông báo", MessageBoxButtons.OKCancel ,MessageBoxIcon.Error);
                txbHoten.Focus();
                return false;
            }
            if (txbMssv.Text.Trim() == "")
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                txbMssv.Focus();
                return false;
            }
            // Kiểm tra mssv đã tồn tại hay chưa 
            else
            {
                List<string> listMSSV = new List<string>();
                foreach (DataGridViewRow p in dgvData.Rows)
                {
                    listMSSV.Add(p.Cells["MSSV"].Value.ToString());
                }
                foreach (string mssv in listMSSV)
                {
                    if (txbMssv.Text == mssv)
                    {
                        MessageBox.Show("Mã số sinh viên đã tồn tại", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        txbMssv.Focus();
                        return false;
                    }
                }
            }



            if (txbLop.Text.Trim() == "")
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                txbLop.Focus();
                return false;
            }

            // Kiểm tra số tuổi từ 17-35
            if (DateTime.Now.Year - dtpNgaySinh.Value.Year < 17 || DateTime.Now.Year - dtpNgaySinh.Value.Year > 35)
            {
                MessageBox.Show("Ngày sinh phải giới hạn từ 17 đến 35 tuổi", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                dtpNgaySinh.Focus();
                return false;
            }

            if (cbbMaVung.Text.Trim() == "")
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                cbbMaVung.Focus();
                return false;
            }
            if (txbDienthoai.Text.Trim() == "")
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                txbDienthoai.Focus();
                return false;
            }

            // Kiểm tra số điện thoại
            if (txbDienthoai.Text.Length > 10 || txbDienthoai.Text.Length < 9)
            {
                MessageBox.Show("Số điện thoại phải có 9 đến 10 chữ số", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                txbDienthoai.Focus();
                return false;
            }
            else
            {
                if (txbDienthoai.Text.Substring(0,1) == "0")
                {
                    MessageBox.Show("Số điện thoại không thể bắt đầu bằng số 0", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    txbDienthoai.Focus();
                    return false;
                }
            }


            if (txbEmail.Text.Trim() == "")
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                txbEmail.Focus();
                return false;
            }
            // Kiểm tra cú pháp Email
            else
            {
                if (!isValidEmail(txbEmail.Text))
                {
                    MessageBox.Show("Email không hợp lệ", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    txbEmail.Focus();
                    return false;
                }
            }


            return true;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (btnThem.Text == "Thêm")
            {
                // Clear các trường dữ liêu
                ClearDataInsert();
                          
                btnThem.Text = "Lưu";
                AllowInsert();

                // Focos tới textbox nhập họ tên
                txbHoten.Focus();

                // Tắt chức năng Sữa , Xóa
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
            else if (btnThem.Text == "Lưu")
            {
                
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn lưu hay không ?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    if (KiemTraDataInsert())
                    {
                        btnThem.Text = "Thêm";
                        MessageBox.Show("Thêm thành công !");
                        btnSua.Enabled = true;
                        btnXoa.Enabled = true;

                        // Gọi hàm thêm sinh viên
                        InsertSinhVien();
                        LockInsert();
                        
                    }
                }
               
            }
            LoadData();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            int i = dgvData.CurrentRow.Index;
            if (mssv == null)
            {
                MessageBox.Show("Vui lòng chọn 1 sinh viên để sửa", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            if (btnSua.Text == "Sửa")
            {
                btnSua.Text = "Lưu";
                btnThem.Enabled = false;
                btnXoa.Enabled = false;
                
                AllowInsert();
                dgvData.Rows[i].Selected = true;
            }
            else if (btnSua.Text == "Lưu")
            {
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn sửa  thông tin sinh vien có MSSV  " + mssv + "  hay không ?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    btnSua.Text = "Sửa";
                    btnThem.Enabled = true;
                    btnXoa.Enabled = true;
                    MessageBox.Show("Sửa thành công !");
                    UpdateSinhVien();
                    LockInsert();
                }                
            }
            LoadData();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh vien có MSSV  " + mssv + "  hay không ?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                MessageBox.Show("Xóa thành công !");
                DeleteSinhVien();               
            }
            LoadData();
        }

        // Binding 1 dòng trong datagridview
        private string mssv;

        private void InsertSinhVien()
        {            
            BLL.SinhVienBLL.InsertSinhVien(GetSinhVien_DTO());            
            
        }

        private void DeleteSinhVien()
        {          
            BLL.SinhVienBLL.DeleteSinhVien(mssv);           
            ClearDataInsert();
        }

        private void UpdateSinhVien()
        {            
            BLL.SinhVienBLL.UpdateSinhVien(mssv, GetSinhVien_DTO());                                       
        }

        private SinhVien_DTO GetSinhVien_DTO()
        {
            SinhVien_DTO sv = new SinhVien_DTO();
            sv.mssv = txbMssv.Text;
            sv.hoten = Hoten(txbHoten.Text);
            sv.lop = txbLop.Text;
            sv.ngaysinh = (DateTime)dtpNgaySinh.Value;
            sv.ngaysinh.ToShortDateString();

            if (rdbChuaXacDinh.Checked)
            {
                sv.gioitinh = "Chưa xác định";
            }
            else if (rdbNam.Checked)
            {
                sv.gioitinh = "Nam";
            }
            else
            {
                sv.gioitinh = "Nữ";
            }

            sv.dienthoai = txbDienthoai.Text;
            sv.mavung = cbbMaVung.Text;
            sv.email = txbEmail.Text;
            return sv;
        }

        private void dgvData_SelectionChanged(object sender, EventArgs e)
        {
            if (btnThem.Text != "Lưu")
            {
                try
                {
                    DataGridViewRow row = dgvData.CurrentRow;
                    txbHoten.Text = Hoten(row.Cells["HoTen"].Value.ToString());
                    txbMssv.Text = row.Cells["MSSV"].Value.ToString();
                    mssv = txbMssv.Text;
                    txbLop.Text = row.Cells["Lop"].Value.ToString();
                    txbDienthoai.Text = row.Cells["DienThoai"].Value.ToString();
                    txbEmail.Text = row.Cells["Email"].Value.ToString();

                    dtpNgaySinh.Value = (DateTime)row.Cells["NgaySinh"].Value;

                    if (row.Cells["GioiTinh"].Value.ToString() == "Nam")
                    {
                        rdbNam.Checked = true;
                    }
                    else if (row.Cells["GioiTinh"].Value.ToString() == "Nữ")
                    {
                        rdbNu.Checked = true;
                    }
                    else
                    {
                        rdbChuaXacDinh.Checked = true;
                    }

                    cbbMaVung.Text = row.Cells["MaVung"].Value.ToString();
                }
                catch { };
            }
        }
    }
}
