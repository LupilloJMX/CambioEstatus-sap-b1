namespace CambioEstatus
{
    partial class FormConfiguracion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label2 = new Label();
            txtUrl = new TextBox();
            label3 = new Label();
            txtUser = new TextBox();
            label4 = new Label();
            txtPass = new TextBox();
            label5 = new Label();
            txtCompany = new TextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            labelEstado = new Label();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 37);
            label2.Name = "label2";
            label2.Size = new Size(102, 15);
            label2.TabIndex = 1;
            label2.Text = "URL Service Layer:";
            // 
            // txtUrl
            // 
            txtUrl.Location = new Point(120, 37);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(224, 23);
            txtUrl.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 78);
            label3.Name = "label3";
            label3.Size = new Size(50, 15);
            label3.TabIndex = 3;
            label3.Text = "Usuario:";
            // 
            // txtUser
            // 
            txtUser.Location = new Point(120, 75);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(100, 23);
            txtUser.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 120);
            label4.Name = "label4";
            label4.Size = new Size(70, 15);
            label4.TabIndex = 5;
            label4.Text = "Contraseña:";
            // 
            // txtPass
            // 
            txtPass.Location = new Point(123, 113);
            txtPass.Name = "txtPass";
            txtPass.Size = new Size(100, 23);
            txtPass.TabIndex = 6;
            txtPass.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 162);
            label5.Name = "label5";
            label5.Size = new Size(77, 15);
            label5.TabIndex = 7;
            label5.Text = "CompanyDB:";
            // 
            // txtCompany
            // 
            txtCompany.Location = new Point(123, 154);
            txtCompany.Name = "txtCompany";
            txtCompany.Size = new Size(221, 23);
            txtCompany.TabIndex = 8;
            // 
            // button1
            // 
            button1.Location = new Point(12, 210);
            button1.Name = "button1";
            button1.Size = new Size(170, 23);
            button1.TabIndex = 9;
            button1.Text = "Probar conexión";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(188, 210);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 10;
            button2.Text = "Guardar";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(269, 210);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 11;
            button3.Text = "Cancelar";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // labelEstado
            // 
            labelEstado.AutoSize = true;
            labelEstado.Location = new Point(12, 259);
            labelEstado.Name = "labelEstado";
            labelEstado.Size = new Size(38, 15);
            labelEstado.TabIndex = 12;
            labelEstado.Text = "label6";
            // 
            // FormConfiguracion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(356, 291);
            Controls.Add(labelEstado);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(txtCompany);
            Controls.Add(label5);
            Controls.Add(txtPass);
            Controls.Add(label4);
            Controls.Add(txtUser);
            Controls.Add(label3);
            Controls.Add(txtUrl);
            Controls.Add(label2);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "FormConfiguracion";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configuración de conexión SAP B1";
            Load += FormConfiguracion_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label2;
        private TextBox txtUrl;
        private Label label3;
        private TextBox txtUser;
        private Label label4;
        private TextBox txtPass;
        private Label label5;
        private TextBox txtCompany;
        private Button button1;
        private Button button2;
        private Button button3;
        private Label labelEstado;
    }
}