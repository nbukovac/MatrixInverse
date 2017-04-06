using System;
using System.IO;
using System.Linq;
using System.Text;
using MatrixInverse.Exceptions;
using MatrixInverse.Strategies;

namespace MatrixInverse
{
    public class Matrix
    {
        private const double EyeZero = 0.0;
        private const double EyeOne = 1.0;
        private const int VectorColumns = 0;
        private const double MyEpsilon = 0.000001;
        private static readonly ICalculate SumCalculation = new CalculateSum();
        private static readonly ICalculate SubtractionCalculation = new CalculateSubtract();
        private int[] _permutationVector;

        private readonly double[,] _elements;
        private Matrix _decomposedMatrix;

        #region LU and LUP decomposition

        public void LuDecomposition()
        {
            _decomposedMatrix = new Matrix(this);

            for (var i = 0; i < _decomposedMatrix.Rows - 1; i++)
            {
                for (var j = i + 1; j < _decomposedMatrix.Columns; j++)
                {
                    if (Math.Abs(_decomposedMatrix.GetMatrixElement(i, i)) < MyEpsilon)
                    {
                        throw new ArgumentException("Pivot element can't be 0.");
                    }

                    _decomposedMatrix.SetMatrixElement(j, i,
                        _decomposedMatrix.GetMatrixElement(j, i)/_decomposedMatrix.GetMatrixElement(i, i));

                    for (var k = i + 1; k < _decomposedMatrix.Columns; k++)
                    {
                        var result = _decomposedMatrix.GetMatrixElement(j, k) -
                                     _decomposedMatrix.GetMatrixElement(j, i)
                                     *_decomposedMatrix.GetMatrixElement(i, k);

                        _decomposedMatrix.SetMatrixElement(j, k, result);
                    }
                }
            }

            GenerateUMatrix();
            GenerateLMatrix();
        }

        public Matrix ForwardSubstitution(Matrix vectorB)
        {
            IsVector(vectorB);
            CheckRowSize(this, vectorB);
            var vectorY = new Matrix(vectorB);

            for (var i = 0; i < _decomposedMatrix.Rows - 1; i++)
            {
                for (var j = i + 1; j < vectorB.Rows; j++)
                {
                    var result = vectorY.GetMatrixElement(j, VectorColumns) -
                                 _decomposedMatrix.GetMatrixElement(j, i)*vectorY.GetMatrixElement(i, VectorColumns);

                    vectorY.SetMatrixElement(j, VectorColumns, result);
                }
            }

            return vectorY;
        }

        public Matrix BackSubstitution(Matrix vectorY)
        {
            IsVector(vectorY);
            CheckRowSize(this, vectorY);
            var vectorX = new Matrix(vectorY);

            for (var i = Rows - 1; i >= 0; i--)
            {
                if (Math.Abs(_decomposedMatrix.GetMatrixElement(i, i)) < MyEpsilon)
                {
                    throw new ArgumentException("Pivot element can't be 0.");
                }

                vectorX.SetMatrixElement(i, VectorColumns,
                    vectorX.GetMatrixElement(i, VectorColumns)/_decomposedMatrix.GetMatrixElement(i, i));

                for (var j = 0; j < i; j++)
                {
                    var result = vectorX.GetMatrixElement(j, VectorColumns) -
                                 _decomposedMatrix.GetMatrixElement(j, i)*
                                 vectorX.GetMatrixElement(i, VectorColumns);
                    vectorX.SetMatrixElement(j, VectorColumns, result);
                }
            }

            return vectorX;
        }

