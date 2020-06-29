using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DAN_XLI
{
    public partial class MainWindow : Window
    {
        #region fields
        private string textInput;
        private string copiesInput;
        //validation logic for number of copies
        public int numberOfCopies
        {
            get
            {
                try
                {
                    int n = Convert.ToInt16(copiesInput);
                    if (n > 0)
                    {
                        return n;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        private BackgroundWorker worker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };
        #endregion

        #region constructor
        public MainWindow()
        {
            InitializeComponent();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.ProgressChanged += ProgressChanged;
        }
        #endregion

        #region Events of BackgroundWorker class
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            int sum = 0;
            if (numberOfCopies != 0)
            {
                string fileName;

                //last step will be bigger for this number if 100%numberOfCopies != 0
                int lastStep = 100 % numberOfCopies;
             
                int step = Convert.ToInt32(100 / numberOfCopies);

                for (int i = 1; i <= numberOfCopies; i++)
                {
                    Thread.Sleep(1000);
                    sum = sum + step;
                    if(i == numberOfCopies)
                    {
                        sum += lastStep;
                    }
                    DateTime dateTime = DateTime.Now;
                    // Calling ReportProgress() method raises ProgressChanged event
                    // To this method pass the percentage of processing that is complete
                    worker.ReportProgress(sum);

                    //create file with textInput content
                    fileName = string.Format(i + "." + dateTime.Day.ToString() + "_" + dateTime.Month.ToString() + "_" + dateTime.Year.ToString() + "_" + dateTime.Hour.ToString() + "_" + dateTime.Minute.ToString() + ".txt");
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine(textInput);
                    }


                    // Check if the cancellation is requested
                    if (worker.CancellationPending)
                    {
                        // Set Cancel property of DoWorkEventArgs object to true
                        e.Cancel = true;
                        // Reset progress percentage to ZERO and return
                        worker.ReportProgress(0);
                        return;
                    }
                }
           
            }
            e.Result = sum;
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                label1.Content = "Processing cancelled";
            }
            else if (e.Error != null)
            {
                label1.Content = e.Error.Message;
            }
            else
            {
                label1.Content = e.Result.ToString();
            }

        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            label1.Content = e.ProgressPercentage.ToString() + "%";
        }
        #endregion


        #region Events handles for button clicks and text inputs
        //get text input 
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textInput = text.Text;
        }

        //get copies input
        private void CurrentCopies_TextChanged(object sender, TextChangedEventArgs e)
        {
            copiesInput = copies.Text;
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
    
                // Check if the backgroundWorker is already busy running the asynchronous operation
                if (!worker.IsBusy)
                {
                    // This method will start the execution asynchronously in the background
                    worker.RunWorkerAsync();
                }
                else
                {
                    label2.Content = "Busy processing, please wait...";
                }
            
            
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (worker.IsBusy)
            {
                // Cancel the asynchronous operation if still in progress
                worker.CancelAsync();
            }
            else
            {
                label2.Content = "No operation in progress to cancel.";
            }
        }
        #endregion

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
