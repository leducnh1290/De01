using De01.Model;
using System;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace QLSV
{
    public partial class Form1 : Form
    {
        bool isAdding = false;
        private Sinhvien student = null; // Biến lưu dữ liệu dòng chọn
        private Model1 db = new Model1(); // DbContext

        public Form1()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
            InitializeComponent();
        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadLop();
        }

        private void LoadData()
        {
            var students = db.Sinhviens
                .Select(s => new
                {
                    s.MaSV,
                    s.HoTenSV,
                    s.NgaySinh,
                    TenLop = s.Lop.TenLop
                }).ToList();
        

            dgvSinhVien.DataSource = students;
            btnLuu.Enabled = false;
            btnKhongLuu.Enabled = false;
            dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvSinhVien.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Giãn DataGridView để vừa với Form
            dgvSinhVien.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
       
        }

        private void LoadLop()
        {
            cboLop2.DataSource = db.Lops.ToList();
            cboLop2.DisplayMember = "TenLop";
            cboLop2.ValueMember = "MaLop";
            cboLop.DataSource = db.Lops.ToList();
            cboLop.DisplayMember = "TenLop";
            cboLop.ValueMember = "MaLop";
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            cboLop2.SelectedIndex = -1;

            // Enable nút Lưu và Không Lưu
            btnLuu.Enabled = true;
            btnKhongLuu.Enabled = true;

            // Đặt cờ để xác định là thêm mới
            isAdding = true;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (student == null) 
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            var sv = db.Sinhviens.Find(student.MaSV);

            if (sv != null)
            {
                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên '{sv.HoTenSV}' không?",
                                              "Xác nhận xóa",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    db.Sinhviens.Remove(sv);
                    db.SaveChanges();
                    LoadData(); 

                  
                    ResetForm();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            
            string maSV = txtTim.Text.Trim();  
            string tenSV = txtTenSv.Text.Trim(); 
            string tenLop = cboLop2.Text.Trim(); 

          
            if (string.IsNullOrEmpty(maSV) && string.IsNullOrEmpty(tenSV) && string.IsNullOrEmpty(tenLop))
            {
                MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm (Mã SV, Tên SV hoặc Khoa).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var db = new Model1()) 
            {

                var searchResults = db.Sinhviens
                    .Where(s =>
                        (string.IsNullOrEmpty(maSV) || s.MaSV.Contains(maSV)) &&      
                        (string.IsNullOrEmpty(tenSV) || s.HoTenSV.Contains(tenSV)) && 
                        (string.IsNullOrEmpty(tenLop) || s.Lop.TenLop.Contains(tenLop))
                    )
                    .Select(s => new
                    {
                        s.MaSV,
                        s.HoTenSV,
                        s.NgaySinh,
                        TenLop = s.Lop.TenLop
                    })
                    .ToList();

                dgvSinhVien.DataSource = searchResults;


                if (searchResults.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào phù hợp với từ khóa tìm kiếm.", "Kết quả tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            if (student != null)
            {

                txtMaSV.Text = student.MaSV;
                txtHoTen.Text = student.HoTenSV;
                dtpNgaySinh.Value = (DateTime)student.NgaySinh;
                cboLop2.Text = student.MaLop;

           
                btnLuu.Enabled = true;
                btnKhongLuu.Enabled = true;

              
                isAdding = false;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng dữ liệu để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }       
        }
        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            cboLop2.SelectedIndex = -1; 
            student = null; 
        }

        private void SaveData()
        {
            using (var db = new Model1())
            {
                if (isAdding) 
                {
                    var sv = new Sinhvien
                    {
                        MaSV = txtMaSV.Text,
                        HoTenSV = txtHoTen.Text,
                        NgaySinh = dtpNgaySinh.Value.Date,
                        MaLop = cboLop.SelectedValue.ToString()
                    };

                    db.Sinhviens.Add(sv);
                }
                else 
                {
                    string maSV = txtMaSV.Text;
                    var sv = db.Sinhviens.Find(maSV);

                    if (sv != null)
                    {
                        sv.HoTenSV = txtHoTen.Text;
                        sv.NgaySinh = dtpNgaySinh.Value.Date;
                        sv.MaLop = cboLop2.SelectedValue.ToString();
                    }
                }

                db.SaveChanges();
            }
            
            LoadData();
            ResetForm();
            btnLuu.Enabled = false;
            btnKhongLuu.Enabled = false;
        }
      
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      

        private void btnLuu_Click(object sender, EventArgs e)
        {
            SaveData();
            MessageBox.Show("Dữ liệu đã được lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnKhongLuu_Click(object sender, EventArgs e)
        {

            txtMaSV.Clear();
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            cboLop2.SelectedIndex = -1;


            btnLuu.Enabled = false;
            btnKhongLuu.Enabled = false;

            MessageBox.Show("Dữ liệu đã được khôi phục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSinhVien.CurrentRow != null)
            {
          
                student = new Sinhvien
                {
                    MaSV = dgvSinhVien.CurrentRow.Cells["MaSV"].Value.ToString(),
                    HoTenSV = dgvSinhVien.CurrentRow.Cells["HoTenSV"].Value.ToString(),
                    NgaySinh = (DateTime)dgvSinhVien.CurrentRow.Cells["NgaySinh"].Value,
                    MaLop = dgvSinhVien.CurrentRow.Cells["TenLop"].Value.ToString() 
                };
            }
        }
    }
}
