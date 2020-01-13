namespace ClientExample
{
    partial class ClientWindow
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
            this.connectionTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.connectButtton = new System.Windows.Forms.Button();
            this.consoleTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.payloadTextBox = new System.Windows.Forms.TextBox();
            this.actionTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.addressLabel = new System.Windows.Forms.Label();
            this.requestButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connectionTextBox
            // 
            this.connectionTextBox.Location = new System.Drawing.Point(16, 49);
            this.connectionTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.connectionTextBox.Name = "connectionTextBox";
            this.connectionTextBox.Size = new System.Drawing.Size(181, 27);
            this.connectionTextBox.TabIndex = 0;
            this.connectionTextBox.Text = "testAddr";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "ConnectionAddress";
            // 
            // connectButtton
            // 
            this.connectButtton.Location = new System.Drawing.Point(578, 14);
            this.connectButtton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.connectButtton.Name = "connectButtton";
            this.connectButtton.Size = new System.Drawing.Size(97, 62);
            this.connectButtton.TabIndex = 2;
            this.connectButtton.Text = "Connect";
            this.connectButtton.UseVisualStyleBackColor = true;
            this.connectButtton.Click += new System.EventHandler(this.connectButtton_Click);
            // 
            // consoleTextBox
            // 
            this.consoleTextBox.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.consoleTextBox.Location = new System.Drawing.Point(16, 89);
            this.consoleTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.consoleTextBox.Multiline = true;
            this.consoleTextBox.Name = "consoleTextBox";
            this.consoleTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.consoleTextBox.Size = new System.Drawing.Size(659, 516);
            this.consoleTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(255, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Status";
            // 
            // payloadTextBox
            // 
            this.payloadTextBox.Location = new System.Drawing.Point(96, 615);
            this.payloadTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.payloadTextBox.Name = "payloadTextBox";
            this.payloadTextBox.Size = new System.Drawing.Size(364, 27);
            this.payloadTextBox.TabIndex = 5;
            // 
            // actionTextBox
            // 
            this.actionTextBox.Location = new System.Drawing.Point(16, 615);
            this.actionTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.actionTextBox.Name = "actionTextBox";
            this.actionTextBox.Size = new System.Drawing.Size(72, 27);
            this.actionTextBox.TabIndex = 6;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(468, 613);
            this.sendButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(97, 31);
            this.sendButton.TabIndex = 7;
            this.sendButton.Text = "send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(368, 14);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Address";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(255, 45);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(49, 20);
            this.statusLabel.TabIndex = 9;
            this.statusLabel.Text = "Status";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressLabel.Location = new System.Drawing.Point(368, 45);
            this.addressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(62, 20);
            this.addressLabel.TabIndex = 10;
            this.addressLabel.Text = "Address";
            this.addressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // requestButton
            // 
            this.requestButton.Location = new System.Drawing.Point(578, 613);
            this.requestButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.requestButton.Name = "requestButton";
            this.requestButton.Size = new System.Drawing.Size(97, 31);
            this.requestButton.TabIndex = 21;
            this.requestButton.Text = "Request";
            this.requestButton.UseVisualStyleBackColor = true;
            this.requestButton.Click += new System.EventHandler(this.requestButton_Click);
            // 
            // ClientWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 660);
            this.Controls.Add(this.requestButton);
            this.Controls.Add(this.addressLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.actionTextBox);
            this.Controls.Add(this.payloadTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.consoleTextBox);
            this.Controls.Add(this.connectButtton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.connectionTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ClientWindow";
            this.Text = "Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox connectionTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button connectButtton;
        private System.Windows.Forms.TextBox consoleTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox payloadTextBox;
        private System.Windows.Forms.TextBox actionTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.Button requestButton;
    }
}

