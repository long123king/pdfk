using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace pdfk
{
    class FontFactory
    {
        private Dictionary<string, BaseFont> m_base_fonts;

        public FontFactory(string fonts_dir)
        {
            m_base_fonts = new Dictionary<string, BaseFont>();

            /*
             *  Steps to use non-embedded fonts:
             *  1. Add actual font file to fonts_dir. (Or replace the dummy placeholder)
             *  2. Register font here as the commented examples.
             */
            //m_base_fonts["kaiti"] = BaseFont.CreateFont(fonts_dir + "simkai.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            //m_base_fonts["songti"] = BaseFont.CreateFont(fonts_dir + "simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            //m_base_fonts["xinsongti"] = BaseFont.CreateFont(fonts_dir + "simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            //m_base_fonts["unifont"] = BaseFont.CreateFont(fonts_dir + "unifont-8.0.01.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            m_base_fonts["courier"] = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["courier_b"] = BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["courier_i"] = BaseFont.CreateFont(BaseFont.COURIER_OBLIQUE, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["courier_bi"] = BaseFont.CreateFont(BaseFont.COURIER_BOLDOBLIQUE, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["helvetica"] = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["helvetica_b"] = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["helvetica_i"] = BaseFont.CreateFont(BaseFont.HELVETICA_OBLIQUE, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["helvetica_bi"] = BaseFont.CreateFont(BaseFont.HELVETICA_BOLDOBLIQUE, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["times"] = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["times_b"] = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["times_i"] = BaseFont.CreateFont(BaseFont.TIMES_ITALIC, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
            m_base_fonts["times_bi"] = BaseFont.CreateFont(BaseFont.TIMES_BOLDITALIC, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
        }

        public Font getFont(string name, Int64 size)
        {
            if (m_base_fonts.ContainsKey(name))
            {
                Font font = new Font(m_base_fonts[name], size);
                return font;
            }

            return new Font(m_base_fonts["times"], 12);
        }

        public BaseFont getBaseFont(string name)
        {
            if (m_base_fonts.ContainsKey(name))
            {
                return m_base_fonts[name];
            }

            return m_base_fonts["times"];
        }
    }
}
