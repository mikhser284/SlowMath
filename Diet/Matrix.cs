
namespace SlowMath
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // https://people.richland.edu/james/lecture/m116/matrices/determinant.html

    /// <summary> Состояние </summary>
    public partial class Matrix
    {
        private Double[][] _matrix;

        /// <summary> Элемент матрицы находящийся на пересечении указанной строки и столбца </summary>
        /// <param name="r"> Индекс строки </param>
        /// <param name="c"> Индекс столбца </param>
        /// <returns> Значение указанного элемента </returns>
        public Double this[Int32 r, Int32 c]
        {
            get => _matrix[r][c];
            set => _matrix[r][c] = value;
        }

        /// <summary> Количество строк матрицы </summary>
        public Int32 RowsCount { get => _matrix.GetLength(0); }

        public List<Matrix> Rows
        {
            get
            {
                List<Matrix> rows = new List<Matrix>();
                for(Int32 r = 0; r < RowsCount; r++) rows.Add(GetRow(r));
                return rows;
            }
        }

        public List<Matrix> Columns
        {
            get
            {
                List<Matrix> columns = new List<Matrix>();
                for(Int32 c = 0; c < ColumnsCount; c++) columns.Add(GetColumn(c));
                return columns;
            }
        }

        /// <summary> Получить строку матрицы </summary>
        /// <param name="rowIndex"> Индекс строки </param>
        /// <returns> Матрица содержащая указанную строку </returns>
        public Matrix GetRow(Int32 rowIndex)
        {
            Double[][] row = new Double[1][] { new Double[ColumnsCount] };
            for(int c = 0; c < ColumnsCount; c++) row[0][c] = this[rowIndex, c];
            return new Matrix(row);
        }

        /// <summary> Количество столбцов матрицы </summary>
        public Int32 ColumnsCount { get => _matrix[0].GetLength(0); }

        /// <summary> Получить столбец матрицы </summary>
        /// <param name="columnIndex"> Индекс столбца </param>
        /// <returns> Мотрица содержащая указанный столбец </returns>
        public Matrix GetColumn(Int32 columnIndex)
        {
            Double[][] column = new double[RowsCount][];
            for(int r = 0; r < RowsCount; r++) column[r] = new Double[] { this[r, columnIndex] };
            return new Matrix(column);
        }

        /// <summary> Является ли матрица квадратной </summary>
        public Boolean IsSquareMatrix { get => RowsCount == ColumnsCount; }

        public Boolean RankIsEqual(Int32 rank) => RowsCount == rank && ColumnsCount == rank;

        public Int32? Rank { get => IsSquareMatrix == false ? null : (Int32?)RowsCount; }

        public override string ToString()
        {
            List<String> rows = new List<string>();
            for(int r = 0; r < RowsCount; r++)
                rows.Add($"{{ {String.Join(", ", _matrix[r].Select(i => $"{i:0.00}"))} }}");
            return $" {String.Join(", ", rows)} ";
        }

        /// <summary> Вывести матрицу на консоль </summary>
        /// <param name="description"> Примечания </param>
        /// <returns> Исходная матрица </returns>
        public Matrix View(Boolean showNulls = false, String description = default(String))
        {
            const Int32 itemWidth = 8;
            String view = String.Empty;
            for(int r = 0; r < RowsCount; r++)
            {
                String row = $" │ {String.Join(", ", _matrix[r].Select(i => i == 0.0 && !showNulls ? "-".PadLeft(itemWidth) : $"{i,itemWidth:0.00}"))} │\n";
                if(r == 0) view += $" ┌{new String(' ', row.Length - 4)}┐\n";
                view += row;
                if(r + 1 == RowsCount) view += $" └{new String(' ', row.Length - 4)}┘";
            }
            Console.WriteLine($"\n {description}\n{view}\n");
            return this;
        }

        public Matrix View(String description) => View(false, description);


    }

    /// <summary> Инициализация </summary>
    public partial class Matrix
    {
        /// <summary> Нулевая квадратная матрица </summary>
        /// <param name="order"> Порядок матрицы </param>
        public Matrix(Int32 order) : this(order, order) { }

        /// <summary> Квадратная матрица </summary>
        /// <param name="matrix"> Массив чисел определяющих матрицу </param>
        public Matrix(Double[] matrix)
        {
            _matrix = CreateMatrixArray(matrix.GetLength(0), 1);
            for(Int32 r = 0; r < RowsCount; r++) for(Int32 c = 0; c < ColumnsCount; c++) this[r, c] = matrix[r];
        }

        /// <summary> Нулевая прямоугольная матрица </summary>
        /// <param name="rowsCount"> Количество строк матрицы </param>
        /// <param name="columnsCount"> Количество столбцов матрицы </param>
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
            for(Int32 r = 0; r < RowsCount; r++) for(Int32 c = 0; c < ColumnsCount; c++) this[r, c] = value(r, c);
        }

        public Matrix(Int32 rank, Func<Int32, Int32, Double> value) : this(rank, rank, value) { }

        public Matrix(Int32 rowsCount, Int32 columnsCount, Double valuesRowCol) : this(rowsCount, columnsCount, (r, c) => valuesRowCol) { }

        public Matrix(Int32 rank, Double valuesRowCol) : this(rank, rank, (r, c) => valuesRowCol) { }

        /// <summary> Прямоугольная матрица </summary>
        /// <param name="matrix"> Массив чисел определяющих матрицу </param>
        public Matrix(Double[,] matrix)
        {
            _matrix = CreateMatrixArray(matrix.GetLength(0), matrix.GetLength(1));
            for(Int32 r = 0; r < RowsCount; r++) for(Int32 c = 0; c < ColumnsCount; c++) this[r, c] = matrix[r, c];
        }

        private Matrix(Double[][] matrix)
        {
            _matrix = CreateMatrixArray(matrix.GetLength(0), matrix[0].GetLength(0));
            for(Int32 r = 0; r < RowsCount; r++) for(Int32 c = 0; c < ColumnsCount; c++) this[r, c] = matrix[r][c];
        }

        /// <summary> Матрица заполненная случайными числами </summary>
        /// <param name="rowsCount"> Количество строк матрицы </param>
        /// <param name="columnsCount"> Количество столбцов матрицы </param>
        /// <param name="min"> Минимальное число, которое может быть присвоено элементу матрицы (по умолчанию 0) </param>
        /// <param name="max"> Максимальное число, которое может быть присвоено элементу матрицы (по умолчанию < 100) </param>
        /// <returns> Случайная матрица </returns>
        public static Matrix Random(Int32 rowsCount, Int32 columnsCount, Int32 min = 0, Int32 max = 100)
        {
            Random rnd = new Random();
            Matrix m = new Matrix(rowsCount, columnsCount);
            for(Int32 r = 0; r < m.RowsCount; r++) for(Int32 c = 0; c < m.ColumnsCount; c++) m[r, c] = rnd.NextDouble() + rnd.Next(min, max);
            return m;
        }

        /// <summary> Получить единичную матрицу указанного порядка </summary>
        /// <param name="order"> Порядок матрицы </param>
        /// <returns> Единичная матрица указанного порядка </returns>
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

        /// <summary> Получить копию матрицы </summary>
        /// <returns> Копия матрицы </returns>
        public Matrix Clone()
        {
            Matrix clone = new Matrix(RowsCount, ColumnsCount);
            for(Int32 r = 0; r < RowsCount; r++) for(Int32 c = 0; c < ColumnsCount; c++) clone[r, c] = this[r, c];
            return clone;
        }
    }


    /// <summary> Операции </summary>
    public partial class Matrix
    {
        /// <summary> Транспонировать матрицу </summary>
        /// <returns> Транспонированная матрица </returns>
        public Matrix Transpose()
        {
            Matrix m = new Matrix(ColumnsCount, RowsCount);
            for (Int32 r = 0; r < RowsCount; r++) for (Int32 c = 0; c < ColumnsCount; c++) m[c, r] = this[r, c];
            return m;
        }

        public static Matrix operator -(Matrix a)
        {
            Matrix m = new Matrix(a.RowsCount, a.ColumnsCount);
            for (int r = 0; r < m.RowsCount; r++) for (int c = 0; c < m.ColumnsCount; c++) m[r, c] = -a[r, c];
            return m;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix m = new Matrix(b.RowsCount, b.ColumnsCount);
            for (int r = 0; r < m.RowsCount; r++) for (int c = 0; c < m.ColumnsCount; c++) m[r, c] = a[r, c] + b[r, c];
            return m;
        }

        public static Matrix operator *(Double a, Matrix b)
        {
            Matrix m = new Matrix(b.RowsCount, b.ColumnsCount);
            for (int r = 0; r < m.RowsCount; r++) for (int c = 0; c < m.ColumnsCount; c++) m[r, c] = a * b[r, c];
            return m;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (b.RowsCount != a.ColumnsCount) throw new ArgumentException($"Количество строк матрицы B должно соответствовать количеству столбцов матрицы A (т.е. {a.ColumnsCount})");
            Matrix m = new Matrix(b.RowsCount, b.ColumnsCount);
            for (int r = 0; r < m.RowsCount; r++)
                for (int c = 0; c < m.ColumnsCount; c++)
                    for (int i = 0; i < b.RowsCount; i++)
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

    /// <summary> В разработке </summary>
    public partial class Matrix
    {
        

        [Obsolete]
        public Int32? FindCheapestRowOrCol(out VectorOrientation matrixOrientation)
        {
            Int32[] nullsByCols = new Int32[ColumnsCount];
            Int32 maxNullsInSingleCol = 0;
            Int32? colWithMaxNullsIndex = null;
            for(Int32 c = 0; c < ColumnsCount; c++)
            {
                for(Int32 r = 0; r < RowsCount; r++) nullsByCols[c] += this[r, c] == 0 ? 1 : 0;
                if(nullsByCols[c] > maxNullsInSingleCol)
                {
                    maxNullsInSingleCol = nullsByCols[c];
                    colWithMaxNullsIndex = c;
                    if(maxNullsInSingleCol == ColumnsCount)
                    {
                        matrixOrientation = VectorOrientation.Column;
                        return null;
                    }
                }
            }

            Int32[] nullsByRows = new Int32[RowsCount];
            Int32 maxNullsInSingleRow = 0;
            Int32? rowWithMaxNullsIndex = null;
            for(Int32 r = 0; r < RowsCount; r++)
            {
                for(Int32 c = 0; c < ColumnsCount; c++) nullsByRows[r] += this[r, c] == 0 ? 1 : 0;
                if(nullsByRows[r] > maxNullsInSingleRow)
                {
                    maxNullsInSingleRow = nullsByRows[r];
                    rowWithMaxNullsIndex = r;
                    if(maxNullsInSingleRow == RowsCount)
                    {
                        matrixOrientation = VectorOrientation.Row;
                        return null;
                    }
                }
            }

            if(maxNullsInSingleCol >= maxNullsInSingleRow)
            {
                matrixOrientation = VectorOrientation.Column;
                return colWithMaxNullsIndex ?? 0;
            }

            if(maxNullsInSingleRow >= maxNullsInSingleCol)
            {
                matrixOrientation = VectorOrientation.Row;
                return rowWithMaxNullsIndex ?? 0;
            }

            matrixOrientation = VectorOrientation.Column;
            return 0;
        }

        private static Double Det2x2(Matrix m)
        {
            return m[0, 0] * m[1, 1] - m[1, 0] * m[0, 1];
        }

        public Double Determinant()
        {
            if(RankIsEqual(2)) return Det2x2(this);

            VectorOrientation matrixOrientation;
            Int32? index = FindCheapestRowOrCol(out matrixOrientation);
            Double determinant = 0;
            if(index == null) return determinant;

            if(matrixOrientation == VectorOrientation.Column)
            {
                Matrix minors = GetRowOrColMinors(this, matrixOrientation, (Int32)index);
                Matrix cofactors = GetRowOrColCofactors(minors, matrixOrientation, (Int32)index);
                for(Int32 r = 0; r < RowsCount; r++)
                    determinant += this[r, (Int32)index] * cofactors[r, 0];
            }
            else if(matrixOrientation == VectorOrientation.Row)
            {
                Matrix minors = GetRowOrColMinors(this, matrixOrientation, (Int32)index);
                Matrix cofactors = GetRowOrColCofactors(minors, matrixOrientation, (Int32)index);
                for(Int32 c = 0; c < ColumnsCount; c++)
                    determinant += this[(Int32)index, c] * cofactors[0, c];
            }
            
            return determinant;
        }

        

        public static Matrix GetRowOrColMinors(Matrix matrix, VectorOrientation vectorOrientation, Int32 index)
        {
            Matrix minors = null;
            if(vectorOrientation == VectorOrientation.Column)
            {
                minors = new Matrix(matrix.RowsCount, 1);
                for(Int32 r = 0; r < matrix.RowsCount; r++)
                {
                    Matrix subMatrix = matrix.GetSubMatrix(r, index);
                    if(matrix[r, index] == 0)
                        minors[r, 0] = 0;
                    else if(subMatrix.RankIsEqual(2))
                        minors[r, 0] = Det2x2(subMatrix);
                    else
                        minors[r, 0] = matrix[r, index] * subMatrix.Determinant();
                }
            }
            else if(vectorOrientation == VectorOrientation.Row)
            {
                minors = new Matrix(1, matrix.ColumnsCount);
                for(Int32 c = 0; c < matrix.ColumnsCount; c++)
                {
                    Matrix subMatrix = matrix.GetSubMatrix(index, c);
                    if(matrix[index, c] == 0)
                        minors[0, c] = 0;
                    else if(subMatrix.RankIsEqual(2))
                        minors[0, c] = Det2x2(subMatrix);
                    else
                        minors[0, c] = matrix[index, c] * subMatrix.Determinant();
                }
            }

            return minors;
        }

        public static Matrix GetRowOrColCofactors(Matrix minors, VectorOrientation vectorOrientation, Int32 index)
        {
            Matrix cofactors = minors.Clone();
            if(vectorOrientation == VectorOrientation.Column)
                for(Int32 r = 0; r < minors.RowsCount; r++)
                    cofactors[r, 0] *= (IsChangableItem(r, index) ? -1 : 1);
            else if(vectorOrientation == VectorOrientation.Row)
                for(Int32 c = 0; c < minors.ColumnsCount; c++)
                    cofactors[0, c] *= (IsChangableItem(index, c) ? -1 : 1);

            return cofactors;
            static Boolean IsChangableItem(Int32 rowIndex, Int32 colIndex) => (rowIndex % 2 == 1) != (colIndex % 2 == 1);
        }

        private Matrix GetSubMatrix(Int32 excludableRowIndex, Int32 excludableColumnIndex)
        {
            Matrix lowerMatrix = new Matrix(RowsCount - 1, ColumnsCount - 1);
            for(int r = 0; r < RowsCount; r++)
            {
                if(r == excludableRowIndex) continue;
                for(int c = 0; c < ColumnsCount; c++)
                {
                    if(c == excludableColumnIndex) continue;
                    lowerMatrix[RowA(r), ColA(c)] = this[r, c];
                }
            }
            return lowerMatrix;
            Int32 RowA(Int32 rowB) => rowB > excludableRowIndex ? rowB - 1 : rowB;
            Int32 ColA(Int32 colB) => colB > excludableColumnIndex ? colB - 1 : colB;
        }

        // ----------------------------------------------

        private Matrix FindCheapestVector(out Int32 indexInOriginalMatrix)
        {
            Int32[] nullsByCols = new Int32[ColumnsCount];
            Int32 maxNullsInSingleCol = 0;
            Int32? colWithMaxNullsIndex = null;
            for(Int32 c = 0; c < ColumnsCount; c++)
            {
                for(Int32 r = 0; r < RowsCount; r++) nullsByCols[c] += this[r, c] == 0 ? 1 : 0;
                if(nullsByCols[c] > maxNullsInSingleCol)
                {
                    maxNullsInSingleCol = nullsByCols[c];
                    colWithMaxNullsIndex = c;
                }
            }

            Int32[] nullsByRows = new Int32[RowsCount];
            Int32 maxNullsInSingleRow = 0;
            Int32? rowWithMaxNullsIndex = null;
            for(Int32 r = 0; r < RowsCount; r++)
            {
                for(Int32 c = 0; c < ColumnsCount; c++) nullsByRows[r] += this[r, c] == 0 ? 1 : 0;
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

        public static Matrix GetCofactorsVector(Matrix matrix, VectorOrientation vectorOrientation, out Int32 indexInOriginalMatrix)
        {
            Matrix minors = GetMinorsVector(matrix, matrix.FindCheapestVector(out indexInOriginalMatrix), indexInOriginalMatrix);
            //Matrix cofactors = new Matrix(matrix.RowsCount, matrix.ColumnsCount, (r, c) => ((r % 2 == 1) != (c % 2 == 1)) ? -1 : 1);



            throw new NotImplementedException();
        }

        private static Matrix GetMinorsVector(Matrix matrix, Matrix cheapestVector, Int32 index)
        {
            throw new NotImplementedException();
        }

        /// <summary> Получить обратную матрицу </summary>
        /// <returns> Обратная матрица </returns>
        public Matrix Reverse()
        {
            if(!IsSquareMatrix) throw new InvalidOperationException("Обратная матрица может быть получена только для квадратных матриц.");
            throw new NotImplementedException("Operation not implemented: 'Matrix;  public Matrix Reverse()'");
        }
    }

    /// <summary> Временные </summary>
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
            Matrix cofactors = new Matrix(x.RowsCount, x.ColumnsCount);
            for(int r = 0; r < x.RowsCount; r++)
                for(int c = 0; c < x.ColumnsCount; c++)
                    cofactors[r, c] = IsChangableItem(r, c) ? -x[r, c] : x[r, c];
            return cofactors;

            static Boolean IsChangableItem(Int32 rowIndex, Int32 colIndex) => (rowIndex % 2 == 1) != (colIndex % 2 == 1);
        }
    }
}