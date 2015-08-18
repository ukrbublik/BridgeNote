using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.Drawing.Imaging;

namespace BridgeProject
{
    public partial class CardSelector : Control
    {
        CardSelectControl selControl;

        static Hashtable hashCoordinates;
        static Bitmap bmpSelector1;
        static Bitmap bmpSelector2;
        static int borderWidth;
        static public void LoadSettingsFromXML()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlReader xmlCardSelector = XmlReader.Create(Program.ExeDir + "Resources\\card_selector.xml", settings);
            xmlCardSelector.Read();
            xmlCardSelector.ReadStartElement("CardSelector");
            xmlCardSelector.ReadStartElement("Images");
            if (xmlCardSelector.IsStartElement("Image") && xmlCardSelector.GetAttribute("selected").Equals("0"))
                bmpSelector1 = new Bitmap(Program.ExeDir + "Resources\\" + xmlCardSelector.GetAttribute("file"));
            else
                throw new XmlException("Не найден тег Image с атрибутом selected='0'");
            xmlCardSelector.Skip();
            if (xmlCardSelector.IsStartElement("Image") && xmlCardSelector.GetAttribute("selected").Equals("1"))
                bmpSelector2 = new Bitmap(Program.ExeDir + "Resources\\" + xmlCardSelector.GetAttribute("file"));
            else
                throw new XmlException("Не найден тег Image с атрибутом selected='1'");
            xmlCardSelector.Skip();
            xmlCardSelector.ReadEndElement();
            xmlCardSelector.ReadStartElement("Coordinates");
            hashCoordinates = new Hashtable(17);

            String key, x, y, width, height;
            bool isSuit;
            while (xmlCardSelector.NodeType != XmlNodeType.EndElement || !xmlCardSelector.Name.Equals("Coordinates"))
            {
                switch(xmlCardSelector.Name)
                {
                    case "CardValue":
                    case "CardSuit":
                        isSuit = xmlCardSelector.Name.Equals("CardSuit");
                        key = xmlCardSelector.GetAttribute(isSuit ? "suit" : "value");
                        if (!xmlCardSelector.IsEmptyElement)
                        {
                            do
                            {
                                xmlCardSelector.Read();
                                if (xmlCardSelector.IsStartElement("Rectangle"))
                                {
                                    x = xmlCardSelector.GetAttribute("x");
                                    y = xmlCardSelector.GetAttribute("y");
                                    width = xmlCardSelector.GetAttribute("width");
                                    height = xmlCardSelector.GetAttribute("height");
                                    hashCoordinates.Add(key, new Rectangle(int.Parse(x), int.Parse(y), int.Parse(width), int.Parse(height)));
                                }
                            }
                            while (xmlCardSelector.NodeType != XmlNodeType.EndElement || !xmlCardSelector.Name.Equals(isSuit ? "CardSuit" : "CardValue"));
                        }
                        xmlCardSelector.Read(); //читать дальше
                        break;
                    case "Border":
                        borderWidth = int.Parse(xmlCardSelector.GetAttribute("width"));
                        xmlCardSelector.Skip(); //перепрыгнуть дальше
                        break;
                    default:
                        xmlCardSelector.Skip(); //пропустить
                        break;
                }
            }
            xmlCardSelector.ReadEndElement();
            xmlCardSelector.ReadEndElement();
        }

        public CardSelector(CardSelectControl s)
        {
            selControl = s;
            InitializeComponent();
        }

