using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace pdfk
{
    class DirectOperator
    {
        private PdfContentByte m_under_canvas;
        private PdfContentByte m_canvas;
        private PageProperty m_page_property;
        private FontFactory m_font_factory;

        private Int64 m_last_td_x;
        private Int64 m_last_td_y;
        
        public DirectOperator(
            PdfContentByte under_canvas, 
            PdfContentByte canvas,
            PageProperty page_property,
            FontFactory font_factory
            )
        {
            m_under_canvas = under_canvas;
            m_canvas = canvas;
            m_page_property = page_property;
            m_font_factory = font_factory;

            m_last_td_x = 0;
            m_last_td_y = 0;
        }

        public double dkw(Int64 percent)
        {
            return dkw_d(percent);
        }

        public double dkh(Int64 percent)
        {
            return dkh_d(percent);
        }

        public double dkw_d(Int64 percent)
        {
            return m_page_property.W_P2R_d(percent);
        }

        public double dkh_d(Int64 percent)
        {
            return m_page_property.H_P2R_d(percent);
        }

        public void Bg(int red, int green, int blue)
        {
            m_under_canvas.SetRGBColorFill(red, green, blue);
            m_under_canvas.Rectangle(
                dkw(0),
                dkh(0),
                dkw(m_page_property.RESOLUTION),
                dkh(m_page_property.RESOLUTION)
                );
            m_under_canvas.Fill();
        }

        public void fill(int red, int green, int blue)
        {
            m_canvas.SetRGBColorFillF((float)red / (float)255.0, (float)green / (float)255.0, (float)blue / (float)255.0);
        }

        public void stroke(int red, int green, int blue)
        {
            m_canvas.SetRGBColorStrokeF((float)red / (float)255.0, (float)green / (float)255.0, (float)blue / (float)255.0);
        }

        public void BT()
        {
            m_canvas.BeginText();
            m_last_td_x = 0;
            m_last_td_y = 0;
        }

        public void ET()
        {
            m_canvas.EndText();
        }

        public void Tr(int mode)
        {
            m_canvas.SetTextRenderingMode(mode);
        }

        public void Td(Int64 x, Int64 y)
        {
            m_canvas.MoveText(
                (float)(dkw(x) - dkw(m_last_td_x)),
                (float)(dkh(y) - dkh(m_last_td_y))
                );

            m_last_td_x = x;
            m_last_td_y = y;
        }

        public void w(Int64 width)
        {
            m_canvas.SetLineWidth(dkw_d(width));
        }

        public void Tc(Int64 spacing)
        {
            m_canvas.SetCharacterSpacing((float)dkw_d(spacing));
        }

        public void Tw(Int64 spacing)
        {
            m_canvas.SetWordSpacing((float)dkw_d(spacing));
        }

        public void TL(Int64 leading)
        {
            m_canvas.SetLeading((float)dkh_d(leading));
        }

        public void Tz(Int64 scaling)
        {
            m_canvas.SetHorizontalScaling(scaling);
        }

        public void Ts(Int64 rise)
        {
            m_canvas.SetTextRise((float)dkh_d(rise));
        }

        public void Tf(string base_font, Int64 size)
        {
            m_canvas.SetFontAndSize(
                m_font_factory.getBaseFont(base_font),
                (float)dkh_d(size)
                );
        }

        public void Tj(string content)
        {
            m_canvas.ShowText(content);
        }

        public void J(int cap)
        {
            m_canvas.SetLineCap(cap);
        }

        public void dash(Int64 pat)
        {
            if (pat == 0)
            {
                double[] patterns = {  };
                m_canvas.SetLineDash(0);
            }
            else
            {
                double[] patterns = { pat };
                m_canvas.SetLineDash(patterns, 0);
            }
        }

        public void j(int join)
        {
            m_canvas.SetLineJoin(join);
        }

        public void M(int miterLimit)
        {
            m_canvas.SetMiterLimit(miterLimit);
        }

        public void moveTo(Int64 x, Int64 y)
        {
            m_canvas.MoveTo(dkw(x), dkh(y));
        }

        public void rect(Int64 x, Int64 y, Int64 width, Int64 height)
        {
            m_canvas.Rectangle(dkw(x), dkh(y), dkw(width), dkh(height));
            //stroke();
        }

        public void grid(Int64 x, Int64 y, Int64 width, Int64 height, Int64 columns, Int64 rows)
        {
            for (Int64 i = 0; i < columns; i++)
            {
                rect(x, y, width * (i+1) / columns, height);
            }

            for (Int64 i = 0; i < rows; i++)
            {
                rect(x, y, width, height * (i+1) / rows);
            }
            stroke();
        }
        

        public void lineTo(Int64 x, Int64 y)
        {
            m_canvas.LineTo(dkw(x), dkh(y));
        }

        public void curveTo(Int64 x1, Int64 y1, Int64 x2, Int64 y2, Int64 x3, Int64 y3)
        {
            m_canvas.CurveTo(dkw(x1), dkh(y1), dkw(x2), dkh(y2), dkw(x3), dkh(y3));
        }

        public void closePath()
        {
            m_canvas.ClosePath();
        }

        public void saveState()
        {
            m_canvas.SaveState();
        }

        public void stroke()
        {
            m_canvas.Stroke();
        }

        public void fill()
        {
            m_canvas.Fill();
        }

        public void fillStroke()
        {
            m_canvas.FillStroke();
        }

        public void line(Int64 x1, Int64 y1, Int64 x2, Int64 y2)
        {
            moveTo(x1, y1);
            lineTo(x2, y2);
            stroke();
        }
    }
}
