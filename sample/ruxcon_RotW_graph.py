'''
This sample is from "Rainbow over the Windows"(Ruxcon 2016 presentation by zer0mem and me).
'''
import os

src_dir = os.path.join(os.getcwd(), "ruxcon_RotW_graph")

basename = os.path.basename(src_dir)

res_dir = os.path.join(src_dir, "fonts")

style = os.path.join(src_dir, "style.json")
graphics = os.path.join(src_dir, "graphics.json")

pdfk = os.path.join(os.getcwd(), "../bin/Debug/pdfk.exe")

output = os.path.join(src_dir, "%s.pdf" % basename)

cmd = " ".join([pdfk, res_dir, style, graphics, output])
print cmd

os.system(cmd)

os.system(output)
