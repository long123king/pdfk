using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

using iTextSharp.text;
using iTextSharp.text.pdf;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace pdfk
{
    class DKOperators
    {
        private PdfContentByte m_direct_content;
        private PageProperty m_page_property;
        private FontFactory m_font_factory;
        private DirectOperator m_direct_op;

        private Int64 m_x = 0;
        private Int64 m_y = 0;
        private Int64 m_width = 0;
        private Int64 m_height = 0;


        public DKOperators(
            PdfContentByte direct_content,
            PageProperty page_property,
            FontFactory font_factory,
            DirectOperator direct_op)
        {
            m_direct_content = direct_content;
            m_page_property = page_property;
            m_font_factory = font_factory;
            m_direct_op = direct_op;

            m_width = m_page_property.RESOLUTION;
            m_height = m_page_property.RESOLUTION;
        }

        public void SetMetrics(Int64 x0, Int64 y0, Int64 width0, Int64 height0)
        {
            m_x = x0;
            m_y = y0;
            m_width = width0;
            m_height = height0;
        }

        private Int64 dkw(Int64 x)
        {
            return m_x + x * m_width / m_page_property.RESOLUTION;
        }

        private Int64 dkh(Int64 y)
        {
            return m_y + y * m_height / m_page_property.RESOLUTION;
        }

        private Int64 dkrh(Int64 r)
        {
            /*return r;//*/return r * m_height / m_page_property.RESOLUTION;
        }

        private Int64 dkrw(Int64 r)
        {
            /*return r;// */return r * m_width / m_page_property.RESOLUTION;
        }

        public void Render(Dictionary<string, List<string>> custom_oprs, string opr_name, Int64 idx, Int64 cells)
        {
            //int operator_count = operators.Count;
            //for (int j = 0; j < operator_count; j++)
            foreach (var item in custom_oprs)
            {
                //int op_content_count = ((JArray)operators[j]["contents"]).Count;

                //for (int oc = 0; oc < op_content_count; oc++)
                //foreach (string op_content in op_contents)
                if (item.Key == opr_name)
                {
                    //string op_content = (string)operators[j]["contents"][oc];
                    List<string> op_contents = item.Value;

                    string content = string.Join("", op_contents.ToArray());
                    if (cells == 1)
                    {
                        op_contents.Clear();
                        op_contents.Add(content);
                    }

                    for (int i = 0; i < op_contents.Count; i++)
                    {
                        if ((i % cells) == idx)
                        {
                            //custom_oprs.Remove("main");
                            string[] opstrs = op_contents[i].Split(';');

                            string op_pattern = @"(\w*)\((.*)\)";
                            foreach (var opstr in opstrs)
                            {
                                string opstr1 = opstr.Trim();
                                Match result = Regex.Match(opstr1, op_pattern);
                                if (result != Match.Empty)
                                {
                                    string opr = result.Groups[1].Value;
                                    string oprnds_str = result.Groups[2].Value;

                                    string[] oprnds = oprnds_str.Split(',');
                                    if (opr == "Tj")
                                    {
                                        oprnds_str = oprnds_str.Trim('\"');
                                    }


                                    for (int l = 0; l < oprnds.Length; l++)
                                        oprnds[l] = oprnds[l].Trim();

                                    if (opr == "frame" && oprnds.Length == 4)
                                    {
                                        SetMetrics(
                                            Int64.Parse(oprnds[0]),
                                            Int64.Parse(oprnds[1]),
                                            Int64.Parse(oprnds[2]),
                                            Int64.Parse(oprnds[3]));
                                    }
                                    else if (custom_oprs.ContainsKey(opr))
                                    {
                                        Dictionary<string, List<string>> sub_custom_oprs = new Dictionary<string, List<string>>(custom_oprs);
                                        //sub_custom_oprs["main"] = custom_oprs[opr];
                                        Render(sub_custom_oprs, opr, 0, 1);
                                    }
                                    else if (opr == "Bg" && oprnds.Length == 3)
                                    {
                                        m_direct_op.Bg(int.Parse(oprnds[0]), int.Parse(oprnds[1]), int.Parse(oprnds[2]));
                                    }
                                    else if (opr == "fill")
                                    {
                                        if (oprnds.Length == 1 && oprnds[0] == "")
                                        {
                                            m_direct_op.fill();
                                        }
                                        else if (oprnds.Length == 3)
                                        {
                                            m_direct_op.fill(int.Parse(oprnds[0]), int.Parse(oprnds[1]), int.Parse(oprnds[2]));
                                        }
                                    }
                                    else if (opr == "stroke")
                                    {
                                        if (oprnds.Length == 1 && oprnds[0] == "")
                                        {
                                            m_direct_op.stroke();
                                        }
                                        else if (oprnds.Length == 3)
                                        {
                                            m_direct_op.stroke(int.Parse(oprnds[0]), int.Parse(oprnds[1]), int.Parse(oprnds[2]));
                                        }
                                    }
                                    else if (opr == "fs")
                                    {
                                        if (oprnds.Length == 1 && oprnds[0] == "")
                                        {
                                            m_direct_op.fillStroke();
                                        }
                                    }
                                    else if (opr == "BT")
                                    {
                                        m_direct_op.BT();
                                    }
                                    else if (opr == "ET")
                                    {
                                        m_direct_op.ET();
                                    }
                                    else if (opr == "closePath")
                                    {
                                        m_direct_op.closePath();
                                    }
                                    else if (opr == "saveState")
                                    {
                                        m_direct_op.saveState();
                                    }
                                    else if (opr == "Tr" && oprnds.Length == 1)
                                    {
                                        m_direct_op.Tr(int.Parse(oprnds[0]));
                                    }
                                    else if (opr == "Td" && oprnds.Length == 2)
                                    {
                                        m_direct_op.Td(dkw(Int64.Parse(oprnds[0])), dkh(Int64.Parse(oprnds[1])));
                                    }
                                    else if (opr == "w" && oprnds.Length == 1)
                                    {
                                        m_direct_op.w(dkrh(Int64.Parse(oprnds[0])));
                                    }
                                    else if (opr == "Tc" && oprnds.Length == 1)
                                    {
                                        m_direct_op.Tc(dkrh(Int64.Parse(oprnds[0])));
                                    }
                                    else if (opr == "Tw" && oprnds.Length == 1)
                                    {
                                        m_direct_op.Tw(dkrh(Int64.Parse(oprnds[0])));
                                    }
                                    else if (opr == "TL" && oprnds.Length == 1)
                                    {
                                        m_direct_op.TL(dkrh(Int64.Parse(oprnds[0])));
                                    }
                                    else if (opr == "Tz" && oprnds.Length == 1)
                                    {
                                        m_direct_op.Tz(dkrh(Int64.Parse(oprnds[0])));
                                    }
                                    else if (opr == "Ts" && oprnds.Length == 1)
                                    {
                                        m_direct_op.Ts(dkrh(Int64.Parse(oprnds[0])));
                                    }
                                    else if (opr == "Tf" && oprnds.Length == 2)
                                    {
                                        m_direct_op.Tf(oprnds[0], dkrh(Int64.Parse(oprnds[1])));
                                    }
                                    else if (opr == "Tj")
                                    {
                                        m_direct_op.Tj(oprnds_str);
                                    }
                                    else if (opr == "J" && oprnds.Length == 1)
                                    {
                                        m_direct_op.J(int.Parse(oprnds[0]));
                                    }
                                    else if (opr == "j" && oprnds.Length == 1)
                                    {
                                        m_direct_op.j(int.Parse(oprnds[0]));
                                    }
                                    else if (opr == "M" && oprnds.Length == 1)
                                    {
                                        m_direct_op.M(int.Parse(oprnds[0]));
                                    }
                                    else if (opr == "moveTo" && oprnds.Length == 2)
                                    {
                                        m_direct_op.moveTo(dkw(Convert.ToInt64(Double.Parse(oprnds[0]))), dkh(Convert.ToInt64(Int64.Parse(oprnds[1]))));
                                    }
                                    else if (opr == "lineTo" && oprnds.Length == 2)
                                    {
                                        m_direct_op.lineTo(dkw(Convert.ToInt64(Double.Parse(oprnds[0]))), dkh(Convert.ToInt64(Double.Parse(oprnds[1]))));
                                    }
                                    else if (opr == "line" && oprnds.Length == 4)
                                    {
                                        m_direct_op.line(
                                            dkw(Int64.Parse(oprnds[0])),
                                            dkh(Int64.Parse(oprnds[1])),
                                            dkw(Int64.Parse(oprnds[2])),
                                            dkh(Int64.Parse(oprnds[3])));
                                    }
                                    else if (opr == "curveTo" && oprnds.Length == 6)
                                    {
                                        m_direct_op.curveTo(
                                            dkw(Convert.ToInt64(Double.Parse(oprnds[0]))), dkh(Convert.ToInt64(Double.Parse(oprnds[1]))),
                                            dkw(Convert.ToInt64(Double.Parse(oprnds[2]))), dkh(Convert.ToInt64(Double.Parse(oprnds[3]))),
                                            dkw(Convert.ToInt64(Double.Parse(oprnds[4]))), dkh(Convert.ToInt64(Double.Parse(oprnds[5]))));
                                    }
                                    else if (opr == "rect" && oprnds.Length == 4)
                                    {
                                        m_direct_op.rect(
                                            dkw(Int64.Parse(oprnds[0])), dkh(Int64.Parse(oprnds[1])),
                                            dkrw(Int64.Parse(oprnds[2])), dkrh(Int64.Parse(oprnds[3])));
                                    }
                                    else if (opr == "grid" && oprnds.Length == 6)
                                    {
                                        m_direct_op.grid(
                                            dkw(Int64.Parse(oprnds[0])), dkh(Int64.Parse(oprnds[1])),
                                            dkrw(Int64.Parse(oprnds[2])), dkrh(Int64.Parse(oprnds[3])),
                                            Int64.Parse(oprnds[4]), Int64.Parse(oprnds[5]));
                                    }
                                    else if (opr == "dash" && oprnds.Length == 1)
                                    {
                                        m_direct_op.dash(Int64.Parse(oprnds[0]));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