        public void LupInverseDecomposition()
        {
            _decomposedMatrix = new Matrix(this);
            _permutationVector = new int[_decomposedMatrix.Rows];
            PermutationMatrix = EyeMatrix(_permutationVector.Length);

            for (var j = 0; j < _permutationVector.Length; j++)
            {
                _permutationVector[j] = j;
            }

            for (var i = 0; i < _decomposedMatrix.Rows - 1; i++)
            {
                var pivot = i;

                for (var j = i + 1; j < _decomposedMatrix.Rows; j++)
                {
                    if (Math.Abs(_decomposedMatrix.GetMatrixElement(j, i)) >
                        Math.Abs(_decomposedMatrix.GetMatrixElement(pivot, i)))
                    {
                        pivot = j;
                    }
                }

                var temp = _permutationVector[i];
                _permutationVector[i] = _permutationVector[pivot];
                _permutationVector[pivot] = temp;

                _decomposedMatrix.RowSubstitution(i, pivot);
                PermutationMatrix.RowSubstitution(i, pivot);

                for (var j = i + 1; j < _decomposedMatrix.Rows; j++)
                {
                    var result = _decomposedMatrix.GetMatrixElement(j, i) / _decomposedMatrix.GetMatrixElement(i, i);
                    _decomposedMatrix.SetMatrixElement(j, i, result);

                    for (var k = i + 1; k < _decomposedMatrix.Rows; k++)
                    {
                        result = _decomposedMatrix.GetMatrixElement(j, k) -
                                 _decomposedMatrix.GetMatrixElement(j, i) * _decomposedMatrix.GetMatrixElement(i, k);

                        _decomposedMatrix.SetMatrixElement(j, k, result);
                    }
                }
            }

            GenerateLMatrix();
            GenerateUMatrix();
        }

        public Matrix InverseMatrix()
        {
            LupInverseDecomposition();
            Matrix solution = new Matrix(_decomposedMatrix);

            for (int i = 0; i < solution.Columns; i++)
            {
                var vector = new Matrix(solution.Rows, 1);
                vector.SetMatrixElement(_permutationVector[i], 0, 1);
                var vectorY = ForwardSubstitution(vector);
                var solutionVector = BackSubstitution(vectorY);

                for (int j = 0; j < solution.Rows; j++)
                {
                    solution.SetMatrixElement(j, i, solutionVector.GetMatrixElement(j, 0));
                }
            }

            return solution;
        }

        public void LupDecomposition(Matrix vectorB)
        {
            _decomposedMatrix = new Matrix(this);
            var permutationVector = new int[_decomposedMatrix.Rows];
            PermutationMatrix = EyeMatrix(permutationVector.Length);

            for (var j = 0; j < permutationVector.Length; j++)
            {
                permutationVector[j] = j;
            }

            for (var i = 0; i < _decomposedMatrix.Rows - 1; i++)
            {
                var pivot = i;

                for (var j = i + 1; j < _decomposedMatrix.Rows; j++)
                {
                    if (Math.Abs(_decomposedMatrix.GetMatrixElement(j, i)) >
                        Math.Abs(_decomposedMatrix.GetMatrixElement(pivot, i)))
                    {
                        pivot = j;
                    }
                }

                var temp = permutationVector[i];
                permutationVector[i] = permutationVector[pivot];
                permutationVector[pivot] = temp;

                _decomposedMatrix.RowSubstitution(i, pivot);
                PermutationMatrix.RowSubstitution(i, pivot);
                vectorB.RowSubstitution(i, pivot);

                for (var j = i + 1; j < _decomposedMatrix.Rows; j++)
                {
                    var result = _decomposedMatrix.GetMatrixElement(j, i)/_decomposedMatrix.GetMatrixElement(i, i);
                    _decomposedMatrix.SetMatrixElement(j, i, result);

                    for (var k = i + 1; k < _decomposedMatrix.Rows; k++)
                    {
                        result = _decomposedMatrix.GetMatrixElement(j, k) -
                                 _decomposedMatrix.GetMatrixElement(j, i)*_decomposedMatrix.GetMatrixElement(i, k);

                        _decomposedMatrix.SetMatrixElement(j, k, result);
                    }
                }
            }

            GenerateLMatrix();
            GenerateUMatrix();
        }

        public void CompleteLupDecomposition(Matrix vectorB)
        {
            Console.WriteLine("LUP decomposition");
            Console.WriteLine("Original matrix");
            Console.WriteLine(this);
            Console.WriteLine();

            try
            {
                LupDecomposition(vectorB);
                PrintLuMatrices();
                Console.WriteLine("Permutation matrix");
                Console.WriteLine(PermutationMatrix);
                Console.WriteLine();
                PrintSubstitutionResults(vectorB);
            }
            catch (Exception exception)
            {
                Console.WriteLine("LUP decomposition isn't possible for this matrix! " + exception.Message);
            }
        }

