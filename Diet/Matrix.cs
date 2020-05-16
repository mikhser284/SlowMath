
namespace SlowMath
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;

    // https://people.richland.edu/james/lecture/m116/matrices/determinant.html

    /// <summary> Состояние </summary>
    public partial class Matrix
    {
        private Double[][] _matrix;

        public Double this[Int32 r, Int32 c]
        {
            get => _matrix[r][c];
            set => _matrix[r][c] = value;
        }

        public Int32 CountOfRows { get => _matrix.GetLength(0); }

        public List<Matrix> Rows
        {
            get
            {
                List<Matrix> rows = new List<Matrix>();
                for(Int32 r = 0; r < CountOfRows; r++) rows.Add(GetRow(r));
                return rows;
            }
        }

        public List<Matrix> Columns
        {
            get
            {
                List<Matrix> columns = new List<Matrix>();
                for(Int32 c = 0; c < CountOfColumns; c++) columns.Add(GetColumn(c));
                return columns;
            }
        }

        public Matrix GetRow(Int32 rowIndex)
        {
            Double[][] row = new Double[1][] { new Double[CountOfColumns] };
            for(int c = 0; c < CountOfColumns; c++) row[0][c] = this[rowIndex, c];
            return new Matrix(row);
        }

        public Int32 CountOfColumns { get => _matrix[0].GetLength(0); }

        public Matrix GetColumn(Int32 columnIndex)
        {
            Double[][] column = new double[CountOfRows][];
            for(int r = 0; r < CountOfRows; r++) column[r] = new Double[] { this[r, columnIndex] };
            return new Matrix(column);
        }

        public Boolean IsSquareMatrix
        {
            get => CountOfRows == CountOfColumns; 
        }

        public Int32? Rank { get => IsSquareMatrix == false ? null : (Int32?)CountOfRows; }

        public override string ToString()
        {
            List<String> rows = new List<string>();
            for(int r = 0; r < CountOfRows; r++)
                rows.Add($"{{ {String.Join(", ", _matrix[r].Select(i => $"{i:0.00}"))} }}");
            return $" {String.Join(", ", rows)} ";
        }

        public Matrix View(Int32 digits = 4, Int32 precision = 2, Boolean showNulls = false, String description = default(String))
        {
            Console.WriteLine(GetViewString(digits, precision, showNulls, description));
            return this;
        }

        public Matrix View(String description, Int32 digits = 4, Int32 precision = 2) => View(digits, precision, false, description);

        String GetViewString(Int32 digits = 4, Int32 precision = 2, Boolean showNulls = false, String description = default(String))
        {
            Int32 digitsAfterPoint = precision < 0 ? 0 : precision;
            Int32 itemWidth = digits + 1 + digitsAfterPoint;
            String itemFormatString = $"{{0,{itemWidth}:0.{new String('0', digitsAfterPoint)}}}";

            String view = String.Empty;

            for (int r = 0; r < CountOfRows; r++)
            {
                String row = $" │ {String.Join(", ", _matrix[r].Select(i => Math.Abs(i) < 0.000_000_000_000_01 && !showNulls ? "∙".PadLeft(itemWidth) : String.Format(itemFormatString, i)))} │\n";
                if (r == 0) view += $" ┌{new String(' ', row.Length - 4)}┐\n";
                view += row;
                if (r + 1 == CountOfRows) view += $" └{new String(' ', row.Length - 4)}┘";
            }
            return $"\n {description}\n{view}\n";
        }

        public Boolean IsRowVector
        {
            get =>  CountOfRows == 1 && CountOfColumns > 1;
        }

        public Boolean IsColumnVector
        {
            get => CountOfRows > 1 && CountOfColumns == 1;
        }

        public Double SumOfMatrixItems()
        {
            Double sum = 0;
            for (Int32 r = 0; r < CountOfRows; r++)
                for (Int32 c = 0; c < CountOfColumns; c++)
                    sum += this[r, c];
            return sum;
        }
    }

    /// <summary> Инициализация </summary>
    public partial class Matrix
    {
        public Matrix(Int32 order) : this(order, order) { }

        public Matrix(Double[] matrix)
        {
            _matrix = CreateMatrixArray(matrix.GetLength(0), 1);
            for(Int32 r = 0; r < CountOfRows; r++) for(Int32 c = 0; c < CountOfColumns; c++) this[r, c] = matrix[r];
        }

        public Matrix(Int32 rowsCount, Int32 columnsCount)
        {
            if(rowsCount < 1) throw new ArgumentException("Количество столбцов матрицы не может быть меньше 1");
            if(columnsCount < 1) throw new ArgumentException("Количество строк матрицы не может быть меньше 1");
            _matrix = CreateMatrixArray(rowsCount, columnsCount);
        }

        public Matrix(Int32 rowsCount, Int32 columnsCount, Func<Int32, Int32, Double> value)
        {
            if(rowsCount < 1) throw new ArgumentException("Количество столбцов матрицы не может быть меньше 1");
            if(columnsCount < 1) throw new ArgumentException("Количество строк матрицы не может быть меньше 1");
            _matrix = CreateMatrixArray(rowsCount, columnsCount);
            for(Int32 r = 0; r < CountOfRows; r++) for(Int32 c = 0; c < CountOfColumns; c++) this[r, c] = value(r, c);
        }

        public Matrix(Int32 rank, Func<Int32, Int32, Double> value)
            : this(rank, rank, value) { }

        public Matrix(Int32 rowsCount, Int32 columnsCount, Double valuesRowCol)
            : this(rowsCount, columnsCount, (r, c) => valuesRowCol) { }

        public Matrix(Int32 rank, Double valuesRowCol)
            : this(rank, rank, (r, c) => valuesRowCol) { }

        public Matrix(Double[,] matrix)
        {
            _matrix = CreateMatrixArray(matrix.GetLength(0), matrix.GetLength(1));
            for(Int32 r = 0; r < CountOfRows; r++) for(Int32 c = 0; c < CountOfColumns; c++) this[r, c] = matrix[r, c];
        }

        private Matrix(Double[][] matrix)
        {
            _matrix = CreateMatrixArray(matrix.GetLength(0), matrix[0].GetLength(0));
            for(Int32 r = 0; r < CountOfRows; r++) for(Int32 c = 0; c < CountOfColumns; c++) this[r, c] = matrix[r][c];
        }

        public static Matrix Random(Int32 rowsCount, Int32 columnsCount, Int32 min = 0, Int32 max = 100)
        {
            Random rnd = new Random();
            Matrix m = new Matrix(rowsCount, columnsCount);
            for(Int32 r = 0; r < m.CountOfRows; r++) for(Int32 c = 0; c < m.CountOfColumns; c++) m[r, c] = rnd.NextDouble() + rnd.Next(min, max);
            return m;
        }

        public static Matrix IdentityMatrix(Int32 order)
        {
            Matrix m = new Matrix(order, order);
            for(int i = 0; i < order; i++) m[i, i] = 1;
            return m;
        }

        private static Double[][] CreateMatrixArray(Int32 rowsCount, Int32 columnsCount)
        {
            Double[][] matrix = new Double[rowsCount][];
            for(int r = 0; r < rowsCount; r++) matrix[r] = new Double[columnsCount];
            return matrix;
        }

        public Matrix Clone()
        {
            Matrix clone = new Matrix(CountOfRows, CountOfColumns);
            for(Int32 r = 0; r < CountOfRows; r++) for(Int32 c = 0; c < CountOfColumns; c++) clone[r, c] = this[r, c];
            return clone;
        }

        public Matrix GetEmptyCopy()
        {
            return new Matrix(CountOfRows, CountOfColumns);
        }
    }

    /// <summary> Операции </summary>
    public partial class Matrix
    {
        /// <summary> Транспонировать матрицу </summary>
        /// <returns> Транспонированная матрица </returns>
        public Matrix Transpose()
        {
            Matrix m = new Matrix(CountOfColumns, CountOfRows);
            for (Int32 r = 0; r < CountOfRows; r++) for (Int32 c = 0; c < CountOfColumns; c++) m[c, r] = this[r, c];
            return m;
        }

        public static Matrix operator -(Matrix a)
        {
            Matrix m = new Matrix(a.CountOfRows, a.CountOfColumns);
            for (int r = 0; r < m.CountOfRows; r++) for (int c = 0; c < m.CountOfColumns; c++) m[r, c] = -a[r, c];
            return m;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix m = new Matrix(b.CountOfRows, b.CountOfColumns);
            for (int r = 0; r < m.CountOfRows; r++) for (int c = 0; c < m.CountOfColumns; c++) m[r, c] = a[r, c] + b[r, c];
            return m;
        }

        public static Matrix operator *(Double a, Matrix b)
        {
            Matrix m = new Matrix(b.CountOfRows, b.CountOfColumns);
            for (int r = 0; r < m.CountOfRows; r++) for (int c = 0; c < m.CountOfColumns; c++) m[r, c] = a * b[r, c];
            return m;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (b.CountOfRows != a.CountOfColumns) throw new ArgumentException($"Количество строк матрицы B должно соответствовать количеству столбцов матрицы A (т.е. {a.CountOfColumns})");
            Matrix m = new Matrix(b.CountOfRows, b.CountOfColumns);
            for (int r = 0; r < m.CountOfRows; r++)
                for (int c = 0; c < m.CountOfColumns; c++)
                    for (int i = 0; i < b.CountOfRows; i++)
                        m[r, c] += a[r, i] * b[i, c];
            return m;
        }
    }

    public enum VectorOrientation
    {
        Row,
        Column,
        NotFound
    }

    /// <summary> Определитель матрицы </summary>
    public partial class Matrix
    {
        public Double Det()
        {
            if (!IsSquareMatrix) throw new InvalidOperationException("Детерминант может быть получен только для квадратной матрицы.");
            if (Rank == 2) return Det2x2(this);
            
            Int32 indexInOriginalMatrix;
            Matrix cofactorsVector = GetVectorOfCofactors(this, out indexInOriginalMatrix);
            
            Double determinant = 0;

            if (cofactorsVector.IsColumnVector)
                for (Int32 r = 0; r < CountOfRows; r++)
                    determinant += this[r, (Int32)indexInOriginalMatrix] * cofactorsVector[r, 0];
            else if(cofactorsVector.IsRowVector)
                for (Int32 c = 0; c < CountOfColumns; c++)
                    determinant += this[(Int32)indexInOriginalMatrix, c] * cofactorsVector[0, c];
            
            return determinant;            
        }

        private static Double Det2x2(Matrix m)
        {
            return m[0, 0] * m[1, 1] - m[1, 0] * m[0, 1];
        }

        private Matrix GetSubMatrixExceptThisRowAndCol(Int32 excludableRowIndex, Int32 excludableColumnIndex)
        {
            Matrix subMatrix = new Matrix(CountOfRows - 1, CountOfColumns - 1);
            for(int r = 0; r < CountOfRows; r++)
            {
                if(r == excludableRowIndex) continue;
                for(int c = 0; c < CountOfColumns; c++)
                {
                    if(c == excludableColumnIndex) continue;
                    subMatrix[RowA(r), ColA(c)] = this[r, c];
                }
            }
            return subMatrix;
            Int32 RowA(Int32 rowB) => rowB > excludableRowIndex ? rowB - 1 : rowB;
            Int32 ColA(Int32 colB) => colB > excludableColumnIndex ? colB - 1 : colB;
        }

        private Matrix FindCheapestVector(out Int32 indexInOriginalMatrix)
        {
            Int32[] nullsByCols = new Int32[CountOfColumns];
            Int32 maxNullsInSingleCol = 0;
            Int32? colWithMaxNullsIndex = null;
            for(Int32 c = 0; c < CountOfColumns; c++)
            {
                for(Int32 r = 0; r < CountOfRows; r++) nullsByCols[c] += this[r, c] == 0 ? 1 : 0;
                if(nullsByCols[c] > maxNullsInSingleCol)
                {
                    maxNullsInSingleCol = nullsByCols[c];
                    colWithMaxNullsIndex = c;
                }
            }

            Int32[] nullsByRows = new Int32[CountOfRows];
            Int32 maxNullsInSingleRow = 0;
            Int32? rowWithMaxNullsIndex = null;
            for(Int32 r = 0; r < CountOfRows; r++)
            {
                for(Int32 c = 0; c < CountOfColumns; c++) nullsByRows[r] += this[r, c] == 0 ? 1 : 0;
                if(nullsByRows[r] > maxNullsInSingleRow)
                {
                    maxNullsInSingleRow = nullsByRows[r];
                    rowWithMaxNullsIndex = r;
                }
            }

            if(maxNullsInSingleRow > maxNullsInSingleCol)
            {
                indexInOriginalMatrix = rowWithMaxNullsIndex ?? 0;
                return GetRow(indexInOriginalMatrix);
            }

            indexInOriginalMatrix = colWithMaxNullsIndex ?? 0;
            return GetColumn(indexInOriginalMatrix);
        }

        private static Matrix GetVectorOfCofactors(Matrix matrix, out Int32 indexInOriginalMatrix)
        {
            Matrix cofactors = GetVectorOfMinors(matrix, matrix.FindCheapestVector(out indexInOriginalMatrix), indexInOriginalMatrix);

            if (cofactors.IsColumnVector)
                for (Int32 r = 0; r < cofactors.CountOfRows; r++)
                    cofactors[r, 0] *= GetItemMultiplicator(r, indexInOriginalMatrix);
            if (cofactors.IsRowVector)
                for (Int32 c = 0; c < cofactors.CountOfColumns; c++)
                    cofactors[0, c] *= GetItemMultiplicator(indexInOriginalMatrix, c);

            return cofactors;

        }

        private static Matrix GetVectorOfMinors(Matrix matrix, Matrix cheapestVector, Int32 indexInOriginalMatrix)
        {
            Matrix minors = cheapestVector.Clone();
            Matrix subMatrix;

            if(minors.IsColumnVector)
            {

                for(Int32 r = 0; r < minors.CountOfRows; r++)
                {
                    subMatrix = matrix.GetSubMatrixExceptThisRowAndCol(r, indexInOriginalMatrix);
                    if (matrix[r, indexInOriginalMatrix] == 0)
                        minors[r, 0] = 0;
                    else if (subMatrix.Rank == 2)
                        minors[r, 0] = Det2x2(subMatrix);
                    else
                        minors[r, 0] = subMatrix.Det();
                }
            }
            else if(minors.IsRowVector)
            {
                for(Int32 c = 0; c < minors.CountOfColumns; c++)
                {
                    subMatrix = matrix.GetSubMatrixExceptThisRowAndCol(indexInOriginalMatrix, c);
                    if (matrix[indexInOriginalMatrix, c] == 0)
                        minors[0, c] = 0;
                    else if (subMatrix.Rank == 2)
                        minors[0, c] = Det2x2(subMatrix);
                    else
                        minors[0, c] = subMatrix.Det();
                }
            }

            return minors;
            
        }

        private static Int32 GetItemMultiplicator(Int32 rowIndex, Int32 colIndex)
        {
            if ((rowIndex % 2 == 1) != (colIndex % 2 == 1)) return -1;
            return 1;
        }
    }

    /// <summary> Обратная матрица </summary>
    public partial class Matrix
    {
        private static Matrix GetMatrixOfAdjoint(Matrix matrix, out Double determinant)
        {
            Matrix adjoints = GetMatrixOfCofactors(matrix);
            
            determinant = 0;
            for (Int32 c = 0; c < matrix.CountOfColumns; c++)
                determinant += matrix[0, c] * adjoints[0, c];

            return determinant == 0 ? null : adjoints.Transpose();
        }

        private static Matrix GetMatrixOfCofactors(Matrix matrix)
        {
            Matrix cofactors = GetMatrixOfMinors(matrix);

            for (Int32 r = 0; r < cofactors.CountOfRows; r++)
                for (Int32 c = 0; c < cofactors.CountOfColumns; c++)
                    cofactors[r, c] *= GetItemMultiplicator(r, c);

            return cofactors;
        }

        private static Matrix GetMatrixOfMinors(Matrix matrix)
        {
            if (matrix.Rank == 2) return matrix;
            Matrix minors = matrix.GetEmptyCopy();

            for (Int32 r = 0; r < minors.CountOfRows; r++)
                for (Int32 c = 0; c < minors.CountOfColumns; c++)
                    //minors[r, c] = matrix[r, c] == 0 ? 0 : matrix.GetSubMatrixExceptThisRowAndCol(r, c).Det();
                    minors[r, c] = matrix.GetSubMatrixExceptThisRowAndCol(r, c).Det();

            return minors;
        }

        public Matrix Reverse(out Double determinantOfMatrix)
        {
            if (!IsSquareMatrix) throw new InvalidOperationException("Обратная матрица может быть получена только для квадратной матрицы.");
            
            Matrix adjoints = GetMatrixOfAdjoint(this, out determinantOfMatrix);
            if (determinantOfMatrix == 0.0) return null;
            for (Int32 r = 0; r < adjoints.CountOfRows; r++)
                for (Int32 c = 0; c < adjoints.CountOfColumns; c++)
                    adjoints[r, c] /= determinantOfMatrix;

            return adjoints;
        }
    }

    /// <summary> Delete me </summary>
    public partial class Matrix
    {

        public static Matrix Diagonals(Int32 order, Int32 offset = 0)
        {
            Matrix m = new Matrix(order, order);
            for(int i = 0; i < order; i++)
            {
                m[Correct(i), Correct(i + offset)] += 1;
                m[CorrectR(order - i - 1), Correct(i + offset)] += 2;
            }
            return m;
            Int32 CorrectR(Int32 initValue) => initValue < 0 ? order + initValue : initValue;
            Int32 Correct(Int32 initValue) => initValue < order ? initValue : initValue - order;
        }


        public static Matrix GetCofactors(Matrix x)
        {
            Matrix cofactors = new Matrix(x.CountOfRows, x.CountOfColumns);
            for(int r = 0; r < x.CountOfRows; r++)
                for(int c = 0; c < x.CountOfColumns; c++)
                    cofactors[r, c] = IsChangableItem(r, c) ? -x[r, c] : x[r, c];
            return cofactors;

            static Boolean IsChangableItem(Int32 rowIndex, Int32 colIndex) => (rowIndex % 2 == 1) != (colIndex % 2 == 1);
        }
    }
}