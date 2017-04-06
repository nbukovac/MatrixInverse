using System;

namespace MatrixInverse.Exceptions
{
    public class MatrixDimensionException : Exception
    {
        public MatrixDimensionException() : base("Matrices aren't the same dimensions")
        {
        }

        public MatrixDimensionException(string message) : base(message)
        {
        }
    }
}