using InnoMetricDataAccess;
using InnoMetricsCollector;
using InnoMetricsCollector.classes;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataCollectorUI
{
    public partial class frmMain : Form
    {
        ReportGenerator myCollector;

        CollectorReport myLastReport;
        Thread check;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            log.Info("Loading app...");
            //loadData();
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", "The data collector is running now...", ToolTipIcon.Info);

            check = new Thread(loadReport);
            check.Start();
        }

        private void loadData()
        {
            log.Info("Loading data...");
            richTextBox1.Text = "";
            List<String> records = DataAccess.loadProcessHistory();

            log.Info(records.ToString());

            foreach (string r in records)
            {
                richTextBox1.Text += r + "\n";
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadData();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            //await loadReport();
        }

        private void loadReport()
        {
            while (true)
            {
                myCollector = new ReportGenerator();
                CollectorReport myReport = myCollector.getCurrentProcessReport();
                log.Info("Currect active activities -> " + myReport.activities.Count);
                if (myLastReport != null)
                {
                    //var result = myLastReport.activities.Where(p => !myReport.activities.Any(p2 => p2.ProcessId == p.ProcessId && p2.ExecutableName == p.ExecutableName));

                    foreach (var app in myReport.activities)
                    {
                        log.Info("App -> " + app.ExecutableName + ", " + app.Description);
                        var lastState = myLastReport.activities.Where(p => app.ProcessId == p.ProcessId && app.ExecutableName == p.ExecutableName).FirstOrDefault();
                        if (lastState != null)
                        {
                            var myLastMeasure = lastState.Measurements.FirstOrDefault(m => m.MeasurementType == "EstimatedChargeRemaining");
                            var myCurrentMeasure = app.Measurements.FirstOrDefault(m => m.MeasurementType == "EstimatedChargeRemaining");

                            /*
                            var BatteryConsumptionLast = lastState.Measurements.FirstOrDefault(m => m.MeasurementType == "BatteryConsumption");

                            if (BatteryConsumptionLast != null)
                            {
                                app.Measurements.Add(new Metrics
                                {
                                    MeasurementType = "BatteryConsumption",
                                    Value = BatteryConsumptionLast.Value
                                });
                            }
                            */
                            if (myCurrentMeasure.Value != "-1")
                            {
                                if (Double.Parse(myLastMeasure.Value) > Double.Parse(myCurrentMeasure.Value))
                                {

                                    var BatteryConsumption = app.Measurements.FirstOrDefault(m => m.MeasurementType == "BatteryConsumption");


                                    if (BatteryConsumption != null)
                                    {
                                        BatteryConsumption.Value = (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value)).ToString();// (Double.Parse(BatteryConsumption.Value) + (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value))).ToString();
                                    }
                                    else
                                    {
                                        app.Measurements.Add(new Metrics
                                        {
                                            MeasurementType = "BatteryConsumption",
                                            Value = (Double.Parse(myLastMeasure.Value) - Double.Parse(myCurrentMeasure.Value)).ToString()
                                        });
                                    }
                                }
                            }
                        }
                    }

                    foreach (var app in myReport.activities)//result)
                    {
                        log.Info("Saving activity data...");
                        DataAccess.SaveMyActivity(app);
                    }

                }

                myLastReport = myReport;

                Thread.Sleep(1000);
            }

        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (check.ThreadState == ThreadState.Running)
                check.Abort();
            Application.Exit();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmSettings myForm = new frmSettings();
            myForm.ShowDialog();
            myForm.Dispose();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            ShowInTaskbar = true;
            Visible = true;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to quit InnoMetrics Collector?", "InnoMetrics data collector", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (check.ThreadState == ThreadState.Running)
                    check.Abort();
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = false;
                Visible = false;
                notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", "The data collector is running now...", ToolTipIcon.Info);
            }
            notifyIcon1.Visible = true;

        }
        public void showNotification(String message, ToolTipIcon icon)
        {
            notifyIcon1.ShowBalloonTip(1000, "InnoMetrics data collector", message, icon);
        }
    }
}
