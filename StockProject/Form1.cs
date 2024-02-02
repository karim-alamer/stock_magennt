using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StockProject
{
    public partial class Form1 : Form
    {
        const string connectionString = "Data Source=DESKTOP-21J1CL1\\SQLEXPRESS;Initial Catalog=stock_ayman;Integrated Security=True;";
        DataTable productsDataTable;
        decimal total_price = 0;
        int product_id = 0;
        int quantity = 0;
        String modeName = "";
        int billID = 0;
        int sell_billID = 0;
        String modelname;
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataAdapter adapter;
        private DataSet dataSet;
        private SqlDependency dependency;
        String selectQueryForShowSuppliers = "SELECT supp_name,supp_add,supp_phone,cash FROM suppliers";
        // string selectALLProducts = "SELECT quantity,model_name FROM model";
        string selectQueryForroduct = "SELECT pro_name, pro_quantity FROM product";
        string selectQueryForCustomer = "SELECT cust_add, cust_add ,cust_type,cust_cash,cust_phone  FROM customer";


        string selectALLProducts = "SELECT p.pro_name, p.pro_quantity, m.model_name, m.quantity  " +
                            "FROM model m " +
                            "JOIN product p ON m.pro_id = p.pro_id";

        public Form1()
        {
            InitializeComponent();
            dgv_stoc.CellContentClick += dgv_stoc_CellContentClick;

            InitializeDataTable();
            FillComboBox();
            FillComboBoxModel();
            FillComboBoxModelForBill();
            FillComboBoxForSupplier();
            FillComboBoxForCustomers();
            ShowDataInDataGridView(selectQueryForroduct, dgvProduct);
            ShowDataInDataGridView(selectQueryForShowSuppliers, dgv_supplirs);
            
            ShowDataInDataGridView(selectQueryForCustomer, dgvCust);
            ShowDataInDataGridViewMain(selectALLProducts, dgv_stoc);


            cb_mod.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_sb_quantity.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_sb_model.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_sb_product.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_supplier.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            cb_mod.Enabled = false;

            cb_sb_quantity.Enabled = false;

            cb_sb_model.Enabled = false;
            dataGridView1.RightToLeft = RightToLeft.Yes;
            dgv_stoc.RightToLeft = RightToLeft.Yes;




        }
        private int GetSupplierId(String SupplierName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string selectQuery = "SELECT id FROM suppliers WHERE supp_name = @Value1 ";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Value1", SupplierName);
                        // command.Parameters.AddWithValue("@modelId", modelid);

                        int supplierID = Convert.ToInt32(command.ExecuteScalar());
                        return supplierID;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    return -1; // Return a default or error value
                }
            }
        }
        private void comboBox1_Click(object sender, EventArgs e)
        {

            cb_mod.Enabled = true;
        }
        private void cb_cust_Click(object sender, EventArgs e)
        {

            cb_cust.Enabled = true;
        }
        private void FillComboBoxQuantitiesForProduct(int productid, String modelname)
        {
            cb_sb_quantity.Items.Clear();  // Clear existing items

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create a SqlCommand to retrieve distinct quantities for a specific product
                    string selectQuery = "SELECT DISTINCT quantity FROM model WHERE model_name = @MName ORDER BY quantity DESC";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        // Add parameter for the product name
                        command.Parameters.AddWithValue("@MName", GetModelName(cb_sb_model.Text));
                        //command.Parameters.AddWithValue("@ProducrID", productid);

                        // Execute the command and get the data in a SqlDataReader
                        SqlDataReader reader = command.ExecuteReader();

                        // Read the distinct quantities and add them to the ComboBox
                        while (reader.Read())
                        {
                            int quantity = reader.GetInt32(0);
                            for (int i = 1; i <= quantity; i++)
                            {

                                cb_sb_quantity.Items.Add(i);
                            }

                        }

                        // Close the SqlDataReader
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void FillComboBoxForCustomers()
        {
            comboBox1.Font = new Font("Arial", 12, FontStyle.Regular);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create a SqlCommand to retrieve suppliers names
                    string selectQuery = "SELECT cust_name FROM customer";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        // Execute the command and get the data in a SqlDataReader
                        SqlDataReader reader = command.ExecuteReader();

                        // Clear the existing items in the ComboBox
                        cb_cust.Items.Clear();

                        // Read the product names and add them to the ComboBox
                        while (reader.Read())
                        {
                            string productName = reader.GetString(0);
                            cb_cust.Items.Add(productName);
                        }

                        // Close the SqlDataReader
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private void FillComboBoxForSupplier()
        {
            comboBox1.Font = new Font("Arial", 12, FontStyle.Regular);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create a SqlCommand to retrieve suppliers names
                    string selectQuery = "SELECT supp_name FROM suppliers";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        // Execute the command and get the data in a SqlDataReader
                        SqlDataReader reader = command.ExecuteReader();

                        // Clear the existing items in the ComboBox
                        cb_supplier.Items.Clear();

                        // Read the product names and add them to the ComboBox
                        while (reader.Read())
                        {
                            string productName = reader.GetString(0);
                            cb_supplier.Items.Add(productName);
                        }

                        // Close the SqlDataReader
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private void FillComboBox()
        {
            comboBox1.Font = new Font("Arial", 12, FontStyle.Regular);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create a SqlCommand to retrieve product names
                    string selectQuery = "SELECT pro_name FROM product";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        // Execute the command and get the data in a SqlDataReader
                        SqlDataReader reader = command.ExecuteReader();

                        // Clear the existing items in the ComboBox
                        comboBox1.Items.Clear();
                        cb_sb_product.Items.Clear();

                        // Read the product names and add them to the ComboBox
                        while (reader.Read())
                        {
                            string productName = reader.GetString(0);
                            comboBox1.Items.Add(productName);
                            cb_sb_product.Items.Add(productName);
                        }

                        // Close the SqlDataReader
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void FillComboBoxModelForBill()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create a SqlCommand to retrieve product names
                    string selectQuery = "SELECT model_name FROM model";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        // Execute the command and get the data in a SqlDataReader
                        SqlDataReader reader = command.ExecuteReader();

                        // Clear the existing items in the ComboBox
                        cb_mod.Items.Clear();

                        // Read the product names and add them to the ComboBox
                        while (reader.Read())
                        {
                            string productName = reader.GetString(0);
                            cb_mod.Items.Add(productName);
                        }

                        // Close the SqlDataReader
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private void FillComboBoxModel()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create a SqlCommand to retrieve product names
                    string selectQuery = "SELECT model_name FROM model";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        // Execute the command and get the data in a SqlDataReader
                        SqlDataReader reader = command.ExecuteReader();

                        // Clear the existing items in the ComboBox
                        cb_mod.Items.Clear();
                        cb_sb_model.Items.Clear();

                        // Read the product names and add them to the ComboBox
                        while (reader.Read())
                        {
                            string productName = reader.GetString(0);
                            cb_mod.Items.Add(productName);
                            cb_sb_model.Items.Add(productName);
                        }

                        // Close the SqlDataReader
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        // ...

        private void InitializeDataTable()
        {

            productsDataTable = new DataTable();

            productsDataTable.Columns.Add("ProductName", typeof(string));
            productsDataTable.Columns.Add("ProductModel", typeof(string));

            productsDataTable.Columns.Add("Quantity", typeof(int));
            productsDataTable.Columns.Add("Price", typeof(decimal));
            productsDataTable.Columns.Add("Total product price", typeof(decimal));


        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
        private void button9_Click(object sender, EventArgs e)
        {

        }
        private void button10_Click(object sender, EventArgs e)
        {

        }
        private void button8_Click(object sender, EventArgs e)
        {

        }
        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void addData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {

                    connection.Open();
                    // Create a SqlCommand with an INSERT statement
                    string insertQuery = "INSERT INTO product (pro_name, pro_quantity) VALUES (@Value1, @Value2)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Replace @Value1, @Value2, @Value3 with the actual values you want to insert
                        command.Parameters.AddWithValue("@Value1", tb_pn.Text);
                        // command.Parameters.AddWithValue("@Value2", GetModelID(cb_model.Text));
                        command.Parameters.AddWithValue("@Value2", tb_qantity.Text);

                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();

                        // Check if any rows were affected (insert successful)
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data added to the database successfully.");
                            string selectQuery = "SELECT pro_name, pro_quantity FROM product";
                            ShowDataInDataGridView(selectQuery, dgvProduct);

                            FillComboBox();
                        }
                        else
                        {
                            MessageBox.Show("No rows were affected. Data may not have been added.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void addSuppliersData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Check if the supplier already exists
                    string checkQuery = "SELECT COUNT(*) FROM suppliers WHERE supp_name = @Value1";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Value1", tb_sn.Text);

                        int existingCount = (int)checkCommand.ExecuteScalar();

                        if (existingCount > 0)
                        {
                            // Supplier already exists, display a message
                            MessageBox.Show("This supplier already exists in the database.");
                            return; // Exit the method without proceeding with the insertion
                        }
                    }

                    // Create a SqlCommand with an INSERT statement
                    string insertQuery = "INSERT INTO suppliers (supp_name, supp_add, supp_phone, cash) VALUES (@Value1, @Value2, @Value3, @Value4)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Replace @Value1, @Value2, @Value3 with the actual values you want to insert
                        command.Parameters.AddWithValue("@Value1", tb_sn.Text);
                        command.Parameters.AddWithValue("@Value2", tb_sa.Text);
                        command.Parameters.AddWithValue("@Value3", tb_sp.Text);
                        command.Parameters.AddWithValue("@Value4", tb_sc.Text);

                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();

                        // Check if any rows were affected (insert successful)
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data added to the database successfully.");
                            string selectQuery = "SELECT supp_name, supp_add, supp_phone, cash FROM suppliers";
                            ShowDataInDataGridView(selectQuery, dgv_supplirs);
                            //FillComboBox();
                        }
                        else
                        {
                            MessageBox.Show("No rows were affected. Data may not have been added.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }


        private void but_add_Click(object sender, EventArgs e)
        {
            string productName = comboBox1.Text;
            string productModel = cb_mod.Text;
            // string productCompany = tb_company.Text;
            int productCountity = Convert.ToInt32(textBox2.Text);
            decimal price = Convert.ToDecimal(tb_pri.Text);
            decimal total_product_price = productCountity * price;
            product_id = GetProductID(productName);
            modelname = GetModelName(productModel);
            //modelId = GetModelName(product_id);
            // Get the product ID for the specified conditions


            // Add the product to the DataTable
            DataRow row = productsDataTable.NewRow();

            row["ProductName"] = productName;
            row["ProductModel"] = productModel;

            row["Quantity"] = productCountity;
            row["Price"] = price;
            row["Total product price"] = total_product_price; // Add a column "ProductID" to your DataTable
            productsDataTable.Rows.Add(row);
            dataGridView1.DataSource = productsDataTable;
            total_price += total_product_price;
            tb_total.Text = Convert.ToString(total_price);

            // Clear TextBoxes after adding the product
            ClearTextBoxes();
            cb_mod.Enabled = false;
            ShowDataInDataGridViewInArabic(dataGridView1);
            ShowDataInDataGridViewMain(selectALLProducts, dgv_stoc);
            //ShowDataInDataGridView(selectALLProducts, dgv_stoc);

        }

        internal int GetProductID(string productName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string selectQuery = "SELECT pro_id FROM product WHERE pro_name = @ProductName ";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProductName", productName);
                        // command.Parameters.AddWithValue("@modelId", modelid);

                        int productID = Convert.ToInt32(command.ExecuteScalar());
                        return productID;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    return -1; // Return a default or error value
                }
            }
        }
        internal int GetCustomerID(string customerName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string selectQuery = "SELECT cust_id FROM customer WHERE cust_name = @CustomerName ";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerName ", customerName);
                        // command.Parameters.AddWithValue("@modelId", modelid);

                        int customerID = Convert.ToInt32(command.ExecuteScalar());
                        return customerID;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    return -1; // Return a default or error value
                }
            }
        }
        private String GetModelName(String mName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string selectQuery = "SELECT model_name FROM model WHERE model_name = @ModelName";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ModelName", mName);


                        String modelN = Convert.ToString(command.ExecuteScalar());
                        return modelN;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    return ""; // Return a default or error value
                }
            }
        }
        private void ClearTextBoxes()
        {
            comboBox1.Items.Clear();
            cb_mod.Items.Clear();


            tb_pri.Clear();
            FillComboBox();
            FillComboBoxModel();
        }
        private void ClearAllBoxes()
        {
            comboBox1.Items.Clear();
            cb_mod.Items.Clear();
            tb_pri.Clear();
            tb_en.Clear();
            tb_total.Clear();
            productsDataTable.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            FillComboBox();
            FillComboBoxModel();
        }
        private async Task AddDataToBillTableAsync()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    await connection.OpenAsync();

                    if (!string.IsNullOrEmpty(tb_en.Text))
                    {
                        string insertBillQuery = "INSERT INTO bill (bill_number, total_price ,supplier_id ,bill_date,paid_amount,creditor_cash) VALUES (@BillNumber, @TotalPrice , @CustomerName, @BillDate, @PaiedMount, @CreditorCash)";
                        using (SqlCommand billCommand = new SqlCommand(insertBillQuery, connection))
                        {
                            billCommand.Parameters.AddWithValue("@BillNumber", Convert.ToInt32(tb_en.Text));
                            billCommand.Parameters.AddWithValue("@TotalPrice", Convert.ToDecimal(tb_total.Text));
                            billCommand.Parameters.AddWithValue("@CustomerName", GetSupplierId(cb_supplier.Text));
                            billCommand.Parameters.AddWithValue("@BillDate", DateTime.Now);
                            billCommand.Parameters.AddWithValue("@PaiedMount", Convert.ToDecimal(tb_paied_mount.Text));
                            billCommand.Parameters.AddWithValue("@CreditorCash", Convert.ToDecimal(tb_creadit.Text));
                            // Execute the command
                            await billCommand.ExecuteNonQueryAsync();
                            // Get the ID of the recently inserted bill
                            string selectBillIDQuery = "SELECT @@IDENTITY";
                            using (SqlCommand selectBillIDCommand = new SqlCommand(selectBillIDQuery, connection))
                            {
                                billID = Convert.ToInt32(await selectBillIDCommand.ExecuteScalarAsync());
                                MessageBox.Show(billID.ToString());
                            }
                        }




                    }

                    // Get the ID of the recently inserted bill

                    // Iterate through the rows in the DataGridView
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // Skip the last row if it is the new row for adding data
                        if (!row.IsNewRow)
                        {


                            string productName = row.Cells["ProductName"].Value.ToString();
                            string productModel = row.Cells["ProductModel"].Value.ToString();

                            quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                            decimal price = Convert.ToDecimal(row.Cells["Price"].Value);



                            try
                            {

                                string insertBillProductQuery = "INSERT INTO bill_product (bill_id, product_id, quantity, product_total_price) VALUES (@BillID, @ProductID, @Quantity, @TotalPrice)";
                                using (SqlCommand billProductCommand = new SqlCommand(insertBillProductQuery, connection))
                                {
                                    billProductCommand.Parameters.AddWithValue("@BillID", billID);
                                    billProductCommand.Parameters.AddWithValue("@ProductID", GetProductID(productName));
                                    //product_id
                                    billProductCommand.Parameters.AddWithValue("@Quantity", quantity);
                                    billProductCommand.Parameters.AddWithValue("@TotalPrice", price * quantity);

                                    await billProductCommand.ExecuteNonQueryAsync();
                                }
                                string updateModelQuery = "UPDATE model SET quantity = quantity + @Quantity WHERE model_name = @ValueOfModel ";
                                using (SqlCommand updateModelCommand = new SqlCommand(updateModelQuery, connection))
                                {
                                    updateModelCommand.Parameters.AddWithValue("@ValueOfModel", productModel);
                                    // updateModelCommand.Parameters.AddWithValue("@ProductID", GetProductID(productName));
                                    updateModelCommand.Parameters.AddWithValue("@Quantity", quantity);

                                    await updateModelCommand.ExecuteNonQueryAsync();
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error there: {ex.Message}");
                            }
                            string updateSupplierQuery = "UPDATE suppliers SET cash = cash + @Quantity WHERE id = @SuppID ";
                            using (SqlCommand updateModelCommand = new SqlCommand(updateSupplierQuery, connection))
                            {
                                updateModelCommand.Parameters.AddWithValue("@SuppID", GetSupplierId(cb_supplier.Text));

                                updateModelCommand.Parameters.AddWithValue("@Quantity", Convert.ToDecimal(tb_creadit.Text));

                                await updateModelCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    MessageBox.Show("Data added to the Bill and BillProduct tables successfully.");
                    ClearAllBoxes();

                    ShowDataInDataGridViewMain(selectALLProducts, dgv_stoc);
                    // ShowDataInDataGridViewInArabic(selectALLProducts, dgv_stoc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private async Task AddDataToSellBillTableAsync()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    await connection.OpenAsync();

                    if (!string.IsNullOrEmpty(tb_sbn.Text))
                    {
                        string insertBillQuery = "INSERT INTO sell_bill (bill_number, total_price ,cust_id ,bill_date,paid_amount,creditor_cash) VALUES (@BillNumber, @TotalPrice , @CustomerName, @BillDate, @PaiedMount, @CreditorCash)";
                        using (SqlCommand billCommand = new SqlCommand(insertBillQuery, connection))
                        {
                            billCommand.Parameters.AddWithValue("@BillNumber", Convert.ToInt32(tb_sbn.Text));
                            billCommand.Parameters.AddWithValue("@TotalPrice", Convert.ToDecimal(tb_sbtotal_price.Text));
                            billCommand.Parameters.AddWithValue("@CustomerName", GetCustomerID(cb_cust.Text));
                            billCommand.Parameters.AddWithValue("@BillDate", DateTime.Now);
                            billCommand.Parameters.AddWithValue("@PaiedMount", Convert.ToDecimal(tb_sb_paied.Text));
                            billCommand.Parameters.AddWithValue("@CreditorCash", Convert.ToDecimal(tb_sb_cash.Text));

                            // Execute the command
                            await billCommand.ExecuteNonQueryAsync();

                            // Get the ID of the recently inserted bill
                            string selectBillIDQuery = "SELECT @@IDENTITY";
                            using (SqlCommand selectBillIDCommand = new SqlCommand(selectBillIDQuery, connection))
                            {
                                sell_billID = Convert.ToInt32(await selectBillIDCommand.ExecuteScalarAsync());
                                MessageBox.Show(sell_billID.ToString());
                            }
                        }




                    }

                    // Get the ID of the recently inserted bill

                    // Iterate through the rows in the DataGridView
                    foreach (DataGridViewRow row in dgv_customers.Rows)
                    {
                        // Skip the last row if it is the new row for adding data
                        if (!row.IsNewRow)
                        {
                            // Get data from DataGridView
                            string productName = row.Cells["ProductName"].Value.ToString();
                            string productModel = row.Cells["ProductModel"].Value.ToString();

                            int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                            decimal price = Convert.ToDecimal(row.Cells["Price"].Value);

                            //string productName = row.Cells["اسم المنتج"].Value.ToString();
                            //string productModel = row.Cells["الموديل"].Value.ToString();
                            //quantity = Convert.ToInt32(row.Cells["الكمية"].Value);
                            //decimal price = Convert.ToDecimal(row.Cells["السعر"].Value);


                            try
                            {

                                string insertBillProductQuery = "INSERT INTO sell_bill_product (bill_id, product_id, quantity, product_total_price) VALUES (@BillID, @ProductID, @Quantity, @TotalPrice)";
                                using (SqlCommand billProductCommand = new SqlCommand(insertBillProductQuery, connection))
                                {
                                    billProductCommand.Parameters.AddWithValue("@BillID", sell_billID);
                                    billProductCommand.Parameters.AddWithValue("@ProductID", GetProductID(productName));
                                    //product_id
                                    billProductCommand.Parameters.AddWithValue("@Quantity", quantity);
                                    billProductCommand.Parameters.AddWithValue("@TotalPrice", price * quantity);

                                    await billProductCommand.ExecuteNonQueryAsync();
                                }
                                string updateModelQuery = "UPDATE model SET quantity = quantity - @Value WHERE model_name = @Mname ";
                                using (SqlCommand updateModelCommand = new SqlCommand(updateModelQuery, connection))
                                {
                                    updateModelCommand.Parameters.AddWithValue("@Mname", productModel);
                                    //updateModelCommand.Parameters.AddWithValue("@ProductID", GetProductID(productName));
                                    updateModelCommand.Parameters.AddWithValue("@Value", quantity);

                                    await updateModelCommand.ExecuteNonQueryAsync();
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error there: {ex.Message}");
                            }
                            string updateSupplierQuery = "UPDATE customer SET cust_cash = cust_cash - @Quantity WHERE cust_id = @CustID ";
                            using (SqlCommand updateModelCommand = new SqlCommand(updateSupplierQuery, connection))
                            {
                                updateModelCommand.Parameters.AddWithValue("@CustID", GetCustomerID(cb_cust.Text));

                                updateModelCommand.Parameters.AddWithValue("@Quantity", Convert.ToDecimal((tb_sb_cash.Text)));

                                await updateModelCommand.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    MessageBox.Show("Data added to the Bill and BillProduct tables successfully.");
                    ClearAllBoxes();
                    ShowDataInDataGridViewMain(selectALLProducts, dgv_stoc);
                    //ShowDataInDataGridView(selectALLProducts, dgv_stoc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }


        private void ShowDataInDataGridView(string query, DataGridView dataGridView)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        // Fill the DataTable with data from the SQL Server table
                        dataAdapter.Fill(dataTable);

                        // Bind the DataTable to the DataGridView
                        dataGridView.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private void dgv_stoc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the button column
            // if (e.ColumnIndex == 0 && e.RowIndex >= 0) // Assuming the button column is at index 0
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                // Get the data from the clicked row
                string productName = dgv_stoc.Rows[e.RowIndex].Cells["pro_name"].Value.ToString();
                string model_name = dgv_stoc.Rows[e.RowIndex].Cells["model_name"].Value.ToString();

                // Example: Show a message with the data
                MessageBox.Show($"Clicked on row {e.RowIndex + 1} - Product: {productName}, Model: {model_name}");

                // Add your logic to update the data in the database or perform any other action
                // ...
            }
        }
        private void ShowDataInDataGridViewMain(string query, DataGridView dataGridView)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        // Fill the DataTable with data from the SQL Server table
                        dataAdapter.Fill(dataTable);


                        // Bind the DataTable to the DataGridView
                        dataGridView.DataSource = dataTable;
                        // Add button columns after the last column at the left side
                        //DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                        //buttonColumn.HeaderText = "";
                        //buttonColumn.Text = "تعديل";
                        //buttonColumn.UseColumnTextForButtonValue = true;
                        //dataGridView.Columns.Insert(4, buttonColumn);
                        foreach (DataGridViewColumn column in dataGridView.Columns)
                        {
                            // Set the RightToLeft property for each column's header
                            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                            // If you want to customize the header text, you can do so like this:
                            switch (column.Name)
                            {

                                case "model_name":
                                    column.HeaderText = "اسم الموديل";
                                    break;
                                case "pro_name":
                                    column.HeaderText = "المنتج";
                                    break;
                                case "quantity":
                                    column.HeaderText = "  الكمية الموديل";
                                    break;
                                case "pro_quantity":
                                    column.HeaderText = "كمية المنتج";
                                    break;


                                    // Add more cases for other columns as needed
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ShowDataInDataGridViewInArabic(DataGridView dataGridView)
        {


            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                // Set the RightToLeft property for each column's header
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                // If you want to customize the header text, you can do so like this:
                switch (column.Name)
                {
                    case "ProductName":
                        column.HeaderText = "اسم المنتج";
                        break;
                    case "ProductModel":
                        column.HeaderText = "نموذج المنتج";
                        break;
                    case "Quantity":
                        column.HeaderText = "الكمية";
                        break;
                    case "Price":
                        column.HeaderText = "السعر";
                        break;
                    case "Total product price":
                        column.HeaderText = "  السعر الاجمالي";
                        break;

                        // Add more cases for other columns as needed
                }
            }


        }

        private async void button6_Click(object sender, EventArgs e)
        {
            await AddDataToBillTableAsync();
            ShowDataInDataGridView(selectQueryForShowSuppliers, dgv_supplirs);
            ShowDataInDataGridView(selectQueryForroduct, dgvProduct);

            ShowDataInDataGridView(selectQueryForCustomer, dgvCust);
            ShowDataInDataGridViewMain(selectALLProducts, dgv_stoc);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addData();
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Clean up resources when the form is closing
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }



        private void bt_add_model_Click(object sender, EventArgs e)
        {
            AddModelForm addModelForm = new AddModelForm(this);
            // Set the StartPosition property to CenterScreen
            addModelForm.StartPosition = FormStartPosition.CenterScreen;
            addModelForm.ShowDialog();

            // After the AddModelForm is closed, you can refresh the ComboBox or perform other actions if needed.
            FillComboBoxModel();
        }
        private void addCustomer()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                bool checkedValue()
                {
                    bool valueToSave = radioButton1.Checked;  // If radioButton2 is checked, valueToSave will be true; otherwise, it will be false
                    if (valueToSave == true)
                    {
                        return true;
                    }
                    return false;

                }
                try
                {

                    connection.Open();
                    // Create a SqlCommand with an INSERT statement
                    string insertQuery = "INSERT INTO customer (cust_name, cust_add, cust_type,cust_cash, cust_phone) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Replace @Value1, @Value2, @Value3 with the actual values you want to insert
                        command.Parameters.AddWithValue("@Value1", tb_cname.Text);
                        command.Parameters.AddWithValue("@Value2", tb_cadd.Text);
                        command.Parameters.AddWithValue("@Value3", checkedValue());
                        command.Parameters.AddWithValue("@Value4", Convert.ToDecimal(tb_ccash.Text));
                        command.Parameters.AddWithValue("@Value5", tb_cphone.Text);

                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();

                        // Check if any rows were affected (insert successful)
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data added to the database successfully.");
                            string selectQuery = "SELECT cust_add, cust_add ,cust_type,cust_cash,cust_phone  FROM customer";
                            ShowDataInDataGridView(selectQuery, dgvCust);


                            //  FillComboBox();
                        }
                        else
                        {
                            MessageBox.Show("No rows were affected. Data may not have been added.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error i can not: {ex.Message}");
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            addCustomer();
            FillComboBoxForCustomers();
            ShowDataInDataGridView(selectQueryForCustomer, dgvCust);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            addSuppliersData();
            FillComboBoxForSupplier();
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }
        private void comboBox4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Assuming comboBox1 contains the selected product name
            FillComboBoxModelForBill();

            // Enable cb_mod when a product is selected
            cb_mod.Enabled = true;

        }

        private void cb_mod_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddModelForm addModelForm = new AddModelForm(this);
            // Set the StartPosition property to CenterScreen
            addModelForm.StartPosition = FormStartPosition.CenterScreen;
            addModelForm.ShowDialog();

            // After the AddModelForm is closed, you can refresh the ComboBox or perform other actions if needed.
            FillComboBoxModel();
        }

        private void cb_supplier_SelectedIndexChanged(object sender, EventArgs e)
        {

            int supp_id = GetSupplierId(cb_supplier.Text);


        }
        private void cb_sb_product_Click(object sender, EventArgs e)
        {
            FillComboBox();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cust_id = GetCustomerID(cb_supplier.Text);
        }

        private void cb_sb_product_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboBoxModel();
            cb_sb_model.Enabled = true;
            //comboBox4.Enabled = true;

        }

        private void btn_add_sb_card_Click(object sender, EventArgs e)
        {
            string productName = cb_sb_product.Text;
            string productModel = cb_sb_model.Text;
            int productCountity = Convert.ToInt32(cb_sb_quantity.Text);
            decimal price = Convert.ToDecimal(tb_sb_price.Text);
            decimal total_product_price = productCountity * price;
            product_id = GetProductID(productName);
            // Add the product to the DataTable
            DataRow row = productsDataTable.NewRow();
            row["ProductName"] = productName;
            row["ProductModel"] = productModel;
            row["Quantity"] = productCountity;
            row["Price"] = price;
            row["Total product price"] = total_product_price; // Add a column "ProductID" to your DataTable
            productsDataTable.Rows.Add(row);
            dgv_customers.DataSource = productsDataTable;
            total_price += total_product_price;
            tb_sbtotal_price.Text = Convert.ToString(total_price);
            // Clear TextBoxes after adding the product
            ClearTextBoxes();
            cb_sb_model.Enabled = false;
            ShowDataInDataGridViewInArabic(dgv_customers);
            ShowDataInDataGridViewMain(selectALLProducts, dgv_stoc);
            //ShowDataInDataGridView(selectALLProducts, dgv_stoc);

        }

        private void cb_sb_model_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedProduct = cb_sb_product.Text;

            int productId = GetProductID(selectedProduct);
            String model_name = GetModelName(cb_sb_model.Text);
            FillComboBoxQuantitiesForProduct(productId, model_name);

            cb_sb_quantity.Enabled = true;



        }

        private async void button2_Click(object sender, EventArgs e)
        {

            await AddDataToSellBillTableAsync();
            ShowDataInDataGridView(selectQueryForShowSuppliers, dgv_supplirs);
            ShowDataInDataGridView(selectQueryForroduct, dgvProduct);

            ShowDataInDataGridView(selectQueryForCustomer, dgvCust);
            ShowDataInDataGridViewMain(selectALLProducts, dgv_stoc);
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

