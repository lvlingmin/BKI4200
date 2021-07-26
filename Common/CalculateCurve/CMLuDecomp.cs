namespace BioBaseCLIA.CalculateCurve
{
    using System;

    internal class CMLuDecomp
    {
        private int m_col;
        private CMMatrix m_inverse;
        private CMMatrix m_matrix;
        private CMVector m_pivot;
        private int m_row;
        private CMVector m_rvector;
        private const double TINY = 1E-20;

        public CMLuDecomp(CMMatrix inMat, CMVector rvector)
        {
            this.m_matrix = inMat;
            this.m_row = inMat.RowCount;
            this.m_col = inMat.ColumnCount;
            this.m_rvector = rvector;
            this.m_pivot = null;
            this.m_inverse = null;
        }

        public bool Decompose()
        {
            CMVector vec = new CMVector(this.m_row);
            this.m_pivot = new CMVector(this.m_col);
            int parity = 1;
            int maxidx = 0;
            try
            {
                if (this.IsSingular(this.m_matrix, vec))
                {
                    return false;
                }
                for (int i = 0; i < this.m_col; i++)
                {
                    this.SolveUpperEntry(this.m_matrix, i);
                    this.SolveLowerEntry(this.m_matrix, i);
                    this.LargestPivot(this.m_matrix, vec, i, ref maxidx);
                    if (i != maxidx)
                    {
                        this.SwitchRows(i, maxidx, this.m_matrix, vec, ref parity);
                    }
                    this.m_pivot.SetAt(i, (double)maxidx);
                    this.DivideLowerEntries(this.m_matrix, i);
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                vec = null;
            }
            return true;
        }

        public double Determinant()
        {
            return 0.0;
        }

        private void DivideLowerEntries(CMMatrix matrix, int col)
        {
            int rowid = col;
            if (matrix.GetAt(rowid, rowid) == 0.0)
            {
                matrix.SetAt(rowid, rowid, 1E-20);
            }
            if ((rowid >= 0) && (rowid < this.m_col))
            {
                double num2 = 1.0 / matrix.GetAt(rowid, rowid);
                for (int i = rowid + 1; i < this.m_row; i++)
                {
                    double val = matrix.GetAt(i, rowid) * num2;
                    matrix.SetAt(i, rowid, val);
                }
            }
        }

        public void Invert()
        {
            CMVector rvector = new CMVector(this.m_row);
            this.m_inverse = new CMMatrix(this.m_row, this.m_col);
            for (int i = 0; i < this.m_row; i++)
            {
                for (int j = 0; j < this.m_col; j++)
                {
                    rvector.SetAt(j, 0.0);
                }
                rvector.SetAt(i, 1.0);
                this.LuSubstitute(this.m_matrix, rvector);
            }
        }

        private bool IsSingular(CMMatrix matrix, CMVector vec)
        {
            for (int i = 0; i < this.m_row; i++)
            {
                double num2 = 0.0;
                for (int j = 0; j < this.m_col; j++)
                {
                    double num;
                    if ((num = Math.Abs(matrix.GetAt(i, j))) > num2)
                    {
                        num2 = num;
                    }
                }
                if (num2 == 0.0)
                {
                    return true;
                }
                vec.SetAt(i, 1.0 / num2);
            }
            return false;
        }

        private void LargestPivot(CMMatrix matrix, CMVector svec, int col, ref int maxidx)
        {
            double num = 0.0;
            for (int i = col; i < this.m_row; i++)
            {
                double num4 = svec.GetAt(i) * Math.Abs(matrix.GetAt(i, col));
                if (num4 > num)
                {
                    num = num4;
                    maxidx = i;
                }
            }
        }

        public void LuSubstitute(CMMatrix matrix, CMVector rvector)
        {
            double at;
            int num2 = -1;
            for (int i = 0; i < this.m_row; i++)
            {
                int idx = Convert.ToInt32(this.m_pivot.GetAt(i));
                at = rvector.GetAt(idx);
                rvector.SetAt(idx, rvector.GetAt(i));
                if (num2 >= 0)
                {
                    for (int k = num2; k <= (i - 1); k++)
                    {
                        at -= matrix.GetAt(i, k) * rvector.GetAt(k);
                    }
                }
                else if (!at.Equals((double)0.0))
                {
                    num2 = i;
                }
                rvector.SetAt(i, at);
            }
            for (int j = this.m_row - 1; j >= 0; j--)
            {
                at = rvector.GetAt(j);
                for (int m = j + 1; m < this.m_col; m++)
                {
                    at -= matrix.GetAt(j, m) * rvector.GetAt(m);
                }
                rvector.SetAt(j, at / matrix.GetAt(j, j));
            }
        }

        public bool Solve()
        {
            if (this.Decompose())
            {
                this.LuSubstitute(this.m_matrix, this.m_rvector);
                this.Invert();
                return true;
            }
            return false;
        }

        private void SolveLowerEntry(CMMatrix matrix, int col)
        {
            for (int i = col; i < this.m_row; i++)
            {
                double at = matrix.GetAt(i, col);
                for (int j = 0; j < col; j++)
                {
                    at -= matrix.GetAt(i, j) * matrix.GetAt(j, col);
                }
                matrix.SetAt(i, col, at);
            }
        }

        private void SolveUpperEntry(CMMatrix matrix, int col)
        {
            for (int i = 0; i < col; i++)
            {
                double at = matrix.GetAt(i, col);
                for (int j = 0; j < i; j++)
                {
                    at -= matrix.GetAt(i, j) * matrix.GetAt(j, col);
                }
                matrix.SetAt(i, col, at);
            }
        }

        private void SwitchRows(int cur_row, int maxidx, CMMatrix matrix, CMVector svec, ref int parity)
        {
            for (int i = 0; i < this.m_col; i++)
            {
                double at = matrix.GetAt(maxidx, i);
                matrix.SetAt(maxidx, i, matrix.GetAt(cur_row, i));
                matrix.SetAt(cur_row, i, at);
            }
            parity = -parity;
            svec.SetAt(maxidx, svec.GetAt(cur_row));
        }

        public CMMatrix InverseMatrix
        {
            get
            {
                return this.m_inverse;
            }
        }

        public CMMatrix Matrix
        {
            get
            {
                return this.m_matrix;
            }
        }

        public CMVector PivotVector
        {
            get
            {
                return this.m_pivot;
            }
        }
    }
}

