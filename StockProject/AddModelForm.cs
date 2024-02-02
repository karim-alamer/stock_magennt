using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StockProject
{
    public partial class AddModelForm : Form
    {
             private readonly  string connectionString = "Data Source=DESKTOP-21J1CL1\\SQLEXPRESS;Initial Catalog=stock_ayman;Integrated Security=True;";
                private readonly Form1 form1;
        public AddModelForm(Form1 form1)
        {
            this.form1 = form1;
            InitializeComponent();
            FillComboBox();
        }



        private void InitializeComponent()
        {
            label1 = new Label();
            tb_mn = new TextBox();
            bt_add = new Button();
            label2 = new Label();
            comboBox1 = new ComboBox();
            tb_mq = new TextBox();
            label3 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Arial Narrow", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(179, 27);
            label1.Name = "label1";
            label1.Size = new Size(67, 20);
            label1.TabIndex = 5;
            label1.Text = "اسم الموديل";
            // 
            // tb_mn
            // 
            tb_mn.Location = new Point(25, 24);
            tb_mn.Name = "tb_mn";
            tb_mn.Size = new Size(148, 23);
            tb_mn.TabIndex = 4;
            // 
            // bt_add
            // 
            bt_add.Font = new Font("Arial Narrow", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bt_add.Location = new Point(65, 149);
            bt_add.Name = "bt_add";
            bt_add.Size = new Size(75, 23);
            bt_add.TabIndex = 7;
            bt_add.Text = "اضافة موديل";
            bt_add.UseVisualStyleBackColor = true;
            bt_add.Click += bt_add_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Arial Narrow", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(193, 71);
            label2.Name = "label2";
            label2.Size = new Size(38, 20);
            label2.TabIndex = 9;
            label2.Text = "المنتج";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(27, 68);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(148, 23);
            comboBox1.TabIndex = 10;
            // 
            // tb_mq
            // 
            tb_mq.Location = new Point(25, 108);
            tb_mq.Name = "tb_mq";
            tb_mq.Size = new Size(148, 23);
            tb_mq.TabIndex = 11;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Arial Narrow", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(193, 107);
            label3.Name = "label3";
            label3.Size = new Size(38, 20);
            label3.TabIndex = 12;
            label3.Text = "الكمية";
            // 
            // AddModelForm
            // 
            ClientSize = new Size(288, 184);
            Controls.Add(label3);
            Controls.Add(tb_mq);
            Controls.Add(comboBox1);
            Controls.Add(label2);
            Controls.Add(bt_add);
            Controls.Add(label1);
            Controls.Add(tb_mn);
            Name = "AddModelForm";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label label1;
        private TextBox tb_mn;
        private Label label2;
        private ComboBox comboBox1;
        private TextBox tb_mq;
        private Label label3;
        private Button bt_add;

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

                        // Read the product names and add them to the ComboBox
                        while (reader.Read())
                        {
                            string productName = reader.GetString(0);
                            comboBox1.Items.Add(productName);
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
        private void bt_add_Click(object sender, EventArgs e)
        {
            string modelName = tb_mn.Text;
            string modelQuantity = tb_mq.Text;

            if (!string.IsNullOrEmpty(modelName))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Check if the model already exists for the selected product
                        int existingModelId = GetModelId(modelName, form1.GetProductID(comboBox1.Text));

                        if (existingModelId != -1)
                        {
                            // Model already exists, update the quantity
                            string updateQuery = "UPDATE model SET quantity = quantity + @Value1 WHERE model_id = @ModelId";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@Value1", modelQuantity);
                                updateCommand.Parameters.AddWithValue("@ModelId", existingModelId);

                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Model quantity updated successfully.");
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("No rows were affected. Model quantity may not have been updated.");
                                }
                            }
                        }
                        else
                        {
                            // Model does not exist, insert a new row
                            string insertQuery = "INSERT INTO model (model_name, quantity, pro_id) VALUES (@Value1, @Value2, @Value3)";
                            using (SqlCommand command = new SqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@Value1", modelName);
                                command.Parameters.AddWithValue("@Value2", modelQuantity);
                                command.Parameters.AddWithValue("@Value3", form1.GetProductID(comboBox1.Text));

                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Model added successfully.");
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("No rows were affected. Model may not have been added.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a model name.");
            }
        }

        private int GetModelId(string modelName, int productId)
        {
            // Function to retrieve the model_id for a given model name and product_id
            // Returns -1 if the model does not exist
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT model_id FROM model WHERE model_name = @ModelName AND pro_id = @ProductId";
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@ModelName", modelName);
                    command.Parameters.AddWithValue("@ProductId", productId);

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (int)result;
                    }

                    return -1; // Model not found
                }
            }
        }
        //private void bt_add_Click(object sender, EventArgs e)
        //{
        //    string modelName = tb_mn.Text;
        //    string modelQuantity = tb_mq.Text;

        //    if (!string.IsNullOrEmpty(modelName))
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            try
        //            {
        //                connection.Open();

        //                string insertQuery = "INSERT INTO model (model_name,quantity,pro_id) VALUES (@Value1,@Value2 , @Value3)";
        //                using (SqlCommand command = new SqlCommand(insertQuery, connection))
        //                {
        //                    command.Parameters.AddWithValue("@Value1", modelName);
        //                    command.Parameters.AddWithValue("@Value2", modelQuantity);
        //                    command.Parameters.AddWithValue("@Value3", form1.GetProductID(comboBox1.Text));
        //                    int rowsAffected = command.ExecuteNonQuery();

        //                    if (rowsAffected > 0)
        //                    {
        //                        MessageBox.Show("Model added successfully.");
        //                        this.Close();  // Close the form after successful addition
        //                    }
        //                    else
        //                    {
        //                        MessageBox.Show("No rows were affected. Model may not have been added.");
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Error: {ex.Message}");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please enter a model name.");
        //    }

        //}
    }
}
