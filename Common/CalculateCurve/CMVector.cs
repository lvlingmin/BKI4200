namespace BioBaseCLIA.CalculateCurve
{
    using System;
    using System.Collections.Generic;

    public class CMVector
    {
        private List<double> m_vector;

        private CMVector()
        {
        }

        public CMVector(int n)
        {
            this.m_vector = new List<double>(n);
            for (int i = 0; i < n; i++)
            {
                this.m_vector.Add(0.0);
            }
        }

        public void AddElement(double val)
        {
            this.m_vector.Add(val);
        }

        public CMVector Clone()
        {
            return new CMVector { m_vector = new List<double>(this.m_vector) };
        }

        public bool Compare(CMVector rVec)
        {
            if (this.m_vector.Count != rVec.m_vector.Count)
            {
                return false;
            }
            for (int i = 0; i < this.m_vector.Count; i++)
            {
                if (this.m_vector[i] != rVec.m_vector[i])
                {
                    return false;
                }
            }
            return true;
        }

        public double this[int index]
        {
            get
            {
                return m_vector[index];
            }
            set { m_vector[index] = value; }
        }

        public double GetAt(int idx)
        {
            if ((idx < 0) || (idx >= this.m_vector.Count))
            {
                throw new IndexOutOfRangeException("Index is out of range of CMVector");
            }
            return this.m_vector[idx];
        }

        public bool IsEmpty()
        {
            if (this.m_vector.Count != 0)
            {
                return false;
            }
            return true;
        }

        public void SetAt(int idx, double val)
        {
            if ((idx < 0) || (idx >= this.m_vector.Count))
            {
                throw new IndexOutOfRangeException("Index is out of range of CMVector");
            }
            this.m_vector[idx] = val;
        }

        public int Count
        {
            get
            {
                return this.m_vector.Count;
            }
        }
    }
}

