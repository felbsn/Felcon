namespace ServerServiceExample
{
    partial class ServerServiceWindow
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
            this.requestButton = new System.Windows.Forms.Button();
            this.clientsListView = new System.Windows.Forms.ListView();
            this.sendButton = new System.Windows.Forms.Button();
            this.actionTextBox = new System.Windows.Forms.TextBox();
            this.payloadTextBox = new System.Windows.Forms.TextBox();
            this.consoleTextBox = new System.Windows.Forms.TextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.serverAddressTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // requestButton
            // 
            this.requestButton.Location = new System.Drawing.Point(523, 493);
            this.requestButton.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.requestButton.Name = "requestButton";
            this.requestButton.Size = new System.Drawing.Size(66, 27);
            this.requestButton.TabIndex = 29;
            this.requestButton.Text = "Request";
            this.requestButton.UseVisualStyleBackColor = true;
            this.requestButton.Click += new System.EventHandler(this.requestButton_Click);
            // 
            // clientsListView
            // 
            this.clientsListView.HideSelection = false;
            this.clientsListView.Location = new System.Drawing.Point(439, 53);
            this.clientsListView.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.clientsListView.Name = "clientsListView";
            this.clientsListView.Size = new System.Drawing.Size(150, 425);
            this.clientsListView.TabIndex = 28;
            this.clientsListView.UseCompatibleStateImageBehavior = false;
            this.clientsListView.View = System.Windows.Forms.View.List;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(436, 493);
            this.sendButton.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(77, 27);
            this.sendButton.TabIndex = 27;
            this.sendButton.Text = "send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // actionTextBox
            // 
            this.actionTextBox.Location = new System.Drawing.Point(14, 493);
            this.actionTextBox.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.actionTextBox.Name = "actionTextBox";
            this.actionTextBox.Size = new System.Drawing.Size(95, 27);
            this.actionTextBox.TabIndex = 26;
            // 
            // payloadTextBox
            // 
            this.payloadTextBox.Location = new System.Drawing.Point(119, 493);
            this.payloadTextBox.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.payloadTextBox.Name = "payloadTextBox";
            this.payloadTextBox.Size = new System.Drawing.Size(307, 27);
            this.payloadTextBox.TabIndex = 25;
            // 
            // consoleTextBox
            // 
            this.consoleTextBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.consoleTextBox.Location = new System.Drawing.Point(17, 53);
            this.consoleTextBox.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.consoleTextBox.Multiline = true;
            this.consoleTextBox.Name = "consoleTextBox";
            this.consoleTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.consoleTextBox.Size = new System.Drawing.Size(412, 424);
            this.consoleTextBox.TabIndex = 24;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(439, 10);
            this.startButton.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(150, 27);
            this.startButton.TabIndex = 23;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 20);
            this.label1.TabIndex = 22;
            this.label1.Text = "ServerAddress";
            // 
            // serverAddressTextBox
            // 
            this.serverAddressTextBox.Location = new System.Drawing.Point(189, 10);
            this.serverAddressTextBox.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.serverAddressTextBox.Name = "serverAddressTextBox";
            this.serverAddressTextBox.Size = new System.Drawing.Size(240, 27);
            this.serverAddressTextBox.TabIndex = 21;
            this.serverAddressTextBox.Text = "testAddr";
            // 
            // ServerServiceWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 526);
            this.Controls.Add(this.requestButton);
            this.Controls.Add(this.clientsListView);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.actionTextBox);
            this.Controls.Add(this.payloadTextBox);
            this.Controls.Add(this.consoleTextBox);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serverAddressTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ServerServiceWindow";
            this.Text = "ServerService";
            this.Load += new System.EventHandler(this.ServerServiceWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button requestButton;
        private System.Windows.Forms.ListView clientsListView;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox actionTextBox;
        private System.Windows.Forms.TextBox payloadTextBox;
        private System.Windows.Forms.TextBox consoleTextBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverAddressTextBox;
    }
}

