using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text;

namespace JournalModule.Views
{
    public partial class JournalView : UserControl
    {
        public JournalView()
        {
            InitializeComponent();

            return;

            string text = @"{\rtf1\ansi\ansicpg1251\deff0\deflang1049\fs20{\fonttbl{\f0\fnil\fprq2\fcharset204 Arial;}
{\f99\froman\fcharset0\fprq2{\*\panose 02020603050405020304}Arial;}}
{\colortbl ;\red0\green0\blue0;\red51\green102\blue255;}
\paperw11907\paperh16839\margl0\margr0\margt0\margb0
\pard\plain\sb0\ql\fs20\lang1049 \pard\plain \fi-180\li360 \fs20\lang1049\bullet\tab Превышение времени движения заслонки
\par\pard\sb0\fs20\lang1049 \pard\plain \fi-180\li360 \fs20\lang1049\bullet\tab Устройство: МДУ-1 2.73\pard}";

            MemoryStream stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(text));
            var xxx = GetStringFromStream(stream);
            richTextBox2.Text = text;
            richTextBox.Selection.Load(stream, DataFormats.Rtf);
        }

        public string GetStringFromStream(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}