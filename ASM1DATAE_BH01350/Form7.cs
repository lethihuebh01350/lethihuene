using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ASM1DATAE_BH01350
{

    public partial class Form7 : Form
    {
        private int CustomerID;
        string connectionstring = @"Data Source=LAPTOP-IG12I1HR\SQLEXPRESS;Initial Catalog=asm22;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adt;
        DataTable dt = new DataTable();
        public Form7(int customerID)
        {
            InitializeComponent();
            customerID = CustomerID;
            //LoadCustomerName();

        }

        private void LoadCustomerName()
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                string query = "SELECT CustomerName FROM Customer WHERE CustomerID = @CustomerID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", CustomerID);
                var result = command.ExecuteScalar();

                if (result != null) // Check if the result is not null
                {
                    string customerName = result.ToString();
                    txtB_name.Text = "Hi, " + customerName + " Welcome..!";
                }
                else
                {
                    txtB_name.Text = "Customer not found."; // Handle the case where the customer is not found
                }
            }
        }
        private void Form7_Load(object sender, EventArgs e)
        {
            con = new SqlConnection(connectionstring); // Khởi tạo kết nối với cơ sở dữ liệu
            try
            {
                con.Open(); // Mở kết nối
                cmd = new SqlCommand("SELECT * FROM Product", con); // Tạo lệnh SQL để lấy tất cả sản phẩm
                adt = new SqlDataAdapter(cmd); // Tạo SqlDataAdapter từ lệnh
                adt.Fill(dt); // Điền dữ liệu vào DataTable
                dgv_User.DataSource = dt; // Gán DataTable cho DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // Hiển thị thông báo lỗi nếu có
            }
            finally
            {
                con.Close(); // Đóng kết nối
            }
        }

        private void dgv_User_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Kiểm tra nếu chỉ số dòng hợp lệ
            {
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dgv_User.Rows[e.RowIndex];

                // Hiển thị dữ liệu trong TextBox
                try
                {
                    // Gán giá trị từ dòng được chọn vào các TextBox tương ứng
                    txtB_idUser.Text = selectedRow.Cells["ProductID"].Value.ToString();
                    txtB_nameUser.Text = selectedRow.Cells["Name"].Value.ToString();
                    cbB_sizeUser.Text = selectedRow.Cells["Size"].Value.ToString();
                   txtB_priceUser.Text = selectedRow.Cells["Price"].Value.ToString();
                    txtB_quantity.Text = selectedRow.Cells["quantity"].Value.ToString();

                    // Hiển thị hình ảnh trong PictureBox
                    if (selectedRow.Cells["Image"].Value != DBNull.Value)
                    {
                        byte[] imageData = (byte[])selectedRow.Cells["Image"].Value; // Lấy dữ liệu hình ảnh
                        if (imageData != null && imageData.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imageData)) // Chuyển đổi byte array thành hình ảnh
                            {
                                picB_imageUser.Image = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            picB_imageUser.Image = null; // Hoặc hình ảnh mặc định
                        }
                    }
                    else
                    {
                        picB_imageUser.Image = null; // Hoặc hình ảnh mặc định
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message); // Hiển thị thông báo lỗi nếu có
                }
            }
        }
        decimal grandTotal = 0; // Biến lưu tổng tiền toàn bộ sản phẩm
        private void btn_select_Click(object sender, EventArgs e)
        {
             if (!string.IsNullOrEmpty(cbB_sizeUser.Text) && !string.IsNullOrEmpty(txtB_purchaseUser.Text)) 
            {
                int productID = int.Parse(txtB_idUser.Text);
                int purchase = int.Parse(txtB_purchaseUser.Text);
                int quantity = int.Parse(txtB_quantity.Text);
                decimal price = decimal.Parse(txtB_priceUser.Text);
                decimal total = price * purchase;
                if (purchase > quantity) 
                {
                    MessageBox.Show("Số lượng đặt mua nhiều hơn số lượng sản phẩm trong kho. Không thể thanh toán", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btn_total.Enabled = false;
                }
                else 
                {
                    grandTotal += total; // Cộng dồn tổng tiền
                    txtB_totalUser.Text = grandTotal.ToString();
                    
                }

            }
            else 
            {
                MessageBox.Show("Vui lòng chọn sản phẩm và nhập số lượng hợp lệ.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //txtB_totalUser.Text = "";
                //btn_total.Enabled = false;
            }
        }

        private void btn_total_Click(object sender, EventArgs e)
        {
            int productID = int.Parse(txtB_idUser.Text);
            decimal price = decimal.Parse(txtB_priceUser.Text);
            int purchase = int.Parse(txtB_purchaseUser.Text);
            decimal total = price * purchase;
            MessageBox.Show($"Paying {total} for {purchase} item of prodcut {productID}.");
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            // Kiểm tra các TextBox có dữ liệu
            if (!string.IsNullOrEmpty(txtB_purchaseUser.Text) && grandTotal > 0)
            {
                try
                {
                    // Lấy thông tin sản phẩm từ các TextBox
                    int purchase = int.Parse(txtB_purchaseUser.Text);
                    decimal price = decimal.Parse(txtB_priceUser.Text);
                    decimal cancelTotal = purchase * price; // Tính số tiền cần trừ

                    // Giảm tổng tiền
                    grandTotal -= cancelTotal;
                    txtB_totalUser.Text = grandTotal.ToString();

                    // Reset các trường liên quan đến sản phẩm được hủy
                    txtB_purchaseUser.Text = "";
                    txtB_idUser.Text = "";
                    txtB_nameUser.Text = "";
                    cbB_sizeUser.SelectedIndex = -1;
                    txtB_priceUser.Text = "";
                    txtB_quantity.Text = "";
                    picB_imageUser.Image = null; // Xóa hình ảnh nếu có

                    MessageBox.Show("Hủy sản phẩm vừa chọn thành công.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Không có sản phẩm nào để hủy.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtB_userName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