        public Matrix CompleteLup(Matrix vectorB)
        {
            LupDecomposition(vectorB);
            var vectorY = ForwardSubstitution(vectorB);
            return BackSubstitution(vectorY);
        }

        public void CompleteLuDecomposition(Matrix vectorB)
        {
            Console.WriteLine("LU decomposition");
            Console.WriteLine("Original matrix");
            Console.WriteLine(this);
            Console.WriteLine();

            try
            {
                LuDecomposition();
                PrintLuMatrices();
                PrintSubstitutionResults(vectorB);
            }
            catch (Exception exception)
            {
                Console.WriteLine("LU decomposition isn't possible for this matrix! " + exception.Message);
                Console.WriteLine();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a <see cref="Matrix"/> of the specified dimensions.
        /// </summary>
        /// <exception cref="MatrixDimensionException">
        /// Is thrown if either the number of <code>rows</code> or <code>columns</code> is less than 1.
        /// </exception>
        /// <param name="rows">Number of matrix rows</param>
        /// <param name="columns">Number of matrix columns</param>
        public Matrix(int rows, int columns)
        {
            CheckDimension(rows);
            CheckDimension(columns);

            Rows = rows;
            Columns = columns;
            _elements = new double[Rows, Columns];
        }

        /// <summary>
        /// Constructs a <see cref="Matrix"/> that is the same as the one passed as a parameter but with a deep copied 
        /// <code>elements</code> array.
        /// </summary>
        /// <param name="matrix"><see cref="Matrix"/> to copy</param>
        public Matrix(Matrix matrix)
        {
            Rows = matrix.Rows;
            Columns = matrix.Columns;
            _elements = new double[Rows, Columns];
            Array.Copy(matrix._elements, _elements, matrix._elements.Length);
        }

        public Matrix(string[] lines)
        {
            Rows = lines.Length;
            Columns = Rows;
            _elements = new double[Rows, Columns];


            var row = 0;
            foreach (var line in lines)
            {
                var column = 0;
                var separatedLine = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                foreach (var s in separatedLine)
                {
                    try
                    {
                        var value = double.Parse(s);
                        SetMatrixElement(row, column, value);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Invalid argument for double parameter! " + exception.Message);
                    }
                    column++;
                }

                row++;
            }
        }

        /// <summary>
        /// Constructs a <see cref="Matrix"/> from the file specified by the parameter <code>fileName</code>.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        public Matrix(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            Rows = lines.Length;

            var row = 0;
            foreach (var line in lines)
            {
                var separatedLine = line.Split(null);
                var column = 0;

                if (Columns == 0)
                {
                    Columns = separatedLine.Length;
                    _elements = new double[Rows, Columns];
                }

                foreach (var s in separatedLine)
                {
                    try
                    {
                        var value = double.Parse(s);
                        SetMatrixElement(row, column, value);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Invalid argument for double parameter! " + exception.Message);
                    }
                    column++;
                }

                row++;
            }
        }

        /// <summary>
        /// Constructs an eye <see cref="Matrix"/> which dimension is defined by the <code>dimension</code> 
        /// parameter specified.
        /// </summary>
        /// <exception cref="MatrixDimensionException">
        /// Is thrown if the <code>dimension</code> parameter is less than 1.
        /// </exception>
        /// <param name="dimension">Eye matrix dimension</param>
        public static Matrix EyeMatrix(int dimension)
        {
            CheckDimension(dimension);
            var eyeMatrix = new Matrix(dimension, dimension);

            for (var row = 0; row < eyeMatrix.Rows; row++)
            {
                for (var column = 0; column < eyeMatrix.Columns; column++)
                {
                    eyeMatrix.SetMatrixElement(row, column, row == column ? EyeOne : EyeZero);
                }
            }

            return eyeMatrix;
        }

        #endregion

        #region Properties/ Setters and Getters

        /// <summary>
        /// Number of matrix rows
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Number of matrix columns
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Sets the specified matrix element to the specified value.
        /// </summary>
        /// <param name="row">Row in which the element is located</param>
        /// <param name="column">Column in which the element is located</param>
        /// <param name="value">New element value</param>
        public void SetMatrixElement(int row, int column, double value)
        {
            CheckMatrixBounds(row, column);
            _elements[row, column] = value;
        }

        /// <summary>
        /// Return the specified matrix element.
        /// </summary>
        /// <param name="row">Row in which the element is located</param>
        /// <param name="column">Column in which the element is located</param>
        /// <returns></returns>
        public double GetMatrixElement(int row, int column)
        {
            CheckMatrixBounds(row, column);
            return _elements[row, column];
        }

        public Matrix LMatrix { get; private set; }

        public Matrix UMatrix { get; private set; }

        public Matrix PermutationMatrix { get; private set; }

        private void GenerateLMatrix()
        {
            LMatrix = new Matrix(_decomposedMatrix);

            for (var row = 0; row < LMatrix.Rows; row++)
            {
                for (var column = row; column < LMatrix.Columns; column++)
                {
                    LMatrix.SetMatrixElement(row, column, row == column ? 1 : 0);
                }
            }
        }

        private void GenerateUMatrix()
        {
            UMatrix = new Matrix(_decomposedMatrix);

            for (var row = 1; row < UMatrix.Rows; row++)
            {
                for (var column = 0; column < row; column++)
                {
                    UMatrix.SetMatrixElement(row, column, 0);
                }
            }
        }

        #endregion

        #region Overloaded Operators

        /// <summary>
        /// Sums the two <see cref="Matrix"/> parameters and returns the sum as a new <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrixA">First <see cref="Matrix"/> parameter</param>
        /// <param name="matrixB">Second <see cref="Matrix"/> parameter</param>
        /// <returns>New summed <see cref="Matrix"/></returns>
        public static Matrix operator +(Matrix matrixA, Matrix matrixB)
        {
            return CalculationIterate(matrixA, matrixB, SumCalculation);
        }

        /// <summary>
        /// Subtracts the two <see cref="Matrix"/> parameters and returns the difference 
        /// as a new <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrixA">First <see cref="Matrix"/> parameter</param>
        /// <param name="matrixB">Second <see cref="Matrix"/> parameter</param>
        /// <returns>New subtracted <see cref="Matrix"/></returns>
        public static Matrix operator -(Matrix matrixA, Matrix matrixB)
        {
            return CalculationIterate(matrixA, matrixB, SubtractionCalculation);
        }


        public static bool operator ==(Matrix matrixA, Matrix matrixB)
        {
            return Equals(matrixA, matrixB);
        }

        public static bool operator !=(Matrix matrixA, Matrix matrixB)
        {
            return !(matrixA == matrixB);
        }

        public static bool operator <=(Matrix matrixA, Matrix matrixB)
        {
            for (var i = 0; i < matrixA.Rows; i++)
            {
                for (var j = 0; j < matrixA.Columns; j++)
                {
                    if (Math.Abs(matrixA.GetMatrixElement(i, j)) > matrixB.GetMatrixElement(i, j))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool operator >=(Matrix matrixA, Matrix matrixB)
        {
            return !(matrixA <= matrixB);
        }

        /// <summary>
        /// Returns a transposed <see cref="Matrix"/> of <code>this</code> <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrixA"><see cref="Matrix"/> for transposition</param>
        /// <returns>Transposed <see cref="Matrix"/></returns>
        public static Matrix operator ~(Matrix matrixA)
        {
            return MatrixTransposition(matrixA);
        }


        /// <summary>
        /// Multiplies the specified matrices and returns the result. 
        /// </summary>
        /// <exception cref="MatrixDimensionException">If the two matrices aren't chained</exception>
        /// <param name="matrixA">First <see cref="Matrix"/></param>
        /// <param name="matrixB">Second <see cref="Matrix"/></param>
        /// <returns>Result <see cref="Matrix"/></returns>
        public static Matrix operator *(Matrix matrixA, Matrix matrixB)
        {
            return Multiply(matrixA, matrixB);
        }

        #endregion

        #region Matrix Calculation Methods

        /// <summary>
        /// Returns <code>this</code> <see cref="Matrix"/> multiplied by a scalar defined by 
        /// the parameter <code>scalar</code>
        /// </summary>
        /// <param name="scalar">Scalar for multiplication</param>
        /// <returns>Result <see cref="Matrix"/></returns>
        public Matrix ScalarMultiply(double scalar)
        {
            var matrix = new Matrix(this);

            for (var row = 0; row < matrix.Rows; row++)
            {
                for (var column = 0; column < matrix.Columns; column++)
                {
                    matrix.SetMatrixElement(row, column, matrix.GetMatrixElement(row, column)*scalar);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Multiplies the specified matrices and returns the result. 
        /// </summary>
        /// <exception cref="MatrixDimensionException">If the two matrices aren't chained</exception>
        /// <param name="matrixA">First <see cref="Matrix"/></param>
        /// <param name="matrixB">Second <see cref="Matrix"/></param>
        /// <returns></returns>
        public static Matrix Multiply(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.Columns != matrixB.Rows)
            {
                throw new MatrixDimensionException("Matrices aren't chained! Dimensions are : "
                                                   + matrixA.Columns + " columns for matrixA and " + matrixB.Rows +
                                                   " rows for matrixB.");
            }

            var multipliedMatrix = new Matrix(matrixA.Rows, matrixB.Columns);

            for (var row = 0; row < multipliedMatrix.Rows; row++)
            {
                for (var column = 0; column < multipliedMatrix.Columns; column++)
                {
                    multipliedMatrix.SetMatrixElement(row, column, 0);
                    for (var i = 0; i < matrixA.Columns; i++)
                    {
                        var result = matrixA.GetMatrixElement(row, i)*matrixB.GetMatrixElement(i, column);
                        multipliedMatrix.SetMatrixElement(row, column,
                            result + multipliedMatrix.GetMatrixElement(row, column));
                    }
                }
            }

            return multipliedMatrix;
        }

        private static Matrix CalculationIterate(Matrix matrixA, Matrix matrixB, ICalculate calculation)
        {
            CheckMatrixDimensions(matrixA, matrixB);
            var sumMatrix = new Matrix(matrixA);

            for (var row = 0; row < matrixA.Rows; row++)
            {
                for (var column = 0; column < matrixA.Columns; column++)
                {
                    var sum = calculation.Calculate(matrixA.GetMatrixElement(row, column),
                        matrixB.GetMatrixElement(row, column));
                    sumMatrix.SetMatrixElement(row, column, sum);
                }
            }

            return sumMatrix;
        }

        /// <summary>
        /// Returns a transposed <see cref="Matrix"/> of <code>this</code> <see cref="Matrix"/>.
        /// </summary>
        /// <returns>Transposed <see cref="Matrix"/></returns>
        public Matrix Transpose()
        {
            return MatrixTransposition(this);
        }

        private static Matrix MatrixTransposition(Matrix matrix)
        {
            var transposedMatrix = new Matrix(matrix.Columns, matrix.Rows);

            for (var row = 0; row < matrix.Rows; row++)
            {
                for (var column = 0; column < matrix.Columns; column++)
                {
                    transposedMatrix.SetMatrixElement(column, row, matrix.GetMatrixElement(row, column));
                }
            }

            return transposedMatrix;
        }

        private void RowSubstitution(int firstRow, int secondRow)
        {
            var temp = _elements.Cast<double>()
                .Skip(firstRow*Columns)
                .Take(Columns)
                .ToArray();

            for (var column = 0; column < Columns; column++)
            {
                SetMatrixElement(firstRow, column, GetMatrixElement(secondRow, column));
                SetMatrixElement(secondRow, column, temp[column]);
            }
        }

        #endregion

        #region Equals and Hascode

        public override bool Equals(object obj)
        {
            var matrix = obj as Matrix;

            if ((Rows != matrix?.Rows) || (Columns != matrix.Columns))
            {
                return false;
            }

            for (var row = 0; row < Rows; row++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    var abs = Math.Abs(GetMatrixElement(row, column) - matrix.GetMatrixElement(row, column));
                    if (abs > MyEpsilon)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _elements?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ Rows;
                hashCode = (hashCode*397) ^ Columns;
                return hashCode;
            }
        }

        #endregion

        #region Validation Methods

        private static void CheckMatrixDimensions(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.Rows != matrixB.Rows)
            {
                throw new MatrixDimensionException("The matrices don't have the same number of rows!" +
                                                   "Matrix A has " + matrixA.Rows + " rows and matrix B has " +
                                                   matrixB.Rows + " rows.");
            }
            if (matrixA.Columns != matrixB.Columns)
            {
                throw new MatrixDimensionException("The matrices don't have the same number of columns!" +
                                                   "Matrix A has " + matrixA.Columns + " columns and matrix B has " +
                                                   matrixB.Columns + " columns.");
            }
        }

        /// <summary>
        /// Checks if the <code>dimension</code> parameter is at least 1.
        /// </summary>
        /// <exception cref="MatrixDimensionException">
        /// Is thrown if the <code>dimension</code> parameter is less than 1.
        /// </exception>
        /// <param name="dimension">Matrix dimension</param>
        private static void CheckDimension(int dimension)
        {
            if (dimension <= 0)
            {
                throw new MatrixDimensionException("Matrix dimension has to be at least 1");
            }
        }

        /// <summary>
        /// Checks if the specified <code>row</code> and <code>column</code> parameters are possible for this 
        /// <see cref="Matrix"/>.
        /// </summary>
        /// <exception cref="MatrixDimensionException">
        /// Is thrown if either the <code>row</code> and <code>column</code> parameters are outside the <see cref="Matrix"/>
        /// row and column bounds.
        /// </exception>
        /// <param name="row">Row position</param>
        /// <param name="column">Column position</param>
        private void CheckMatrixBounds(int row, int column)
        {
            if ((row >= Rows) || (row < 0))
            {
                throw new MatrixDimensionException("Specified row is out of matrix bounds! Specified row = " + (row + 1)
                                                   + ", matrix rows = " + Rows);
            }
            if ((column >= Columns) || (column < 0))
            {
                throw new MatrixDimensionException("Specified column is out of matrix bounds! Specified column = "
                                                   + (column + 1) + ", matrix columns = " + Columns);
            }
        }

        private void IsVector(Matrix vector)
        {
            if (vector.Columns != 1)
            {
                throw new MatrixDimensionException("The provided Matrix has to be a vector but isn't");
            }
        }

        private void CheckRowSize(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.Rows != matrixB.Rows)
            {
                throw new MatrixDimensionException("Matrices don't have the same row dimension");
            }
        }

        #endregion

        #region Print Methods

        /// <summary>
        /// Writes <code>this</code> <see cref="Matrix"/> to the file specified by the parameter 
        /// <code>fileName</code>.
        /// </summary>
        /// <param name="fileName">File name</param>
        public void WriteToFile(string fileName)
        {
            File.WriteAllText(fileName, ToString());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var row = 0; row < Rows; row++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    sb.Append(GetMatrixElement(row, column) + "\t");
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        private void PrintSubstitutionResults(Matrix vectorB)
        {
            var vectorY = ForwardSubstitution(vectorB);
            Console.WriteLine("Vector y");
            Console.WriteLine(vectorY);
            Console.WriteLine();

            var vectorX = BackSubstitution(vectorY);
            Console.WriteLine("Vector x");
            Console.WriteLine(vectorX);
        }

        private void PrintLuMatrices()
        {
            Console.WriteLine("L matrix");
            Console.WriteLine(LMatrix);
            Console.WriteLine();
            Console.WriteLine("U matrix");
            Console.WriteLine(UMatrix);
            Console.WriteLine();
        }

        #endregion
    }
}