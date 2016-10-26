using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace pdfk
{
    class TextBox
    {
        private Paragraph m_content;
        private FontFactory m_font_factory;
        private PdfContentByte m_direct_content;
        private ColumnText m_column_text;
        private Int64 m_leading;
        private int m_alignment;
        private Int64 m_height;
        private PageProperty m_page_property;

        public TextBox(
            PdfContentByte direct_content,
            PageProperty page_property, 
            FontFactory font_factory
            )
        {
            m_page_property = page_property;
            m_direct_content = direct_content;
            m_font_factory = font_factory;

            m_content = new Paragraph();            
            m_column_text = new ColumnText(m_direct_content);

            m_leading = dkh(100);
            m_alignment = Element.ALIGN_LEFT;

            m_content.Font = m_font_factory.getFont("songti", dkh(100));           
        }

        private Int64 dkw(Int64 percent)
        {
            return m_page_property.W_P2R(percent);
        }

        private Int64 dkh(Int64 percent)
        {
            return m_page_property.H_P2R(percent);
        }

        public void setFont(string font, Int64 size)
        {
            m_content.Font = m_font_factory.getFont(font, dkh(size));
        }

        public void setFont(string font)
        {
            m_content.Font = m_font_factory.getFont(font, dkh(100));
        }

        public void addLine(string line)
        {
            m_content.Add(line);
            m_content.Add(Chunk.NEWLINE);
        }

        public void addPart(string part)
        {
            m_content.Add(part);
        }

        public void setAlignment(int alignment)
        {
            m_alignment = alignment;
        }

        public void setLeading(Int64 leading)
        {
            m_leading = dkh(leading);
        }

        public void newParagraph()
        {
            m_column_text.AddText(m_content);
            m_content = new Paragraph();
        }

        public void setParagraphSpace(Int64 space)
        {
            m_column_text.ExtraParagraphSpace = dkh(space);
        }

        public void setIndent(Int64 indent)
        {
            m_column_text.Indent = dkw(indent);
        }

        public Int64 getHeight()
        {
            return m_height;
        }

        public void go(Int64 x, Int64 y, Int64 width, Int64 height, bool outline)
        {
            m_column_text.AddText(m_content);
            m_column_text.Leading = m_leading;
            m_column_text.Alignment = m_alignment;

            m_column_text.SetSimpleColumn(
                dkw(x),
                dkh(y),
                dkw(width),
                dkh(height)
                );

            m_column_text.Go();

            m_height = height - m_page_property.H_R2P((Int64)m_column_text.YLine);
            if (outline)
            {
                m_direct_content.Rectangle(
                    dkw(x),
                    dkh(y),
                    dkw(width - x),
                    dkh(height - y));
                m_direct_content.Stroke();
            }
        }
    }
}
