using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace ASM1DATAE_BH01350
{
    public partial class Form3 : Form
    {
        string connectionstring = @"Data Source=LAPTOP-IG12I1HR\SQLEXPRESS;Initial Catalog=asm22;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adt;
        DataTable dt = new DataTable();
        public Form3()
        {
            InitializeComponent();
            

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            con = new SqlConnection(connectionstring); // Khởi tạo kết nối với cơ sở dữ liệu
            try
            {
                con.Open(); // Mở kết nối
                cmd = new SqlCommand("select * from Product", con); // Tạo lệnh SQL để lấy tất cả sản phẩm
                adt = new SqlDataAdapter(cmd); // Tạo SqlDataAdapter từ lệnh
                adt.Fill(dt); // Điền dữ liệu vào DataTable
                dgv_Product.DataSource = dt; // Gán DataTable cho DataGridView
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

        private void dgv_Product_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Kiểm tra nếu chỉ số dòng hợp lệ
            {
                // Lấy dòng được chọn
                DataGridViewRow selectedRow = dgv_Product.Rows[e.RowIndex];

                // Hiển thị dữ liệu trong TextBox
                try
                {
                    // Gán giá trị từ dòng được chọn vào các TextBox tương ứng
                    txtB_idProduct.Text = selectedRow.Cells["ProductID"].Value.ToString();
                    txtB_nameProduct.Text = selectedRow.Cells["Name"].Value.ToString(); // Name of the product
                    txtB_sizeProduct.Text = selectedRow.Cells["Size"].Value.ToString(); // Size of the product
                    txtB_priceProduct.Text = selectedRow.Cells["Price"].Value.ToString(); // Price of the product
                    txtB_quantityProduct.Text = selectedRow.Cells["Quantity"].Value.ToString(); // Quantity in inventory
                                                                                                // Hide the ID column
                    


                    // Hiển thị hình ảnh trong PictureBox
                    if (selectedRow.Cells["Image"].Value != DBNull.Value)
                    {
                        byte[] imageData = (byte[])selectedRow.Cells["Image"].Value; // Lấy dữ liệu hình ảnh
                        if (imageData != null && imageData.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imageData)) // Chuyển đổi byte array thành hình ảnh
                            {
                                picB_imageProduct.Image = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            picB_imageProduct.Image = null; // Hoặc hình ảnh mặc định
                        }
                    }
                    else
                    {
                        picB_imageProduct.Image = null; // Hoặc hình ảnh mặc định
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message); // Hiển thị thông báo lỗi nếu có
                }
            }
        }
        private void btn_addProduct_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtB_nameProduct.Text) ||
            string.IsNullOrWhiteSpace(txtB_sizeProduct.Text) ||
            string.IsNullOrWhiteSpace(txtB_idProduct.Text) || // Thêm kiểm tra cho ProductID
            !decimal.TryParse(txtB_priceProduct.Text, out decimal Price) || // Thay đổi tên thành Price
            !int.TryParse(txtB_quantityProduct.Text, out int Quantity)) // Thay đổi thành kiểu int cho Quantity
            {
                MessageBox.Show("Please enter valid values for Product ID, Product Name, Size, Quantity, and Price.");
                return; // Thoát nếu dữ liệu không hợp lệ
            }

            // Chuyển đổi đường dẫn hình ảnh thành mảng byte
            byte[] productImage = PathToByteArray(this.Text); // Đảm bảo this.Text chứa đường dẫn hình ảnh

            using (SqlConnection con = new SqlConnection(connectionstring)) // Tạo kết nối mới
            {
                try
                {
                    con.Open(); // Mở kết nối

                    // Tạo câu lệnh INSERT để thêm sản phẩm mới
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Product (ProductID, Name, Size, Image, Quantity, Price) VALUES (@ProductID, @Name, @Size, @Image, @Quantity, @Price)", con))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", int.Parse(txtB_idProduct.Text)); // Lấy giá trị ProductID từ textbox
                        cmd.Parameters.AddWithValue("@Name", txtB_nameProduct.Text); // Lấy tên sản phẩm (Name)
                        cmd.Parameters.AddWithValue("@Size", txtB_sizeProduct.Text); // Lấy kích thước sản phẩm (Size)
                        cmd.Parameters.AddWithValue("@Image", productImage); // Lấy hình ảnh (Image)
                        cmd.Parameters.AddWithValue("@Quantity", Quantity); // Lấy số lượng sản phẩm (Quantity)
                        cmd.Parameters.AddWithValue("@Price", Price); // Lấy giá bán (Price)

                        cmd.ExecuteNonQuery(); // Thực thi câu lệnh INSERT
                    }

                    MessageBox.Show("Product added successfully!"); // Thông báo thành công
                    LoadProducts(); // Tải lại danh sách sản phẩm
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}"); // Hiển thị lỗi cơ sở dữ liệu nếu có
                }
            }
        }

        // Chuyển đổi file hình ảnh thành mảng byte
        byte[] PathToByteArray(string path)
        {
            if (!File.Exists(path)) // Kiểm tra file có tồn tại hay không
            {
                throw new FileNotFoundException($"File not found at {path}");
            }

            using (MemoryStream m = new MemoryStream()) // Sử dụng MemoryStream để lưu trữ hình ảnh
            {
                using (Image img = Image.FromFile(path)) // Tải hình ảnh từ file
                {
                    img.Save(m, System.Drawing.Imaging.ImageFormat.Png); // Lưu hình ảnh vào MemoryStream
                }
                return m.ToArray(); // Trả về mảng byte
            }
        }

        private void LoadProducts()
        {
            dt.Clear(); // Xóa dữ liệu cũ trong DataTable
            using (SqlConnection con = new SqlConnection(connectionstring)) // Tạo kết nối mới
            {
                try
                {
                    con.Open(); // Mở kết nối
                    cmd = new SqlCommand("SELECT * FROM Product", con); // Tạo lệnh SQL để lấy tất cả sản phẩm
                    adt = new SqlDataAdapter(cmd); // Tạo SqlDataAdapter từ lệnh
                    adt.Fill(dt); // Điền dữ liệu vào DataTable
                    dgv_Product.DataSource = dt; // Gán DataTable cho DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); // Hiển thị thông báo lỗi nếu có
                }
            }
        }
        private void picB_imageProduct_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog(); // Tạo hộp thoại mở file
            if (open.ShowDialog() == DialogResult.OK) // Kiểm tra xem người dùng đã chọn file chưa
            {
                picB_imageProduct.Image = Image.FromFile(open.FileName); // Hiển thị hình ảnh đã chọn
                this.Text = open.FileName; // Hiển thị đường dẫn file cho mục đích gỡ lỗi
            }
        }

        private void btn_editProduct_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtB_nameProduct.Text) ||
            string.IsNullOrWhiteSpace(txtB_sizeProduct.Text) ||
            string.IsNullOrWhiteSpace(txtB_idProduct.Text) || // Kiểm tra ProductID
            !decimal.TryParse(txtB_priceProduct.Text, out decimal Price) || // Đổi thành Price
            !int.TryParse(txtB_quantityProduct.Text, out int Quantity)) // Đổi thành kiểu int cho Quantity
            {
                MessageBox.Show("Please enter valid values for Product ID, Product Name, Size, Quantity, and Price.");
                return; // Thoát nếu dữ liệu không hợp lệ
            }

            // Chuyển đổi đường dẫn hình ảnh thành mảng byte
            byte[] productImage = PathToByteArray(this.Text); // Đảm bảo this.Text chứa đường dẫn hình ảnh

            using (SqlConnection con = new SqlConnection(connectionstring)) // Tạo kết nối mới
            {
                try
                {
                    con.Open(); // Mở kết nối

                    // Tạo câu lệnh UPDATE để sửa đổi thông tin sản phẩm
                    using (SqlCommand cmd = new SqlCommand("UPDATE Product SET Name = @Name, Size = @Size, Image = @Image, Quantity = @Quantity, Price = @Price WHERE ProductID = @ProductID", con))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", int.Parse(txtB_idProduct.Text)); // Lấy giá trị ProductID từ textbox
                        cmd.Parameters.AddWithValue("@Name", txtB_nameProduct.Text); // Lấy tên sản phẩm
                        cmd.Parameters.AddWithValue("@Size", txtB_sizeProduct.Text); // Lấy kích thước sản phẩm
                        cmd.Parameters.AddWithValue("@Image", productImage); // Lấy hình ảnh
                        cmd.Parameters.AddWithValue("@Quantity", Quantity); // Lấy số lượng tồn kho
                        cmd.Parameters.AddWithValue("@Price", Price); // Lấy giá bán

                        cmd.ExecuteNonQuery(); // Thực thi câu lệnh UPDATE
                    }

                    MessageBox.Show("Product updated successfully!"); // Thông báo thành công
                    LoadProducts(); // Tải lại danh sách sản phẩm
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}"); // Hiển thị lỗi cơ sở dữ liệu nếu có
                }
            }
        }

        private void btn_clearProduct_Click(object sender, EventArgs e)
        {
            if (dgv_Product.SelectedRows.Count > 0) // Kiểm tra xem có dòng nào được chọn không
            {
                // Lấy dòng được chọn
                var selectedRow = dgv_Product.SelectedRows[0];
                int productIdToDelete = Convert.ToInt32(selectedRow.Cells["ProductID"].Value); // Lấy ProductID từ dòng đã chọn

                using (SqlConnection con = new SqlConnection(connectionstring)) // Tạo kết nối mới
                {
                    try
                    {
                        con.Open(); // Mở kết nối
                                    // Tạo câu lệnh DELETE để xóa sản phẩm theo ProductID
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Product WHERE ProductID = @ProductID", con))
                        {
                            cmd.Parameters.AddWithValue("@ProductID", productIdToDelete); // Thêm tham số ProductID
                            cmd.ExecuteNonQuery(); // Thực thi câu lệnh DELETE
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Database error: {ex.Message}"); // Hiển thị lỗi cơ sở dữ liệu nếu có
                    }
                }

                // Xóa dòng khỏi DataGridView
                dgv_Product.Rows.RemoveAt(selectedRow.Index);
            }
            else
            {
                MessageBox.Show("Please select a row to delete."); // Thông báo nếu không có dòng nào được chọn
            }
        }

        private void btn_outProduct_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Hàm loại bỏ các ký tự không phải số hoặc dấu thập phân

    }
    
}
