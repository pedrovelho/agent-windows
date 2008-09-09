using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using ConfigParser;

namespace AgentForAgent
{
    public partial class Chart : Form
    {
        private Rectangle rect;
        private Pen pen;
        private Brush brush;
        private Graphics G;
        private ArrayList rects;
        private Configuration conf;

        private static int WITH_BAR = 15;

        public Chart(ref Configuration conf)
        {
            InitializeComponent();
            this.conf = conf;
            pen = new Pen(Color.BlueViolet);
            brush = new SolidBrush(Color.BlueViolet);
            rects = new ArrayList();

            //--
            ArrayList list = new ArrayList();
            Label labelHour1;
            Font font;
            font = new Font(FontFamily.GenericSansSerif, 7f);
            for (int i = 0; i <= 24; i++)
            {
                //--Hour
                labelHour1 = new Label();
                labelHour1.Font = font;
                labelHour1.AutoSize = true;
                labelHour1.Location = new System.Drawing.Point(15, ((10 * i)+25));
                labelHour1.Name = i.ToString();
                labelHour1.Size = new System.Drawing.Size(28, 13);
                labelHour1.TabIndex = 0;
                labelHour1.Text = i.ToString();
                this.Controls.Add(labelHour1);
            }
            loadEvents();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            G = e.Graphics;
            for (int i = 0; i <= 24; i++)
            {
                //--Line
                G.DrawLine(pen, 40, ((10 * i) + 30), 270, ((10 * i) + 30));
            }
            
            //--Draw timeslots
            foreach (Rectangle rec in rects)
            {
                G.DrawRectangle(pen, rec);
                G.FillRectangle(brush, rec);
            }
        }

        public void loadEvents()
        {
            rects.Clear();
            foreach (Event ev in conf.events.events)
            {
                CalendarEvent cEv = (CalendarEvent)ev;

                //Rectangle rect = new Rectangle(new Point(43 + cEv.resolveDay()*35, 30), new Size(15, 240));

                //--Start
                int dayTimeslot = 43 + cEv.resolveDay() * 35;
                int startTimeslot = (30 + (10 * cEv.startHour)) + ((cEv.startMinute * 10)/60);
                

                //--Duration
                int timeRemain = 1440 - (cEv.startHour*60) - cEv.startMinute;
                //--On compare le nb de min qu'il reste avec le nb de minute d'une journée
                int duration = (cEv.durationDays*1440) + (cEv.durationHours*60) + cEv.durationMinutes;
                int heightBar = 0;
                Console.Write("dur="+duration+" rem="+timeRemain);
                if (duration <= timeRemain)
                {
                    //--No overruning
                    heightBar = duration*10/60;
                }
                else
                {
                    heightBar = timeRemain*10/60;
                    //--Overflow
                    duration -= timeRemain;
                    int startDay = cEv.resolveDay();
                    int heightBarNext = 0;
                    Rectangle rectNext;

                    while (duration > 0)
                    {
                        //--Day column selection
                        if (startDay > 5)
                            startDay = 0;
                        else
                            startDay++;
                        if (duration <= 1440)
                            heightBarNext = duration * 10 / 60;
                        else
                            heightBarNext = 1440 * 10 / 60;

                        rectNext = new Rectangle(new Point(43 + startDay * 35, 30), new Size(WITH_BAR, heightBarNext));
                        rects.Add(rectNext);
                        duration -= 1440;
                    }
                }
                Rectangle rect = new Rectangle(new Point(dayTimeslot, startTimeslot), new Size(WITH_BAR, heightBar));
                rects.Add(rect);
            }
        }

        private void Chart_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Chart_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
