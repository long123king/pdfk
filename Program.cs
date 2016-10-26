using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using iTextSharp.text;
using iTextSharp.text.pdf;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace pdfk
{
    class Program
    {
        private JObject m_style_root;
        private JObject m_graphics_root;
        private FontFactory m_font_factory;
        private PageProperty m_page_property;

        private Int64 m_page_width;
        private Int64 m_page_height;

        private Document m_document;
        private PdfWriter m_writer;
        private PdfContentByte m_direct_content;
        private PdfContentByte m_direct_content_under;
        private DirectOperator m_direct_op;

        int m_page_count;
        bool m_show_page_num;

        Dictionary<int, List<string>> m_op_page_map;
        Dictionary<string, List<string>> m_custom_oprs;


        public Program(string res_dir, string style_path, string graphics_path, string output_path)
        {
            m_font_factory = new FontFactory(res_dir);
            m_page_property = new PageProperty();

            m_style_root = JObject.Parse(File.ReadAllText(style_path));
            m_graphics_root = JObject.Parse(File.ReadAllText(graphics_path));

            m_page_width = m_page_property.A4_SHORT;
            m_page_height = m_page_property.A4_LONG;

            m_page_count = 1;
            m_show_page_num = false;

            if (m_style_root["main"] != null)
            {
                if (m_style_root["main"]["width"] != null)
                    m_page_width = (Int64)m_style_root["main"]["width"];

                if (m_style_root["main"]["height"] != null)
                    m_page_height = (Int64)m_style_root["main"]["height"];

                if (m_style_root["main"]["pages"] != null)
                    m_page_count = (int)m_style_root["main"]["pages"];

                if (m_style_root["main"]["show_page"] != null)
                    m_show_page_num = (int)m_style_root["main"]["show_page"] != 0;
            }

            m_page_property.reset(m_page_width, m_page_height);
            m_document = new Document(new RectangleReadOnly(m_page_width, m_page_height));
            m_writer = PdfWriter.GetInstance(m_document, new FileStream(output_path, FileMode.Create));

            m_document.Open();

            m_direct_content = m_writer.DirectContent;
            m_direct_content_under = m_writer.DirectContentUnder;
            m_direct_op = new DirectOperator(
                m_direct_content_under, m_direct_content, m_page_property, m_font_factory);

            m_op_page_map = new Dictionary<int, List<string>>();
            m_custom_oprs = new Dictionary<string, List<string>>();
        }

        public void parse_operators()
        {
            foreach (KeyValuePair<string, JToken> opr in m_graphics_root)
            {
                string opr_name = opr.Key;//.Split(':')[1];

                JArray opr_draws = (JArray)opr.Value;

                List<string> contents = new List<string>();
                string content = "";
                foreach (string op in opr_draws)
                {
                    content += op;
                    contents.Add(op);
                }

                m_custom_oprs.Add(opr_name, contents);

                JObject block_style = (JObject)m_style_root[opr_name];

                if (block_style != null)
                {
                    int page_idx = 0;
                    if (block_style["page"] != null)
                    {
                        string page_range = (string)block_style["page"];
                        if (page_range == "even")
                        {
                            for (int i = 0; i < m_page_count; i += 2)
                            {
                                if (!m_op_page_map.ContainsKey(i))
                                    m_op_page_map[i] = new List<string>();

                                m_op_page_map[i].Add(opr_name);
                            }
                        }
                        else if (page_range == "odd")
                        {
                            for (int i = 1; i < m_page_count; i += 2)
                            {
                                if (!m_op_page_map.ContainsKey(i))
                                    m_op_page_map[i] = new List<string>();

                                m_op_page_map[i].Add(opr_name);
                            }
                        }
                        else if (page_range == "all")
                        {
                            for (int i = 0; i < m_page_count; i += 1)
                            {
                                if (!m_op_page_map.ContainsKey(i))
                                    m_op_page_map[i] = new List<string>();

                                m_op_page_map[i].Add(opr_name);
                            }
                        }
                        else if (page_range == "none")
                        {

                        }
                        else if (page_range.Contains(':'))
                        {
                            var idxes = page_range.Split(':');
                            if (idxes.Length == 2)
                            {
                                string start = idxes[0];
                                string end = idxes[1];

                                int start_idx = 0;
                                int end_idx = 0;

                                if (start != "")
                                    start_idx = int.Parse(start);

                                if (end != "")
                                    end_idx = int.Parse(end);

                                if (start_idx < 0 && start_idx > 0 - m_page_count)
                                {
                                    start_idx = m_page_count + start_idx;
                                }

                                if (end_idx < 0 && end_idx > 0 - m_page_count)
                                {
                                    end_idx = m_page_count + end_idx;
                                }

                                if (start_idx >= 0 && end_idx > start_idx && end_idx < m_page_count)
                                {
                                    for (int i = start_idx; i <= end_idx; i += 1)
                                    {
                                        if (!m_op_page_map.ContainsKey(i))
                                            m_op_page_map[i] = new List<string>();

                                        m_op_page_map[i].Add(opr_name);
                                    }
                                }
                            }
                        }
                        else
                        {
                            page_idx = (int)block_style["page"];

                            if (!m_op_page_map.ContainsKey(page_idx))
                                m_op_page_map[page_idx] = new List<string>();

                            m_op_page_map[page_idx].Add(opr_name);
                        }
                    }
                }
            }
        }

        public void render_operators()
        {
            for (int i = 0; i < m_page_count; i++)
            {
                int page_idx = i;

                if (page_idx != 0)
                    m_document.NewPage();

                m_direct_op.BT();
                m_direct_op.Tf("times", 100);
                m_direct_op.Td(4950, m_show_page_num ? 200 : -200);
                string page_num = (page_idx + 1).ToString();
                m_direct_op.Tj(page_num);
                m_direct_op.ET();

                if (m_op_page_map.ContainsKey(page_idx))
                {
                    foreach (var opr_name in m_op_page_map[page_idx])
                    {
                        Int64 x = 0;
                        Int64 y = 0;
                        Int64 width = m_page_property.RESOLUTION;
                        Int64 height = m_page_property.RESOLUTION;

                        Int64 rows = 1;
                        Int64 columns = 1;

                        if (m_style_root[opr_name] != null)
                        {
                            if (m_style_root[opr_name]["x"] != null)
                                x = (Int64)m_style_root[opr_name]["x"];

                            if (m_style_root[opr_name]["y"] != null)
                                y = (Int64)m_style_root[opr_name]["y"];

                            if (m_style_root[opr_name]["width"] != null)
                                width = (Int64)m_style_root[opr_name]["width"];

                            if (m_style_root[opr_name]["height"] != null)
                                height = (Int64)m_style_root[opr_name]["height"];

                            if (m_style_root[opr_name]["rows"] != null)
                                rows = (Int64)m_style_root[opr_name]["rows"];

                            if (m_style_root[opr_name]["columns"] != null)
                                columns = (Int64)m_style_root[opr_name]["columns"];
                        }

                        for (Int64 r = 0; r < rows; r++)
                        {
                            for (Int64 c = 0; c < columns; c++)
                            {
                                Int64 cell_x = x + c * width / columns;
                                Int64 cell_y = y + (rows - r - 1) * height / rows;

                                Int64 cell_width = width / columns;
                                Int64 cell_height = height / rows;

                                DKOperators draw_imp = new DKOperators(m_direct_content, m_page_property, m_font_factory, m_direct_op);

                                draw_imp.SetMetrics(cell_x, cell_y, cell_width, cell_height);
                                draw_imp.Render(m_custom_oprs, opr_name, r * columns + c, rows * columns);
                            }
                        }
                    }
                }
            }
        }

        public void close()
        {
            m_document.Close();
        }

        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: pdfk.exe fonts_dir style graphics output");
                return;
            }

            string fonts_dir = args[0];

            if (fonts_dir[fonts_dir.Length - 1] != '/' && fonts_dir[fonts_dir.Length - 1] != '\\')
                fonts_dir = fonts_dir.Insert(fonts_dir.Length, "\\");

            Program pdfk_instance = new Program(fonts_dir, args[1], args[2], args[3]);

            pdfk_instance.parse_operators();
            pdfk_instance.render_operators();
            pdfk_instance.close();
        }
    }
}
