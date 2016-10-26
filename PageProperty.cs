using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfk
{
    class PageProperty
    {
        private const Int64 _A4_LONG = 842;
        private const Int64 _A4_SHORT = 595;
        private const Int64 _RESOLUTION = 10000;

        public Int64 A4_LONG
        {
            get {
                return _A4_LONG;
            }
        }

        public Int64 A4_SHORT
        {
            get {
                return _A4_SHORT;
            }
        }

        public Int64 RESOLUTION
        {
            get
            {
                return _RESOLUTION;
            }
        }

        private Int64 m_page_height;
        private Int64 m_page_width;

        public PageProperty(Int64 width, Int64 height)
        {
            m_page_width = width;
            m_page_height = height;
        }

        public PageProperty()
        {
            m_page_width = _A4_SHORT;
            m_page_height = _A4_LONG;
        }

        public void reset(Int64 width, Int64 height)
        {
            m_page_height = height;
            m_page_width = width;
        }

        public Int64 W_P2R(Int64 percent)
        {
            return m_page_width * percent / _RESOLUTION;
        }

        public Int64 H_P2R(Int64 percent)
        {
            return m_page_height * percent / _RESOLUTION;
        }

        public double W_P2R_d(Int64 percent)
        {
            return (double)m_page_width * (double)percent / (double)_RESOLUTION;
        }

        public double H_P2R_d(Int64 percent)
        {
            return (double)m_page_height * (double)percent / (double)_RESOLUTION;
        }

        public Int64 W_R2P(Int64 real)
        {
            return real * _RESOLUTION / m_page_width;
        }

        public Int64 H_R2P(Int64 real)
        {
            return real * _RESOLUTION / m_page_height;
        }
    }
}
