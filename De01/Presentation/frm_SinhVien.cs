using De01.BLL;
using De01.Model;
using De01.BLL;
using De01.DAL;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;

namespace De01.Presentation
{
    public partial class frm_SinhVien : Form
    {
        private SinhvienBLL bll = new SinhvienBLL();
        private Model1 db = new Model1();
        private bool isAdding = false;
        private Sinhvien student = null; // Biến lưu dữ liệu dòng chọn

        public frm_SinhVien()
        {
          
            InitializeComponent();
            dtpNgaySinh.Format = DateTimePickerFormat.Custom;
            dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadLop();
        }

        private void LoadData()
        {
            var students = bll.GetAllSinhviens()
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
        }

        private void LoadLop()
        {
            cboLop2.DataSource = bll.GetLop().Distinct().ToList(); // Lấy lớp
            cboLop2.DisplayMember = "TenLop";
            cboLop2.ValueMember = "MaLop";
            cboLop.DataSource = bll.GetLop().Distinct().ToList(); // Lấy lớp
            cboLop.DisplayMember = "TenLop";
            cboLop.ValueMember = "MaLop";

        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
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

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên '{student.HoTenSV}' không?",
                                          "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                bll.DeleteSinhvien(student.MaSV);
                LoadData();
                MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string maSV = txtTim.Text.Trim();  // Mã sinh viên từ TextBox
            string tenSV = txtTenSv.Text.Trim(); // Tên sinh viên từ TextBox
            string tenLop = cboLop2.Text.Trim(); // Tên lớp/ngành học từ ComboBox

            // Kiểm tra nếu tất cả các ô tìm kiếm đều trống
            if (string.IsNullOrEmpty(maSV) && string.IsNullOrEmpty(tenSV) && string.IsNullOrEmpty(tenLop))
            {
                MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm (Mã SV, Tên SV hoặc Khoa).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gọi phương thức tìm kiếm từ BLL
            var searchResults = bll.SearchSinhvien(maSV, tenSV, tenLop)
                .Select(s => new
                {
                    s.MaSV,
                    s.HoTenSV,
                    s.NgaySinh,
                    TenLop = s.Lop.TenLop
                }).ToList();

            dgvSinhVien.DataSource = searchResults;

            // Kiểm tra nếu không có kết quả tìm kiếm
            if (searchResults.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sinh viên nào phù hợp với từ khóa tìm kiếm.", "Kết quả tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void SaveData()
        {
            var sv = new Sinhvien
            {
                MaSV = txtMaSV.Text,
                HoTenSV = txtHoTen.Text,
                NgaySinh = dtpNgaySinh.Value.Date,
                MaLop = cboLop.SelectedValue.ToString()
            };

            if (isAdding)
            {
                bll.AddSinhvien(sv);
            }
            else
            {
                bll.UpdateSinhvien(sv);
            }

            LoadData();
            ResetForm();
            MessageBox.Show("Dữ liệu đã được lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void btnKhongLuu_Click(object sender, EventArgs e)
        {
            ResetForm();
            btnLuu.Enabled = false;
            btnKhongLuu.Enabled = false;
        }

        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            cboLop2.SelectedIndex = -1;
            student = null;
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
