using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BioBaseCLIA.CalculateCurve
{
    

    public class CMMatrix
    {
        private int m_col;
        private List<CMVector> m_matrix;
        private int m_row;

        private CMMatrix()
        {
            this.m_row = 0;
            this.m_col = 0;
            this.m_matrix = new List<CMVector>();
        }

        public CMMatrix(int n, int m)
        {
            this.m_row = n;
            this.m_col = m;
            this.CreateMatrix(n, m);
        }

        public void AddRowVector(CMVector rvec)
        {
            if (!this.m_matrix.Contains(rvec))
            {
                this.m_matrix.Add(rvec);
                this.m_col = rvec.Count;
                this.m_row++;
            }
        }

        public CMMatrix Clone()
        {
            CMMatrix matrix = new CMMatrix();
            for (int i = 0; i < this.m_row; i++)
            {
                CMVector item = this.m_matrix[i].Clone();
                matrix.m_matrix.Add(item);
            }
            matrix.m_row = this.m_row;
            matrix.m_col = this.m_col;
            return matrix;
        }

        public bool Compare(CMMatrix rmat)
        {
            if ((this.m_row != rmat.m_row) || (this.m_col != rmat.m_col))
            {
                return false;
            }
            for (int i = 0; i < rmat.m_row; i++)
            {
                CMVector vector = this.m_matrix[i];
                CMVector rVec = rmat.m_matrix[i];
                if (!vector.Compare(rVec))
                {
                    return false;
                }
            }
            return true;
        }

        private void CreateMatrix(int n, int m)
        {
            this.m_matrix = null;
            this.m_matrix = new List<CMVector>(n);
            for (int i = 0; i < n; i++)
            {
                CMVector item = new CMVector(m);
                for (int j = 0; j < m; j++)
                {
                    item.AddElement(0.0);
                }
                this.m_matrix.Add(item);
            }
            this.m_row = n;
            this.m_col = m;
        }

        public double GetAt(int rowid, int colid)
        {
            if (((rowid >= 0) && (rowid < this.m_row)) && ((colid >= 0) && (colid < this.m_col)))
            {
                CMVector vector = this.m_matrix[rowid];
                if (colid < vector.Count)
                {
                    return vector.GetAt(colid);
                }
            }
            throw new IndexOutOfRangeException("Index is out of range of CMMatrix");
        }

        public void SetAt(int rowid, int colid, double val)
        {
            if (((rowid >= 0) && (rowid < this.m_row)) && ((colid >= 0) && (colid < this.m_col)))
            {
                CMVector vector = this.m_matrix[rowid];
                if (colid < vector.Count)
                {
                    vector.SetAt(colid, val);
                    return;
                }
            }
            throw new IndexOutOfRangeException("Index is out of range of CMMatrix");
        }

        public int ColumnCount
        {
            get
            {
                return this.m_col;
            }
        }

        public int RowCount
        {
            get
            {
                return this.m_row;
            }
        }
    }
}