        public void DefineLocation()
        {
            this.Size = CardSelector.bmpSelector1.Size;
            Point loc = selControl.Location;
            //MessageBox.Show(loc.Y.ToString());
            loc.Offset(0, selControl.Size.Height + 1);
            // если жмёт справа:
            if ((loc.X + this.Width) > selControl.Parent.ClientRectangle.Width)
                loc.X = selControl.Parent.ClientRectangle.Width - this.Width;
            // если жмёт снизу:
            if ((loc.Y + this.Height) > selControl.Parent.ClientRectangle.Height)
                loc.Y = selControl.Location.Y - this.Height - 1;
            // если жмёт слева:
            if (loc.X < 0)
                loc.X = 0;
            // если жмёт сверху:
            if (loc.Y < 0)
                loc.Y = 0;

            this.Location = loc;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calling the base class OnPaint
            base.OnPaint(pe);

            Graphics g = pe.Graphics;

            ImageAttributes imgattr = new ImageAttributes();
            g.DrawImage(bmpSelector1, new Rectangle(0, 0, bmpSelector1.Width, bmpSelector1.Height), 0, 0, bmpSelector1.Width, bmpSelector1.Height, GraphicsUnit.Pixel, imgattr);

            // !!!!!!!! рисовать выбор !!!!!!!!
            Rectangle rect = new Rectangle();
            if (selControl.m_card.value != CardValue.Unknown)
            {
                switch (selControl.m_card.value)
                {
                    case CardValue.Two:
                    case CardValue.Three:
                    case CardValue.Four:
                    case CardValue.Five:
                    case CardValue.Six:
                    case CardValue.Seven:
                    case CardValue.Eight:
                    case CardValue.Nine:
                    case CardValue.Ten:
                        rect = (Rectangle)hashCoordinates[((int)selControl.m_card.value).ToString()];
                        break;
                    case CardValue.Jack:
                        rect = (Rectangle)hashCoordinates["J"];
                        break;
                    case CardValue.Queen:
                        rect = (Rectangle)hashCoordinates["Q"];
                        break;
                    case CardValue.King:
                        rect = (Rectangle)hashCoordinates["K"];
                        break;
                    case CardValue.Ace:
                        rect = (Rectangle)hashCoordinates["A"];
                        break;
                }
                rect.Location.Offset(-borderWidth, -borderWidth);
                rect.Inflate(borderWidth, borderWidth);
                g.DrawImage(bmpSelector2, rect.X, rect.Y, rect, GraphicsUnit.Pixel);
            }

            if(selControl.m_card.suit != CardSuit.Unknown)
            {
                switch (selControl.m_card.suit)
                {
                    case CardSuit.Hearts:
                        rect = (Rectangle)hashCoordinates["Hearts"];
                        break;
                    case CardSuit.Diamonds:
                        rect = (Rectangle)hashCoordinates["Diamonds"];
                        break;
                    case CardSuit.Spades:
                        rect = (Rectangle)hashCoordinates["Spades"];
                        break;
                    case CardSuit.Clubs:
                        rect = (Rectangle)hashCoordinates["Clubs"];
                        break;
                 }
                rect.Location.Offset(-borderWidth, -borderWidth);
                rect.Inflate(borderWidth, borderWidth);
                g.DrawImage(bmpSelector2, rect.X, rect.Y, rect, GraphicsUnit.Pixel);
            }

        }

        String strChoise1, strChoise2;

        private void CardSelector_MouseDown(object sender, MouseEventArgs e)
        {
            Program.MainForm.Text = "down  " + e.X.ToString() + " , " + e.Y.ToString();
            strChoise1 = WhereMousePoint(e.X, e.Y);
        }

        private void CardSelector_MouseUp(object sender, MouseEventArgs e)
        {
            Program.MainForm.Text = "up  " + e.X.ToString() + " , " + e.Y.ToString();

            if (e.Button == MouseButtons.Left)
            {
                strChoise2 = WhereMousePoint(e.X, e.Y);
                String strChoiseFinal = strChoise2.Equals("") ? strChoise1 : strChoise2;
                if (!strChoiseFinal.Equals(""))
                {
                    switch (strChoiseFinal)
                    {
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                        case "6":
                        case "7":
                        case "8":
                        case "9":
                        case "10":
                            selControl.m_card.value = (CardValue)int.Parse(strChoiseFinal);
                            break;
                        case "J":
                            selControl.m_card.value = CardValue.Jack;
                            break;
                        case "Q":
                            selControl.m_card.value = CardValue.Queen;
                            break;
                        case "K":
                            selControl.m_card.value = CardValue.King;
                            break;
                        case "A":
                            selControl.m_card.value = CardValue.Ace;
                            break;
                        case "Hearts":
                            selControl.m_card.suit = CardSuit.Hearts;
                            break;
                        case "Diamonds":
                            selControl.m_card.suit = CardSuit.Diamonds;
                            break;
                        case "Clubs":
                            selControl.m_card.suit = CardSuit.Clubs;
                            break;
                        case "Spades":
                            selControl.m_card.suit = CardSuit.Spades;
                            break;
                        default:
                            break;
                    }
                    this.Refresh();
                    selControl.Refresh();
                }
                else
                {
                    //this.selControl.Parent.Controls.Remove(this);
                }
                strChoise1 = "";
                strChoise2 = "";
            }

            this.BringToFront();
            this.Capture = true;
        }

        String WhereMousePoint(int x, int y)
        {
            IDictionaryEnumerator enumer = hashCoordinates.GetEnumerator();
            enumer.Reset();
            while(enumer.MoveNext())
            {
                if (((Rectangle)enumer.Value).Contains(x, y))
                    return (String)enumer.Key;
            }
            return "";
        }

        private void CardSelector_GotFocus(object sender, EventArgs e)
        {

        }

        private void CardSelector_LostFocus(object sender, EventArgs e)
        {

        }
    }
}
