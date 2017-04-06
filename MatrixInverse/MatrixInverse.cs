using System;
using System.Windows.Forms;

namespace MatrixInverse
{
    public partial class MatrixInverse : Form
    {
        private Matrix _matrix;

        public MatrixInverse()
        {
            InitializeComponent();
        }

        private void startCalculationBtn_Click(object sender, EventArgs e)
        {
            var originalMatrixString = originalMatrixTextBox.Text;
            _matrix = new Matrix(originalMatrixString.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries));
            try
            {
                inverseMatrixTextBox.Text = _matrix.InverseMatrix().ToString();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "This matrix isn't regular", "Can't calculate the inverse matrix",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}