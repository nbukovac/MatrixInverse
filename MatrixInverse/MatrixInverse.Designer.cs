namespace MatrixInverse
{
    partial class MatrixInverse
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
            this.originalMatrixTextBox = new System.Windows.Forms.TextBox();
            this.matrixInputLabel = new System.Windows.Forms.Label();
            this.startCalculationBtn = new System.Windows.Forms.Button();
            this.inverseMatrixTextBox = new System.Windows.Forms.TextBox();
            this.inverseMatrixLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // originalMatrixTextBox
            // 
            this.originalMatrixTextBox.Location = new System.Drawing.Point(26, 27);
            this.originalMatrixTextBox.Multiline = true;
            this.originalMatrixTextBox.Name = "originalMatrixTextBox";
            this.originalMatrixTextBox.Size = new System.Drawing.Size(283, 189);
            this.originalMatrixTextBox.TabIndex = 0;
            // 
            // matrixInputLabel
            // 
            this.matrixInputLabel.AutoSize = true;
            this.matrixInputLabel.Location = new System.Drawing.Point(26, 8);
            this.matrixInputLabel.Name = "matrixInputLabel";
            this.matrixInputLabel.Size = new System.Drawing.Size(108, 13);
            this.matrixInputLabel.TabIndex = 1;
            this.matrixInputLabel.Text = "Input your matrix here";
            // 
            // startCalculationBtn
            // 
            this.startCalculationBtn.Location = new System.Drawing.Point(127, 240);
            this.startCalculationBtn.Name = "startCalculationBtn";
            this.startCalculationBtn.Size = new System.Drawing.Size(182, 23);
            this.startCalculationBtn.TabIndex = 3;
            this.startCalculationBtn.Text = "Get the inverse matrix";
            this.startCalculationBtn.UseVisualStyleBackColor = true;
            this.startCalculationBtn.Click += new System.EventHandler(this.startCalculationBtn_Click);
            // 
            // inverseMatrixTextBox
            // 
            this.inverseMatrixTextBox.Location = new System.Drawing.Point(26, 276);
            this.inverseMatrixTextBox.Multiline = true;
            this.inverseMatrixTextBox.Name = "inverseMatrixTextBox";
            this.inverseMatrixTextBox.Size = new System.Drawing.Size(283, 184);
            this.inverseMatrixTextBox.TabIndex = 5;
            // 
            // inverseMatrixLabel
            // 
            this.inverseMatrixLabel.AutoSize = true;
            this.inverseMatrixLabel.Location = new System.Drawing.Point(26, 250);
            this.inverseMatrixLabel.Name = "inverseMatrixLabel";
            this.inverseMatrixLabel.Size = new System.Drawing.Size(70, 13);
            this.inverseMatrixLabel.TabIndex = 8;
            this.inverseMatrixLabel.Text = "InverseMatrix";
            // 
            // MatrixInverse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 472);
            this.Controls.Add(this.inverseMatrixLabel);
            this.Controls.Add(this.inverseMatrixTextBox);
            this.Controls.Add(this.startCalculationBtn);
            this.Controls.Add(this.matrixInputLabel);
            this.Controls.Add(this.originalMatrixTextBox);
            this.Name = "MatrixInverse";
            this.Text = "Matrix inverse calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox originalMatrixTextBox;
        private System.Windows.Forms.Label matrixInputLabel;
        private System.Windows.Forms.Button startCalculationBtn;
        private System.Windows.Forms.TextBox inverseMatrixTextBox;
        private System.Windows.Forms.Label inverseMatrixLabel;
    }
}

